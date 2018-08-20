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
				// Creates a dialog stack for the new conversation, adds MainDialog to the stack, and forwards all messages to the dialog stack.
                await Conversation.SendAsync(activity, () => new MainDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
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
    }
}