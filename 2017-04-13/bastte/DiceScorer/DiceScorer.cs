using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceScorer
{
    using static ScoreConstants;

    public static class ScoreConstants
    {
        public static readonly int[] DiceValues = Enumerable.Range(1, 6).ToArray();
        public static readonly int[] ZeroDice = {0, 0, 0, 0, 0, 0};
    }


    public class DiceScorer
    {
        private readonly IEnumerable<Func<int[], (int score, int[] counts)>> _rules =
            Enumerable.Range(0, 6)
            .Select<int, Func<int[], (int score, int[] counts)>>(value => ((int[] counts) =>
                {
                    int c = counts[value] - 3;
                    if (c < 0)
                    {
                        return (0, counts);
                    }

                    counts = counts.ToArray();
                    counts[value] = 0;
                    return ((value == 0 ? 1000 : 100 * (value + 1)) << c,
                        counts);
                }))
            .Concat(
                new List<Func<int[], (int score, int[] counts)>>
                {
                    counts =>
                    {
                        if (counts.Sum() == 6 && counts.All(count => count == 0 || count == 2))
                        {
                            return (800, ZeroDice);
                        }
                        return (0, counts);
                    },
                    counts =>
                    {
                        if (counts.All(count => count == 1))
                        {
                            return (1200, ZeroDice);
                        }
                        return (0, counts);
                    },
                    counts =>
                    {
                        if (counts[1 - 1] != 1)
                        {
                            return (0, counts);
                        }

                        counts = counts.ToArray();
                        counts[1 - 1] = 0;

                        return (100, counts);
                    },
                    counts =>
                    {
                        if (counts[5 - 1] != 1)
                        {
                            return (0, counts);
                        }

                        counts = counts.ToArray();
                        counts[5 - 1] = 0;

                        return (50, counts);
                    }
                });

        public int Score(List<int> dice)
        {
            int[] originalCounts = DiceValues.Select(value => dice.Count(die => die == value)).ToArray();



            return _rules.Aggregate<Func<int[], (int score, int[] counts)>, (int score, int[] counts)>(
                    (0, originalCounts),
                    ((int score, int[] counts) _, Func<int[], (int score, int[] counts)> rule) =>
                    {
                        (int score, int[] counts) = rule(_.counts);
                        return (_.score + score, counts);
                    })
                .score;
        }
    }
}
