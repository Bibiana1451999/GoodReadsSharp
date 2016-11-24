using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodReadsSharp.Models;

namespace GoodReadsSharp
{
    public class ListBooksOnShelf
    {
        public Request Request { get; set; }
        public ShelfReviewList Reviews { get; set; }

    }

    public class ShelfReviewList
    {
        public String shelfname { set; get; }
        public List<Book> booksOnShelf { get; set;}
        public Int32 Start { get; set; }
        public Int32 End { get; set; }
        public Int32 Total { get; set; }
        public List<ShelfReview> Value { get; set; }
    }
}
