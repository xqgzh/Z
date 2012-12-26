using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Z.Entity
{
    /// <summary>
    /// IEntity扩展方法, 通过对虚接口IEntity增加扩展方法实现EntityTools功能
    /// </summary>
    public static class IEntityExtension
    {

        /// <summary>
        /// 取值
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="name">属性名称</param>
        /// <param name="IgnoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static object GetEntityValue<T>(this IEntity<T> entity, string name, bool IgnoreCase)
        {
            return EntityTools<T>.GetValue((T)entity, name, IgnoreCase);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="name">属性名称</param>
        /// <param name="IgnoreCase">是否忽略大小写</param>
        /// <param name="obj">属性值</param>
        public static void SetEntityValue<T>(this IEntity<T> entity, string name, bool IgnoreCase, object obj)
        {
            EntityTools<T>.SetValue((T)entity, name, IgnoreCase, obj);
        }
    }

}
