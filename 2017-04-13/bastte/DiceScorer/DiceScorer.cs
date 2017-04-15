using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Counts = System.Collections.Immutable.ImmutableArray<int>;
using Score = System.Int32;

namespace DiceScorer
{
    using Rule = Func<(Score, Counts), (Score s, Counts c)>;

    public class DiceScorer
    {
        private static Rule Rule(
            Predicate<Counts> condition, Func<Counts, Score> score, Func<Counts, Counts> cost)
            => ((Score s, Counts c) _) => !condition(_.c) ? _ : (_.s + score(_.c), cost(_.c));

        private static readonly Counts Zero = Enumerable.Repeat(0, 6).ToImmutableArray();

        private static Rule Compose(params Rule[] rules) => rules.Aggregate((f, g) => _ => g(f(_)));

        private static readonly Rule Rules =
            Compose(
                Rule(c => c.Sum() == 6 && c.All(_ => (_ & ~2) == 0), c => 800, c => Zero),
                Rule(c => c.All(_ => _ == 1), c => 1200, c => Zero),
                Compose(Enumerable.Range(0, 6).Select(v => Rule(
                        c => c[v] >= 3,
                        c => (100 * ((1 >> v) * 9 + v + 1)) << (c[v] - 3),
                        c => c.SetItem(v, 0))).ToArray()),
                Rule(c => c[0] == 1, c => 100, c => c.SetItem(0, 0)),
                Rule(c => c[4] == 1, c => 50, c => c.SetItem(4, 0)));

        public int Score(IEnumerable<int> dice) => Rules((0, Enumerable.Range(1, 6).Select(v => dice.Count(d => d == v)).ToImmutableArray())).s;
    }
}
