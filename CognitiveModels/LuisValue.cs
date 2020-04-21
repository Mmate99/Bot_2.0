using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.CognitiveModels {

    public class LuisValue {
        private string value;
        private bool negated = false;
        private bool smaller = false;
        private bool bigger = false;

        public string Value { get => value; set => this.value = value; }
        public bool Negated { get => negated; set => negated = value; }
        public bool Smaller { get => smaller; set => smaller = value; }
        public bool Bigger { get => bigger; set => bigger = value; }

        public LuisValue(string v) {
            value = v;
        }
    }
}
