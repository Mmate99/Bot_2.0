using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Clause{

    public class LuisClause {
        private string value;
        private string searchKey;
        private bool negated = false;
        private bool smaller = false;
        private bool bigger = false;

        public string Value { get => value; set => this.value = value; }
        public string SearchKey { get => searchKey; set => this.searchKey = value; }
        public bool Negated { get => negated; set => negated = value; }
        public bool Smaller { get => smaller; set => smaller = value; }
        public bool Bigger { get => bigger; set => bigger = value; }

        public LuisClause(string sk, string v) {
            searchKey = sk;
            value = v;
        }
    }
}
