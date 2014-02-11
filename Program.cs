using System;
using System.Collections.Generic;
using System.Linq;

namespace rhul
{
    class Program
    {
        const int CoinsToCalculate = 36;

        static void Main(string[] args)
        {
            PlayGames(CoinsToCalculate);
        }

        /// <summary>
        /// Plays the game on every combination for each number of coins. 
        /// Outputs the highest score, loop and the combination they occur at for each number of coins.
        /// </summary>
        static void PlayGames(int maxCoins)
        {
            DateTime start = DateTime.Now;

            PositionGenerator pg = new PositionGenerator();
            for (int coins = 1; coins <= maxCoins; coins++)
            {
                DateTime gspStart = DateTime.Now;
                List<StartPos> startingPositions = pg.Generate(coins);
                TimeSpan gspDur = DateTime.Now - gspStart;

                DateTime gpStart = DateTime.Now;
                GamePlayer gp = new GamePlayer(startingPositions);
                IGamePlayerResults results = gp.Play(startingPositions);
                TimeSpan gpDur = DateTime.Now - gpStart;

                // the question said output *any* position for highest loop/score, so it only shows one (which can vary due to the multiple threads).
                // comment these out for more accurate algorithm execution time
                Console.WriteLine("Coins={0}", coins);
                Console.WriteLine(" Highest Score is {0} (shared by {1}/{2} positions) and first found at [{3}]",
                    results.HighestScore,
                    results.HighestScoreOccurrences,
                    startingPositions.Count,
                    String.Join(",", Array.ConvertAll<int, string>(results.HighestScorePosition, Convert.ToString)));
                Console.WriteLine(" Highest Loop is {0} (shared by {1}/{2} positions) and first found at [{3}]",
                    results.HighestLoop,
                    results.HighestLoopOccurrences,
                    startingPositions.Count,
                    String.Join(",", Array.ConvertAll<int, string>(results.HighestLoopPosition, Convert.ToString)));
                Console.WriteLine("TIMES pos-gen: {0}ms, game: {1}ms", gspDur.TotalMilliseconds, gpDur.TotalMilliseconds);
                Console.WriteLine();
            }


            Console.WriteLine("Execution time {0}ms", (DateTime.Now - start).TotalMilliseconds);
            Console.ReadKey();
        }
    }
}