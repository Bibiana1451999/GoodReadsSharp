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
            var key = System.Configuration.ConfigurationManager.AppSettings["key"];         // enter your API Key here
            var secret = System.Configuration.ConfigurationManager.AppSettings["secret"];   // enter your API secret here
            var client = new GoodReadsClient(key, secret);                                  // creating a new Client object usig key and secret
            var login = client.GetTokenAndBuildUrl();                                       // generates the url where the user authorizes the program
            Process.Start(login);                                                           // opens the link

            Console.WriteLine("Please hit Enter when you have accepted the request");
            Console.Read();
            var a = client.GetAccessToken();                                            // gets the access token for the current user
            client = new GoodReadsClient(key, secret, a.Token, a.Secret);               // updates the client object
            var accInfo = client.AccountInfo();                                         // Account object that where u can see information

            while (true)
            {
                Menu:
                Console.Clear();
                Console.WriteLine(@"---------------------------");
                Console.WriteLine(@"[1] Lookup ISBN");
                Console.WriteLine(@"[2] Search book");
                Console.WriteLine(@"[3] Search author");
                Console.WriteLine(@"[4] Show top 10");
                Console.WriteLine(@"[5] Show your shelves");
                Console.WriteLine(@"[Q] Quit");
                Console.WriteLine(@"---------------------------");
                var result = Console.ReadLine();

                if (result == "")
                {
                    goto Menu;
                }

                if (result.ToLower() == "q")
                {
                    Environment.Exit(0);
                }

                switch (result)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("Enter ISBN:");
                        var isbn = Console.ReadLine();
                        var book = Task.Run(() => client.BookIdForIsbn(isbn)).Result;
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine($"Title: {book.Title}");

                        if (book.Authors.Count > 1)
                        {
                            var i = 1;
                            foreach (var variable in book.Authors)
                            {
                                Console.WriteLine($"{i}. Author: {variable.Name}");
                                i++;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Author: {book.Authors}");
                        }

                        Console.WriteLine($"Description: {book.Description}");
                        Console.WriteLine(@"---------------------------");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine("Enter title:");
                        var title = Console.ReadLine();
                        var bookList = Task.Run(() => client.SearchBook(title)).Result;

                        Console.WriteLine();
                        foreach (var variable in bookList)
                        {
                            Console.WriteLine(variable);
                        }
                        Console.WriteLine(@"---------------------------");
                        Console.ReadKey();
                        break;

                    default:
                        Console.WriteLine("\nInvalid input! Retry!");
                        Console.ReadKey();
                        goto Menu;
                }
            }
        }
    }
}