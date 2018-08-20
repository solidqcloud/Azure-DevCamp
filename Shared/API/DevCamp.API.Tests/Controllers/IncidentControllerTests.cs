using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevCamp.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevCamp.API.Models;
using DevCamp.API.Tests.Utils;
using System.Web.Http.Results;
using System.Configuration;
using Newtonsoft.Json;

namespace DevCamp.API.Controllers.Tests
{
    [TestClass()]
    public class IncidentControllerTests
    {
        [TestMethod()]
        public async Task ShouldGetAllIncidentsAPITest()
        {
            var controller = new IncidentController();
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Incident testIncident = IncidentGenerator.GetTestIncident(methodName);
            var result = controller.CreateIncident(testIncident);
            Assert.IsNotNull(result);

            var incidents = await controller.GetAllIncidents();

            //Assert.AreNotEqual(0, incidents.Count);
        }

        [TestMethod()]
        public void ShouldGetAllIncidentsCountAPITest()
        {
            var controller = new IncidentController();
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Incident testIncident = IncidentGenerator.GetTestIncident(methodName);
            var result = controller.CreateIncident(testIncident);
            Assert.IsNotNull(result);

            var incidentCount = controller.GetIncidentCount();

            Assert.AreNotEqual(0, incidentCount);
            Console.WriteLine($"Incident count is {incidentCount}");
        }


        [TestMethod()]
        public void ShouldGetByIdAPITest()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Incident testIncident = IncidentGenerator.GetTestIncident(methodName);
            var controller = new IncidentController();
            var result = controller.CreateIncident(testIncident);
            Assert.IsNotNull(result);

            //var newIncident = await controller.GetById();
        }

        [TestMethod()]
        public void ShouldCreateIncidentAPITest()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Incident testIncident = IncidentGenerator.GetTestIncident(methodName);
            var controller = new IncidentController();
            var result = controller.CreateIncident(testIncident);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void ShouldUpdateIncidentTest()
        {
            //string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            //Incident testIncident = IncidentGenerator.GetTestIncident(methodName);
            //var controller = new IncidentController();
            //var result = controller.CreateIncident(testIncident);
            //Assert.IsNotNull(result);

            ////Update the incident
            //string thumbnailUri = $"http://newuri/thumbnail/{DateTime.Now.Ticks.ToString()}";
            //Incident updatedIncident = JsonConvert.DeserializeObject<Incident>(result.ToString());
            //updatedIncident.ThumbnailUri = new Uri(thumbnailUri);

            //var updateResult = controller.UpdateIncident(updatedIncident.Id, updatedIncident).ContinueWith(task =>
            //{
            //    if (task.Status == TaskStatus.RanToCompletion)
            //    {
            //        Assert.IsNotNull(task.Result);
            //        updateResult;
            //    }
            //}
            //Assert.AreEqual(, updatedIncident.ThumbnailUri);
        }
    }
}