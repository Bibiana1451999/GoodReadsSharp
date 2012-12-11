﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GoodReadsSharp.Models;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;
using RestSharp.Deserializers;

namespace GoodReadsSharp
{
    public partial class GoodReadsClient
    {
        private const string ApiBaseUrl = @"http://www.goodreads.com";

        private UserLogin _userLogin { get; set; }

        private readonly string _apiKey;
        private readonly string _appsecret;

        private RestClient _restClient;
        private RestClient _restClientContent;

        /// <summary>
        /// Default Constructor for the DropboxClient
        /// </summary>
        /// <param name="apiKey">The Api Key to use for the Dropbox Requests</param>
        /// <param name="appSecret">The Api Secret to use for the Dropbox Requests</param>
        public GoodReadsClient(string apiKey, string appSecret)
        {
            _apiKey = apiKey;
            _appsecret = appSecret;

            LoadClient();
        }

        /// <summary>
        /// Creates an instance of the DropNetClient given an API Key/Secret and a User Token/Secret
        /// </summary>
        /// <param name="apiKey">The Api Key to use for the Dropbox Requests</param>
        /// <param name="appSecret">The Api Secret to use for the Dropbox Requests</param>
        /// <param name="userToken">The User authentication token</param>
        /// <param name="userSecret">The Users matching secret</param>
        public GoodReadsClient(string apiKey, string appSecret, string userToken, string userSecret)
        {
            _apiKey = apiKey;
            _appsecret = appSecret;

            LoadClient();

            _userLogin = new UserLogin( userToken,  userSecret );
        }

        private void LoadClient()
        {
            
        }

        private IAuthenticator PublicMethods()
        {
            return null;
        }
        private IAuthenticator AuthMethods()
        {
            return OAuth1Authenticator.ForProtectedResource(_apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);
        }

    }
}
