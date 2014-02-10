using System.Collections.Generic;

namespace rhul
{
    public class Game
    {
        private int[] currentPos;
        private int ones = 0, score, loop;
        // stores all previous positions, including the starting position, so the algorithm can detect a repeated position
        private List<int[]> moveHistory = new List<int[]>();

        public Game(StartPos startPos)
        {
            this.currentPos = startPos.position;
            this.ones = startPos.ones;
            this.moveHistory.Add(this.currentPos);
        }

        public void Play()
        {
            bool isLastMoveRepeated = false;
            // performs a move on the current position until a repeated position occurs
            while(!isLastMoveRepeated)
            {
                // perform a move on the current position
                this.PerformMove();

                int moveIndex;
                CheckRepeated(out isLastMoveRepeated, out moveIndex);

                if (isLastMoveRepeated)
                {
                    this.loop = this.score - moveIndex;
                }
                else
                {
                    this.moveHistory.Add(this.currentPos);
                }
                this.score++;
            }
        }

        private void CheckRepeated(out bool _isLastMoveRepeated, out int repeatedMoveNumber)
        {
            bool isLastMoveRepeated = false;
            int moveIndex = this.moveHistory.Count - 1;
            // loop backwards from the last position
            for (; !isLastMoveRepeated && moveIndex >= 0; moveIndex--)
            {
                int[] prevPos = this.moveHistory[moveIndex];

                // only compare positions with equal length to the current move
                if (prevPos.Length == this.currentPos.Length)
                {
                    // compare the previous position with the current
                    bool matches = true;
                    for (int i = 0; matches && i < this.currentPos.Length; i++)
                    {
                        if (prevPos[i] != this.currentPos[i])
                        {
                            matches = false;
                        }
                    }

                    // there is a repeated position if they match
                    isLastMoveRepeated = matches;
                }
            }

            _isLastMoveRepeated = isLastMoveRepeated;
            repeatedMoveNumber = moveIndex;
        }

        /*
         * Performs a move on the current position in the game. The algorithm works as follows:
         * 
         * 1) Subtracts one coin from each stack in the current position
         * 2) Creates a new stack from the coins subtracted, which is just the input position's size.
         * 3) Inserts the new stack at the correct location for the new position to be in descending order.
        */
        private void PerformMove()
        {
            // use the number of ones in the current position to calculate the exact size for the new position array
            int[] newPos = new int[this.currentPos.Length - this.ones + 1];
            int newPile = this.currentPos.Length;
            int newPosIndex = 0;
            // reset the number of ones, as its next value will be determined by the new position created by this method
            this.ones = 0;

            if (newPile == 1)
                this.ones++;

            bool hasAddedNewPile = false;
            // add the stacks to the new position by subtracting one from each stack and insert the new stack at the correct location to maintain descending order
            foreach(int pile in this.currentPos)
            {
                int updatedPile = pile - 1;
                if (updatedPile > 0)
                {
                    if (updatedPile == 1)
                        this.ones++;

                    // add the new stack
                    if (!hasAddedNewPile && updatedPile <= newPile)
                    {
                        newPos[newPosIndex++] = newPile;
                        hasAddedNewPile = true;
                    }

                    newPos[newPosIndex++] = updatedPile;
                }
            }

            // if the new stack was too small to be addded above, it must go on the end
            if (!hasAddedNewPile)
            {
                newPos[newPosIndex] = newPile;
            }

            this.currentPos = newPos;
        }

        public int Score
        {
            get { return this.score; }
        }

        public int Loop
        {
            get { return this.loop; }
        }
    }
}
