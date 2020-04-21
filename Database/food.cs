using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Database {
    public class Food : IData {
        private string name;
        private string type;
        private int calories;

        public string Name { get => name; set => name = value; }
        public string Type { get => type; set => type = value; }
        public int Calories { get => calories; set => calories = value; }

        public Food(string n, string t, int c) {
            name = n;
            type = t;
            calories = c;
        }
    }
}
