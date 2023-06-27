using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace EdgeFilteredDataPOC
{
    public class EdgeFilteredDataPOC
    {
        [FunctionName("EdgeFilteredDataPOC")]
        public async Task RunAsync([BlobTrigger("edge-sku-storage/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            if (!name.EndsWith(".csv"))
            {
                log.LogInformation($"Blob '{name}' doesn't have the .csv extension. Skipping processing.");
                return;
            }

            log.LogInformation($"Blob '{name}' found. Uploading to Redis");

            var keysWritten = await ReadBlobAndUpdateRedisCacheAsync(myBlob, log);

            log.LogInformation($"Blob '{name}' keys. written - {keysWritten}");

            log.LogInformation($"Blob '{name}' uploaded");
        }

        private static async Task<int> ReadBlobAndUpdateRedisCacheAsync(Stream myBlob, ILogger log)
        {
            log.LogInformation($" Uploading to Redis started");

            int keysWrittenToCache = 0;

            /* Read contents of the updated BLOB */
            using (var reader = new StreamReader(myBlob))
            {
                var counter = 0;
                var batch = new Dictionary<string, string>();

                while (!reader.EndOfStream)
                {
                    // assuming data format of xxxx,yyyyy
                    var line = reader.ReadLine()?.Split(new char[] { ',' });

                    /* Ignore lines which could not be read or do not have at least one delimiter. */
                    if (line != null && line.Length > 1)
                    {
                        batch.Add(line[0], line[1]);
                        counter++;

                    }
                }
                foreach (var item in batch)
                {
                    log.LogInformation($"Key = {item.Key}, Value = {item.Value}");
                }
                keysWrittenToCache += batch.Count;

                // calling Redis to upload file
                log.LogInformation($"Connecting to redis");

                var redis = ConnectionMultiplexer.Connect("redis-product-catalog-edge-cache-np.redis.cache.windows.net:6380,password=KqpQvcETO3pZ4UIESsdBLJ7lkAuCpb4LvAzCaDviixs=,ssl=True,abortConnect=False");
                var redisDatabase = redis.GetDatabase();

                List<KeyValuePair<RedisKey, RedisValue>> load = new List<KeyValuePair<RedisKey, RedisValue>>();

                // getting current keys                
                log.LogInformation("Getting data back before pushing load-");
                var server = redis.GetServer("redis-product-catalog-edge-cache-np.redis.cache.windows.net:6380");
                foreach (var key in server.Keys())
                {
                    log.LogInformation($"KeyName - {key}");
                }

                log.LogInformation("Loading  new  data-");
                batch.ToList().ForEach((item) =>
                {
                    load.Add(new KeyValuePair<RedisKey, RedisValue>(item.Key, item.Value));
                });

                // push  to redis
                await redisDatabase.StringSetAsync(load.ToArray());
                log.LogInformation("uploading to redis ends");

                // get all keys back
                log.LogInformation("Getting data back-");
                //var server = redis.GetServer("FilteredProductData-Edge.redis.cache.windows.net:6380");
                foreach (var key in server.Keys())
                {
                    log.LogInformation($"KeyName - {key}");
                }

                return keysWrittenToCache;

                
            }
        }
    }
}
