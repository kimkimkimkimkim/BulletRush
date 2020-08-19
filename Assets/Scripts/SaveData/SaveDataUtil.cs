public class SaveDataUtil
{
    public static class Status
    {
        #region KEY
        private const string RATE_LEVEL = "rate_level";
        private const string DAMAGE_LEVEL = "damage_level";
        private const string COIN_LEVEL = "coin_level";
        private const string OFFLINE_REWARD_LEVEL = "offline_reward_level";
        #endregion

        #region GET
        public static int GetRateLevel() 
        {
            return SaveData.GetInt(RATE_LEVEL, 1);
        }

        public static int GetDamageLevel() 
        {
            return SaveData.GetInt(DAMAGE_LEVEL, 1);
        }

        public static int GetCoinLevel()
        {
            return SaveData.GetInt(COIN_LEVEL, 1);
        }

        public static int GetOfflineRewardLevel()
        {
            return SaveData.GetInt(OFFLINE_REWARD_LEVEL, 1);
        }
        #endregion

        #region SET
        public static void SetRateLevel(int rateLevel)
        {
            SaveData.SetInt(RATE_LEVEL, rateLevel);
            SaveData.Save();
        }

        public static void SetDamageLevel(int damageLevel)
        {
            SaveData.SetInt(DAMAGE_LEVEL, damageLevel);
            SaveData.Save();
        }

        public static void SetCoinLevel(int coinLevel)
        {
            SaveData.SetInt(COIN_LEVEL, coinLevel);
            SaveData.Save();
        }

        public static void SetOfflineRewardLevel(int offlineRewardLevel)
        {
            SaveData.SetInt(OFFLINE_REWARD_LEVEL, offlineRewardLevel);
            SaveData.Save();
        }
        #endregion
    }
}
