using DevCamp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCamp.API.Tests.Utils
{
    public class IncidentGenerator
    {
        public static Incident GetTestIncident(string Name)
        {
            DateTime utcNow = DateTime.UtcNow;
            string unique = Guid.NewGuid().ToString();
            Incident newIncident = new Incident()
            {
                FirstName = $"first-{Name}",
                LastName = $"last-{Name}",
                City = $"city-{Name}",
                Description = $"description-{unique}-{Name}",
                IsEmergency = false,
                OutageType = "Outage",
                State = "State",
                Street = $"123 main st-{unique}",
                PhoneNumber = "555-555-5555",
                ZipCode = "55555",
                ImageUri = new Uri($"http://uri/{unique}/{Name}"),
                ThumbnailUri = new Uri($"http://thumbnailuri/{unique}/{Name}"),
                Resolved = false,
				Tags = "Street, Cars"
            };

            return newIncident;
        }
    }
}
