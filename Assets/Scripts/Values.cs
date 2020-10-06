public class Values
{
    public int MaxFavorityLevel { get { return accumFavTable.Length; } }
    public int MaxFavorityValue { get { return accumFavTable[MaxFavorityLevel - 1]; } }

    public int[] accumFavTable;

    public int[] scopeTimeUpCost;
    public int[] scopeTimeTable;
    public int[] observeCost;
    public int[] fastCompleteCost;
    public int[] scopeCharUpCost;
    public int[] scopeCharCount;

    public int[] lobbyCharUpCost;
    public int[] lobbyCharCount;

    public int[] storyUnlockCost;
}