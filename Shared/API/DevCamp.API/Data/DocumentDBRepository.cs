using DevCamp.API.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace DevCamp.API.Data
{
    public class DocumentDBRepository<T> where T : class
    {
        private static string AUTHORIZATIONKEY = ConfigurationManager.AppSettings["DOCUMENTDB_PRIMARY_KEY"];
        private static string COLLECTIONID = ConfigurationManager.AppSettings["DOCUMENTDB_COLLECTIONID"];
        private static string DATABASEID = ConfigurationManager.AppSettings["DOCUMENTDB_DATABASEID"];
        private static string ENDPOINTURL = ConfigurationManager.AppSettings["DOCUMENTDB_ENDPOINT"];
        private static DocumentClient client;

        public static async Task<Document> CreateItemAsync(T item)
        {
            return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DATABASEID, COLLECTIONID), item);
        }

        public static async Task DeleteItemAsync(string id)
        {
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DATABASEID, COLLECTIONID, id));
        }

        public static async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DATABASEID, COLLECTIONID, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public static async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DATABASEID, COLLECTIONID),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public static int GetItemsCount()
        {
            return GetItemsCount(false);
        }
        public static int GetItemsCount(bool IncludeResolved)
        {
            string excludeResolvedQuery = "SELECT c.id FROM c WHERE c.Resolved = false";
            string allQuery = "SELECT c.id FROM c";
            string query = excludeResolvedQuery;

            if (IncludeResolved)
            {
                query = allQuery;
            }

            var docList = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DATABASEID, COLLECTIONID),
                query)
                .ToList();

            return docList.Count;
        }

        private static List<Incident> getSampleIncidents()
        {
            List<Incident> samples = new List<Incident>();

            try
            {
                string sampleFile = HttpContext.Current.Server.MapPath("\\Data\\sampleincidents.json");
                JObject sampleData = JObject.Parse(File.ReadAllText(sampleFile));
                samples = JsonConvert.DeserializeObject<List<Incident>>(sampleData.Root["sampleincidents"].ToString());
                foreach(Incident s in samples)
                {
                    //Generate a sort key
                    string sortKey = string.Format("{0:D19}", DateTime.MaxValue.Ticks - s.Created.Value.Ticks);
                    s.SortKey = sortKey;
                }
            }
            catch
            {
                //If there are any errors, generate a generic sample set
                int MAX_INCIDENTS = 10;

                for (int i = 0; i < MAX_INCIDENTS; i++)
                {
                    Incident sampleIncident = new Incident();
                    DateTime utcNow = DateTime.UtcNow;
                    sampleIncident.FirstName = "first-" + i;
                    sampleIncident.LastName = "last-" + i;
                    sampleIncident.OutageType = "";
                    sampleIncident.City = "city-" + i;
                    sampleIncident.Description = "description-" + i;
                    sampleIncident.IsEmergency = false;
                    sampleIncident.OutageType = "Outage-" + i;
                    sampleIncident.PhoneNumber = "555555000-" + i;
                    sampleIncident.Resolved = false;
                    sampleIncident.State = "IL";
                    sampleIncident.Street = "street-" + i;
                    sampleIncident.ZipCode = "50500-" + i;
                    sampleIncident.Created = utcNow;
                    sampleIncident.LastModified = utcNow;
					sampleIncident.Tags = "Street, Cars";
                    samples.Add(sampleIncident);
                }
            }
            return samples;
        }

        private static List<Incident> getFakeIncidents()
        {
            List<Incident> fakeData = new List<Incident>();

            try
            {
                string fakeDataFile = HttpContext.Current.Server.MapPath("\\Data\\fakeincidents.json");
                JObject sampleData = JObject.Parse(File.ReadAllText(fakeDataFile));
                fakeData = JsonConvert.DeserializeObject<List<Incident>>(sampleData.Root["fakeincidents"].ToString());
                foreach (Incident s in fakeData)
                {
                    //Generate a sort key based on the time it was uploaded
                    string sortKey = string.Format("{0:D19}", DateTime.MaxValue.Ticks - s.Created.Value.Ticks);
                    s.SortKey = sortKey;
                }
            }
            catch(Exception e)
            {
                //If there is an error loading the fakes, load the samples
                fakeData = getSampleIncidents();
            }
            return fakeData;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public static async Task Initialize()
        {
            client = new DocumentClient(new Uri(ENDPOINTURL), AUTHORIZATIONKEY);
            await CreateDatabaseIfNotExistsAsync(DATABASEID);
            await CreateCollectionIfNotExistsAsync(DATABASEID, COLLECTIONID);
        }

        /// <summary>
        /// Overrides for testing
        /// </summary>
        /// <param name="EndpointUrl"></param>
        /// <param name="AuthKey"></param>
        /// <param name="DatabaseId"></param>
        /// <param name="CollectionId"></param>
        public static async Task Initialize(string EndpointUrl, string AuthKey, string DatabaseId, string CollectionId)
        {
            client = new DocumentClient(new Uri(EndpointUrl), AuthKey);
            DATABASEID = DatabaseId;
            COLLECTIONID = CollectionId;
            ENDPOINTURL = EndpointUrl;
            COLLECTIONID = CollectionId;

            await CreateDatabaseIfNotExistsAsync(DatabaseId);
            await CreateCollectionIfNotExistsAsync(DatabaseId, CollectionId);
        }

        public static async Task ClearDatabase()
        {
            client = new DocumentClient(new Uri(ENDPOINTURL), AUTHORIZATIONKEY);
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DATABASEID));
                await client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(DATABASEID));
                await CreateDatabaseIfNotExistsAsync(DATABASEID);
                await CreateCollectionIfNotExistsAsync(DATABASEID, COLLECTIONID);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await CreateDatabaseIfNotExistsAsync(DATABASEID);
                }
                else
                {
                    throw;
                }
            }
        }

        public static async Task LoadSampleData(bool Cleardata)
        {
            if (Cleardata)
            {
                await ClearDatabase();
            }

            List<Incident> sampleIncidents = getSampleIncidents();
            foreach (Incident i in sampleIncidents)
            {
                await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DATABASEID, COLLECTIONID), i);
            }
        }

        public static async Task LoadFakeData(bool Cleardata)
        {
            if (Cleardata)
            {
                await ClearDatabase();
            }

            List<Incident> fakeIncidents = getFakeIncidents();
            foreach (Incident fi in fakeIncidents)
            {
                await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DATABASEID, COLLECTIONID), fi);
            }
        }

        public static async Task<Document> UpdateItemAsync(string id, T item)
        {
            return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DATABASEID, COLLECTIONID, id), item);
        }
        private static async Task CreateCollectionIfNotExistsAsync(string databaseId, string collectionId)
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseId),
                        new DocumentCollection { Id = collectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateDatabaseIfNotExistsAsync(string databaseId)
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = databaseId });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}