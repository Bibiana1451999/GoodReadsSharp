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
                Console.WriteLine(@"[5] Add a Shelf");
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

                        if (isbn.ToLower() == "b")
                            goto Menu;

                        var book = Task.Run(() => client.BookIdForIsbn(isbn)).Result;
                        if(book.Title=="")
                            Console.WriteLine("There is no book with that ISBN, press Enter to go back!");
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine($"Title: {book.Title}");
                        Console.WriteLine();

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
                            Console.WriteLine($"Author: {book.Authors.First().Name}");
                        }

                        Console.WriteLine();
                        Console.WriteLine($"Description: {book.Description}");
                        Console.WriteLine();
                        Console.WriteLine($"Number of pages: {book.NumPages}");
                        Console.WriteLine(@"---------------------------");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine("Enter title:");
                        var title = Console.ReadLine();

                        if (title.ToLower() == "b")
                            goto Menu;

                        var bookList = Task.Run(() => client.SearchBook(title)).Result;
                        string[] arr = new string[25];

                        Console.WriteLine();

                        

                        int nm = 1;
                        foreach (var variable in bookList)
                        {
                            if (variable.ToLower().Contains(title.ToLower()))
                            {
                                Console.WriteLine(nm + "." + variable);
                                arr[nm] = variable;
                                nm++;
                                //   Console.WriteLine(bookList.ElementAt<Book>(nm).);
                            }
                        }

                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine("Choose the number of the book you want");
                        string numb = Console.ReadLine();
                        Console.Clear();
                        // Console.WriteLine(arr[Convert.ToInt16(numb)]);

                        var bookie = Task.Run(() => client.BookIdForTitle(arr[Convert.ToInt16(numb)])).Result;
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine($"Title: {bookie.Title}");
                        Console.WriteLine();

                        if (bookie.Authors.Count > 1)
                        {
                            var i = 1;
                            foreach (var variable in bookie.Authors)
                            {
                                Console.WriteLine($"{i}. Author: {variable.Name}");
                                i++;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Author: {bookie.Authors.First().Name}");
                        }

                        Console.WriteLine();
                        Console.WriteLine($"Description: {bookie.Description}");
                        Console.WriteLine();
                        Console.WriteLine($"Number of pages: {bookie.NumPages}");
                        Console.WriteLine(@"---------------------------");
                        
                        Console.ReadKey();
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine("Enter name:");
                        var name = Console.ReadLine();

                        if (name.ToLower() == "b")
                            goto Menu;

                        var authList = Task.Run(() => client.SearchAuthors(name)).Result;
                        string[] arr2 = new string[25];


                        Console.WriteLine();
                        int num = 1;
                        foreach (var variable in authList)
                        {
                            String input = new String(variable.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());
                            if (variable.ToLower().Contains(name.ToLower()))
                            {
                                Console.WriteLine(num + "." + input);
                                arr2[num] = input;
                                num++;
                            }
                        }

                        Console.ReadKey();
                        break;


                    case "5":

                        Console.Clear();
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine("Write the name of the shelf");
                        var shelfName = Console.ReadLine();

                        client.AddShelf(shelfName);
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine("Shelf created, go back and check all your shelves [6]");

                        Console.ReadKey();
                        goto Menu;
                                            
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