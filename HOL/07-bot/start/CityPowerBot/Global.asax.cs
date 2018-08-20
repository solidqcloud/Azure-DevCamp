using System;
using Autofac;
using System.Web.Http;
using System.Configuration;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;

namespace CityPowerBot
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			var uri = new Uri(ConfigurationManager.AppSettings["DocumentDbUrl"]);
			var key = ConfigurationManager.AppSettings["DocumentDbKey"];
			var store = new DocumentDbBotDataStore(uri, key);

			Conversation.UpdateContainer(
						builder =>
						{
							builder.Register(c => store)
								.Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
								.AsSelf()
								.SingleInstance();

							builder.Register(c => new CachingBotDataStore(store, CachingBotDataStoreConsistencyPolicy.ETagBasedConsistency))
								.As<IBotDataStore<BotData>>()
								.AsSelf()
								.InstancePerLifetimeScope();

						});

			GlobalConfiguration.Configure(WebApiConfig.Register);
		}
	}
}
