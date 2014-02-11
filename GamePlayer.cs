using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace rhul
{
    /// <summary>
    /// Immutifies the 
    /// </summary>
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
    /// concurrently playing the game on each position.
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
            game.Play();
            UpdateScores(pos, game);
        }

        private void UpdateScores(StartPos position, Game game)
        {
            // multiple threads could collide here without a lock
            lock (this.scoreLock)
            {
                //score stats
                if (game.Score > this.results.HighestScore)
                {
                    this.results.HighestScorePosition = position.position;
                    this.results.HighestScore = game.Score;
                    this.results.HighestScoreOccurrences = 1;
                }
                else if (game.Score == this.results.HighestScore)
                {
                    this.results.HighestScoreOccurrences++;
                }
                // loop stats
                if (game.Loop > this.results.HighestLoop)
                {
                    this.results.HighestLoopPosition = position.position;
                    this.results.HighestLoop = game.Loop;
                    this.results.HighestLoopOccurrences = 1;
                }
                else if (game.Loop == this.results.HighestLoop)
                {
                    this.results.HighestLoopOccurrences++;
                }
            }
        }
    }
}
