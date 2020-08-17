using System.ComponentModel;

public class ClearWindowRequest
{
    public ClearResultData clearResultData;
}

public class ClearWindowResponse
{

}

public class ClearResultData
{
    public int rewardCoin { get; set; }
    public int rewardGem { get; set; }
}
