using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevCamp.WebApp.ViewModels
{
	public class UserProfileViewModel
	{
		public string Id { get; set; }
		public List<string> BusinessPhones { get; set; }
		public string DisplayName { get; set; }
		public string GivenName { get; set; }
		public string JobTitle { get; set; }
		public string Mail { get; set; }
		public string MobilePhone { get; set; }
		public string OfficeLocation { get; set; }
		public string PreferredLanguage { get; set; }
		public string Surname { get; set; }
		public string UserPrincipalName { get; set; }
	}

	public enum SendMessageStatusEnum
	{
		NotSent,
		Sent,
		Fail
	}

	// Data / schema contracts between this app and the Office 365 unified API server.
	public class SendMessageResponse
	{
		public SendMessageStatusEnum Status { get; set; }
		public string StatusMessage { get; set; }
	}

	public class SendMessageRequest
	{
		public Message Message { get; set; }

		public bool SaveToSentItems { get; set; }
	}

	public class Message
	{
		public string Subject { get; set; }
		public MessageBody Body { get; set; }
		public List<Recipient> ToRecipients { get; set; }
	}
	public class Recipient
	{
		public UserProfileViewModel EmailAddress { get; set; }
	}

	public class MessageBody
	{
		public string ContentType { get; set; }
		public string Content { get; set; }
	}
}