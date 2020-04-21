using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Database {
    public class Book: IData{
        private string title;
        private string author;
        private int pageCount;

        public string Name { get => title; set => title = value; }
        public string Author { get => author; set => author = value; }
        public int Pagecount { get => pageCount; set => pageCount = value; }

        public Book(string t, string a, int pc) {
            title = t;
            author = a;
            pageCount = pc;
        }
    }
}
