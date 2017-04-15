using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DiceScorer
{
    public class DiceScorerShould
    {
        [Theory]
        [InlineData(0, new[] { 3 })]
        [InlineData(100, new[] { 1 })]
        [InlineData(50, new[] { 5 })]
        [InlineData(150, new[] { 1, 5 })]
        [InlineData(150, new[] { 1, 2, 3, 5 })]
        [InlineData(1000, new[] { 1, 1, 1 })]
        [InlineData(1050, new[] { 1, 1, 1, 5 })]
        [InlineData(500, new[] { 2, 2, 2, 3, 3, 3 })]
        [InlineData(600, new[] { 6, 6, 6 })]
        [InlineData(1200, new[] { 6, 6, 6, 6 })]
        [InlineData(2000, new[] { 1, 1, 1, 1 })]
        [InlineData(800, new[] { 1, 1, 2, 2, 3, 3 })]
        [InlineData(800, new[] { 1, 2, 3, 3, 2, 1 })]
        [InlineData(100, new[] { 1, 2, 2, 3, 3, 4 })]
        [InlineData(1200, new[] { 1, 2, 3, 4, 5, 6 })]
        [InlineData(1200, new[] { 4, 2, 3, 1, 5, 6 })]
        public void CorrectlyScore(int expectedScore, int[] dice)
        {
            var scorer = new DiceScorer();
            int score = scorer.Score(dice.ToList());
            Assert.Equal(expectedScore, score);
        }
    }
}