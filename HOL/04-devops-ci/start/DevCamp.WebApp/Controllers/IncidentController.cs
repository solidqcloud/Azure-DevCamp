using DevCamp.WebApp.Mappers;
using DevCamp.WebApp.Models;
using DevCamp.WebApp.Utils;
using DevCamp.WebApp.ViewModels;
using IncidentAPI;
using IncidentAPI.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DevCamp.WebApp.Controllers
{
    public class IncidentController : Controller
    {
		[Authorize]
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

		[Authorize]
		public async Task<ActionResult> Create()
		{
			//####### FILL IN THE DETAILS FOR THE NEW INCIDENT BASED ON THE USER
			IncidentViewModel incident = new IncidentViewModel();

			string userObjId = ClaimsPrincipal.Current.FindFirst(Settings.AAD_OBJECTID_CLAIMTYPE).Value;
			SessionTokenCache tokenCache = new SessionTokenCache(userObjId, HttpContext);

			string tenantId = ClaimsPrincipal.Current.FindFirst(Settings.AAD_TENANTID_CLAIMTYPE).Value;
			string authority = string.Format(Settings.AAD_INSTANCE, tenantId, "");
			AuthHelper authHelper = new AuthHelper(authority, Settings.AAD_APP_ID, Settings.AAD_APP_SECRET, tokenCache);
			string accessToken = await authHelper.GetUserAccessToken(Url.Action("Create", "Incident", null, Request.Url.Scheme));

			UserProfileViewModel userProfile = new UserProfileViewModel();
			try
			{

				using (var client = new HttpClient())
				{
					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					// New code:
					HttpResponseMessage response = await client.GetAsync(Settings.GRAPH_CURRENT_USER_URL);
					if (response.IsSuccessStatusCode)
					{
						string resultString = await response.Content.ReadAsStringAsync();

						userProfile = JsonConvert.DeserializeObject<UserProfileViewModel>(resultString);
					}
				}
			}
			catch (Exception eee)
			{
				ViewBag.Error = "An error has occurred. Details: " + eee.Message;
			}

			incident.FirstName = userProfile.GivenName;
			incident.LastName = userProfile.Surname;
			//####### 
			return View(incident);
		}

		[Authorize]
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

					//##### SEND EMAIL #####
					await SendIncidentEmail(incidentToSave, Url.Action("Index", "Dashboard", null, Request.Url.Scheme));
					//##### SEND EMAIL  #####

					return RedirectToAction("Index", "Dashboard");
				}
			}
			catch
			{
				return View();
			}

			return View(incident);
		}

		private async Task SendIncidentEmail(Incident incidentData, string AuthRedirectUrl)
		{
			string userObjId = ClaimsPrincipal.Current.FindFirst(Settings.AAD_OBJECTID_CLAIMTYPE).Value;

			//The email is the UPN of the user from the claim
			string emailAddress = getUserEmailAddressFromClaims(ClaimsPrincipal.Current);

			SessionTokenCache tokenCache = new SessionTokenCache(userObjId, HttpContext);

			string tenantId = ClaimsPrincipal.Current.FindFirst(Settings.AAD_TENANTID_CLAIMTYPE).Value;
			string authority = string.Format(Settings.AAD_INSTANCE, tenantId, "");
			AuthHelper authHelper = new AuthHelper(authority, Settings.AAD_APP_ID, Settings.AAD_APP_SECRET, tokenCache);
			string accessToken = await authHelper.GetUserAccessToken(Url.Action("Create", "Incident", null, Request.Url.Scheme));

			EmailMessage msg = getEmailBodyContent(incidentData, emailAddress);

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				// New code:
				StringContent msgContent = new StringContent(JsonConvert.SerializeObject(msg), System.Text.Encoding.UTF8, "application/json");
				msgContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
				HttpResponseMessage response = await client.PostAsync(Settings.GRAPH_SENDMESSAGE_URL, msgContent);
				if (response.IsSuccessStatusCode)
				{
					string resultString = await response.Content.ReadAsStringAsync();
				}
			}
		}

		private string getUserEmailAddressFromClaims(ClaimsPrincipal CurrentIdentity)
		{
			string email = string.Empty;
			//see of the name claim looks like an email address
			//Another option is to access the graph again to get the email addresss
			if (CurrentIdentity.Identity.Name.Contains("@"))
			{
				email = CurrentIdentity.Identity.Name;
			}
			else
			{
				foreach (Claim c in CurrentIdentity.Claims)
				{
					if (c.Value.Contains("@"))
					{
						//Might be an email address, use it
						email = c.Value;
						break;
					}
				}
			}
			return email;
		}

		private static EmailMessage getEmailBodyContent(Incident incidentData, string EmailFromAddress)
		{
			EmailMessage msg = new EmailMessage();
			msg.Message.body.contentType = Settings.EMAIL_MESSAGE_TYPE;
			msg.Message.body.content = string.Format(Settings.EMAIL_MESSAGE_BODY, incidentData.FirstName, incidentData.LastName);
			msg.Message.subject = Settings.EMAIL_MESSAGE_SUBJECT;
			Models.EmailAddress emailTo = new Models.EmailAddress() { name = EmailFromAddress, address = EmailFromAddress };
			ToRecipient sendTo = new ToRecipient();
			sendTo.emailAddress = emailTo;
			msg.Message.toRecipients.Add(sendTo);
			msg.SaveToSentItems = true;
			return msg;
		}
	}
}