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
        private static readonly Counts ZeroDice = new[]{0, 0, 0, 0, 0, 0}.ToImmutableArray();

        private static Func<Counts, (Score s, Counts c)> Rule(Predicate<Counts> condition, Score s,
            Func<Counts, Counts> cost) => Rule(condition, _ => s, cost);

        private static Func<Counts, (Score s, Counts c)> Rule(Predicate<Counts> condition, Func<Counts, Score> s,
            Func<Counts, Counts> cost) => c => !condition(c) ? (0, c) : (s(c), cost(c));

        private static readonly Func<Counts, Counts> Zero = _ => ZeroDice;

        private static Func<Counts, Counts> CloneZero(int index) => src => src.SetItem(index, 0);

        private readonly IEnumerable<Func<Counts, (Score s, Counts c)>> _rules =
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

        public int Score(List<int> dice) =>
            _rules.Aggregate<Func<Counts, (Score s, Counts c)>, (Score s, Counts c)>(
                    (0, DiceValues.Select(value => dice.Count(die => die == value)).ToImmutableArray()),
                    (_, rule) =>
                    {
                        (Score s, Counts c) = rule(_.c);
                        return (_.s + s, c);
                    })
                .s;
    }
}
