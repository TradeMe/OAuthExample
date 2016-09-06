using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using TradeMeExampleOAuthApplication.Helpers;

namespace TradeMeExampleOAuthApplication.Controllers
{
    public class OAuthController : Controller
    {
        //don't forget to add your credentials for the consumer you are using from sandbox to the OauthHeaderData.cs model
        
        private const string TradeMeOAuthApiUrl = "https://secure.tmsandbox.co.nz/Oauth/";
        private const string MyTradeMeSummaryUrl = "https://api.tmsandbox.co.nz/v1/MyTradeMe/Summary.json";
        
        private readonly Regex _oauthRequestTokenRegex = new Regex("oauth_token=(\\w+)&oauth_token_secret=(\\w+)&oauth_callback_confirmed=(\\w+)");
        private readonly Regex _oauthAccessTokenRegex = new Regex("oauth_token=(\\w+)&oauth_token_secret=(\\w+)");

        private const string RequestTokenKeyName = "RequestToken";
        private const string RequestTokenSecretKeyName = "RequestTokenSecret";
        private const string AccessTokenKeyName = "AccessToken";
        private const string AccessTokenSecretKeyName = "AccessTokenSecret";

        // GET: /OAuth/GetRequestToken
        //this is the first step. Before the page loads we make a request to TradeMe to get some request tokens, these are temporary.
        //You will use the request token to send to Trade Me when the user is asked to login to their Trade Me account, this is so
        //that you know who has approved access to your application
        [AllowAnonymous]
        public ActionResult GetRequestToken()
        {
            //set up the URL we will bhe using to get the request tokens, in this case we want to be able to read and write to the users account
            var url = string.Format("{0}RequestToken?scope=MyTradeMeRead,MyTradeMeWrite", TradeMeOAuthApiUrl);

            var authHeader = OauthHelper.GetBaseOAuthHeader();

            //make the request to get the request tokens and store the result in responseText
            //responseText looks like this:
            //oauth_token=F4B999BE86A4EA64CB82E7F6A1ED151E&oauth_token_secret=13791AEA7D1592067B2AA9F549DA82C8&oauth_callback_confirmed=true
            string responseText;
            try
            {
                responseText = OauthHelper.MakeHttpRequest("POST", new Uri(url), authHeader);

            }
            catch (WebException e)
            {
               
                ViewBag.ErrorMessage = e.Message;
                var inner = (WebException)e.InnerException;
                var statusCode = (int)((HttpWebResponse) inner.Response).StatusCode;
                ViewBag.HelpMessage =
                    "Make sure you have entered valid credentials for consumer key and consumer secret";
                ViewBag.StatusCode = statusCode;
                ViewBag.HasError = true;
                return View();

            }

            //get the values out using regex
            var matches = _oauthRequestTokenRegex.Match(responseText);
            
            //store the token and token secret in the session for access later on
            Session[RequestTokenKeyName] = matches.Groups[1].ToString();
            Session[RequestTokenSecretKeyName] = matches.Groups[2].ToString();

            //set this Url on the viewbag so that the user can click this link when the page loads
            //this URL will send the user back to Trade Me and ask them to log in and give permission to your application if they haven't done so before
            //if they already have given your application permission then they will just need to login
            ViewBag.AuthorizeUrl = string.Format("https://secure.tmsandbox.co.nz/Oauth/Authorize?oauth_token={0}", Session[RequestTokenKeyName]);
            ViewBag.RequestToken = Session[RequestTokenKeyName];
            ViewBag.RequestTokenSecret = Session[RequestTokenSecretKeyName];
            return View();
        }


        // GET: /oauth/Callback
        //this is the callback we set up upon registration of our application and was used in step one as the oauth_callback,
        //this URL is hit when Trade Me redirects the user back to this application after they have approved access or logged in
        //the query string will contain the oauth token (same token as the one above) and a verifier to say the user has given access to their account
        [AllowAnonymous]
        public ActionResult Callback(string oauth_token, string oauth_verifier)
        {
            //this just makes sure if we hit this url without the token, verifier or having a current request token we return an error
            if (oauth_token.IsNullOrWhiteSpace() || oauth_verifier.IsNullOrWhiteSpace() || Session[RequestTokenKeyName] == null || Session[RequestTokenSecretKeyName] == null)
            {
                ViewBag.Error =
                    "Are you sure you meant to come to this URL? You do not have any request tokens. You should start by <a href='./GetRequestToken'>requesting some request token</a>";
                return View();
            }
            var url = string.Format("{0}AccessToken", TradeMeOAuthApiUrl);

            //still use the base header, but add the token secret to the existing signature "<consumer_key>&" + "<oauth_token_secret>"
            //the Session["RequestTokenSecret"] is the oauth_token_secret and the base header already ends in "&"
            var authHeader = string.Format(
                "{0}{1}, oauth_verifier={2}, oauth_token={3}",
                OauthHelper.GetBaseOAuthHeader(), Session[RequestTokenSecretKeyName], oauth_verifier, Session[RequestTokenKeyName]);

            //make the last request to get our ACCESS tokens, these are permenant and can be used to authorize as the user on the API
            var responseText = OauthHelper.MakeHttpRequest("POST", new Uri(url), authHeader);

            var matches = _oauthAccessTokenRegex.Match(responseText);

            //save our final tokens
            Session[AccessTokenKeyName] = matches.Groups[1].ToString();
            Session[AccessTokenSecretKeyName] = matches.Groups[2].ToString();
            ViewBag.AccessToken = Session[AccessTokenKeyName];
            ViewBag.AccessTokenSecret = Session[AccessTokenSecretKeyName];
            return View();
        }

        // GET: /oauth/MyTradeMeSummary
        //this is the first authenticated request we make as the user after finishing the OAuth flow
        [AllowAnonymous]
        public ActionResult MyTradeMeSummary()
        {
            if (Session[AccessTokenKeyName] == null || Session[AccessTokenSecretKeyName] == null)
            {
                ViewBag.Error =
                    "Are you sure you meant to come to this URL? You do not have any access tokens. You should start by <a href='./GetRequestToken'>requesting some request token</a>";
                return View();
            }
            //now we can just use the base header along with the (access) oauth token and secret
            var authHeader = string.Format("{0}{1}, oauth_token={2}", OauthHelper.GetBaseOAuthHeader(), Session[AccessTokenSecretKeyName], Session[AccessTokenKeyName]);
            var response = OauthHelper.MakeHttpRequest("GET", new Uri(MyTradeMeSummaryUrl), authHeader);
            var json = JsonConvert.DeserializeObject(response);
            var formattedResponse = JsonConvert.SerializeObject(json, Formatting.Indented);
            ViewBag.Response = formattedResponse;
            return View();
        }
    }
}