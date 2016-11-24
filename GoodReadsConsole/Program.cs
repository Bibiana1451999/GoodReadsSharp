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
            try
            {
                Console.Title = "GoodReads client";
                DoStuff();
            }
            catch (Exception)
            {
                Console.Clear();
                Console.WriteLine("There was an error! Press any key to exit");
                Console.ReadKey();
                Environment.Exit(-1);
            }
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
                Console.WriteLine(@"[6] Show your acoount information");
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
                        Console.WriteLine("\nPress any key to return");
                        Console.WriteLine(@"---------------------------");
                        Console.ReadKey();
                        break;
                    case "6":

                        Console.Clear();
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine($"Shelves of {accInfo.User.Name}\n");
                        var shelf = new Shelf();
                        var booksOnShelfs = new ShelfReviewList();

                        var count = 1;
                        var dict = new Dictionary<int, UserShelf>();
                        foreach (var item in client.ListShelves().Shelves)
                        {
                            Console.WriteLine($"[{count}] {item.Name}");
                            dict.Add(count, item);
                            count++;                   
                        }
                        
                        Console.WriteLine("[B] Go back");
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine("Enter option:");

                        var option = Console.ReadLine();

                        if (option.ToLower() == "b")
                        {
                            goto Menu;
                        }

                        if (!dict.ContainsKey(int.Parse(option)))
                        {
                            goto error;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(@"---------------------------");
                            Console.WriteLine($"Books in shelf {dict[int.Parse(option)].Name}\n");

                            var booksOnShelf = Task.Run(()=>client.ListBooksOnSpecificShelf(dict[int.Parse(option)].Name)).Result;

                            foreach(var item in booksOnShelf)
                            {
                                Console.WriteLine(item);
                            }

                            Console.WriteLine("\nPress any key to return");
                            Console.WriteLine(@"---------------------------");
                        }

                        Console.ReadKey();
                        
                        break;
                    default:
                        error:
                        Console.WriteLine("\nInvalid input! Retry!");
                        Console.ReadKey();
                        goto Menu;
                }
            }
        }
    }
}