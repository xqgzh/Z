using System;
using System.Collections.Generic;

namespace Z.OAuth
{
    /// <summary>
    /// Comparer class used to perform the sorting of the query parameters
    /// </summary>
    public class QueryParameterComparer : IComparer<QueryParameter>
    {
        #region IComparer<QueryParameter> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(QueryParameter x, QueryParameter y)
        {
            if (x.Name == y.Name)
            {
                if (x.Value is IComparable && y.Value is IComparable)
                {
                    return (x.Value as IComparable).CompareTo(y.Value);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            else
            {
                return String.Compare(x.Name, y.Name);
            }
        }

        #endregion
    }
}
