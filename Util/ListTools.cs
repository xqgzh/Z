using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Z.Util
{
    /// <summary>
    /// 集合操作函数
    /// </summary>
    public class ListTools
    {

        /// <summary>
        /// 将集合平均拆分为N个子集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="SplitCount"></param>
        /// <returns></returns>
        public static List<T>[] Split<T>(List<T> source, int SplitCount)
        {
            if (source == null || source.Count == 0 || SplitCount <= 0) return null;

            List<T>[] ListArray = new List<T>[SplitCount];

            for (int i = 0; i < SplitCount; i++) ListArray[i] = new List<T>();

            int CurrentArrayIndex = 0;

            for (int index = 0, j = source.Count; index < j; index++)
            {
                ListArray[CurrentArrayIndex].Add(source[index]);
                CurrentArrayIndex = (CurrentArrayIndex + 1) % SplitCount;
            }

            return ListArray;
        }

        /// <summary>
        /// 将集合平均拆分为N个子集合
        /// </summary>
        /// <param name="source"></param>
        /// <param name="ListArray"></param>
        public static void Split(IList source, params IList[] ListArray)
        {
            if (source == null || source.Count == 0 || ListArray == null || ListArray.Length == 0) return;

            int SplitCount = ListArray.Length;

            int CurrentArrayIndex = 0;

            for (int index = 0, j = source.Count; index < j; index++)
            {
                ListArray[CurrentArrayIndex].Add(source[index]);
                CurrentArrayIndex = (CurrentArrayIndex + 1) % SplitCount;
            }
        }
    }
}
