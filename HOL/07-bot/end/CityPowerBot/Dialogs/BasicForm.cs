using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
#pragma warning disable 649

namespace CityPowerBot
{
	public enum IncidentTypes { GasLeak = 1, StreetLightStaysOn };

	// For more information about this template visit http://aka.ms/azurebots-csharp-form
	[Serializable]
	public class BasicForm
	{
		[Prompt("What is your {&}?")]
		public string FirstName { get; set; }

		[Prompt("And your {&}?")]
		public string LastName { get; set; }

		[Prompt("What type of outage would you like to report? {||}")]
		public IncidentTypes IncidentType { get; set; }

		[Prompt("Is this issue an {&}? {||}")]
		public bool Emergency { get; set; }

		[Prompt("Please give a {&} of the problem.")]
		public string Description { get; set; }

		[Pattern(@"(<Undefined control sequence>\d)?\s*\d{3}(-|\s*)\d{4}")]
		[Prompt("What is the {&} where we can currently reach you?")]
		public string PhoneNumber { get; set; }

		[Prompt("In which {&} do you live?")]
		public string City { get; set; }

		[Prompt("And in which {&}?")]
		public string State { get; set; }

		[Prompt("Lastly, what {&} do you live on?")]
		public string Street { get; set; }

		[Pattern(@"^\d{5}(?:[-\s]\d{4})?$")]
		[Prompt("What is your {&}?")]
		public string ZipCode { get; set; }

		public static IForm<BasicForm> BuildForm()
		{
			OnCompletionAsyncDelegate<BasicForm> processReport = async (context, state) =>
			{
				await context.PostAsync("We are currently processing your report. We will message you the status.");
				if (await DataWriter.IncidentController.CreateAsync(state.FirstName, state.LastName, state.Street, state.City, state.State, state.ZipCode, state.PhoneNumber, state.Description + (String.IsNullOrWhiteSpace(MessagesController.LastImageDescription) ? String.Empty : (Environment.NewLine + MessagesController.LastImageDescription)), state.IncidentType.ToString(), state.Emergency, MessagesController.LastImage, MessagesController.LastImageName, MessagesController.LastImageType, MessagesController.LastImageTags))
				{
					await context.PostAsync("The incident report has been logged.");
				}
				else
				{
					await context.PostAsync("An error occured logging the incident.");
				}
			};

			// Builds an IForm<T> based on BasicForm
			return new FormBuilder<BasicForm>()
				.Message("I am the City Power Bot! You can file a new incident report with me :-)")
				.Message("Did you know? At any point during our conversation you can send me an image that I will attach to your report.")
				.Field(nameof(FirstName))
				.Field(nameof(LastName))
				.Message("Hello {FirstName} {LastName}! Let's file your report!")
				.Field(nameof(Emergency))
				.Field(nameof(IncidentType))
				.Field(nameof(Description))
				.Field(nameof(City))
				.Field(nameof(State))
				.Field(nameof(ZipCode))
				.Field(nameof(Street))
				.Field(nameof(PhoneNumber))
				.Confirm(async (state) =>
				{
					return new PromptAttribute($"OK, we have got all your data. Would you like to send your incident report now?");
				})
				.OnCompletion(processReport)
				.Build();
		}

		public static IFormDialog<BasicForm> BuildFormDialog(FormOptions options = FormOptions.PromptInStart)
		{
			// Generated a new FormDialog<T> based on IForm<BasicForm>
			return FormDialog.FromForm(BuildForm, options);
		}
	}
}