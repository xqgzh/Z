using System;

namespace Z.Entity
{
    /// <summary>
    /// Entity别名属性, 用于定义实体类属性的别名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class EntityAttribute : Attribute
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 是否使用IDataObject接口设置/读取(对象必须实现IDataObject接口)
        /// </summary>
        public EnumDataObjectUsage DataObjectUsage = EnumDataObjectUsage.NONE;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        public EntityAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 当前实体类是否使用IDataObject接口进行读取/设置
        /// </summary>
        /// <param name="HowToUserIDataObject"></param>
        public EntityAttribute(EnumDataObjectUsage HowToUserIDataObject)
        {
            DataObjectUsage = HowToUserIDataObject;
        }
    }
}
