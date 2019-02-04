using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Nest;

namespace Amapi.Web
{
    internal class Repository
    {
        private static Repository repo;
        private static readonly object _locker = new object();
        private ElasticClient _client;

        private Repository(IConfiguration conf)
        {
            Init(conf);
        }


        public static Repository GetInstance(IConfiguration conf)
        {
            if (repo != null)
            {
                return repo;
            }
            lock (_locker)
            {
                var repoLocal = new Repository(conf);
                Interlocked.Exchange(ref repo, repoLocal);
            }
            return repo;
        }

        public async Task<IEnumerable<string>> ListAsync()
        {
            var res = await _client.SearchAsync<SimpleValue>(x => x
                .From(0)
                .Size(10)
                .Index("values"));
            return res.Documents.Select(x => x.Value).ToArray();
        }

        public Task PutAsync(string value)
        {
            var indexed = new SimpleValue(value);
            return _client.IndexAsync(indexed, idx => idx.Index("values"));
        }

        public async Task<DateTime?> SearchAsync(string value)
        {
            var res = await _client.SearchAsync<SimpleValue>(x => x
                .From(0)
                .Size(1)
                .Index("values")
                .Query(q => q
                    .Term(t => t.Value, value))
                );
            return res.Documents.First()?.CreatedDate;
        }

        public async Task DeleteAsync(string value)
        {
            var response =
                await _client.DeleteByQueryAsync<SimpleValue>(q => q
                      .Index("values")
                      .Query(rq => rq
                          .Term(f => f.Value, value)
                      )
                  );
        }

        #region private

        private void Init(IConfiguration conf)
        {
            _client = new ElasticClient(new Uri(conf.GetValue<string>("SearchEndpointUrl")));

            if (_client.IndexExists("values").Exists)
            {
                _client.DeleteIndex("values");
            }

            var settings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 2 };

            var indexConfig = new IndexState
            {
                Settings = settings
            };

            _client.CreateIndex("values", c => c
            .InitializeUsing(indexConfig)
            .Mappings(m => m.Map<SimpleValue>(mp => mp.AutoMap())));
        }

        #endregion
    }
}
