using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    static int[] a, b;
    static int[] nextSameA, nextSameB, prevSameA, prevSameB;
    static int[] posA, posB;
    static int n;

    static void BuildNextPrev(int[] arr, int[] nextSame, int[] prevSame)
    {
        int[] last = new int[n + 2];
        Array.Fill(last, n + 1);
        for (int i = n; i >= 1; i--)
        {
            nextSame[i] = last[arr[i]];
            last[arr[i]] = i;
        }
        Array.Fill(last, 0);
        for (int i = 1; i <= n; i++)
        {
            prevSame[i] = last[arr[i]];
            last[arr[i]] = i;
        }
    }

    static long SolveOne()
    {
        n = int.Parse(Console.ReadLine());
        a = new int[n + 2];
        b = new int[n + 2];
        posA = new int[n + 2];
        posB = new int[n + 2];
        nextSameA = new int[n + 2];
        nextSameB = new int[n + 2];
        prevSameA = new int[n + 2];
        prevSameB = new int[n + 2];

        var ia = Console.ReadLine().Split().Select(int.Parse).ToArray();
        var ib = Console.ReadLine().Split().Select(int.Parse).ToArray();
        for (int i = 1; i <= n; i++)
        {
            a[i] = ia[i - 1];
            b[i] = ib[i - 1];
        }

        Array.Fill(posA, 0);
        Array.Fill(posB, 0);
        for (int i = 1; i <= n; i++)
        {
            posA[a[i]] = i;
            posB[b[i]] = i;
        }

        BuildNextPrev(a, nextSameA, prevSameA);
        BuildNextPrev(b, nextSameB, prevSameB);

        int[] maxReachL = new int[n + 2];
        for (int i = 1; i <= n; i++) maxReachL[i] = i;

        int[] lastPosForValue = new int[n + 2];

        for (int i = 1; i <= n; i++)
        {
            if (a[i] == b[i]) continue;

            int valA = a[i];
            int valB = b[i];

            int lastA = lastPosForValue[valA];
            int lastB = lastPosForValue[valB];

            if (lastA > 0 || lastB > 0)
                maxReachL[i] = Math.Max(maxReachL[i], Math.Max(lastA, lastB) + 1);

            int prevA = prevSameA[i];
            if (prevA > 0 && b[prevA] == b[i])
                maxReachL[i] = Math.Max(maxReachL[i], prevA + 1);

            int prevB = prevSameB[i];
            if (prevB > 0 && a[prevB] == a[i])
                maxReachL[i] = Math.Max(maxReachL[i], prevB + 1);

            int posValBInA = posA[valB];
            if (posValBInA > 0 && posValBInA < i && b[posValBInA] == valA)
                maxReachL[i] = Math.Max(maxReachL[i], posValBInA + 1);

            int posValAInB = posB[valA];
            if (posValAInB > 0 && posValAInB < i && a[posValAInB] == valB)
                maxReachL[i] = Math.Max(maxReachL[i], posValAInB + 1);

            lastPosForValue[valA] = i;
            lastPosForValue[valB] = i;
        }

        long ans = 0;
        int curMin = 1;
        for (int r = 1; r <= n; r++)
        {
            if (maxReachL[r] > curMin) curMin = maxReachL[r];
            ans += r - curMin + 1;
        }
        return ans;
    }

    public static void Main()
    {
        int t = int.Parse(Console.ReadLine());
        while (t-- > 0)
            Console.WriteLine(SolveOne());
    }
}