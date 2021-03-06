﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodReadsSharp.Models;
using GoodReadsSharp.ResponseModel;
using RestSharp;

namespace GoodReadsSharp
{
    public partial class GoodReadsClient
    {

        /// <summary>
        /// Lists shelves for a user 
        /// </summary>
        /// <param name="page">The page.</param>
        public ListShelves ListShelves(string userid)
        {

            var request = new RestRequest("shelf/list.xml", Method.GET);
            request.AddParameter("key", _apiKey);
            request.AddParameter("user_id", userid);
            var response = _restClient.Execute<ListShelves>(request);
            return response.Data;
        }

        public void AddShelf(String shelfName)
        {

            var request = new RestRequest("user_shelves.xml", Method.POST);
            request.AddParameter("user_shelf[name]", shelfName);


            var response = _restClient.Execute(request);
            return;
        }
        public void DeleteShelf(String shelfId)
        {

            var request = new RestRequest(String.Format("user_shelves/destroy.xml?id={0}", shelfId), Method.DELETE);


            var response = _restClient.Execute(request);
            return;
        }


        /// <summary>
        /// Add a book to a shelf using OAuth. This method can also be used to remove from shelf. You'll need to register your app (required). 
        /// </summary>
        public Shelf AddBookToShelf(String shelfName, String bookId)
        {

            var request = new RestRequest("shelf/add_to_shelf.xml", Method.POST);
            request.AddParameter("name", shelfName);
            request.AddParameter("book_id", bookId);

            var response = _restClient.Execute<Shelf>(request);
            return response.Data;
        }

        public Hash RemoveBookFromShelf(String shelfName, String bookId)
        {
            shelfName = shelfName.ToLower();

            var request = new RestRequest("shelf/add_to_shelf.xml", Method.POST);
            request.AddParameter("name", shelfName);
            request.AddParameter("book_id", bookId);
            request.AddParameter("a", "remove");


            var response = _restClient.Execute<Hash>(request);
            return response.Data;
        }

        /// <summary>
        /// Lists the books on shelf.
        /// </summary>
        /// <param name="shelfName">Name of the shelf.</param>
        /// <param name="page">The page.</param>
        public List<Book> ListBooksOnShelf(String shelfName, Int32 page = 1)
        {
            //Unfortunately the review/list api is case sensitive even though none of the other shelf api's are. Just to keep it consistent we've called toLower on all other shelfName parameters. 

            var request = new RestRequest("shelf/list.xml", Method.GET);
            request.AddParameter("v", "2");
            request.AddParameter("id", _userLogin.Id);
            request.AddParameter("shelf", shelfName);
            request.AddParameter("per_page", 40); //TODO: Hard coded 40 books per page. 

            if(page>1)
            {
                request.AddParameter("page", page);
            }

            var response = _restClient.Execute<Book>(request);
            var listOfBooks = new List<Book>();
            listOfBooks.Add(response.Data);
            return listOfBooks;
        }

        

    }
}
