using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using swishes.Models;

namespace swishes
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            OAuthWebSecurity.RegisterTwitterClient(
                consumerKey: "tXFGPQFvHgxTH2OetnaMvg",
                consumerSecret: "c0VxXdaQOGMKTYGWTptMvVPeyxuUcpQIUEON8HSo");

            OAuthWebSecurity.RegisterFacebookClient(
                appId: "193648527455277",
                appSecret: "9c056ef30a45c8d4c184d7d54c95d7b9");

            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
