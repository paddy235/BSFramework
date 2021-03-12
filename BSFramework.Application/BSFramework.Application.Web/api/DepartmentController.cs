using BSFramework.Application.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BSFramework.Application.Web.api
{
    /// <summary>
    /// 部门同步接口
    /// </summary>
    public class DepartmentController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 同步部门
        /// </summary>
        /// <param name="model"></param>
        // POST api/<controller>
        public void Post([FromBody]DepartmentModel model)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}