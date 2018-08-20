using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevCamp.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevCamp.API.Models;
using System.Configuration;
using Microsoft.Azure.Documents;
using DevCamp.API.Tests.Utils;

namespace DevCamp.API.Data.Tests
{
    [TestClass()]
    public class DocumentDBTests
    {
        private static string databaseId = ConfigurationManager.AppSettings["DOCUMENTDB_DATABASEID"];
        private static string endpointUrl = ConfigurationManager.AppSettings["DOCUMENTDB_ENDPOINT"];
        private static string authorizationKey = ConfigurationManager.AppSettings["DOCUMENTDB_PRIMARY_KEY"];
        private static string collectionId = ConfigurationManager.AppSettings["DOCUMENTDB_COLLECTIONID"];

        [TestMethod()]
        public async Task ShouldCreateNewIncidentTest()
        {
            string methodName = System.Reflection.MethodInfo.GetCurrentMethod().Name;
            Incident testIncident = IncidentGenerator.GetTestIncident(methodName);
            await DocumentDBRepository<Incident>.Initialize(endpointUrl, authorizationKey, databaseId, collectionId);
            Document newIncident = await DocumentDBRepository<Incident>.CreateItemAsync(testIncident);
            Assert.IsNotNull(newIncident);
        }

        [TestMethod()]
        public async Task ShouldGetAllIncidentsTest()
        {
            await DocumentDBRepository<Incident>.Initialize(endpointUrl, authorizationKey, databaseId, collectionId);
            var incidents = await DocumentDBRepository<Incident>.GetItemsAsync(i => !i.Resolved);
            Assert.IsNotNull(incidents);
            Assert.AreNotEqual(0, incidents.ToList().Count);
        }

        [TestMethod()]
        public async Task ShouldGetIncidentCountTest()
        {
            await DocumentDBRepository<Incident>.Initialize(endpointUrl, authorizationKey, databaseId, collectionId);
            var incidentCount = DocumentDBRepository<Incident>.GetItemsCount();
            Assert.AreNotEqual(0, incidentCount);
            Console.WriteLine($"Incident count is {incidentCount}");
        }

        [TestMethod()]
        public async Task ShouldGetAllIncidentCountTest()
        {
            await DocumentDBRepository<Incident>.Initialize(endpointUrl, authorizationKey, databaseId, collectionId);
            var incidentCount = DocumentDBRepository<Incident>.GetItemsCount(true);
            Assert.AreNotEqual(0, incidentCount);
            Console.WriteLine($"Incident count is {incidentCount}");
        }



        [TestMethod()]
        public async Task ShouldGetIncidentByIdTest()
        {
            await DocumentDBRepository<Incident>.Initialize(endpointUrl, authorizationKey, databaseId, collectionId);
            string methodName = System.Reflection.MethodInfo.GetCurrentMethod().Name;
            Incident testIncident = IncidentGenerator.GetTestIncident(methodName);
            var newIncident = await DocumentDBRepository<Incident>.CreateItemAsync(testIncident);
            Assert.IsNotNull(newIncident);

            string incId = newIncident.Id;
            var savedIncident = await DocumentDBRepository<Incident>.GetItemAsync(incId);
            Assert.IsNotNull(savedIncident);
            Assert.AreEqual(savedIncident.Id, incId);
        }
    }
}