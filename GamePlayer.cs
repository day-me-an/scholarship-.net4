using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace rhul
{
    /*
     * The program was only using 25% of processing power on my PC, so to use the other cores I made it multithreaded.
     * It runs about 2.5 times faster than singlethreaded on my quad core PC (about 500ms with the Console.WriteLine()'s commented)
     */
    public class GamePlayer
    {
        public class GameScores
        {
            public readonly int meh;
            internal GameScores()
            {
                meh = 1;
            }
        }


        private List<StartPos> positions;   
        private object  scoreLock = new object();

        // marked as 'volatile' because multiple threads will be reading these for comparisons and setting them, so a thread could get a stale cached value without this
        private volatile int highestScore, highestLoop;

        // these are only written to by the multiple threads. all writes are volatile in C#, so no need for the 'volatile' keyword (learnt this from here: http://igoro.com/archive/volatile-keyword-in-c-memory-model-explained/)
        private int highestLoopOccurrences = 0, highestScoreOccurrences = 0;
        private int[] highestScorePosition, highestLoopPosition;

        public GamePlayer(List<StartPos> positions)
        {
            this.positions = positions;
            var gs = new GameScores();
        }

        public void Play(List<StartPos> positions)
        {
            Parallel.ForEach<StartPos>(this.positions, this.PlayGame);
        }

        private void PlayGame(StartPos pos)
        {
            Game game = new Game(pos);
            game.Play();

            UpdateScores(pos, game);
        }

        private void UpdateScores(StartPos item, Game game)
        {
            // multiple threads could collide here without a lock
            lock (this.scoreLock)
            {
                //score stats
                if (game.Score > this.highestScore)
                {
                    this.highestScorePosition = item.position;
                    this.highestScore = game.Score;
                    this.highestScoreOccurrences = 1;
                }
                else if (game.Score == this.highestScore)
                {
                    this.highestScoreOccurrences++;
                }
                // loop stats
                if (game.Loop > this.highestLoop)
                {
                    this.highestLoopPosition = item.position;
                    this.highestLoop = game.Loop;
                    this.highestLoopOccurrences = 1;
                }
                else if (game.Loop == this.highestLoop)
                {
                    this.highestLoopOccurrences++;
                }
            }
        }

        public int HighestScore
        {
            get { return this.highestScore; }
        }

        public int HighestLoop
        {
            get { return this.highestLoop; }
        }

        public int[] HighestScorePosition
        {
            get { return this.highestScorePosition; }
        }

        public int[] HighestLoopPosition
        {
            get { return this.highestLoopPosition; }
        }

        public int HighestScoreOccurrences
        {
            get { return this.highestScoreOccurrences; }
        }

        public int HighestLoopOccurrences
        {
            get { return this.highestLoopOccurrences; }
        }
    }
}
