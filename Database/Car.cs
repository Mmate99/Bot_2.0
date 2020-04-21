using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Database {
    public class Car: IData{
        private string brand;
        private int horsepower;
        private string colour;

        public string Brand { get => brand; set => brand = value; }
        public int Horsepower { get => horsepower; set => horsepower = value; }
        public string Colour { get => colour; set => colour = value; }

        public Car(string b, string c, int hp) {
            brand = b;
            colour = c;
            horsepower = hp;
        }
    }
}
