
namespace rhul
{
    /// <summary>
    /// Represents a starting position.
    /// The number of ones in the position is stored because it can be used to calculate 
    /// the size of arrays when playing the game, avoiding resizing of lists.
    /// </summary>
    public class StartPos
    {
        public int[] Piles { get; set; }
        /// <summary>
        /// The number of piles in this position with just one coin.
        /// Examples: [1] = 1 ones
        ///           [2,2,1,1,1] = 3 ones
        /// </summary>
        public int Ones { get; set; }
    }
}
