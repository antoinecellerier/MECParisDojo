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
        public int Score(List<int> dice)
        {
            int[] originalCounts = DiceValues.Select(value => dice.Count(die => die == value)).ToArray();

            return new List<Func<int[], (int score, int[] counts)>>
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
                    bool cloned = false;
                    return (Enumerable.Range(0, 6)
                        .Aggregate(0, (score, value) =>
                        {
                            if (counts[value] >= 3)
                            {
                                score += (value == 0 ? 1000 : 100 * (value + 1)) << (counts[value] - 3);
                                if (!cloned)
                                {
                                    cloned = true;
                                    counts = counts.ToArray();
                                }
                                counts[value] = 0;
                            }
                            return score;
                        }), counts);
                },
                counts =>
                {
                    int score = 0;

                    if (counts[1-1] == 1)
                    {
                        score += 100;
                        counts = counts.ToArray();
                        counts[1-1] = 0;
                    }

                    return (score, counts);
                },
                counts =>
                {
                    int score = 0;

                    if (counts[5-1] == 1)
                    {
                        score += 50;
                        counts = counts.ToArray();
                        counts[5-1] = 0;
                    }

                    return (score, counts);
                }
            }
                .Aggregate<Func<int[], (int score, int[] counts)>, (int score, int[] counts)>(
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
