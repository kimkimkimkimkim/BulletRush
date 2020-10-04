using System.ComponentModel;

public class CommonDialogRequest
{
    /// <summary>
    /// タイトル
    /// </summary>
    [Description("タイトル")]
    public string title { get; set; }

    /// <summary>
    /// コンテンツテキスト
    /// </summary>
    [Description("コンテンツテキスト")]
    public string content { get; set; }
}

public class CommonDialogResponse
{
    /// <summary>
    /// レスポンスタイプ
    /// </summary>
    [Description("レスポンスタイプ")]
    public DialogUtil.DialogResponseType responseType { get; set; }
}

