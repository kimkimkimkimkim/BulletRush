using System.ComponentModel;

public class GameWindowRequest
{
    /// <summary>
    /// シミュレーション結果（平均、最大）
    /// </summary>
    public string simulationResultText { get; set; }

    /// <summary>
    /// ステージID
    /// </summary>
    public int stageId { get; set; }
}

public class GameWindowResponse
{

}
