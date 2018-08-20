using DevCamp.WebApp.Mappers;
using DevCamp.WebApp.Utils;
using DevCamp.WebApp.ViewModels;
using IncidentAPI;
using IncidentAPI.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DevCamp.WebApp.Controllers
{
    public class IncidentController : Controller
    {
        public ActionResult Details(string Id)
        {
			IncidentViewModel incidentView = null;

			using (IncidentAPIClient client = IncidentApiHelper.GetIncidentAPIClient())
			{
				var result = client.IncidentOperations.GetById(Id);
				Newtonsoft.Json.Linq.JObject jobj = (Newtonsoft.Json.Linq.JObject)result;
				Incident incident = jobj.ToObject<Incident>();
				incidentView = IncidentMapper.MapIncidentModelToView(incident);
			}

			return View(incidentView);
		}
		
        public ActionResult Create()
        {
            //### TO BE REPLACED WITH API CALLS ###
            return View();
        }

		[HttpPost]
		public async Task<ActionResult> Create([Bind(Include = "City,Created,Description,FirstName,ImageUri,IsEmergency,LastModified,LastName,OutageType,PhoneNumber,Resolved,State,Street,ZipCode")] IncidentViewModel incident, HttpPostedFileBase imageFile)
		{
			try
			{
				if (ModelState.IsValid)
				{
					Incident incidentToSave = IncidentMapper.MapIncidentViewModel(incident);

					using (IncidentAPIClient client = IncidentApiHelper.GetIncidentAPIClient())
					{
						var result = client.IncidentOperations.CreateIncident(incidentToSave);
						Newtonsoft.Json.Linq.JObject jobj = (Newtonsoft.Json.Linq.JObject)result;
						incidentToSave = jobj.ToObject<Incident>();
					}

					//Now upload the file if there is one
					if (imageFile != null && imageFile.ContentLength > 0)
					{
						//### Add Blob Upload code here #####
						//Give the image a unique name based on the incident id
						var imageUrl = await StorageHelper.UploadFileToBlobStorage(incidentToSave.Id, imageFile);
						//### Add Blob Upload code here #####


						//### Add Queue code here #####
						//Add a message to the queue to process this image
						await StorageHelper.AddMessageToQueue(incidentToSave.Id, imageFile.FileName);
						//### Add Queue code here #####
					}

					//##### CLEAR CACHE ####
					RedisCacheHelper.ClearCache(Settings.REDISCCACHE_KEY_INCIDENTDATA);
					//##### CLEAR CACHE ####

					return RedirectToAction("Index", "Dashboard");
				}
			}
			catch
			{
				return View();
			}

			return View(incident);
		}
	}
}