using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMeExampleOAuthApplication.Models
{
    public static class OauthHeaderData
    {
        //your credentials for the consumer you are using in sandbox
        //do NOT store your credentials like this, they are only here for example purposes and yours should be stored securely
        //create your own application here https://www.tmsandbox.co.nz/MyTradeMe/Api/RegisterNewApplication.aspx and enter your consumer key and secret below        
        public static string ConsumerKey = string.Empty;
        public static string ConsumerSecret = string.Empty;
        //this callback must have the same domain as the one you registered with us when you created your application
        public static readonly string Callback = HttpUtility.UrlEncode("http://" + HttpContext.Current.Request.Url.Authority + "/OAuth/Callback");
        public const string OAuthVersion = "1.0";
        public const string OAuthNonce = "7O3kEe";
        public const string SignatureMethod = "PLAINTEXT";
    }
}