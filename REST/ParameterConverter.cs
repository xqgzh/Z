using System;

namespace Z.Rest
{
    /// <summary>
    /// 参数类型转换
    /// </summary>
    public static class ParameterConverter
    {
        /// <summary>
        ///  将请求中参数的字符串值转换为指定的枚举类型
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Enum ParseEnum(string s, Type type, Enum defaultValue)
        {
            return s == null ? defaultValue : (Enum)Enum.Parse(type, s, true);
        }

        /// <summary>
        ///  将请求中参数的字符串值转换为指定的枚举类型
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Enum ParseEnumNotNullable(string s, Type type)
        {
            Enum en = (Enum)Enum.Parse(type, s, true);
            return en;
        }
    }
}
