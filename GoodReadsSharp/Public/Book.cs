﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodReadsSharp.Models;
using GoodReadsSharp.ResponseModel;
using RestSharp;
using System.Threading.Tasks;

namespace GoodReadsSharp
{
    public partial class GoodReadsClient
    {
        //Not working.
        public async Task<Book> BookIdForIsbn(String isbn)
        {
            try
            {
                _restClient.BaseUrl = ApiBaseUrl;
                _restClient.Authenticator = PublicMethods();

                var request = new RestRequest("book/isbn", Method.GET);
                request.AddParameter("isbn", isbn);
                request.AddParameter("key", _apiKey);
                request.AddParameter("format", "xml");
                //_restClient.AddHandler();
                var response = _restClient.Execute<Book>(request);


                if (response.ResponseStatus == ResponseStatus.Error)
                {
                    return null;
                }
                else
                {
                    return response.Data;
                }

            }
            catch (Exception ex)
            {
                return null;

            }


        }



        /// <summary>
        /// Reviews the counts for isbns. Only populates the Id, Isbn and Isbn13 fields
        /// </summary>
        /// <param name="isbns">The isbns.</param>
        public async Task<ReviewCountsForIsbns> ReviewCountsForIsbns(List<String> isbns)
        {
            _restClient.BaseUrl = ApiBaseUrl;
            _restClient.Authenticator = PublicMethods();

            var request = new RestRequest("book/review_counts.json", Method.GET);
            request.AddParameter("format", "xml");
            request.AddParameter("key", _apiKey);
            request.AddParameter("isbns", String.Join(",", isbns));

            var response = _restClient.Execute<ReviewCountsForIsbns>(request);

            return response.Data;
        }

       // public async Task
    }
}
