using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TokenUtils;
using TokenUtils.Entity;

namespace MvcApi.Controllers
{
    public class TokenController : ApiController
    {
        [HttpPost]
        public TokenInfoEntity Get(IdentityCheckEntity entity)
        {
            var service = new UzaiSecurityVerificationLogicService();
            return service.GetToken(entity);
        }

        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}