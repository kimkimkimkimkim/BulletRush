using System.ComponentModel;

public class DefeatWindowRequest
{

}

public class DefeatWindowResponse
{
    /// <summary>
    /// コンティニューしたかどうか
    /// </summary>
    [Description("コンティニューしたかどうか")]
    public bool isContinue { get; set; }
}
