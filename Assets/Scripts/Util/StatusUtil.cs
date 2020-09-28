using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusUtil
{
    /// <summary>
    /// 必要コストを適切な表記に変換して返す
    /// </summary>
    public static string GetCostText(int cost) {
        return GetCostText((float)cost);
    }

    private static string GetCostText(float cost, int index = 0) {
        var UNIT_NAME_LIST = new List<string> { "", "k", "m", "b" };

        if(cost < 1000)
        {
            if (index >= UNIT_NAME_LIST.Count) return "-"; // TODO: 対応桁数を増やす

            string cost_str;
            if(cost < 10)
            {
                cost_str = String.Format("{0:F2}", cost);
            }
            else if(cost < 100)
            {
                cost_str = String.Format("{0:F1}", cost);
            }
            else
            {
                cost_str = String.Format("{0:F0}", cost);
            }
            return cost_str + UNIT_NAME_LIST[index];
        }
        else {
            return GetCostText(cost / 1000, index + 1);
        }
    }
}
