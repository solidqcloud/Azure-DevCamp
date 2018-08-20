using IncidentAPI;
using Microsoft.Rest;
using System;

namespace DevCamp.WebApp.Utils
{
    public class IncidentApiHelper
    {
        public static IncidentAPIClient GetIncidentAPIClient()
        {
            ServiceClientCredentials creds = new BasicAuthenticationCredentials();
            var client = new IncidentAPIClient(new Uri(Settings.INCIDENT_API_URL),creds);
            return client;
        }
    }
}
