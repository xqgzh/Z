using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Z.Entity;

namespace Z.Test.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class DataObjectModel : DynamicModel, IDataObject, ICloneable, IEntity<DataObjectModel>
    {
        #region IDataObject Members

        public bool SetValue(string dataName, bool IgnoreCase, object dataValue)
        {
            if (!IgnoreCase)
            {
                switch (dataName)
                {
                    case "IDataObjectSetProperty":
                    case "Prop2":
                        IDataObjectSetProperty = Convert.ToString(dataValue);
                        break;
                    case "Property": Property = Convert.ToString(dataValue); break;
                    case "Name": Name = Convert.ToString(dataValue); break;
                    case "Key": Key = Convert.ToString(dataValue); break;
                    case "Namespace": Namespace = Convert.ToString(dataValue); break;
                    case "Hash": Hash = Convert.ToString(dataValue); break;
                    case "ReferenceCount": ReferenceCount = Convert.ToString(dataValue); break;
                    case "NameField": NameField = Convert.ToString(dataValue); break;
                    case "KeyField": KeyField = Convert.ToString(dataValue); break;
                    case "StructTest": StructTest = (MyStruct)Convert.ChangeType(dataValue, typeof(MyStruct)); break;
                    case "CreateTime": CreateTime = Convert.ToDateTime(dataValue); break;
                    case "MyDay": MyDay = (Day)Enum.Parse(typeof(Day), Convert.ToString(dataValue), true); break;
                    case "BookCount": BookCount = Convert.ToInt32(dataValue); break;
                    default:
                        return false;
                }
            }
            else
            {
                switch (dataName.ToLower())
                {
                    case "idataobjectsetproperty":
                    case "prop2":
                        IDataObjectSetProperty = Convert.ToString(dataValue);
                        break;

                    case "property": Property = Convert.ToString(dataValue); break;
                    case "name": Name = Convert.ToString(dataValue); break;
                    case "key": Key = Convert.ToString(dataValue); break;
                    case "namespace": Namespace = Convert.ToString(dataValue); break;
                    case "hash": Hash = Convert.ToString(dataValue); break;
                    case "referencecount": ReferenceCount = Convert.ToString(dataValue); break;
                    case "namefield": NameField = Convert.ToString(dataValue); break;
                    case "keyfield": KeyField = Convert.ToString(dataValue); break;
                    case "structtest": StructTest = (MyStruct)Convert.ChangeType(dataValue, typeof(MyStruct)); break;
                    case "createtime": CreateTime = Convert.ToDateTime(dataValue); break;
                    case "myday": MyDay = (Day)Enum.Parse(typeof(Day), Convert.ToString(dataValue), true); break;
                    case "bookcount": BookCount = Convert.ToInt32(BookCount); break;
                    default:
                        return false;
                }
            }

            return true;
        }

        public object GetValue(string dataName, bool IgnoreCase)
        {
            if (!IgnoreCase)
            {
                switch (dataName)
                {
                    case "Property": return Property;
                    case "Name": return Name;
                    case "Key": return Key;
                    case "Namespace": return Namespace;
                    case "Hash": return Hash;
                    case "ReferenceCount": return ReferenceCount;
                    case "NameField": return NameField;
                    case "KeyField": return KeyField;
                    case "CreateTime": return CreateTime;
                    case "MyDay": return MyDay;
                    case "BookCount": return BookCount;
                    case "StructTest": return StructTest;
                    default:
                        return null;
                }
            }
            else
            {
                switch (dataName.ToLower())
                {
                    case "property": return Property;
                    case "name": return Name;
                    case "key": return Key;
                    case "namespace": return Namespace;
                    case "hash": return Hash;
                    case "referencecount": return ReferenceCount;
                    case "namefield": return NameField;
                    case "keyfield": return KeyField;
                    case "createtime": return CreateTime;
                    case "myday": return MyDay;
                    case "bookcount": return BookCount;
                    case "structtest": return StructTest;
                    default:
                        return null;
                }
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }

    public class DynamicModel
    {
        [Entity("Prop")]
        public string Property { get; set; }

        [Entity("Prop2", DataObjectUsage=EnumDataObjectUsage.SET)]
        public string IDataObjectSetProperty { get; set; }

        [Entity("sda")]
        public string Name { get; set; }

        public string Key { get; set; }

        public string Namespace { get; set; }

        public string Hash { get; set; }

        public string ReferenceCount { get; set; }

        public string NameField;

        public string KeyField;

        public Day MyDay = Day.A1;

        public MyStruct StructTest;

        public int BookCount;

        public DateTime CreateTime;

        public void Set(object o)
        {
            Name = Convert.ToString(o);
        }

        public object Get()
        {
            return Name;
        }
    }

    public enum Day
    {
        A1 = 1,
        A2, A3, A4, A5
    }

    public struct MyStruct
    {
        public int MyInt32;
        public string MyString;
        public Day MyEnum;
    }
}
