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
            //VerifyStartingPositions(CoinsToCalculate);
            PlayGames(CoinsToCalculate);
        }

        /*
         * Plays the game on every combination for each number of coins.
         * Outputs the highest score, loop and the combination they occur at for each number of coins.
         */
        static void PlayGames(int maxCoins)
        {
            DateTime start = DateTime.Now;

            PositionGenerator pg = new PositionGenerator();
            for (int coins = 1; coins <= maxCoins; coins++)
            {
                DateTime gspStart = DateTime.Now;
                List<StartPos> startingPositions = pg.Generate(coins);
                TimeSpan gspDur = DateTime.Now - gspStart;

                DateTime pgStart = DateTime.Now;
                GamePlayer pgame = new GamePlayer(startingPositions);
                pgame.Play(startingPositions);
                TimeSpan pgDur = DateTime.Now - pgStart;

                // the question said output *any* position for highest loop/score, so it only shows one (which can vary due to the multiple threads).
                // comment these out for more accurate algorithm execution time
                /*Console.WriteLine("Coins={0}", coins);
                Console.WriteLine(" Highest Score is {0} (shared by {1}/{2} positions) and first found at [{3}]",
                    pgame.HighestScore,
                    pgame.HighestScoreOccurrences,
                    startingPositions.Count,
                    String.Join(",", Array.ConvertAll<int, string>(pgame.HighestScorePosition, Convert.ToString)));
                Console.WriteLine(" Highest Loop is {0} (shared by {1}/{2} positions) and first found at [{3}]",
                    pgame.HighestLoop,
                    pgame.HighestLoopOccurrences,
                    startingPositions.Count,
                    String.Join(",", Array.ConvertAll<int, string>(pgame.HighestLoopPosition, Convert.ToString)));
                Console.WriteLine("TIMES pgen: {0}ms, pgame: {1}ms", gspDur.TotalMilliseconds, pgDur.TotalMilliseconds);
                Console.WriteLine();*/
            }


            Console.WriteLine("execution time {0}ms", (DateTime.Now - start).TotalMilliseconds);
            Console.ReadKey();
        }
    }
}