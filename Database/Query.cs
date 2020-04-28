using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBot.Clause;

namespace CoreBot.Database {
    public class Query {
        private Dictionary<int, LuisClause> clauses = new Dictionary<int, LuisClause>();
        private string text;

        public Query(Dictionary<LuisClause, string> cl, string txt) {
            int n = 1;
            foreach(var clause in cl) {
                clauses.Add(n++, clause.Key);
            }

            text = txt;
        }

        public Dictionary<int, LuisClause> getClauses() {
            return clauses;
        }

        public string getText() {
            return text;
        }
    }
}
