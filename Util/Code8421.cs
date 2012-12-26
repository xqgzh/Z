using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Z.Util
{
    /// <summary>
    /// 8421编码
    /// </summary>
    public class Code8421 : IComparer<Code8421>, IComparable<Code8421>
    {
        /// <summary>
        /// 实际值
        /// </summary>
        protected int value;

        /// <summary>
        /// 构造函数
        /// </summary>
        protected Code8421() { }

        /// <summary>
        /// 检查类型是否匹配
        /// </summary>
        /// <param name="searchType"></param>
        /// <returns></returns>
        public bool IsTrue(Code8421 searchType)
        {
            return (searchType.value & value) == searchType.value;
        }


        /// <summary>
        /// 转换ITEMS_SEARCH_TYPE为Int32
        /// </summary>
        /// <param name="searchType"></param>
        /// <returns></returns>
        public static implicit operator int(Code8421 searchType)
        {
            return searchType.value;
        }

        /// <summary>
        /// 转换Int32为ITEMS_SEARCH_TYPE
        /// </summary>
        /// <param name="searchType"></param>
        /// <returns></returns>
        public static implicit operator Code8421(int searchType)
        {
            var item = new Code8421() { value = searchType };

            return item;
        }


        #region IComparable<T> Members

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Code8421 other)
        {
            return other.value - value;
        }


        #endregion

        #region IComparer<Code8421> Members

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(Code8421 x, Code8421 y)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
