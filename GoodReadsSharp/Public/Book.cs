using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodReadsSharp.Models;
using GoodReadsSharp.ResponseModel;
using RestSharp;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace GoodReadsSharp
{
    public partial class GoodReadsClient
    {
        //Not working.
        public async Task<Book> BookIdForIsbn(string isbn)
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

        public async Task<List<string>> SearchBook(string searchString)
        {
            try
            {
                _restClient.BaseUrl = ApiBaseUrl;
                _restClient.Authenticator = PublicMethods();

                var request = new RestRequest("/search/index.xml", Method.GET);
                request.AddParameter("q", searchString);
                request.AddParameter("key", _apiKey);
                request.AddParameter("format", "xml");
                //_restClient.AddHandler();
                var response = _restClient.Execute(request);
                var responseAsXML = new XmlDocument();
                responseAsXML.Load(GenerateStreamFromString(response.Content));
                var authorList = responseAsXML.GetElementsByTagName("title");

                var resultList = new List<string>();
                
                foreach (XmlNode variable in authorList)
                {
                    if (!resultList.Contains(variable.InnerText))
                        resultList.Add(variable.InnerText);
                }


                if (response.ResponseStatus == ResponseStatus.Error)
                {
                    return null;
                }
                else
                {
                    return resultList;
                }

            }
            catch (Exception ex)
            {
                return null;

            }

        }

        

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
