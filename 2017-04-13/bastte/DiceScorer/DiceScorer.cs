using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceScorer
{
    public class DiceScorer
    {
        private static readonly int[] DiceValues = Enumerable.Range(1, 6).ToArray();
        private static readonly int[] ZeroDice = { 0, 0, 0, 0, 0, 0 };

        private static Func<int[], (int s, int[] c)> Rule(Predicate<int[]> condition, int s,
            Func<int[], int[]> cost)
        {
            return Rule(condition, _ => s, cost);
        }

        private static Func<int[], (int s, int[] c)> Rule(Predicate<int[]> condition, Func<int[], int> s,
            Func<int[], int[]> cost)
        {
            return c => !condition(c) ? (0, c) : (s(c), cost(c));
        }

        private static readonly Func<int[], int[]> Zero = _ => ZeroDice;
        private static Func<int[], int[]> CloneZero(int index)
        {
            return src =>
            {
                int[] dst = src.ToArray();
                dst[index] = 0;
                return dst;
            };
        }

        private readonly IEnumerable<Func<int[], (int s, int[] c)>> _rules =
            Enumerable.Range(0, 6)
                .Select(value =>
                    Rule(c => c[value] >= 3,
                        c => (value == 0 ? 1000 : 100 * (value + 1)) << (c[value] - 3),
                        CloneZero(value)))
                .Concat(
                    new[]
                    {
                        Rule(c => c.Sum() == 6 && c.All(_ => _ == 0 || _ == 2), 800, Zero),
                        Rule(c => c.All(count => count == 1), 1200, Zero),
                        Rule(c => c[0] == 1, 100, CloneZero(0)),
                        Rule(c => c[4] == 1, 50, CloneZero(4))
                    });

        public int Score(List<int> dice)
        {
            int[] originalc = DiceValues.Select(value => dice.Count(die => die == value)).ToArray();

            return _rules.Aggregate<Func<int[], (int s, int[] c)>, (int s, int[] c)>(
                    (0, originalc),
                    (_, rule) =>
                    {
                        (int s, int[] c) = rule(_.c);
                        return (_.s + s, c);
                    })
                .s;
        }
    }
}
