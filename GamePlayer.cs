using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace rhul
{
    public interface IGamePlayerResults
    {
        int HighestScore { get; }
        int HighestLoop { get; }
        int[] HighestScorePosition { get; }
        int[] HighestLoopPosition { get; }
        int HighestScoreOccurrences { get; }
        int HighestLoopOccurrences { get; }
    }

    /// <summary>
    /// Calculates the highest 'score' and 'loop' values for a list of starting positions by
    /// concurrently playing multiple games.
    /// </summary>
    public class GamePlayer
    {
        private class GameScores : IGamePlayerResults
        {
            public int HighestScore { get; set; }
            public int HighestLoop { get; set; }
            public int[] HighestScorePosition { get; set; }
            public int[] HighestLoopPosition { get; set; }
            public int HighestScoreOccurrences { get; set; }
            public int HighestLoopOccurrences { get; set; }
        }

        private readonly List<StartPos> positions;
        private readonly GameScores results;
        private object scoreLock = new object();

        public GamePlayer(List<StartPos> positions)
        {
            this.positions = positions;
            this.results = new GameScores();
        }

        public IGamePlayerResults Play(List<StartPos> positions)
        {
            Parallel.ForEach<StartPos>(this.positions, this.PlayGame);
            return this.results;
        }

        private void PlayGame(StartPos pos)
        {
            Game game = new Game(pos);
            IGameResult result = game.Play();
            UpdateScores(pos, result);
        }

        private void UpdateScores(StartPos position, IGameResult result)
        {
            // multiple threads could collide here without a lock
            lock (this.scoreLock)
            {
                //score stats
                if (result.Score > this.results.HighestScore)
                {
                    this.results.HighestScorePosition = position.Piles;
                    this.results.HighestScore = result.Score;
                    this.results.HighestScoreOccurrences = 1;
                }
                else if (result.Score == this.results.HighestScore)
                {
                    this.results.HighestScoreOccurrences++;
                }
                // loop stats
                if (result.Loop > this.results.HighestLoop)
                {
                    this.results.HighestLoopPosition = position.Piles;
                    this.results.HighestLoop = result.Loop;
                    this.results.HighestLoopOccurrences = 1;
                }
                else if (result.Loop == this.results.HighestLoop)
                {
                    this.results.HighestLoopOccurrences++;
                }
            }
        }
    }
}
