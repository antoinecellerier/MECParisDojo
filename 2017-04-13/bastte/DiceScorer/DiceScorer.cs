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

    interface ScoreRule
    {
        (int score, int[] remaining) Score(int[] counts);
    }

    public class SingleDieScoreRule : ScoreRule
    {
        private readonly int _value;
        private readonly int _score;

        public SingleDieScoreRule(int value, int score)
        {
            _value = value;
            _score = score;
        }

        public (int score, int[] remaining) Score(int[] counts)
        {
            int score = 0;

            if (counts[_value-1] == 1)
            {
                score += _score;
                counts = counts.ToArray();
                counts[_value-1] = 0;
            }

            return (score, counts);
        }
    }

    class TripleAndMoreScoreDice : ScoreRule
    {
        public (int score, int[] remaining) Score(int[] counts)
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
        }
    }

    public class PairScoreRule : ScoreRule
    {
        public (int score, int[] remaining) Score(int[] counts)
        {
            if (counts.Sum() == 6 && counts.All(count => count == 0 || count == 2))
            {
                return (800, ZeroDice);
            }
            return (0, counts);
        }
    }

    public class StraightScoreRule : ScoreRule
    {
        public (int score, int[] remaining) Score(int[] counts)
        {
            if (counts.All(count => count == 1))
            {
                return (1200, ZeroDice);
            }
            return (0, counts);
        }
    }

    public class DiceScorer
    {
        private readonly List<ScoreRule> _rules = new List<ScoreRule>
        {
            new PairScoreRule(),
            new StraightScoreRule(),
            new TripleAndMoreScoreDice(),
            new SingleDieScoreRule(1, 100),
            new SingleDieScoreRule(5, 50)
        };

        public int Score(List<int> dice)
        {
            int[] originalCounts = DiceValues.Select(value => dice.Count(die => die == value)).ToArray();

            return _rules.Aggregate<ScoreRule, (int score, int[] counts)>(
                    (0, originalCounts),
                    ((int score, int[] counts) _, ScoreRule rule) =>
                    {
                        (int score, int[] counts) = rule.Score(_.counts);
                        return (_.score + score, counts);
                    })
                .score;
        }
    }
}
