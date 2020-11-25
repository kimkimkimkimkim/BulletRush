using System.ComponentModel;

public class ClearWindowRequest
{
    /// <summary>
    /// リザルトデータ
    /// </summary>
    public ClearResultData clearResultData;
}

public class ClearWindowResponse
{
    /// <summary>
    /// ボーナスかどうか
    /// </summary>
    public bool isBonus;
}

public class ClearResultData
{
    public int rewardCoin { get; set; }
    public int rewardGem { get; set; }
}
