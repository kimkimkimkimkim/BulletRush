using System.Collections.Generic;
using UnityEngine;

public static class MasterRecords
{
    public static List<RateMB.Param> GetRateMB()
    {
        return Resources.Load<RateMB>("MasterRecords/RateMB").param;
    }

    public static List<DamageMB.Param> GetDamageMB()
    {
        return Resources.Load<DamageMB>("MasterRecords/DamageMB").param;
    }

    public static List<CoinMB.Param> GetCoinMB()
    {
        return Resources.Load<CoinMB>("MasterRecords/CoinMB").param;
    }

    public static List<OfflineRewardMB.Param> GetOfflineRewardMB()
    {
        return Resources.Load<OfflineRewardMB>("MasterRecords/OfflineRewardMB").param;
    }
}
