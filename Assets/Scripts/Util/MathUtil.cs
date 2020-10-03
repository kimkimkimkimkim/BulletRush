using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtil
{
    /// <summary>
    /// 極座標から直交座標に変換する(angle:0~360)
    /// </summary>
    public static Vector2 PolarToCartesian(float r,float angle)
    {
        var x = r * Math.Cos((double)DegreeToRadian(angle));
        var y = r * Math.Sin((double)DegreeToRadian(angle));
        return new Vector2((float)x, (float)y);
    }

    /// <summary>
    /// 角度(° : 度数法の角度)をラジアンに変換する
    /// </summary>
    /// <param name="degree">Degree.</param>
    public static float DegreeToRadian(float degree)
    {
        return (float)((degree * Math.PI) / 180);
    }
}
