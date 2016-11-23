using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using GoodReadsSharp;
using System.Windows.Forms;
using GoodReadsSharp.Models;

namespace GoodReadsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            DoStuff();

        }

        public static void DoStuff()
        {
            var key = "eCFjwXhTpBagasFvhf6Sg";                                          // enter your API Key here
            var secret = "lwqcOb8oTkkFE02nmRVWT2eSZeI1GsdAjQQK0Enuibc";                 // enter your API secret here
            var client = new GoodReadsClient(key, secret);                              // creating a new Client object usig key and secret
            var login = client.GetTokenAndBuildUrl();                                   // generates the url where the user authorizes the program
            Process.Start(login);                                                       // opens the link

            Console.WriteLine("Please hit Enter when you have accepted the request");
            Console.Read();
            var a = client.GetAccessToken();                                            // gets the access token for the current user
            client = new GoodReadsClient(key, secret, a.Token, a.Secret);               // updates the client object
            var accInfo = client.AccountInfo();                                         // Account object that where u can see information
            Console.WriteLine("Name=" + accInfo.User.Name);
            Console.WriteLine("Id=" + accInfo.User.Id);
            Console.WriteLine("Link=" + accInfo.User.Link);

            Console.WriteLine();
            Console.WriteLine("Your shelves:");
            Console.WriteLine();
            var shelves = client.ListShelves();                                        // get all shelves created by the user
            foreach (var item in shelves.Shelves)
            {
                Console.WriteLine("Shelf#" + shelves.Shelves.IndexOf(item).ToString() + ":");
                Console.WriteLine("Name=" + item.Name);
                Console.WriteLine("Count=" + item.BookCount);
                Console.WriteLine("Description=" + item.Description);
                Console.WriteLine();
            }

            var book = Task.Run(() => client.BookIdForIsbn("9788580572254")).Result;
            Console.WriteLine(book.Title);
            Console.WriteLine();
            Console.WriteLine("Listing books on shelf 1");
            Console.WriteLine();
            var shelve = client.ListOwnedBooks();                                      // get all books owned by the user

            foreach (var item in shelve.OwnedBooks)
            {
                Console.WriteLine("Title=" + item.Book.Title);
                Console.WriteLine("Author=" + item.Book.Authors[0].Name);

            }




            Console.Read();
            Console.ReadKey();
        }
    }
}