using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using GoodReadsSharp;
using System.Windows.Forms;
using System.Xml.Linq;
using GoodReadsSharp.Models;
using System.IO;

namespace GoodReadsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                try
                {
                    Console.Title = "GoodReads client";
                    DoStuff();
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine("There was an error! Press any key to exit");
                    Console.WriteLine(ex.StackTrace);
                    Console.ReadKey();
                    Environment.Exit(-1);
                }
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
                Console.WriteLine(@"[4] Add a Shelf");
                Console.WriteLine(@"[5] List shelves");
                Console.WriteLine(@"[6] Show your account information");
                Console.WriteLine(@"[S] Save");
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

                        if (isbn != null && isbn.ToLower() == "b")
                            goto Menu;

                        var book = Task.Run(() => client.BookIdForIsbn(isbn)).Result;
                        if (book.Title == "")
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

                        if (title != null && title.ToLower() == "b")
                            goto Menu;

                        var bookList = Task.Run(() => client.SearchBook(title)).Result;
                        var arr = new string[25];

                        Console.WriteLine();



                        var nm = 1;
                        foreach (var variable in bookList)
                        {
                            if (title != null && !variable.ToLower().Contains(title.ToLower())) continue;
                            Console.WriteLine(nm + "." + variable);
                            arr[nm] = variable;
                            nm++;
                            //   Console.WriteLine(bookList.ElementAt<Book>(nm).);
                        }

                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine("Choose the number of the book you want");
                        var numb = Console.ReadLine();
                        Console.Clear();
                        // Console.WriteLine(arr[Convert.ToInt16(numb)]);
                        Book bookie = null;
                        if (numb != null && !(int.Parse(numb) >= arr.Length))
                        {
                            bookie = Task.Run(() => client.BookIdForTitle(arr[Convert.ToInt16(numb)])).Result;
                        }
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
                        Console.WriteLine("[O] open");
                        Console.WriteLine(@"---------------------------");
                        var open = Console.ReadLine();
                        if (open != null && open.ToLower() == "o")
                        {
                            System.Diagnostics.Process.Start(bookie.Link.ToString());

                        }

                        Console.ReadKey();
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine("Enter name:");
                        var name = Console.ReadLine();

                        if (name != null && name.ToLower() == "b")
                            goto Menu;

                        var authList = Task.Run(() => client.SearchAuthors(name)).Result;
                        var arr2 = new string[25];


                        Console.WriteLine();
                        var num = 1;
                        foreach (var variable in authList)
                        {
                            var input = new string(variable.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());
                            if (name != null && !variable.ToLower().Contains(name.ToLower())) continue;
                            Console.WriteLine(num + "." + input);
                            arr2[num] = input;
                            num++;
                        }

                        Console.ReadKey();
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine("Write the name of the shelf");
                        var shelfName = Console.ReadLine();

                        client.AddShelf(shelfName);
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine("Shelf successfully created.");

                        Console.ReadKey();
                        goto Menu;

                    case "5":
                        Console.Clear();
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine($"Shelves of {accInfo.User.Name}\n");
                        var shelf = new Shelf();
                        var booksOnShelfs = new ShelfReviewList();

                        var count = 1;
                        var dict = new Dictionary<int, UserShelf>();
                        foreach (var item in client.ListShelves(accInfo.User.Id).Shelves)
                        {
                            Console.WriteLine($"[{count}] {item.Name}");
                            dict.Add(count, item);
                            count++;
                        }

                        Console.WriteLine("[B] Go back");
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine("Enter option:");

                        var option = Console.ReadLine();

                        if (option != null && option.ToLower() == "b")
                        {
                            goto Menu;
                        }

                        if (option != null && !dict.ContainsKey(int.Parse(option)))
                        {
                            goto error;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(@"---------------------------");
                            if (option != null)
                            {
                                Console.WriteLine($"Books in shelf {dict[int.Parse(option)].Name}\n");

                                var booksOnShelf = Task.Run(() => client.ListBooksOnSpecificShelf(dict[int.Parse(option)].Name, accInfo.User.Id)).Result;

                                foreach (var item in booksOnShelf)
                                {
                                    Console.WriteLine(item);
                                }
                            }

                            Console.WriteLine("\nPress any key to return");
                            Console.WriteLine(@"---------------------------");
                        }

                        Console.ReadKey();
                        goto Menu;


                    case "6":
                        var auth = client.AccountInfo().User;
                        Console.Clear();
                        Console.WriteLine(@"---------------------------");
                        Console.WriteLine("Name: " + auth.Name);
                        Console.WriteLine("Link: " + auth.Link);
                        Console.WriteLine("Id: " + auth.Id);
                        Console.WriteLine("[O] Open your account in browser");
                        Console.WriteLine("Press any key to return");
                        Console.WriteLine(@"---------------------------");
                        // var b = Console.ReadLine();
                        var o = Console.ReadLine();
                        if (o != null && o.ToLower() == "o")
                        {
                            System.Diagnostics.Process.Start(auth.Link.ToString());

                        }

                        goto Menu;




                    case "s":
                    case "S":

                        Console.Clear();
                        var xml = new XElement("GoodReadsUser",
                            new XElement("User",
                            new XAttribute("Name", client.AccountInfo().User.Name),
                            new XAttribute("Id", client.AccountInfo().User.Id),
                            new XAttribute("Link", client.AccountInfo().User.Link)),
                            new XElement("Shelves",
                            from shelve in client.ListShelves(accInfo.User.Id).Shelves
                            select
                            new XElement("Shelve",
                            new XAttribute("Name", shelve.Name),
                            new XAttribute("BookCount", shelve.BookCount),
                            new XAttribute("Description", shelve.Description),
                            new XElement("Books", from bookName in client.ListBooksOnSpecificShelf(shelve.Name, client.AccountInfo().User.Id).Result
                                                  select new XElement("Book", bookName)
                                                  ))));

                        string path = Path.GetFullPath("ProjectXML.txt");
                        xml.Save(path);
                        Console.WriteLine();
                        Console.WriteLine("Saved in: " + path);
                        Console.WriteLine();
                        Console.WriteLine("[O] to open the xml file");

                        var openFile = Console.ReadLine();
                        Console.WriteLine();
                        if (openFile == "o" || openFile == "o" || openFile == "O")
                        {
                            Console.Clear();
                            System.IO.StreamReader objReader = new System.IO.StreamReader(path);
                            Console.WriteLine(objReader.ReadToEnd());
                            objReader.Close();
                        }



                        Console.ReadKey();
                        goto Menu;

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