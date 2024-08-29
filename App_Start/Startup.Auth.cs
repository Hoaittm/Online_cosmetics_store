using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;

[assembly: OwinStartup(typeof(TranThiMinhHoai_2122110262.Startup))]

namespace TranThiMinhHoai_2122110262
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure the application for Cookie-based authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/User/Login"),
            });

            // Use external sign-in cookie
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable login with third-party login providers

            // Google authentication
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "295245461172-tmuo5a3mkgfc5e81staj3dg4jrhb6f3o.apps.googleusercontent.com",
                ClientSecret = "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",
 
            }) ;

            // Facebook authentication
            // app.UseFacebookAuthentication(
            //    appId: "YOUR_FACEBOOK_APP_ID",
            //    appSecret: "YOUR_FACEBOOK_APP_SECRET");

            // Twitter authentication
            // app.UseTwitterAuthentication(
            //    consumerKey: "YOUR_TWITTER_CONSUMER_KEY",
            //    consumerSecret: "YOUR_TWITTER_CONSUMER_SECRET");

            // LinkedIn authentication
            // app.UseLinkedInAuthentication(
            //    clientId: "YOUR_LINKEDIN_CLIENT_ID",
            //    clientSecret: "YOUR_LINKEDIN_CLIENT_SECRET");
        }
    }
}
