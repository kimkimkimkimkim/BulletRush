using System.ComponentModel;

public class OfflineRewardReceiveDialogRequest
{
    /// <summary>
    /// コンテンツテキスト
    /// </summary>
    [Description("コンテンツテキスト")]
    public string content { get; set; }
}

public class OfflineRewardReceiveDialogResponse
{
    /// <summary>
    /// ボーナス（動画視聴）かどうか
    /// </summary>
    [Description("ボーナス（動画視聴）かどうか")]
    public bool isBonus { get; set; }
}

