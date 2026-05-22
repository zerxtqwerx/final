using System;
using System.Collections.Generic;
using System.Diagnostics;

class Solution
{
    static long SolveCase(int n, long[] cards)
    {
        if (n < 3) return 0;

        var freq = new Dictionary<long, long>();
        foreach (var c in cards)
        {
            if (!freq.ContainsKey(c)) freq[c] = 0;
            freq[c]++;
        }

        long totalPairs = 0;
        foreach (var f in freq.Values)
            totalPairs += f / 2;

        if (totalPairs == 0) return 0;

        long best = 0;

        foreach (var kv in freq)
        {
            long a = kv.Key;
            long fa = kv.Value;
            if (fa < 2) continue;

            long slots = fa / 2;

            long pOthers = 0;
            long sOthersSafe = 0; 
            long sPureSingles = 0; 

            foreach (var kv2 in freq)
            {
                if (kv2.Key == a) continue;
                long f = kv2.Value;
                if (f >= 2)
                {
                    pOthers += f / 2;
                    sOthersSafe += f % 2;
                }
                else // f == 1
                {
                    sPureSingles++;
                }
            }

            long safeBlockSize = 2 * pOthers + sOthersSafe;
            long safeBlockCount = (safeBlockSize > 0) ? 1 : 0;

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
                candidate = fa + safeBlockSize + Math.Min(sPureSingles, slots - 1);
            }

            best = Math.Max(best, candidate);
        }

        return best >= 3 ? best : 0;
    }

    static void Main(string[] args)
    {
        int t = int.Parse(Console.ReadLine().Trim());

        var sw = Stopwatch.StartNew();
        long memBefore = GC.GetTotalMemory(true);

        for (int i = 0; i < t; i++)
        {
            int n = int.Parse(Console.ReadLine().Trim());
            var parts = Console.ReadLine().Trim().Split(' ');
            var cards = new long[n];
            for (int j = 0; j < n; j++)
                cards[j] = long.Parse(parts[j]);

            Console.WriteLine(SolveCase(n, cards));
        }

        sw.Stop();
        long memAfter = GC.GetTotalMemory(false);

        Console.Error.WriteLine($"Время: {sw.ElapsedMilliseconds} мс");
        Console.Error.WriteLine($"Память (приблизительно): {(memAfter - memBefore) / 1024} КБ");
    }
}