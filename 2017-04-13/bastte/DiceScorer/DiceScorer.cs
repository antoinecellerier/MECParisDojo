using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceScorer
{
    interface ScoreRule
    {
        int Score(int[] counts);
    }

    public class SingleDieScoreRule : ScoreRule
    {
        public int Score(int[] counts)
        {
            int score = 0;

            if (counts[0] == 1)
            {
                score += 100;
                counts[0] = 0;
            }

            if (counts[4] == 1)
            {
                score += 50;
                counts[4] = 0;
            }

            return score;
        }
    }

    class TripleAndMoreScoreDice : ScoreRule
    {
        public int Score(int[] counts)
        {
            int score = 0;

            if (counts[0] >= 3)
            {
                score += 1000 << (counts[0] - 3);
                counts[0] = 0;
            }

            for (int i = 2; i <= 6; ++i)
            {
                if (counts[i - 1] >= 3)
                {
                    score += (100 * i) << (counts[i - 1] - 3);
                    counts[i - 1] = 0;
                }
            }

            return score;
        }
    }

    public class PairScoreRule : ScoreRule
    {
        public int Score(int[] counts)
        {
            if (counts.Sum() == 6 && counts.All(count => count == 0 || count == 2))
            {
                Array.Clear(counts, 0, 6);
                return 800;
            }
            return 0;
        }
    }

    public class StraightScoreRule : ScoreRule
    {
        public int Score(int[] counts)
        {
            if (counts.All(count => count == 1))
            {
                Array.Clear(counts, 0, 6);
                return 1200;
            }
            return 0;
        }
    }

    public class DiceScorer
    {
        public int Score(List<int> dice)
        {
            int score = 0;

            List<ScoreRule> rules = new List<ScoreRule> { new PairScoreRule(), new StraightScoreRule(), new TripleAndMoreScoreDice(), new SingleDieScoreRule() };

            int[] counts = Enumerable.Range(1, 6).Select(value => dice.Count(die => die == value)).ToArray();

            foreach (ScoreRule rule in rules)
            {
                score += rule.Score(counts);
            }

            return score;
        }
    }
}
