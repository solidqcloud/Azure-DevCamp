using DevCamp.WebApp.App_Start;
using DevCamp.WebApp.Utils;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Globalization;
using System.IdentityModel.Claims;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Web;
using ADAL = Microsoft.IdentityModel.Clients.ActiveDirectory;

[assembly: OwinStartup(typeof(Startup))]
namespace DevCamp.WebApp.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
            new OpenIdConnectAuthenticationOptions
            {
                // The `Authority` represents the auth endpoint - https://login.microsoftonline.com/common/
                // The 'ResponseType' indicates that we want an authorization code and an ID token 
                // In a real application you could use issuer validation for additional checks, like making 
                // sure the user's organization has signed up for your app, for instance.
                ClientId = Settings.AAD_APP_ID,
                Authority = string.Format(CultureInfo.InvariantCulture, Settings.AAD_INSTANCE, "common", ""),
                ResponseType = "code id_token",
                PostLogoutRedirectUri = "/",
                Scope = Settings.AAD_GRAPH_SCOPES,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                },
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    //Set up handlers for the events
                    AuthorizationCodeReceived = OnAuthorizationCodeReceived,
                    AuthenticationFailed = OnAuthenticationFailed
                }
            }
            );
        }


        /// <summary>
        /// Fired when the user authenticates
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification notification)
        {
            // Get the user's object id (used to name the token cache)
            string userObjId = notification.AuthenticationTicket.Identity.FindFirst(Settings.AAD_OBJECTID_CLAIMTYPE).Value;

            // Create a token cache
            HttpContextBase httpContext = notification.OwinContext.Get<HttpContextBase>(typeof(HttpContextBase).FullName);
            SessionTokenCache tokenCache = new SessionTokenCache(userObjId, httpContext);

            // Exchange the auth code for a token
            ADAL.ClientCredential clientCred = new ADAL.ClientCredential(Settings.AAD_APP_ID, Settings.AAD_APP_SECRET);

            // Create the auth context
            ADAL.AuthenticationContext authContext = new ADAL.AuthenticationContext(
              string.Format(CultureInfo.InvariantCulture, Settings.AAD_INSTANCE, "common", ""), false, tokenCache);

            ADAL.AuthenticationResult authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
              notification.Code, notification.Request.Uri, clientCred, Settings.GRAPH_API_URL);
        }

        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            notification.HandleResponse();
            notification.Response.Redirect("/Error?message=" + notification.Exception.Message);
            return Task.FromResult(0);
        }
    }
}