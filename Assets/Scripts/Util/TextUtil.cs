using System;
using System.Linq;

public static class TextUtil { 

    /// <summary>
    /// フォーマット化した文字列をdouble値に戻して返す
    /// </summary>
    public static double GetDeserializedValue(string value) {
        var lastChar = value.Substring(value.Length - 1);
        var unit = "";
        var unitType = default(UnitType);
        foreach(UnitType u in Enum.GetValues(typeof(UnitType)))
        {
            if (lastChar == u.GetName())
            {
                unit = u.GetName();
                unitType = u;
            }
        }

        var numStr = unit == "" ? value : value.Substring(0, value.Length - 1);
        var num = 0d;
        if (double.TryParse(numStr, out num))
        {
            return num * (Math.Pow(1000,(int)unitType));
        }
        else
        {
            return 0d;
        }
    }

    /// <summary>
    /// 与えられたdouble値をフォーマット化し文字列として返す
    /// </summary>
    public static string GetFormattedValue(double value)
    {
        // 小数点の位置と数字のみの配列を作成
        var valueStringList = value.ToString().ToCharArray().Select(c => c.ToString()).ToList();
        var periodIndex = valueStringList.FindIndex(s => s == ".");
        if (periodIndex == -1) periodIndex = valueStringList.Count;
        valueStringList = valueStringList.Where(s => s != ".").ToList();

        var unitType = (UnitType)((int)Math.Floor((double)(periodIndex - 1) / 3));
        var unitName = unitType.GetName();
        var index = periodIndex % 3;

        for (var i = 0; i < 3 - valueStringList.Count; i++)
        {
            valueStringList.Add("0");
        }

        if (index != 0) valueStringList.Insert(index, ".");
        var length = index == 0 ? 3 : 4;
        var valueString = string.Join("", valueStringList.Take(length).ToArray()) + unitName;
        return valueString;
    }

    public static string GetName(this UnitType unitType)
    {
        switch (unitType)
        {
            case UnitType.None:
                return "";
            case UnitType.Kilo:
                return "K";
            case UnitType.Million:
                return "M";
            case UnitType.Billion:
                return "B";
            default:
                return "";
        }
    }

    public enum UnitType
    {
        None = 0,
        Kilo = 1,
        Million = 2,
        Billion = 3,
    }
}