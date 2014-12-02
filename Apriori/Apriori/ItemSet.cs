using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apriori
{
    class ItemSet
    {
        //  public Dictionary<Item, double> items = new Dictionary<Item, double>();
        public List<Item> items = new List<Item>();
        public double support = 0;
        public ItemSet()
        {

        }
        public ItemSet(double supp)
        {
            this.support = supp;
        }
        public void incrementCount()
        {
            support++;
        }

        public ItemSet(Item item)
        {
            items.Add(item);
            support = 0;
        }

        public void add(Item item)
        {
            items.Add(item);
        }

        public void sort()
        {
            //List<Item> y = items;
            List<string> d = items.AsEnumerable()
              .Select(row => row.Name + " " + row.realName)
              .Distinct()
              .OrderBy(x => x)
              .ToList();
            items.Clear();
            foreach (string s in d)
            {
                List<string> splited = s.Split(' ').ToList(); ;
                Item item = new Item(splited[0], splited[1]);
                items.Add(item);
                //add(new Item(d));
            }
            // items = items.Sort();
            //var x = items.OrderBy(i => i.Key);
            // items = x.ToDictionary(pair => pair.Key, pair => pair.Value); ;
        }

        public bool contains(ItemSet itemset)
        {

            int numItemContains = 0;
            List<string> itemNames = items.Select(x => x.Name).ToList();
            foreach (Item i in itemset.items)
            {
                if (itemNames.Contains(i.Name))
                {
                    numItemContains++;
                }
            }

            if (numItemContains >= itemset.items.Count())
            {
                return true;
            }
            return false;
            //items.co
        }


        public string stringRepresentation()
        {

            //List<string> strings = items.Select(x => x.Name).ToList();
            return string.Join("", items.Select(x => x.Name).ToArray());


        }

        public string stringRepresentationWithSpace()
        {   //List<string> strings = items.Select(x => x.Name).ToList();
            return string.Join(" ", items.Select(x => x.Name).ToArray());
        }

        public string stringRepresentationWithRealName()
        {   //List<string> strings = items.Select(x => x.Name).ToList();
            return string.Join(" ", items.Select(x => x.realName).ToArray());
        }

        public int Count()
        {
            return items.Count();
        }



        internal void clone(ItemSet newItemSet)
        {
            foreach (Item item in items)
            {
                newItemSet.add(item);
            }
            newItemSet.support = this.support;
        }
    }
}
