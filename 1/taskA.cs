using System;
using System.Text;

class Program
{
    static void Main()
    {
        int t = int.Parse(Console.ReadLine().Trim());
        var sb = new StringBuilder();

        for (int i = 0; i < t; i++)
        {
            var parts = Console.ReadLine().Trim().Split(' ');
            long n = long.Parse(parts[0]);
            long a = long.Parse(parts[1]);
            long b = long.Parse(parts[2]);

            long full = n / 3;
            long rem  = n % 3;

            long cost = full * Math.Min(b, 3 * a);

            if (rem == 1) cost += Math.Min(a, b);
            else if (rem == 2) cost += Math.Min(2 * a, b);

            sb.AppendLine(cost.ToString());
        }

        Console.Write(sb);
    }
}