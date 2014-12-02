using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apriori
{
    class Item
    {



        public string Name { get; set; }
        public string realName { get; set; }

        public Item(string d)
        {
            this.Name = d;

        }

        public Item(string d, string realName)
        {
            this.Name = d;
            this.realName = realName;

        }



    }
}
