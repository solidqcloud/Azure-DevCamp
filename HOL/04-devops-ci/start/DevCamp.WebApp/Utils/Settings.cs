using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace DevCamp.WebApp.Utils
{
    public class Settings
    {
		//####    HOL 2    ######
		public static string INCIDENT_API_URL = ConfigurationManager.AppSettings["INCIDENT_API_URL"];
		public static string AZURE_STORAGE_ACCOUNT = ConfigurationManager.AppSettings["AZURE_STORAGE_ACCOUNT"];
		public static string AZURE_STORAGE_KEY = ConfigurationManager.AppSettings["AZURE_STORAGE_ACCESS_KEY"];
		public static string AZURE_STORAGE_BLOB_CONTAINER = ConfigurationManager.AppSettings["AZURE_STORAGE_BLOB_CONTAINER"];
		public static string AZURE_STORAGE_QUEUE = ConfigurationManager.AppSettings["AZURE_STORAGE_QUEUE"];
		public static string AZURE_STORAGE_CONNECTIONSTRING = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", AZURE_STORAGE_ACCOUNT, AZURE_STORAGE_KEY);
		public static string REDISCCACHE_KEY_INCIDENTDATA = "incidentdata";

		public static string REDISCACHE_HOSTNAME = ConfigurationManager.AppSettings["REDISCACHE_HOSTNAME"];
		public static string REDISCACHE_PORT = ConfigurationManager.AppSettings["REDISCACHE_PORT"];
		public static string REDISCACHE_SSLPORT = ConfigurationManager.AppSettings["REDISCACHE_SSLPORT"];
		public static string REDISCACHE_PRIMARY_KEY = ConfigurationManager.AppSettings["REDISCACHE_PRIMARY_KEY"];
		public static string REDISCACHE_CONNECTIONSTRING = $"{REDISCACHE_HOSTNAME}:{REDISCACHE_SSLPORT},password={REDISCACHE_PRIMARY_KEY},abortConnect=false,ssl=true";
		//####    HOL 2   ######

		//####    HOL 3    ######
		public static string AAD_APP_ID = ConfigurationManager.AppSettings["AAD_APP_ID"];
		public static string AAD_INSTANCE = ConfigurationManager.AppSettings["AAD_INSTANCE"];
		public static string AAD_APP_REDIRECTURI = ConfigurationManager.AppSettings["AAD_APP_REDIRECTURI"];
		public static string AAD_TENANTID_CLAIMTYPE = "http://schemas.microsoft.com/identity/claims/tenantid";
		public static string AAD_OBJECTID_CLAIMTYPE = "http://schemas.microsoft.com/identity/claims/objectidentifier";
		public static string AAD_AUTHORITY = ConfigurationManager.AppSettings["AAD_AUTHORITY"];
		public static string AAD_LOGOUT_AUTHORITY = ConfigurationManager.AppSettings["AAD_LOGOUT_AUTHORITY"];
		public static string GRAPH_API_URL = ConfigurationManager.AppSettings["GRAPH_API_URL"];
		public static string AAD_APP_SECRET = ConfigurationManager.AppSettings["AAD_APP_SECRET"];
		public static string AAD_GRAPH_SCOPES = ConfigurationManager.AppSettings["AAD_GRAPH_SCOPES"];
		public static string GRAPH_CURRENT_USER_URL = GRAPH_API_URL + "/v1.0/me";
		public static string GRAPH_SENDMESSAGE_URL = GRAPH_CURRENT_USER_URL + "/sendMail";
		public static string SESSIONKEY_ACCESSTOKEN = "accesstoken";
		public static string SESSIONKEY_USERINFO = "userinfo";
		//####    HOL 3    ######

		public static string EMAIL_MESSAGE_BODY = getEmailMessageBody();
		public static string EMAIL_MESSAGE_SUBJECT = "New Incident Reported";
		public static string EMAIL_MESSAGE_TYPE = "HTML";

		static string getEmailMessageBody()
		{
			StringBuilder emailContent = new StringBuilder();
			emailContent.Append(@"<html><head><meta http-equiv='Content-Type' content='text/html; charset=us-ascii\'>");
			emailContent.Append(@"<title></title>");
			emailContent.Append(@"</head>");
			emailContent.Append(@"<body style='font-family:Calibri' > ");
			emailContent.Append(@"<div style='width:50%;background-color:#CCC;padding:10px;margin:0 auto;text-align:center;'> ");
			emailContent.Append(@"<h1>City Power &amp; Light</h1> ");
			emailContent.Append(@"<h2>New Incident was reported by {0} {1}</h2> ");
			emailContent.Append(@"<p>A new incident has been reported to the City Power &amp; Light outage system.</p> ");
			emailContent.Append(@"<br /> ");
			emailContent.Append(@"</div> ");
			emailContent.Append(@"</body> ");
			emailContent.Append(@"</html>");
			return emailContent.ToString();
		}
	}
}