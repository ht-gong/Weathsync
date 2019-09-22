using GeoChemApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeoChemApp.APIServices
{
    class APIService
    {
        public async Task<List<Env_Datapoint>> GetLatest()
        {
            try
            {
                var httpClient = new HttpClient();
                var url = @"http://gzgeochemlab.azurewebsites.net/api/values";
                var json = await httpClient.GetStringAsync(url);
                return JsonConvert.DeserializeObject<List<Env_Datapoint>>(json);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
