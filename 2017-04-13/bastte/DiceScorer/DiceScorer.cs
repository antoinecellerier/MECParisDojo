using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceScorer
{
    public class DiceScorer
    {
        private static readonly int[] DiceValues = Enumerable.Range(1, 6).ToArray();
        private static readonly int[] ZeroDice = { 0, 0, 0, 0, 0, 0 };

        private static Func<int[], (int score, int[] counts)> Rule(Predicate<int[]> condition, int score,
            Func<int[], int[]> cost)
        {
            return Rule(condition, _ => score, cost);
        }

        private static Func<int[], (int score, int[] counts)> Rule(Predicate<int[]> condition, Func<int[], int> score,
            Func<int[], int[]> cost)
        {
            return counts => !condition(counts) ? (0, counts) : (score(counts), cost(counts.ToArray()));
        }

        private readonly IEnumerable<Func<int[], (int score, int[] counts)>> _rules =
            Enumerable.Range(0, 6)
                .Select<int, Func<int[], (int score, int[] counts)>>(value =>
                    Rule(counts => counts[value] >= 3,
                        counts => (value == 0 ? 1000 : 100 * (value + 1)) << (counts[value] - 3), counts =>
                        {
                            counts[value] = 0;
                            return counts;
                        }))
                .Concat(
                    new List<Func<int[], (int score, int[] counts)>>
                    {
                        Rule(counts => counts.Sum() == 6 && counts.All(count => count == 0 || count == 2), 800,
                            counts => ZeroDice),
                        Rule(counts => counts.All(count => count == 1), 1200, counts => ZeroDice),
                        Rule(counts => counts[1 - 1] == 1, 100, counts =>
                        {
                            counts[1 - 1] = 0;
                            return counts;
                        }),
                        Rule(counts => counts[5 - 1] == 1, 50, counts =>
                        {
                            counts[5 - 1] = 0;
                            return counts;
                        })
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
