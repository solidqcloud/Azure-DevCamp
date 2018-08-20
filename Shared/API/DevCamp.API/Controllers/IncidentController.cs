using DevCamp.API.Data;
using DevCamp.API.Models;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DevCamp.API.Controllers
{
    public class IncidentController : ApiController
    {
        [HttpGet]
        [Route("incidents/{IncidentId}")]
        public async Task<IHttpActionResult> GetById(string IncidentId)
        {
            Incident incident = await DocumentDBRepository<Incident>.GetItemAsync(IncidentId);

            if (incident == null)
            {
                return NotFound();
            }
            return Ok(incident);
        }

        [HttpGet]
        [Route("incidents")]
        public async Task<IHttpActionResult> GetAllIncidents()
        {
            var incidents = await DocumentDBRepository<Incident>.GetItemsAsync(i => !i.Resolved);
            IEnumerable sorted = null;
            if (incidents != null)
            {
                sorted = incidents.OrderBy(i => i.SortKey);
            }
            return Ok(sorted);
        }

        [HttpGet]
        [Route("incidents/count")]
        public int GetIncidentCount()
        {
            var incidentCount = DocumentDBRepository<Incident>.GetItemsCount();
            return incidentCount;
        }

        [HttpGet]
        [Route("incidents/count/includeresolved")]
        public int GetAllIncidentsCount()
        {
            var incidentCount = DocumentDBRepository<Incident>.GetItemsCount(true);
            return incidentCount;
        }

        [HttpPost]
        [Route("incidents/")]
        public async Task<IHttpActionResult> CreateIncident(Incident newIncident)
        {
            //Set the created and modified timestamp
            if (newIncident == null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new ApiResponseMsg("Unable to create new incident"),
                    ReasonPhrase = "Incident is null or formatted incorrectly"
                };
                throw new HttpResponseException(resp);
            }

            DateTime utcNow = DateTime.UtcNow;
            newIncident.Created = utcNow;
            newIncident.LastModified = utcNow;

            var newDocDbIncident = await DocumentDBRepository<Incident>.CreateItemAsync(newIncident);

            if (newDocDbIncident == null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new ApiResponseMsg("Unable to create incident"),
                    ReasonPhrase = "An error occurred"
                };
                throw new HttpResponseException(resp);
            }

            return Ok(newDocDbIncident);
        }

        [HttpPut]
        [Route("incidents/")]
        public async Task<IHttpActionResult> UpdateIncident(string IncidentId, Incident newIncident)
        {
            var newDocDbIncident = await DocumentDBRepository<Incident>.UpdateItemAsync(IncidentId, newIncident);

            if (newDocDbIncident == null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new ApiResponseMsg("Unable to update incident"),
                    ReasonPhrase = "An error occurred"
                };
                throw new HttpResponseException(resp);
            }

            return Ok(newDocDbIncident);
        }

        [HttpGet]
        [Route("incidents/clear")]
        public async Task<IHttpActionResult> ClearData()
        {
            await DocumentDBRepository<Incident>.ClearDatabase();
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ApiResponseMsg("Cleared database")
            };
            return Ok(resp);
        }

        [HttpGet]
        [Route("incidents/sampledata")]
        public async Task<IHttpActionResult> SampleData()
        {
            await DocumentDBRepository<Incident>.LoadSampleData(true);
            var recordCount = DocumentDBRepository<Incident>.GetItemsCount();
            var resp = new HttpResponseMessage()
            {
                Content = new ApiResponseMsg($"Initialized sample data with [{recordCount}] incidents")
            };
            return Ok(resp);
        }

        [HttpGet]
        [Route("incidents/fakedata")]
        public async Task<IHttpActionResult> FakeData()
        {
            await DocumentDBRepository<Incident>.LoadFakeData(true);
            var recordCount = DocumentDBRepository<Incident>.GetItemsCount();
            var resp = new HttpResponseMessage()
            {
                Content = new ApiResponseMsg($"Initialized fake data with [{recordCount}] incidents")
            };
            return Ok(resp);
        }
    }
}