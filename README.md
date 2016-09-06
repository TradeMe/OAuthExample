# Trade Me example OAuth application

This README outlines what you need to do to get this application up and running.

This project is an example project to show how you can implement Trade Me's OAuth flow using an oauth_callback. You will need to have HTTPS available

## Prerequisites

* [.Net framework for windows](https://www.microsoft.com/en-us/download/details.aspx?id=48130)
* [OR .Net for Mac] (http://docs.asp.net/en/latest/getting-started/installing-on-mac.html)
* [Visual Studio](https://www.visualstudio.com/en-us/downloads/download-visual-studio-vs.aspx)
* [Consumer keys for Trade Me Sandbox](https://www.tmsandbox.co.nz/MyTradeMe/Api/RegisterNewApplication.aspx) you should set your application's default callback to the localhost port _ "/oauth/Callback" e.g
https://localhost:51172/oauth/Callback (you will need to generate a self signed certificate to enable HTTPS as well, check out google for this)


## Installation

* `git clone` this repository
* `cd TradeMeExampleOAuthApplication`
* Open the solution in Visual Studio

## Running

* In OauthHeaderData.cs enter your consumer key and secret in the code as the values of 'ConsumerKey' and 'ConsumerSecret'
* Enter the default callback you registered as the value of '_callback' (this must be the same as the one you entered on registration of your application)
* Run the project from Visual Studio and follow the instructions on the homepage of the app

## Further Reading / Useful Links

* [Example plaintext OAuth workflow](http://developer.trademe.co.nz/api-overview/authentication/example-plaintext-workflow/)
* [Rate Limiting at Trade Me](http://developer.trademe.co.nz/api-overview/rate-limiting/)

