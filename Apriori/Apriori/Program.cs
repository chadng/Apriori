using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Apriori
{
    class Program
    {

        static List<ItemSet> transactions; //list of itemsets
        static List<ItemSet> candidateItemsets;
        static List<ItemSet> freqitemsets; //list of items and min support
        static List<ItemSet> allfreqItemsets; //list of items and min support
        static Dictionary<ItemSet, List<ItemSet>> closeditemsets; //list of items and min support
        static List<ItemSet> maxitemsets; //list of items and min support
        static double minsup = 50;
        static void Main(string[] args)
        {
                transactions = new List<ItemSet>();
                candidateItemsets = new List<ItemSet>();
                freqitemsets = new List<ItemSet>();
                allfreqItemsets = new List<ItemSet>();
                closeditemsets = new Dictionary<ItemSet, List<ItemSet>>();
                maxitemsets = new List<ItemSet>();

                addTransactions();
                addItems();
                foreach (ItemSet s in transactions)
                {
                    s.sort();
                }
                //candidateItemsets = itemsets;
                // scan(false);
                int patternNum = 0;
                freqitemsets = l1scan(freqitemsets, patternNum);

                do
                {
                    allfreqItemsets.AddRange(freqitemsets.ToList());
                    //candidateItemsets = itemsets;
                    patternNum++;

                    foreach (ItemSet s in transactions)
                    {
                        s.sort();
                    }

                    candidateItemsets = join(freqitemsets);
                    freqitemsets = scan(candidateItemsets, patternNum);
                } while (candidateItemsets.Count() != 0);

                //print allfreqitems based on support from high to low
                var orederdallfreqItemsets = allfreqItemsets.OrderByDescending(k => k.support);
                Console.WriteLine("\nAll frequent items - Finished");

                using (StreamWriter writer =
                    new StreamWriter("frequent-patterns.txt"))
                {
                    foreach (ItemSet iset in orederdallfreqItemsets)
                    {
                        writer.WriteLine(iset.support + " " + iset.stringRepresentationWithRealName());
                    }
                }

                closeditemsets = retrieveClosedItemset(allfreqItemsets);
                maxitemsets = retrievemaxitemset(closeditemsets);
                var orderedcloseditemsets = closeditemsets.OrderByDescending(k => k.Key.support);
                Console.WriteLine("\nClosed Items - Finished");

                using (StreamWriter writer =
                       new StreamWriter("closed-itemsets.txt"))
                {
                    foreach (KeyValuePair<ItemSet, List<ItemSet>> kv in orderedcloseditemsets)
                    {
                        writer.WriteLine(kv.Key.support + " " + kv.Key.stringRepresentationWithRealName());
                    }
                }
                var orederedmaxitemsets = maxitemsets.OrderByDescending(k => k.support);
                Console.WriteLine("\nMax Items - Finished");
                using (StreamWriter writer =
                          new StreamWriter("max-itemsets.txt"))
                {
                    foreach (ItemSet s in orederedmaxitemsets)
                    {

                        writer.WriteLine(s.support + " " + s.stringRepresentationWithRealName());
                    }
                }

            Console.ReadLine();
        }

        #region max itemset
        private static List<ItemSet> retrievemaxitemset(Dictionary<ItemSet, List<ItemSet>> closeditemsets)
        {
            List<ItemSet> maxitemset = new List<ItemSet>();
            foreach (var item in closeditemsets)
            {
                List<ItemSet> parents = item.Value;

                if (parents.Count == 0)
                {
                    maxitemset.Add(item.Key);
                }
            }

            return maxitemset;
        }
        #endregion

        #region closed itemsets
        private static Dictionary<ItemSet, List<ItemSet>> retrieveClosedItemset(List<ItemSet> allfreqItemsets)
        {
            Dictionary<ItemSet, List<ItemSet>> closedItemset = new Dictionary<ItemSet, List<ItemSet>>();
            int i = 0;
            foreach (ItemSet iset in allfreqItemsets)
            {
                List<ItemSet> parents = retrieveParents(iset, ++i, allfreqItemsets);
                if (isclosed(iset, parents) && iset.support >= minsup)
                    closedItemset.Add(iset, parents);
            }
            return closedItemset;

        }

        private static bool isclosed(ItemSet iset, List<ItemSet> parents)
        {
            foreach (ItemSet parent in parents)
            {
                if (iset.support == parent.support)
                {
                    return false;
                }
            }

            return true;
        }

        private static List<ItemSet> retrieveParents(ItemSet iset, int p, List<ItemSet> allfreqItemsets)
        {
            List<ItemSet> parents = new List<ItemSet>();

            for (int j = p; j < allfreqItemsets.Count(); j++)
            {
                ItemSet parent = allfreqItemsets[j];

                if (parent.Count() == iset.Count() + 1)
                {
                    if (parent.contains(iset))
                    {
                        parents.Add(parent);
                    }
                }
            }

            return parents;

        }

        #endregion

        #region read data
        static void addTransactions()
        {
            //   int lineNum = 0;
            string line;
            using (StreamReader sr = new StreamReader("data/topic-1.txt"))
            {

                while ((line = sr.ReadLine()) != null)
                {
                    List<string> data = line.Split(' ').ToList();
                    ItemSet set = new ItemSet();
                    foreach (string d in data)
                    {
                        if (!d.Equals(""))
                        {
                            Item it = new Item(d);
                            set.add(it);
                        }

                    }
                    set.sort();
                    transactions.Add(set);

                    //  lineNum++;
                }
            }
        }

        static void addItems()
        {
            //int lineNum = 0;
            string line;
            using (StreamReader sr = new StreamReader("data/vocab.txt"))
            {

                while ((line = sr.ReadLine()) != null)
                {
                    List<string> data = line.Split('	').ToList();        //change ' ' to '	'      
                    freqitemsets.Add(new ItemSet(new Item(data[0], data[1])));
                    //   lineNum++;
                }
            }
        }
        #endregion

        #region generate freq itemsets
        static List<ItemSet> join(List<ItemSet> itemsets)
        {
            List<ItemSet> newcandidateItemsets = new List<ItemSet>();
            for (int i = 0; i < itemsets.Count() - 1; i++)
            {
                ItemSet itemset1 = itemsets[i];
                for (int j = i + 1; j < itemsets.Count(); j++)
                {
                    ItemSet itemset2 = itemsets[j];
                    itemset2.sort();
                    ItemSet newcandidate = NewCandidateItemSet(itemset1, itemset2);
                    newcandidate.sort();
                    if (newcandidate.Count() != 0)
                    {
                        newcandidate.support = GetSupport(newcandidate);
                        newcandidateItemsets.Add(newcandidate);
                    }

                }
            }
            return newcandidateItemsets;
            //foreach item[i] in itemsets  to itemsets.length-1
            //get item[i] 
            //sort item[i].key
            //foreach item[j=1+1] in itemsets  to itemsets
            //get item[j] 
            //generate new candidate/itemset with item[i] and item[j]
            //if the candidate is not empty
            //get and add min support
            //add to candidates itemsets
            //setmy itemsets to candidates itemset
            //clear candidates itemsets


        }

        static double GetSupport(ItemSet i1)
        {
            double support = 0;

            foreach (ItemSet transaction in transactions)
            {
                if (transaction.contains(i1))
                {
                    support++;
                }
            }

            return support;
        }




        static ItemSet NewCandidateItemSet(ItemSet i1, ItemSet i2)
        {
            ItemSet newItemSet = new ItemSet();
            i1.clone(newItemSet);
            int length = i1.items.Count();

            if (length == 1)
            {
                newItemSet.items.AddRange(i2.items);
                return newItemSet;
            }
            else
            {

                ItemSet firstSubString = new ItemSet();
                ItemSet secondSubString = new ItemSet();

                firstSubString.items = i1.items.GetRange(0, length - 1);
                secondSubString.items = i2.items.GetRange(0, length - 1);
                firstSubString.sort();
                secondSubString.sort();
                if (firstSubString.stringRepresentation().Equals(secondSubString.stringRepresentation()))
                {
                    newItemSet.items.Add(i2.items[length - 1]);
                    return newItemSet;
                }

                ItemSet empt = new ItemSet();
                return empt;

            }

        }

        static List<ItemSet> l1scan(List<ItemSet> candidateItemsets, int patternNum)
        {
            List<ItemSet> newcandidateItemsets = new List<ItemSet>();

            foreach (ItemSet iset in candidateItemsets)
            {
                ItemSet newcandidate = new ItemSet();// = iset;

                iset.support = GetSupport(iset);
                iset.clone(newcandidate);
                if (newcandidate.support >= minsup)
                {
                    newcandidateItemsets.Add(newcandidate);
                }
            }
            //Console.WriteLine("Frequent Pattern for " + patternNum + " - Itemset and min support");
            //foreach (ItemSet i in newcandidateItemsets)
            //Console.WriteLine(i.stringRepresentationWithSpace() +  " " + i.support);
            return newcandidateItemsets;


        }

        static List<ItemSet> scan(List<ItemSet> candidateItemsets, int patternNum)
        {
            List<ItemSet> newfrequentItemsets = new List<ItemSet>();

            //set freq itemsets to null
            // if(resetItemsets)
            // itemsets = new List<ItemSet>();

            foreach (ItemSet s in candidateItemsets)
            {
                if (s.support >= minsup)
                {
                    newfrequentItemsets.Add(s);
                }
            }
            // Console.WriteLine("Frequent Pattern for " + patternNum + " - Itemset and min support");
            // foreach (ItemSet i in newfrequentItemsets)
            // Console.WriteLine(i.stringRepresentationWithSpace() + " " + i.support);
            return newfrequentItemsets;
            //foreach item (first call is item[i](item[0], minsup))  in itemsets
            //foreach transaction i
            //sort items in itemset    
            //check if transaction list i contains itemset[i](item[0], minsup)  aka if itemset[i] list<string> is a subset of transaction list<String>
            //increment support count for item[0]
            //now i have itemsets with its corresponding min support count
            //check if min supp is above threshold
            //remove item[i] checksupport is false
            //we have our new frequent itemsets
            //print it out
        }
        #endregion









    }
}
