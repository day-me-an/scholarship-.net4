using System.Collections.Generic;

namespace rhul
{
    public interface IGameResult
    {
        int Score { get; }
        int Loop { get; }
    }

    /// <summary>
    /// Plays the game on a starting position.
    /// </summary>
    public class Game
    {
        private class GameResult : IGameResult
        {
            public int Score { get; set; }
            public int Loop { get; set; }
        }

        private StartPos currentPos;
        /// <summary>
        /// stores all previous positions, including the starting position, so the algorithm can detect a repeated position
        /// </summary>
        private List<int[]> moveHistory = new List<int[]>();

        public Game(StartPos startPos)
        {
            this.currentPos = startPos;
            this.moveHistory.Add(startPos.Piles);
        }

        /// <summary>
        /// Repeatedly plays a round of the game until a repeated position occurs.
        /// </summary>
        public IGameResult Play()
        {
            GameResult result = new GameResult();

            bool isLastMoveRepeated = false;
            // performs a move on the current position until a repeated position occurs
            while (!isLastMoveRepeated)
            {
                // perform a move on the current position
                this.PerformMove();

                int moveIndex;
                CheckRepeated(out isLastMoveRepeated, out moveIndex);

                if (isLastMoveRepeated)
                {
                    result.Loop = result.Score - moveIndex;
                }
                else
                {
                    this.moveHistory.Add(this.currentPos.Piles);
                }
                result.Score++;
            }

            return result;
        }

        /// <summary>
        /// Determines whether a repeated position has occured and at what move number.
        /// </summary>
        private void CheckRepeated(out bool out_isLastMoveRepeated, out int out_repeatedMoveNumber)
        {
            bool isLastMoveRepeated = false;
            int moveIndex = this.moveHistory.Count - 1;
            // loop backwards from the last position
            for (; !isLastMoveRepeated && moveIndex >= 0; moveIndex--)
            {
                int[] prevPos = this.moveHistory[moveIndex];

                // only compare positions with equal length to the current move
                if (prevPos.Length == this.currentPos.Piles.Length)
                {
                    // compare the previous position with the current
                    bool matches = true;
                    for (int i = 0; matches && i < this.currentPos.Piles.Length; i++)
                    {
                        if (prevPos[i] != this.currentPos.Piles[i])
                        {
                            matches = false;
                        }
                    }

                    // there is a repeated position if they match
                    isLastMoveRepeated = matches;
                }
            }

            out_isLastMoveRepeated = isLastMoveRepeated;
            out_repeatedMoveNumber = moveIndex;
        }

        /// <summary>
        /// Performs a move on the current position in the game. The algorithm works as follows:
        /// 1) Subtracts one coin from each pile in the current position.
        /// 2) Creates a new pile from the coins subtracted, which is just the input position's size.
        /// 3) Inserts the new pile at the correct location for the new position to be in descending order.
        /// </summary>
        private void PerformMove()
        {
            // use the number of ones in the current position to calculate the exact size for the new position array
            StartPos newPos = new StartPos();
            newPos.Piles = new int[this.currentPos.Piles.Length - this.currentPos.Ones + 1];

            int newPile = this.currentPos.Piles.Length;
            if (newPile == 1)
            {
                newPos.Ones++;
            }

            int newPosIndex = 0;
            bool hasAddedNewPile = false;
            // add the piles to the new position by subtracting one from each pile and insert the new pile at the correct location to maintain descending order
            foreach (int pile in this.currentPos.Piles)
            {
                int updatedPile = pile - 1;
                if (updatedPile > 0)
                {
                    if (updatedPile == 1)
                    {
                        newPos.Ones++;
                    }
                    // add the new pile if it belongs at this position
                    if (!hasAddedNewPile && updatedPile <= newPile)
                    {
                        newPos.Piles[newPosIndex++] = newPile;
                        hasAddedNewPile = true;
                    }
                    // add the updated pile
                    newPos.Piles[newPosIndex++] = updatedPile;
                }
            }

            // if the new pile was too small to be addded above, it must go on the end
            if (!hasAddedNewPile)
            {
                newPos.Piles[newPosIndex] = newPile;
            }

            this.currentPos = newPos;
        }
    }
}
