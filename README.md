Apriori
=======

Mining Frequent Patterns with Apriori Algorithm


1. Run Scan and Generate 1-frequent itemset and their support count
2. Accumulate the generated frequent itemsets to my global variable that holds on to all my frequent itemsets
3. Sort the items in all my itemsets according to their term index
4. Run Join to the frequent itemset to generate my candidate itemsets
5. Run Scan on the candidate itemsets to generate another set of frequent items
6. If I have more than one 1 candidate itemset perform process 2-5 again



Closed patterns: I perform a top down approach. Therefore, I find my closed patterns from my frequent patterns. For each item set in all my frequent patterns, I retrieve the parents of the selected item set and if all its parents’ support is not equal to the item sets’ support and the item sets’ support is greater than the min_sup, then the item set is added as a closed pattern.


Maximal patterns: After finding all the closed patterns, in order to find the maximal patterns, I check if a closed pattern has parents’, if it doesn’t then the closed pattern is added as a maximal pattern.
