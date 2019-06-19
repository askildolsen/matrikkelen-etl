using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Npgsql;
using NetTopologySuite.Geometries;

namespace matrikkelen_etl
{
    public class MatrikkelenContext : DbContext
    {
        public DbSet<VegadresseBruksenhet> VegadresseBruksenheter { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"Host=localhost;Port=5434;Database=matrikkelen;Username=matrikkelen;Password=matrikkelen", x => x.UseNetTopologySuite());
        }
    }

    [Table("vegadressebruksenhet")]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class VegadresseBruksenhet
    {
        [Key]
        public int objid { get; set; }
        public string objtype { get; set; }
        public int adressekode { get; set; }
        public string adressenavn { get; set; }
        public string bokstav { get; set; }
        public int nummer { get; set; }
        public string adressetilleggsnavn { get; set; }
        public string kommunenavn { get; set; }
        public string grunnkretsnummer { get; set; }
        public string grunnkretsnavn { get; set; }
        public string valgkretsnummer { get; set; }
        public string valgkretsnavn { get; set; }
        public string postnummer { get; set; }
        public string poststed { get; set; }
        public string tettstednummer { get; set; }
        public string tettstednavn { get; set; }
        public string soknenummer { get; set; }
        public int organisasjonsnummer { get; set; }
        public string soknenavn { get; set; }
        public int gardsnummer { get; set; }
        public int bruksnummer { get; set; }
        public int? festenummer { get; set; }
        public int? seksjonsnummer { get; set; }
        public string offisielladressetekstutenadressetilleggsnavn { get; set; }
        public string bruksenhetid { get; set; }
        public string offisielladressetekst { get; set; }
        public string bruksenhetnummertekst { get; set; }
        public string lokalid { get; set; }
        public string navnerom { get; set; }
        public string versjonid { get; set; }
        public DateTime datauttaksdato { get; set; }
        public DateTime oppdateringsdato { get; set; }
        public bool stedfestingverifisert { get; set; }
        public string adressetilleggsnavnkilde { get; set; }
        public string adresse_kommunenummer { get; set; }
        public string adresse_matrikkelnummerbruksenhet_matrikkelnummer_kommunenummer { get; set; }
        [JsonIgnore]
        public Geometry representasjonspunkt { get; set; }
        [NotMapped]
        [JsonProperty(PropertyName = "representasjonspunkt")]
        public Geometri _representasjonspunkt =>
            new Geometri {
                srid = representasjonspunkt.SRID,
                geometritype = representasjonspunkt.GeometryType,
                wkt = representasjonspunkt.ToString()
            };
    }

    public class Geometri
    {
        public int srid { get; set; }
        public string geometritype { get; set; }
        public string wkt { get; set; }
    }
}
