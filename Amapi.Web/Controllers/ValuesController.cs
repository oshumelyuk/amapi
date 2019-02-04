using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Practices.TransientFaultHandling;

namespace Amapi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private Repository repo { get; set; }

        public ValuesController(IConfiguration conf)
        {
            Guard.ArgumentNotNull(conf, nameof(conf));
            repo = Repository.GetInstance(conf);
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            Guard.ArgumentNotNull(repo, nameof(repo));
            return await repo.ListAsync();
        }

        // GET api/values/5
        [HttpGet("{value}")]
        public async Task<string> Get(string value)
        {
            Guard.ArgumentNotNull(repo, nameof(repo));
            Guard.ArgumentNotNullOrEmptyString(value, nameof(value));

            var createdTime = await repo.SearchAsync(value);
            return createdTime.HasValue ? createdTime.Value.ToLocalTime().ToString("yyyy-mm-dd HH:MM") : "Never";
        }

        // PUT api/values/5
        [HttpPut("{value}")]
        public async Task Put(string value)
        {
            Guard.ArgumentNotNull(repo, nameof(repo));
            Guard.ArgumentNotNullOrEmptyString(value, nameof(value));

            await repo.PutAsync(value);
        }

        // DELETE api/values/5
        [HttpDelete("{value}")]
        public async Task Delete(string value)
        {
            Guard.ArgumentNotNull(repo, nameof(repo));
            Guard.ArgumentNotNullOrEmptyString(value, nameof(value));

            await repo.DeleteAsync(value);
        }
    }
}
