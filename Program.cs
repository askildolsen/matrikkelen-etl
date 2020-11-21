using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Raven.Client.Documents;
using Raven.Client.Documents.BulkInsert;
using Raven.Client.Documents.Operations.Indexes;
using Raven.Client.Json;

namespace matrikkelen_etl
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var store = new DocumentStore { Urls = new string[] { "http://localhost:8080" }, Database = "Digitalisert" })
            {
                store.Conventions.FindCollectionName = t => t.Name;
                store.Initialize();

                var stopwatch = Stopwatch.StartNew();

                using (BulkInsertOperation bulkInsert = store.BulkInsert())
                {
                    using (var context = new MatrikkelenContext())
                    {
                        foreach(var vegadressebruksenhet in context.VegadresseBruksenheter.AsNoTracking())
                        {
                            bulkInsert.Store(
                                vegadressebruksenhet,
                                "Matrikkelen/Vegadresse/" + vegadressebruksenhet.objid,
                                new MetadataAsDictionary(new Dictionary<string, object> { { "@collection", "Matrikkelen"}})
                            );
                        }
                    }
                }

                new MatrikkelenResourceModel.MatrikkelenResourceIndex().Execute(store);

                stopwatch.Stop();
                Console.WriteLine(stopwatch.Elapsed);
            }
        }
    }
}
