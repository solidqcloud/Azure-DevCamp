using System;
using System.Collections.Generic;

namespace DevCamp.WebApp.Models
{
	public class Body
	{
		public string contentType { get; set; }
		public string content { get; set; }
	}

	public class EmailAddress
	{
		public string name { get; set; }
		public string address { get; set; }
	}

	public class Sender
	{
		public Sender()
		{
			emailAddress = new EmailAddress();
		}
		public EmailAddress emailAddress { get; set; }
	}

	public class From
	{
		public From()
		{
			emailAddress = new EmailAddress();
		}
		public EmailAddress emailAddress { get; set; }
	}

	public class ToRecipient
	{
		public ToRecipient()
		{
			emailAddress = new EmailAddress();
		}
		public EmailAddress emailAddress { get; set; }
	}

	public class Message
	{
		public Message()
		{
			sender = new Sender();
			body = new Body();
			from = new From();
			toRecipients = new List<ToRecipient>();
		}
		public string subject { get; set; }
		public Body body { get; set; }
		public Sender sender { get; set; }
		public From from { get; set; }
		public List<ToRecipient> toRecipients { get; set; }
	}
	public class EmailMessage
	{
		public EmailMessage()
		{
			Message = new Message();
		}
		public Message Message { get; set; }
		public bool SaveToSentItems { get; set; }
	}
}