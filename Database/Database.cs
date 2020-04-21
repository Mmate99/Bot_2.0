using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Database {

    public class Database {
        private List<IData> data = new List<IData>();

        public void addItem(IData item) {
            data.Add(item);
        }

        public List<IData> getType(string type) {
            List<IData> ret = new List<IData>();

            foreach(var d in data) {
                if(d.GetType().ToString() == "CoreBot." + type) {
                    ret.Add(d);
                }
            }

            return ret;
        }

        public List<IData> getData() { return data; }
    }
}
