using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using IncidentAPI;
using IncidentAPI.Models;
using Microsoft.Rest;
using System.IO;
using Microsoft.Azure;

namespace DataWriter
{
    public class IncidentController
    {
        [HttpPost]
        public static async Task<bool> CreateAsync(String firstName, String lastName, String street, String city, String state, String zipCode, String phoneNumber, String description, String outageType, Boolean? isEmergency, Stream image, String imageName, String imageType, String tags)
        {
            Boolean added = false;
            try
            {
                Incident newIncident = new Incident();
                newIncident.FirstName = firstName;
                newIncident.LastName = lastName;
                newIncident.Street = street;
                newIncident.City = city;
                newIncident.State = state;
                newIncident.ZipCode = zipCode;
                newIncident.PhoneNumber = phoneNumber;
                newIncident.Description = description;
                newIncident.OutageType = outageType;
                newIncident.IsEmergency = isEmergency;
				newIncident.Tags = tags;

                using (IncidentAPIClient client = GetIncidentAPIClient())
                {
                    var result = client.IncidentOperations.CreateIncident(newIncident);
                    Newtonsoft.Json.Linq.JObject jobj = (Newtonsoft.Json.Linq.JObject)result;
                    newIncident = jobj.ToObject<Incident>();
                    added = true;
                }

                if (image != null)
                {
                    //Give the image a unique name based on the incident id
                    var imageUrl = StorageHelper.UploadFileToBlobStorage(newIncident.Id, image, imageType, imageName);

                    //Add a message to the queue to process this image
                    await StorageHelper.AddMessageToQueue(newIncident.Id, imageName);
                }
            }
            catch (Exception e)
            {
            }
            return added;
        }

        public static IncidentAPIClient GetIncidentAPIClient()
        {
            ServiceClientCredentials creds = new BasicAuthenticationCredentials();
            var client = new IncidentAPIClient(new Uri(CloudConfigurationManager.GetSetting("INCIDENT_API_URL")), creds);
            return client;
        }
    }
}