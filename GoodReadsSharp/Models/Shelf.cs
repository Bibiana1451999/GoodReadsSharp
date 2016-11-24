using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodReadsSharp.Models
{
    public class Shelf
    {
       
        public Int32 Id { get; set; }
        public Int32 Position { get; set; }
        public Int32 ReviewId { get; set; }
        public Int32 UserShelfId { get; set; }
        public String name { get; set; }
    }
}
