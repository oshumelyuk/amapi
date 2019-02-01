using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nest;

namespace Amapi.Web
{
    internal class Repository
    {
        private static Repository repo { get; set; }
        private static  readonly object _locker;
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
                Interlocked.Exchange(ref repoLocal, repo);
            }
            return repo;
        }

        public async Task<IEnumerable<string>> ListAsync()
        {
            var res = await _client.SearchAsync<SimpleValue>(x => x
                .From(0)
                .Size(10));
            return res.Documents.Select(x => x.Value).ToArray();
        }

        public Task PutAsync(string value)
        {
            var indexed = new SimpleValue(value);
            return _client.IndexAsync(value, idx => idx.Index("values"));
        }

        public async Task<DateTime?> SearchAsync(string value)
        {
            var res = await _client.SearchAsync<SimpleValue>(x => x
                .From(0)
                .Size(1)
                .Query(q => q
                    .Term(t => t.Value, value))
                );
            return res.Documents.First()?.CreatedDate;
        }

        public async Task DeleteAsync(string value)
        {
            var val = await _client.SearchAsync<SimpleValue>(x => x
                .From(0)
                .Size(1)
                .Query(q => q
                    .Term(t => t.Value, value))
                );
            var res = await _client.DeleteAsync<SimpleValue>(val.Documents.SingleOrDefault());
        }

        #region private

        private void Init(IConfiguration conf)
        {
            _client = new ElasticClient(new Uri(conf.GetValue<string>("SearchEndpointUrl")));
        }

        #endregion
    }
}
