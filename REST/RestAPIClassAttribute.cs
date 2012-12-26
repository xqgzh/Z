using System;

namespace Z.Rest
{
    /// <summary>
    /// Rest类属性实体
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RestAPIClassAttribute : System.Attribute
    {
        /// <summary>
        /// 资源名称
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// 资源描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Rest类属性构造函数
        /// </summary>
        /// <param name="resourceName"></param>
        public RestAPIClassAttribute(string resourceName)
        {
            this.ResourceName = resourceName;
        }

        /// <summary>
        /// Rest类属性构造函数
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="description"></param>
        public RestAPIClassAttribute(string resourceName, string description)
        {
            this.ResourceName = resourceName;
            this.Description = description;
        }
    }
}