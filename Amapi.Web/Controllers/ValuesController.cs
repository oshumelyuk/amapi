﻿using System;
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
            return await repo.ListAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<string> Get(string value)
        {
            var createdTime = await repo.SearchAsync(value);
            return createdTime.HasValue ? createdTime.Value.ToLocalTime().ToString("yyyy-mm-dd HH:MM") : "Never";
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task Put(string value)
        {
            await repo.PutAsync(value);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task Delete(string value)
        {
            await repo.DeleteAsync(value);
        }
    }
}
