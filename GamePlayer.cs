using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace rhul
{
    public interface IGamePlayerResults
    {
        public int HighestScore { get; }
        public int HighestLoop { get; }
        public int[] HighestScorePosition { get; }
        public int[] HighestLoopPosition { get; }
        public int HighestScoreOccurrences { get; }
        public int HighestLoopOccurrences { get; }
    }

    /*
     * The program was only using 25% of processing power on my PC, so to use the other cores I made it multithreaded.
     * It runs about 2.5 times faster than singlethreaded on my quad core PC (about 500ms with the Console.WriteLine()'s commented)
     */
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

        public void Play(List<StartPos> positions)
        {
            Parallel.ForEach<StartPos>(this.positions, this.PlayGame);
        }

        private IGamePlayerResults PlayGame(StartPos pos)
        {
            Game game = new Game(pos);
            game.Play();
            UpdateScores(pos, game);
            return this.results;
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
