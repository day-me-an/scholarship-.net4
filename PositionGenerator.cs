using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rhul
{
    /// <summary>
    /// Generates all the possible distinct starting positions in descending order for the inputted number of coins.
    /// By "distinct" and "descending order" I mean when calculating the positions 
    /// for 6 coins: [3,2,1] would be an acceptable combination and the following would not: 
    /// [1,2,3], [2,1,3], [2,3,1], [1,3,2] and [3,1,2].
    ///  
    /// 1) The algoritm splits the inputted number of coins into pairs of piles named 'left' and 'right', 
    /// which all sum to the inputted number of coins.
    /// 2) This process initially starts by placing all but one of the coins in the left pile, 
    /// and the remaining coin in the right pile.
    /// 3) The algorithm repeatedly moves a coin from the left to the right pile, on each repetition a 
    /// new pair is formed and the following is done:
    ///     i) add it to the 'positions' list.
    ///    ii) look up the left pile of the new pair in the position cache and for each do:
    ///        I) replace the current pair's left pile with the cached position
    ///        II) add to the positions list.
    /// 4) (3) is repeated until moving another coin would cause the right pile to be greater than the left.
    /// 5) Return the positions list
    /// 
    /// Example input/output: 
    /// input=2 output=[[2], [1,1]]
    /// input=3 output=[[3], [2,1], [1,1,1]]
    /// input=4 output=[[4], [3,1], [2,1,1], [1,1,1,1], [2,2]]
    /// </summary>
    class PositionGenerator
    {
        private Dictionary<int, List<StartPos>> positionCache;
        public PositionGenerator()
        {
            this.positionCache = new Dictionary<int, List<StartPos>>();
        }

        /// <summary>
        /// Fills the position cache with enough data to calculate the positions for upto coins.
        /// </summary>
        private void GeneratePrevious(int upto)
        {
            for (int coins = 1; coins < upto; coins++)
            {
                this.Generate(coins);
            }
        }

        /// <summary>
        /// Calculates the positions.
        /// </summary>
        public List<StartPos> Generate(int coins)
        {
            List<StartPos> positions = new List<StartPos>();
            AddPosition(positions, new[] { coins });
            if (coins > 1)
            {
                bool isEnoughCached = this.positionCache.ContainsKey(coins - 1);
                if (!isEnoughCached)
                {
                    GeneratePrevious(coins);
                }
                CreateRearrangements(coins, positions);
                this.positionCache[coins] = positions;
            }
            return positions;
        }

        /// <summary>
        /// Performs phase 2) of the algorithm.
        /// </summary>
        private void CreateRearrangements(int coins, List<StartPos> positions)
        {
            int pileLeft = coins - 1;
            int pileRight = 1;
            while (pileLeft >= pileRight)
            {
                AddPosition(positions, new[] { pileLeft, pileRight });
                if (pileLeft > 1)
                {
                    List<StartPos> leftPerms = this.positionCache[pileLeft];
                    MergePerms(positions, leftPerms, pileRight);
                }
                pileLeft--;
                pileRight++;
            }
        }

        /// <summary>
        /// Creates a new set of positions by combining each permutation of the left pile with the right pile.
        /// </summary>
        private static void MergePerms(List<StartPos> positions, List<StartPos> leftPerms, int pileRight)
        {
            // start at 1 to skip the first position containing a whole pile of coins
            int startingIndex = 1;
            for (int i = startingIndex; i < leftPerms.Count; i++)
            {
                StartPos perm = leftPerms[i];
                bool isDescOrder = pileRight <= perm.Piles.Last();
                if (isDescOrder)
                {
                    StartPos newPos = new StartPos();
                    newPos.Piles = new int[perm.Piles.Length + 1];
                    newPos.Ones = perm.Ones + (pileRight == 1 ? 1 : 0);

                    // combine `pileRight` and the cached position to form a new position, which has a sum equal to `coins`
                    Array.Copy(perm.Piles, newPos.Piles, perm.Piles.Length);
                    newPos.Piles[newPos.Piles.Length - 1] = pileRight;
                    positions.Add(newPos);
                }
            }
        }

        /// <summary>
        /// Creates a position from an array of piles with the correct number of ones.
        /// </summary>
        private static void AddPosition(List<StartPos> positions, int[] piles)
        {
            StartPos wholePile = new StartPos();
            wholePile.Piles = piles;
            foreach (int pile in piles)
            {
                if (pile == 1)
                {
                    wholePile.Ones++;
                }
            }
            positions.Add(wholePile);
        }
    }
}
