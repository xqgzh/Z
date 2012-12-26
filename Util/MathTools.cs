using System;
using System.Collections.Generic;
using System.Text;

namespace Z.Util
{
    /// <summary>
    /// 数学计算工具
    /// </summary>
    public static class MathTools
    {
        #region MinValue

        /// <summary>
        /// 比较传入的类型为 T的数组， 返回最小值, T必须支持 IComparable 接口
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static T MinValue<T>(params T[] numbers) where T: IComparable
        {
            if (numbers == null || numbers.Length == 0)
                throw new ArgumentNullException("参数为空");

            T min = numbers[0];

            foreach (T i in numbers)
            {
                if ((i as IComparable).CompareTo(min) < 0)
                    min = i;
            }

            return min;
        }

        #endregion

        #region MaxValue

        /// <summary>
        /// 比较传入的类型为 T的数组， 返回最小值, T必须支持 IComparable 接口
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static T MaxValue<T>(params T[] numbers) where T : IComparable
        {
            if (numbers == null || numbers.Length == 0)
                throw new ArgumentNullException("参数为空");

            T max = numbers[0];

            foreach (T i in numbers)
            {
                if ((i as IComparable).CompareTo(max) > 0)
                    max = i;
            }

            return max;
        }

        #endregion
    }
}
