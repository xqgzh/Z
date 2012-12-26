using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.Entity;
using System.Diagnostics;
using Z.Test.Performance;
using System.Runtime.CompilerServices;
using System.Data;
using Z.Test.Model;

namespace Zata.Test
{
    [TestClass]
    public class EntityToolsTest
    {
        #region 基础测试

        [TestMethod]
        public void GetValue()
        {
            DataObjectModel obj = new DataObjectModel();

            string s = "test1234";

            obj.Name = s;

            string v = EntityTools<DataObjectModel>.GetValue(obj, "Name", false) as string;

            Assert.AreEqual(s, v);
        }

        [TestMethod]
        public void GetValueIgnoreCase()
        {
            DataObjectModel obj = new DataObjectModel();

            string s = "test1234";

            obj.Name = s;

            string v = EntityTools<DataObjectModel>.GetValue(obj, "naMe", true) as string;

            Assert.AreEqual(s, v);
        }

        [TestMethod]
        public void SetValue()
        {
            DataObjectModel obj = new DataObjectModel();

            string s = "test1234";

            obj.Name = "temp";
            EntityTools<DataObjectModel>.SetValue(obj, "Name", false, s);

            Assert.AreEqual(s, obj.Name);
        }

        [TestMethod]
        public void SetValueIgnoreCase()
        {
            DataObjectModel obj = new DataObjectModel();

            string s = "test1234";

            obj.Name = "temp";
            EntityTools<DataObjectModel>.SetValue(obj, "namE", true, s);

            Assert.AreEqual(s, obj.Name);
        }

        [TestMethod]
        public void TestEnum()
        {
            DataObjectModel obj = new DataObjectModel();

            obj.MyDay = Day.A1;

            EntityTools<DataObjectModel>.SetValue(obj, "MyDay", false, "A2");

            Assert.AreEqual(Day.A2, obj.MyDay);

            Day d = (Day)EntityTools<DataObjectModel>.GetValue(obj, "myDay", true);

            Assert.AreEqual(d, obj.MyDay);
        }

        [TestMethod]
        public void TestStruct()
        {
            DataObjectModel obj = new DataObjectModel();

            obj.StructTest.MyEnum = Day.A2;
            obj.StructTest.MyInt32 = 123;
            obj.StructTest.MyString = "temp";

            MyStruct m;

            m.MyEnum = Day.A5;
            m.MyInt32 = 999;
            m.MyString = "testStruct";

            EntityTools<DataObjectModel>.SetValue(obj, "StructTest", false, m);

            Assert.AreEqual(m.MyEnum, obj.StructTest.MyEnum);
            Assert.AreEqual(m.MyInt32, obj.StructTest.MyInt32);
            Assert.AreEqual(m.MyString, obj.StructTest.MyString);

            MyStruct d = (MyStruct)EntityTools<DataObjectModel>.GetValue(obj, "STRUCTTEST", true);

            Assert.AreEqual(m.MyEnum, d.MyEnum);
            Assert.AreEqual(m.MyInt32, d.MyInt32);
            Assert.AreEqual(m.MyString, d.MyString);
        }

        [TestMethod]
        public void TestInt32()
        {
            DataObjectModel obj = new DataObjectModel();

            obj.BookCount = 112;

            int i = 234234;

            EntityTools<DataObjectModel>.SetValue(obj, "BookCount", false, i);

            Assert.AreEqual(i, obj.BookCount);

            int d = (int)EntityTools<DataObjectModel>.GetValue(obj, "BookCount", true);

            Assert.AreEqual(d, obj.BookCount);
        }

        [TestMethod]
        public void TestDateTime()
        {
            DataObjectModel obj = new DataObjectModel();

            obj.CreateTime = DateTime.Parse("2010-09-18");

            DateTime i = DateTime.Parse("2012-2-1");

            EntityTools<DataObjectModel>.SetValue(obj, "CreateTime", false, "2012-2-1");

            Assert.AreEqual(i, obj.CreateTime);

            DateTime d = (DateTime)EntityTools<DataObjectModel>.GetValue(obj, "CreateTime", true);

            Assert.AreEqual(d, obj.CreateTime);
        }

        [TestMethod]
        public void TestString()
        {
            DataObjectModel obj = new DataObjectModel();

            obj.CreateTime = DateTime.Parse("2010-09-18");

            DateTime i = DateTime.Parse("2012-2-1");

            EntityTools<DataObjectModel>.SetValueString(obj, "CreateTime", false, "2012-2-1");

            Assert.AreEqual(i, obj.CreateTime);

            string s = EntityTools<DataObjectModel>.GetValueString(obj, "CreateTime", true);

            Assert.AreEqual(s, obj.CreateTime.ToString());
        }

        [TestMethod]
        public void TestEntityAlias()
        {
            DataObjectModel obj = new DataObjectModel();

            obj.Property = "temp";

            string s = "TestAlias";

            EntityTools<DataObjectModel>.SetValueString(obj, "Prop", false, s);

            Assert.AreEqual(s, obj.Property);

            string v = EntityTools<DataObjectModel>.GetValueString(obj, "prop", true);

            Assert.AreEqual(s, v);
        }

        [TestMethod]
        public void TestEntityAliasIDataObject()
        {
            DataObjectModel obj = new DataObjectModel();

            obj.IDataObjectSetProperty = "temp";

            string s = "TestAlias";

            EntityTools<DataObjectModel>.SetValueString(obj, "IDataObjectSetProperty", false, s);
            Assert.AreEqual(s, obj.IDataObjectSetProperty);

            obj.IDataObjectSetProperty = "temp";

            EntityTools<DataObjectModel>.SetValueString(obj, "prop2", true, s);
            Assert.AreEqual(s, obj.IDataObjectSetProperty);

            string v = EntityTools<DataObjectModel>.GetValueString(obj, "prop2", true);

            Assert.AreEqual(s, v);
        }

        [TestMethod]
        public void TestDataRow()
        {
            DataTable dt = EntityTools<DataObjectModel>.Table.Clone();
            DataObjectModel obj = new DataObjectModel();

            DataRow row = dt.NewRow();

            foreach(DataColumn c in dt.Columns)
            {
                if(c.DataType == typeof(System.Int32))
                {
                    row[c] = 345435;
                }
                else if(c.DataType == typeof(System.String))
                {
                    row[c] = c.ColumnName + "Value";
                }
                else if(c.DataType == typeof(System.DateTime))
                {
                    row[c] = DateTime.Now;
                }
                else if(c.DataType == typeof(MyStruct))
                {
                    row[c] = new MyStruct(){ MyEnum= Day.A4, MyInt32 = 3245, MyString = "wewr" };
                }
                else if(c.DataType == typeof(Day))
                {
                    row[c] = Day.A5;
                }
            }

            EntityTools<DataObjectModel>.FromDataRow(row, obj);
            Assert.AreEqual(obj.Name, "NameValue");

            Trace.Write(Z.Util.XmlTools.ToXml(obj));
        }

        #endregion

        #region 性能测试

        [TestMethod]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void Perf_GetSet()
        {
            Trace.WriteLine("获取/设置性能测试， 大小写敏感");
            GetSet(false);
        }

        [TestMethod]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void Perf_GetSet_IgnoreCase()
        {

            Trace.WriteLine("获取/设置性能测试， 忽略大小写");
            GetSet(true);
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private void GetSet(bool ignoreCase)
        {
            var obj = new DataObjectModel();
            IDataObject objIDataObject = obj as IDataObject;
            object glocalInstance = obj;
            string propertyName = "Name";
            var func = (Func<DataObjectModel, string>)Delegate.CreateDelegate(typeof(Func<DataObjectModel, string>), typeof(DataObjectModel).GetMethod("get_Name"));

            TestCase tester = new TestCase();

            tester.Watcher = (i, name, sw) =>
            {
                Trace.WriteLine(string.Format("{0}:{1}", name, sw.Elapsed));
            };

            tester.Build("直接调用时", () => { obj.NameField = obj.Name; });
            tester.Build("委托调用时", () => { var value = func(obj); });
            tester.Build("接口调用时", () => { objIDataObject.SetValue(propertyName, ignoreCase, objIDataObject.GetValue(propertyName, ignoreCase)); });
            tester.Build("工具调用时", () => { EntityTools<DataObjectModel>.SetValue(obj, propertyName, ignoreCase, EntityTools<DataObjectModel>.GetValue(obj, propertyName, ignoreCase)); });
            tester.Build("字符串工具", () => { EntityTools<DataObjectModel>.SetValueString(obj, propertyName, ignoreCase, EntityTools<DataObjectModel>.GetValueString(obj, propertyName, ignoreCase)); });
            tester.Build("虚接口调用", () => { obj.SetEntityValue(propertyName, ignoreCase, obj.GetEntityValue(propertyName, ignoreCase)); });

            tester.StepWatcher = i =>
            {
                Trace.WriteLine(string.Format("执行{0:###,###,###}次的结果: ", i));
            };

            int times = 10000;

            tester.Execute(1000 * times);
        }

        #endregion

    }
}
