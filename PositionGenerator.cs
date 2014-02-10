using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rhul
{
    class PositionGenerator
    {
        private Dictionary<int, List<StartPos>> positionCache;
        public PositionGenerator()
        {
            this.positionCache = new Dictionary<int, List<StartPos>>();
        }

        private void GeneratePrevious(int upto)
        {
            for (int coins = 1; coins < upto; coins++)
            {
                this.Generate(coins);
            }
        }

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

        private void CreateRearrangements(int coins, List<StartPos> positions)
        {
            int pileLeft = coins - 1;
            int pileRight = 1;
            while (pileLeft >= pileRight)
            {
                AddPosition(positions, new[] { pileLeft, pileRight });
                if(pileLeft > 1)
                {
                    List<StartPos> leftPerms = this.positionCache[pileLeft];
                    MergePerms(positions, leftPerms, pileRight);
                }
                pileLeft--;
                pileRight++;
            }
        }

        private static void MergePerms(List<StartPos> positions, List<StartPos> leftPerms, int pileRight)
        {
            // start at 1 to skip the first position containing a whole pile of coins
            int startingIndex = 1;
            for (int i = startingIndex; i < leftPerms.Count; i++)
            {
                StartPos perm = leftPerms[i];
                bool isDescOrder = pileRight <= perm.position.Last();
                if (isDescOrder)
                {
                    StartPos newPos = new StartPos();
                    newPos.position = new int[perm.position.Length + 1];
                    newPos.ones = perm.ones + (pileRight == 1 ? 1 : 0);

                    // combine `pileRight` and the cached position to form a new position, which has a sum equal to `coins`
                    Array.Copy(perm.position, newPos.position, perm.position.Length);
                    newPos.position[newPos.position.Length - 1] = pileRight;
                    positions.Add(newPos);
                }
            }
        }

        private static void AddPosition(List<StartPos> positions, int[] piles)
        {
            StartPos wholePile = new StartPos();
            wholePile.position = piles;
            foreach (int pile in piles)
            {
                if (pile == 1)
                {
                    wholePile.ones++;
                }
            }
            positions.Add(wholePile);
        }
    }
}
