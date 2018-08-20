using DevCamp.WebApp.Utils;
using IncidentAPI;
using IncidentAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DevCamp.WebApp.Controllers
{
	public class DashboardController : Controller
	{
		public async Task<ActionResult> Index()
		{
			//##### API DATA HERE #####
			List<Incident> incidents;
			using (var client = IncidentApiHelper.GetIncidentAPIClient())
			{
				//TODO: BEGIN ADD Caching
				int CACHE_EXPIRATION_SECONDS = 300;

				//Check Cache
				string cachedData = string.Empty;
				if (RedisCacheHelper.UseCachedDataSet(Settings.REDISCCACHE_KEY_INCIDENTDATA, out cachedData))
				{
					incidents = JsonConvert.DeserializeObject<List<Incident>>(cachedData);
				}
				else
				{
					//If stale refresh
					var results = await client.IncidentOperations.GetAllIncidentsAsync();
					Newtonsoft.Json.Linq.JArray ja = (Newtonsoft.Json.Linq.JArray)results;
					incidents = ja.ToObject<List<Incident>>();
					RedisCacheHelper.AddtoCache(Settings.REDISCCACHE_KEY_INCIDENTDATA, incidents, CACHE_EXPIRATION_SECONDS);
				}
				//TODO: END ADD Caching
			}
			return View(incidents);
		}
	}
}