using IncidentAPI;
using IncidentAPI.Models;
using DevCamp.WebApp.Utils;
using DevCamp.WebApp.ViewModels;
using DevCamp.WebApp.Mappers;

namespace DevCamp.WebApp.Mappers
{
	public class IncidentMapper
	{
		public static Incident MapIncidentViewModel(IncidentViewModel incident)
		{
			Incident newIncident = new Incident();
			newIncident.FirstName = incident.FirstName;
			newIncident.LastName = incident.LastName;
			newIncident.Street = incident.Street;
			newIncident.City = incident.City;
			newIncident.State = incident.State;
			newIncident.ZipCode = incident.ZipCode;
			newIncident.PhoneNumber = incident.PhoneNumber;
			newIncident.Description = incident.Description;
			newIncident.OutageType = incident.OutageType;
			newIncident.IsEmergency = incident.IsEmergency;
			newIncident.Tags = incident.Tags;
			return newIncident;
		}

		public static IncidentViewModel MapIncidentModelToView(Incident incident)
		{
			IncidentViewModel newIncidentView = new IncidentViewModel();
			newIncidentView.Id = incident.Id;
			newIncidentView.FirstName = incident.FirstName;
			newIncidentView.LastName = incident.LastName;
			newIncidentView.Street = incident.Street;
			newIncidentView.City = incident.City;
			newIncidentView.State = incident.State;
			newIncidentView.ZipCode = incident.ZipCode;
			newIncidentView.PhoneNumber = incident.PhoneNumber;
			newIncidentView.Description = incident.Description;
			newIncidentView.OutageType = incident.OutageType;
			newIncidentView.IsEmergency = incident.IsEmergency.Value;
			newIncidentView.Tags = incident.Tags;
			newIncidentView.Created = incident.Created.Value.ToUniversalTime();
			newIncidentView.LastModified = incident.LastModified.Value.ToUniversalTime();
			return newIncidentView;
		}
	}
}