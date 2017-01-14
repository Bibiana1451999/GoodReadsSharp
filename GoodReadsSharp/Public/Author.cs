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
        public async Task<List<string>> SearchAuthors(string searchString)
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
                var authorList = responseAsXML.GetElementsByTagName("author");

                var resultList = new List<string>();

                foreach (XmlNode variable in authorList)
                {
                    if(!resultList.Contains(variable.InnerText))
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

       
        
    }
}
