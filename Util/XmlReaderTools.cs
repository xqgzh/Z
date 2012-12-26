using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Z.Util
{
    /// <summary>
    /// 针对XmlReader的处理工具
    /// </summary>
    public static class XmlReaderTools
    {
        #region 从XmlReader中解析实体对象

        /// <summary>
        /// 从XmlReader中解析实体对象
        /// </summary>
        /// <typeparam name="T">IDataObject, 具有空构造函数</typeparam>
        /// <param name="t">实体对象</param>
        /// <param name="reader">XmlReader</param>
        /// <param name="EntityName">实体对象的Xml标签名称</param>
        /// <param name="IsProcessAttribute">是否遍历属性</param>
        /// <param name="IsProcessSubElement">是否遍历子节点</param>
        /// <returns></returns>
        public static bool ExtractEntity<T>(T t, XmlReader reader, string EntityName, bool IsProcessAttribute, bool IsProcessSubElement) where T : IDataObject, new()
        {
            //移动到指定的实体类节点
            if (!reader.ReadToFollowing(EntityName)) return false;


            //解析属性
            if (IsProcessAttribute && reader.HasAttributes)
                while (reader.MoveToNextAttribute())
                    if (reader.HasValue && string.IsNullOrEmpty(reader.Value) == false)
                        t.SetValue(reader.Name, reader.Value);

            if (IsProcessSubElement)
            {
                bool IsSkip = false;
                while (IsSkip || reader.Read())
                {
                    IsSkip = false;
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        string name = reader.Name;
                        string value = reader.ReadString();

                        if (!string.IsNullOrEmpty(value))
                            t.SetValue(name, value);

                        if (string.Compare(name, reader.Name) != 0)
                            //说明有变化
                            IsSkip = true;
                    }
                }
            }

            return true;
        }

        #endregion

        #region 从Xml文档中解析实体对象列表

        /// <summary>
        /// 从Xml文档中解析实体对象列表
        /// </summary>
        /// <typeparam name="T">IDataObject, 具有空构造函数</typeparam>
        /// <param name="reader">XmlReader</param>
        /// <param name="EntityName">实体对象的Xml标签名称</param>
        /// <param name="IsProcessAttribute">是否遍历属性</param>
        /// <returns></returns>
        public static List<T> ExtractEntityList<T>(XmlReader reader, string EntityName, bool IsProcessAttribute) where T : IDataObject, new()
        {
            List<T> list = new List<T>();
            //移动到指定的实体类节点
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && string.Compare(EntityName, reader.Name) == 0)
                {
                    T t = new T();

                    using (XmlReader SubReader = reader.ReadSubtree())
                    {
                        ExtractEntity(t, SubReader, EntityName, IsProcessAttribute, true);
                    }

                    list.Add(t);
                }
            }

            return list;
        }

        #endregion

        #region 从Xml文档中解析复杂实体对象列表

        /// <summary>
        /// 从Xml文档中解析实体对象列表
        /// </summary>
        /// <typeparam name="T">IDataObject, 具有构造函数的列表类型对象</typeparam>
        /// <typeparam name="T1">IDataObject, 具有构造函数的默认对象</typeparam>
        /// <param name="RootName">默认解析对象的根节点</param>
        /// <param name="info">默认解析对象</param>
        /// <param name="reader">XmlReader</param>
        /// <param name="EntityName">实体对象的Xml标签名称</param>
        /// <param name="IsProcessAttribute">是否遍历属性</param>
        /// <returns></returns>
        public static List<T> ExtractEntityList<T1, T>(string RootName, T1 info, XmlReader reader, string EntityName, bool IsProcessAttribute) where T1: IDataObject, new() where T:IDataObject, new()
        {
            if (!reader.ReadToFollowing(RootName)) return null;
            List<T> list = new List<T>();
            //移动到指定的实体类节点
            bool IsSkip = false;
            while (IsSkip == true || reader.Read())
            {
                IsSkip = false;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string NodeName = reader.Name;
                    if (string.Compare(EntityName, NodeName) == 0)
                    {
                        T t = new T();

                        using (XmlReader SubReader = reader.ReadSubtree())
                        {
                            ExtractEntity(t, SubReader, EntityName, IsProcessAttribute, true);
                        }

                        list.Add(t);
                    }
                    else
                    {
                        //不是实体对象的内容, 所以要遍历
                        if (reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                                if (reader.HasValue && string.IsNullOrEmpty(reader.Value) == false)
                                    info.SetValue(NodeName, reader.Value);
                        }
                        else if (reader.IsEmptyElement == false) 
                        {
                            //默认情况下无法被序列化的内容属于属性
                            string value = reader.ReadString();

                            //如果ReadString之后Name变化, 说明已经移动到下一个节点了
                            if(string.Compare(NodeName, reader.Name) != 0)
                                IsSkip = true;
                            if (!string.IsNullOrEmpty(value))
                            {
                                info.SetValue(reader.Name, value);
                            }
                        }
                                

                        

                    }
                }
            }

            return list;
        }

        #endregion

        #region 从Xml文本中解析实体对象

        /// <summary>
        /// 从Xml文本中解析实体对象
        /// </summary>
        /// <typeparam name="T">IDataObject, 具有空构造函数</typeparam>
        /// <param name="t">实体对象</param>
        /// <param name="Xml">Xml内容</param>
        /// <param name="EntityName">实体对象的Xml标签名称</param>
        /// <param name="IsProcessAttribute">是否遍历属性</param>
        /// <param name="IsProcessSubElement">是否遍历子节点</param>
        /// <returns>如果成功赋值, 则返回true</returns>
        public static bool ExtractEntityFromXml<T>(T t, string Xml, string EntityName, bool IsProcessAttribute, bool IsProcessSubElement) where T : IDataObject, new()
        {
            XmlReaderSettings setting = new XmlReaderSettings();

            setting.CloseInput = true;
            setting.IgnoreComments = true;

            using(StringReader sr = new StringReader(Xml))
            {
                using (XmlReader reader = XmlReader.Create(sr, setting))
                {
                    ExtractEntity<T>(t, reader, EntityName, IsProcessAttribute, IsProcessSubElement);
                }
            }
            return true;
        }

        #endregion

        #region 从Xml文档中解析实体对象列表

        /// <summary>
        /// 从Xml文档中解析实体对象列表
        /// </summary>
        /// <typeparam name="T">IDataObject, 具有空构造函数</typeparam>
        /// <param name="Xml">XML文本</param>
        /// <param name="EntityName">实体对象的Xml标签名称</param>
        /// <param name="IsProcessAttribute">是否遍历属性</param>
        /// <returns></returns>
        public static List<T> ExtractEntityListFromXml<T>(string Xml, string EntityName, bool IsProcessAttribute) where T : IDataObject, new()
        {
            List<T> list = null;
            XmlReaderSettings setting = new XmlReaderSettings();

            setting.CloseInput = true;
            setting.IgnoreComments = true;

            using (StringReader sr = new StringReader(Xml))
            {
                using (XmlReader reader = XmlReader.Create(sr, setting))
                {
                    list = ExtractEntityList<T>(reader, EntityName, IsProcessAttribute);
                }
            }

            return list;
        }

        #endregion

    }
}
