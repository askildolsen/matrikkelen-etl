using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq.Indexing;
using static matrikkelen_etl.ResourceModel;
using static matrikkelen_etl.ResourceModelUtils;

namespace matrikkelen_etl
{
    public class MatrikkelenResourceModel
    {
        public class MatrikkelenResourceIndex : AbstractMultiMapIndexCreationTask<Resource>
        {
            public MatrikkelenResourceIndex()
            {
                AddMap<VegadresseBruksenhet>(matrikkelen =>
                    from vegadresse in matrikkelen.WhereEntityIs<VegadresseBruksenhet>("Matrikkelen")
                    let metadata = MetadataFor(vegadresse)
                    where metadata.Value<string>("@id").StartsWith("Matrikkelen/Vegadresse")

                    let adresse =
                        new[] {
                            vegadresse.adressenavn.ToString(),
                            String.Join("", new[] { vegadresse.nummer.ToString(), vegadresse.bokstav.ToString() }.Where(v => !String.IsNullOrEmpty(v)) )
                        }.Where(v => !String.IsNullOrEmpty(v))

                    select new Resource
                    {
                        ResourceId = vegadresse.postnummer + "/" + String.Join("/", adresse),
                        Type = new[] { "Vegadresse" },
                        SubType = new string[] { },
                        Title = new[] { String.Join(" ", adresse) },
                        SubTitle = new[] { vegadresse.postnummer + " " + vegadresse.poststed },
                        Code = new[] { vegadresse.gardsnummer + "/" + vegadresse.bruksnummer },
                        Status = new string[] { },
                        Properties =
                            (
                                new[] {
                                    new Property {
                                        Name = "Adresse",
                                        Value = new[] {
                                            String.Join(" ", adresse),
                                            vegadresse.postnummer + " " + vegadresse.poststed
                                        },
                                        Resources = new[] {
                                            new Resource { Type = new[] { "Poststed" }, Code = new[] { vegadresse.postnummer }, Title = new[] { vegadresse.poststed } },
                                            new Resource { Type = new[] { "Kommune" }, Code = new[] { vegadresse.adresse_kommunenummer }, Title = new[] { vegadresse.kommunenavn } }
                                        }
                                    }
                                }
                            ).Union(
                                from wkt in new[] { vegadresse._representasjonspunkt.wkt }.Where(v => !String.IsNullOrWhiteSpace(v))
                                select new Property { Name = "Representasjonspunkt", Tags = new[] { "@wkt" }, Value = new[] { WKTProjectToWGS84(wkt, 0) } }
                            ),
                        Source = new[] { metadata.Value<string>("@id") },
                        Modified = vegadresse.oppdateringsdato
                    }
                );

                Reduce = results  =>
                    from result in results
                    group result by result.ResourceId into g
                    select new Resource
                    {
                        ResourceId = g.Key,
                        Type = g.SelectMany(r => r.Type).Distinct(),
                        SubType = g.SelectMany(r => r.SubType).Distinct(),
                        Title = g.SelectMany(r => r.Title).Distinct(),
                        SubTitle = g.SelectMany(r => r.SubTitle).Distinct(),
                        Code = g.SelectMany(r => r.Code).Distinct(),
                        Status = g.SelectMany(r => r.Status).Distinct(),
                        Properties = (IEnumerable<Property>)Properties(g.SelectMany(r => r.Properties)),
                        Source = g.SelectMany(resource => resource.Source).Distinct(),
                        Modified = g.Select(resource => resource.Modified).Max()
                    };

                Index(r => r.Properties, FieldIndexing.No);
                Store(r => r.Properties, FieldStorage.Yes);

                OutputReduceToCollection = "MatrikkelenResource";

                AdditionalSources = new Dictionary<string, string>
                {
                    {
                        "ResourceModel",
                        ReadResourceFile("matrikkelen_etl.ResourceModelUtils.cs")
                    }
                };
            }

            public override IndexDefinition CreateIndexDefinition()
            {
                var indexDefinition = base.CreateIndexDefinition();
                indexDefinition.Configuration = new IndexConfiguration { { "Indexing.MapBatchSize", "8192"} };

                return indexDefinition;
            }
        }
    }
}