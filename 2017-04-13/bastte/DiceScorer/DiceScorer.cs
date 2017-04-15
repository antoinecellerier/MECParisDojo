using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Counts = System.Collections.Immutable.ImmutableArray<int>;
using Score = System.Int32;

namespace DiceScorer
{
    public class DiceScorer
    {
        private static readonly int[] DiceValues = Enumerable.Range(1, 6).ToArray();
        private static readonly Counts ZeroDice = Enumerable.Repeat(0, 6).ToImmutableArray();

        private static Func<(Score, Counts), (Score s, Counts c)> Rule(
            Predicate<Counts> condition, Score score, Func<Counts, Counts> cost)
            => Rule(condition, _ => score, cost);

        private static Func<(Score, Counts), (Score s, Counts c)> Rule(
            Predicate<Counts> condition, Func<Counts, Score> score, Func<Counts, Counts> cost)
            => ((Score s, Counts c) _) => !condition(_.c) ? _ : (_.s + score(_.c), cost(_.c));

        private static readonly Func<Counts, Counts> Zero = _ => ZeroDice;

        private static Func<Counts, Counts> CloneZero(int index) => src => src.SetItem(index, 0);

        private readonly Func<(Score, Counts), (Score s, Counts c)> _rules =
            Enumerable.Range(0, 6)
                .Select(value =>
                    Rule(c => c[value] >= 3,
                        c => (value == 0 ? 1000 : 100 * (value + 1)) << (c[value] - 3),
                        CloneZero(value)))
                .Concat(
                    new[]
                    {
                        Rule(c => c.Sum() == 6 && c.All(_ => _ == 0 || _ == 2), 800, Zero),
                        Rule(c => c.All(_ => _ == 1), 1200, Zero),
                        Rule(c => c[0] == 1, 100, CloneZero(0)),
                        Rule(c => c[4] == 1, 50, CloneZero(4))
                    })
                .Aggregate((f, g) => _ => g(f(_)));

        public int Score(List<int> dice) => _rules((0,
                DiceValues.Select(v => dice.Count(d => d == v)).ToImmutableArray()))
            .s;
    }
}
