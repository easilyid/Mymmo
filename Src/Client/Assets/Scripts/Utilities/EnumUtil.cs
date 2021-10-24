using System;


class EnumUtil
{
    public static string GetEnumDescription(Enum enumValue)
    {
        var str = enumValue.ToString();
        var field = enumValue.GetType().GetField(str);
        var objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
        if (objs == null || objs.Length == 0) return str;
        System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
        return da.Description;
    }
}