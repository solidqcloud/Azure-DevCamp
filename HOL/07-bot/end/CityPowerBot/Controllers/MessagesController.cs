﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using System.IO;
using System.Net.Http.Headers;
using System.Configuration;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CityPowerBot
{
	[BotAuthentication]
	public class MessagesController : ApiController
	{
		private const int IMAGE_SIZE_LIMIT = 4000000;
		public static Stream LastImage { get; set; } = null;
		public static String LastImageType { get; set; } = String.Empty;
		public static String LastImageName { get; set; } = String.Empty;
		public static String LastImageTags { get; set; } = String.Empty;
		public static String LastImageDescription { get; set; } = String.Empty;

		public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
		{
			if (activity.Type == ActivityTypes.Message)
			{
				// Stores send images out of order.
				var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
				var imageAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Contains("image"));
				if (imageAttachment != null)
				{
					LastImage = await GetImageStream(connector, imageAttachment);
					LastImageName = imageAttachment.Name;
					LastImageType = imageAttachment.ContentType;
					ComputerVisionResult computerVisionResult = await GetImageInfo(LastImage);
					LastImageTags = String.Join(", ", computerVisionResult.Tags);
					LastImageDescription = computerVisionResult.Description;
					String replyText = "Got your image!";
					if (!String.IsNullOrWhiteSpace(computerVisionResult.Text))
					{
						replyText += $" It probably shows { computerVisionResult.Text}.";
					}
					Activity reply = activity.CreateReply(replyText);
					await connector.Conversations.ReplyToActivityAsync(reply);
				}
				else
				{
					// Creates a dialog stack for the new conversation, adds MainDialog to the stack, and forwards all messages to the dialog stack.
					await Conversation.SendAsync(activity, () => new MainDialog());
				}
			}
			var response = Request.CreateResponse(HttpStatusCode.OK);
			return response;
		}

		private static async Task<Stream> GetImageStream(ConnectorClient connector, Attachment imageAttachment)
		{
			using (var httpClient = new HttpClient())
			{
				// The Skype attachment URLs are secured by JwtToken,
				// you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
				// https://github.com/Microsoft/BotBuilder/issues/662
				var uri = new Uri(imageAttachment.ContentUrl);
				if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
				{
					httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync(connector));
					httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
				}

				return await httpClient.GetStreamAsync(uri);
			}
		}

		/// <summary>
		/// Gets the JwT token of the bot. 
		/// </summary>
		/// <param name="connector"></param>
		/// <returns>JwT token of the bot</returns>
		private static async Task<string> GetTokenAsync(ConnectorClient connector)
		{
			var credentials = connector.Credentials as MicrosoftAppCredentials;
			if (credentials != null)
			{
				return await credentials.GetTokenAsync();
			}

			return null;
		}

		private Activity HandleSystemMessage(Activity message)
		{
			if (message.Type == ActivityTypes.DeleteUserData)
			{
				// Implement user deletion here
				// If we handle user deletion, return a real message
			}
			else if (message.Type == ActivityTypes.ConversationUpdate)
			{
				// Handle conversation state changes, like members being added and removed
				// Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
				// Not available in all channels
			}
			else if (message.Type == ActivityTypes.ContactRelationUpdate)
			{
				// Handle add/remove from contact lists
				// Activity.From + Activity.Action represent what happened
			}
			else if (message.Type == ActivityTypes.Typing)
			{
				// Handle knowing tha the user is typing
			}
			else if (message.Type == ActivityTypes.Ping)
			{
			}

			return null;
		}

		private static async Task<ComputerVisionResult> GetImageInfo(Stream imageStream)
		{
			ComputerVisionResult result = new ComputerVisionResult();
			try
			{
				// Call cognitive services.
				var jsonResult = string.Empty;
				using (HttpClient client = new HttpClient())
				{
					// Request headers.
					client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["AZURE_COGNITIVE_SERVICES_KEY"]);
					var queryString = HttpUtility.ParseQueryString(string.Empty);
					queryString["visualFeatures"] = "Description,Tags";
					queryString["language"] = "en";

					// Assemble the URI for the REST API Call.
					string uri = ConfigurationManager.AppSettings["AZURE_COGNITIVE_SERVICES_URI"] + queryString;

					// Request body. Posts a locally stored JPEG image.
					byte[] byteData = null;
					using (MemoryStream ms = new MemoryStream())
					{
						imageStream.CopyTo(ms);
						if (ms.Length >= IMAGE_SIZE_LIMIT)
							throw new ArgumentException($"Images size should be less than {IMAGE_SIZE_LIMIT / 1024} Kb");

						byteData = ms.ToArray();
					}

					using (ByteArrayContent content = new ByteArrayContent(byteData))
					{
						content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

						// Execute the REST API call.
						var response = await client.PostAsync(uri, content);

						if (response.StatusCode != HttpStatusCode.OK)
						{
							throw new Exception("Image analysis failed.");
						}

						// Get the JSON response.
						jsonResult = await response.Content.ReadAsStringAsync();
					}
				}

				// Retrieve only tags
				JObject json = JObject.Parse(jsonResult);

				JToken captions = json["description"]["captions"];
				JToken tags = json["tags"];

				String caption = captions.First()["text"].Value<String>();
				Double captionConfidence = captions.First()["confidence"].Value<Double>();
				if (captionConfidence >= 0.8)
				{
					result.Description = caption;
					result.Text = caption;
				}

				foreach (JToken item in tags)
				{
					String tag = item["name"].Value<String>();
					Double confidence = item["confidence"].Value<Double>();

					if (confidence >= 0.8)
					{
						result.Tags.Add(tag);
						if (String.IsNullOrWhiteSpace(result.Text))
						{
							result.Text = tag;
						}
					}
				}
			}
			catch (Exception ex)
			{
				result.Description = ex.Message;
			}
			return result;
		}
	}
}