using System;
using System.Collections.Generic;
using System.Linq;

class Solution
{
    static long SolveCase(int n, long[] cards)
    {
        if (n < 3) return 0;
        
        var freq = new Dictionary<long, int>();
        foreach (var c in cards)
        {
            freq[c] = freq.GetValueOrDefault(c) + 1;
        }
        
        long totalPairs = 0;
        foreach (var f in freq.Values)
            totalPairs += f / 2;
        
        if (totalPairs == 0) return 0;
        
        long best = 0;
        var values = freq.Keys.ToList();
        
        foreach (var a in values)
        {
            int fa = freq[a];
            if (fa < 2) continue;
            
            long slots = fa / 2;
            long pOthers = 0;
            long sOthersSafe = 0;
            long sPureSingles = 0;
            
            foreach (var kv in freq)
            {
                if (kv.Key == a) continue;
                int f = kv.Value;
                if (f >= 2)
                {
                    pOthers += f / 2;
                    sOthersSafe += f % 2;
                }
                else
                {
                    sPureSingles++;
                }
            }
            
            long safeBlockSize = 2 * pOthers + sOthersSafe;
            long safeBlockCount = safeBlockSize > 0 ? 1 : 0;
            long blocksNeeded = safeBlockCount + sPureSingles;
            
            long candidate;
            if (blocksNeeded <= slots)
            {
                candidate = fa + safeBlockSize + sPureSingles;
            }
            else if (safeBlockCount == 0)
            {
                candidate = fa + Math.Min(sPureSingles, slots);
            }
            else
            {
                long extra = slots - 1;
                if (extra < 0) extra = 0;
                candidate = fa + safeBlockSize + Math.Min(sPureSingles, extra);
            }
            
            if (candidate > best) best = candidate;
        }
        
        return best >= 3 ? best : 0;
    }
    
    static void Main()
    {
        int t = int.Parse(Console.ReadLine());
        var results = new List<long>();
        for (int i = 0; i < t; i++)
        {
            int n = int.Parse(Console.ReadLine());
            var parts = Console.ReadLine().Split();
            var cards = new long[n];
            for (int j = 0; j < n; j++)
                cards[j] = long.Parse(parts[j]);
            results.Add(SolveCase(n, cards));
        }
        Console.WriteLine(string.Join("\n", results));
    }
}