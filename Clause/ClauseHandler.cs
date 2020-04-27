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
        private int valueCount = 0;
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

        private bool mainStatementValuesContainsWord(string word) {
            foreach(string statementWord in mainStatementValues) {
                string firstWordOnly = statementWord.Split(" ")[0];        //"William Shakespeare" => "William", hogy később ne legyen kétszer is hozzáadva a clauses-hez!
                if (firstWordOnly.Contains(word)) {
                    return true;
                }
            }

            return false;
        }

        private void createClauses() {
            sortWords();

            for(int i = 0; i < words.Length; i++) {
                string word = words[i];

                if (keys.Contains(word)) {
                    searchKey = word;
                }

                else if (mainStatementValuesContainsWord(word.ToLower())) {
                    LuisClause lc = new LuisClause(searchKey, mainStatementValues[valueCount]);
                    lc.Bigger = bigger;
                    lc.Smaller = smaller;
                    lc.Negated = negation;

                    if (words[i - 1].Equals("and") || words[i - 1].Equals("or")) {
                        andOr = words[i - 1];
                    }

                    clauses.Add(lc, andOr);

                    negation = false;
                    bigger = false;
                    smaller = false;
                    valueCount++;
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
