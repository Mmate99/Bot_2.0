using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreBot.Clause {
    public class ClauseHandler {
        private string[] keys;
        private string[] words;
        private GeneralStatemenHandler._Entities.ValueClass[] Luisvalues;
        private List<string> mainStatementValues;
        private Dictionary<LuisClause, string> clauses;
        private Dictionary<string, GeneralStatemenHandler._Entities.ValueClass> otherValues;

        private string searchKey = null;
        private string andOr = "";
        private int keyCount = 0;
        private bool negation = false;
        private bool bigger = false;
        private bool smaller = false;

        public ClauseHandler(GeneralStatemenHandler luisResult, Dictionary<LuisClause, string> c) {
            keys = luisResult.Entities.SearchKey;
            Luisvalues = luisResult.Entities.Value;
            clauses = c;
            words = luisResult.Text.Split(" ");
            mainStatementValues = new List<string>();
            otherValues = new Dictionary<string, GeneralStatemenHandler._Entities.ValueClass>();

            createClauses();
        }

        private void sortWords() {
            for (int i = 0; i < Luisvalues.Length; i++) {
                var val = Luisvalues[i].datetime != null ? Luisvalues[i].datetime[0].Expressions : Luisvalues[i].personName ?? Luisvalues[i].value;
                var other = Luisvalues[i].bigger ?? Luisvalues[i].smaller ?? Luisvalues[i].negation;

                if (val != null) {
                    mainStatementValues.Add(val[0]);
                }
                else if (other != null) {
                    otherValues.Add(other[0], Luisvalues[i]);
                }
            }
        }

        private void createClauses() {
            sortWords();

            foreach (string word in words) {
                if (keys.Contains(word)) {
                    searchKey = word;
                }

                else if (mainStatementValues[keyCount].Contains(word.ToLower()) && searchKey != null) {
                    LuisClause lc = new LuisClause(searchKey, mainStatementValues[keyCount]);
                    lc.Bigger = bigger;
                    lc.Smaller = smaller;
                    lc.Negated = negation;

                    clauses.Add(lc, andOr);

                    searchKey = null;
                    negation = false;
                    bigger = false;
                    smaller = false;
                    keyCount = keyCount < keys.Count() - 1 ? keyCount + 1 : keyCount;
                }

                else {
                    if (word.Equals("and") || word.Equals("or")) {
                        andOr = word;
                    }
                    else {
                        var k = otherValues.Keys.Where(key => key.Contains(word));
                        if (k.Count() != 0) {
                            var luisValue = otherValues[k.First()];

                            negation = luisValue.negation != null;
                            bigger = luisValue.bigger != null;
                            smaller = luisValue.smaller != null;
                        }
                    }
                }
            }
        }
    }
}
