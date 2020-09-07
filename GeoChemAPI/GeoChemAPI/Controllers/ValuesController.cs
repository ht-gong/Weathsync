using GeoChemAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GeoChemAPI.Controllers
{
    public class ValuesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET api/values
        public IEnumerable<Env_datapoint> Get()
        {
            TimeCompare comparer = new TimeCompare();
            var latest = db.Env_Datapoints.ToList().OrderByDescending(user => user.GetDateTime());
            return latest;
        }

        // GET api/values/5
        public Env_datapoint Get(int id)
        {
            var found = db.Env_Datapoints.Where(message => message.Id == id);
            return found.FirstOrDefault();
        }

        // POST api/values
        public IHttpActionResult Post([FromBody]Env_datapoint message)
        {
            var newmessage = new Env_datapoint()
            {
                Concentration = message.Concentration,
                Continued_time = message.Continued_time,
                IsHighPollution = message.IsHighPollution,
                IsMonitorError = message.IsMonitorError,
                Monitor_Error = message.Monitor_Error,
                Pollution_Level = message.Pollution_Level,
                Time = message.Time,
                Field = message.Field
            };

            db.Env_Datapoints.Add(newmessage);
            db.SaveChanges();
            return StatusCode(HttpStatusCode.Created);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
