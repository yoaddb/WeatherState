using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private Dictionary<string, List<User>> users;
        public WeatherController() : base()
        {
            // read JSON file stored in files folder
            string text = System.IO.File.ReadAllText(@"./files/users.json");
            users = JsonConvert.DeserializeObject<Dictionary<string, List<User>>>(text);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserWeather(string username)
        {
            users.TryGetValue("users", out List<User> userList);
            if(userList == null)
            {
                return NotFound();
            }

            User found = userList.ToArray().FirstOrDefault(user => user.name == username);
            if (found == null)
            {
                return BadRequest();
            }

            string json = await GetWeatherStateName(found.woeid);
            if (json == null)
            {
                return BadRequest();
            }

            JObject parsed = JObject.Parse(json);
            List<JToken> items = parsed["consolidated_weather"].Children().ToList();

            List<JToken> list = new List<JToken>();

            foreach(JToken item in items)
            {
                if(item != null && item["weather_state_name"] != null)
                {
                    list.Add(item["weather_state_name"]);
                }
            }
            return Ok(new UserDetailsResponse() 
            { 
                email = found.email,
                weatherStateName = string.Join(",", list.Distinct()),
                updateDate = DateTime.Now.ToString("dd/MM/yyyy")
            });
        }
        private async Task<string> GetWeatherStateName(int Id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://www.metaweather.com/api/location/" + Id;
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
