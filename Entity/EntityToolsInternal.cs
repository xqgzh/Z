using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Diagnostics;
using System.Data;
using System.Collections;

namespace Z.Entity
{
    /// <summary>
    /// 实体对象工具内部实现
    /// </summary>
    static class EntityToolsInternal
    {
        #region Func_SetValue

        /// <summary>
        /// Func_SetValue
        /// </summary>
        /// <returns></returns>
        public static Func<T, string, bool, TValue, bool> Func_SetValue<T, TValue>()
        {
            Type type = typeof(T);
            Type valueType = typeof(TValue);
            ParameterExpression objParameterExpr;
            ParameterExpression nameParameterExpr;
            ParameterExpression ignoreCaseParameterExpr;
            ParameterExpression valueParameterExpression;
            BlockExpression methodBodyExpr;
            
            Expr_SetValueMethodBody(
                type, 
                valueType, 
                out objParameterExpr, 
                out nameParameterExpr, 
                out ignoreCaseParameterExpr, 
                out valueParameterExpression, 
                out methodBodyExpr);

            var expr = Expression.Lambda<Func<T, string, bool, TValue, bool>>(
                methodBodyExpr, 
                objParameterExpr, 
                nameParameterExpr, 
                ignoreCaseParameterExpr, 
                valueParameterExpression);

            return expr.Compile();
        }

        #endregion

        #region Func_GetValue

        /// <summary>
        /// Func_GetValue
        /// </summary>
        /// <returns></returns>
        internal static Func<T, string, bool, TReturn> Func_GetValue<T, TReturn>()
        {
            var type = typeof(T);
            var returnType = typeof(TReturn);

            ParameterExpression objParameterExpr;
            ParameterExpression nameParameterExpr;
            ParameterExpression ignoreCaseParameterExpr;
            BlockExpression methodBodyExpr;
            
            Expr_GetValueMethodBody(
                type, 
                returnType, 
                out objParameterExpr, 
                out nameParameterExpr, 
                out ignoreCaseParameterExpr, 
                out methodBodyExpr);

            return Expression.Lambda<Func<T, string, bool, TReturn>>(
                methodBodyExpr, 
                objParameterExpr, 
                nameParameterExpr, 
                ignoreCaseParameterExpr).Compile();
        }

        #endregion

        #region CalculateFieldPropertys

        /// <summary>
        /// 获取指定类型的属性列表和数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FieldCount"></param>
        /// <param name="PropertyCount"></param>
        /// <param name="Fields"></param>
        /// <param name="Propertys"></param>
        /// <param name="FieldPropertys"></param>
        internal static void CalculateFieldPropertys<T>(
            ref int FieldCount, ref int PropertyCount, 
            ref string[] Fields, ref string[] Propertys,
            ref string[] FieldPropertys)
        {
            Type type = typeof(T);
            List<string> FieldList = new List<string>();
            List<string> PropertyList = new List<string>();
            List<string> FieldOrPropertyList = new List<string>();

            MemberInfo[] memberList = type.GetMembers(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default);

            foreach (var member in memberList)
            {
                if (member.MemberType == MemberTypes.Property)
                {
                    PropertyList.Add(member.Name);
                    FieldOrPropertyList.Add(member.Name);
                }
                else if (member.MemberType == MemberTypes.Field)
                {
                    FieldList.Add(member.Name);
                    FieldOrPropertyList.Add(member.Name);
                }
            }

            FieldCount = FieldList.Count;
            PropertyCount = PropertyList.Count;
            

            Fields = FieldList.ToArray();
            Propertys = PropertyList.ToArray();
            FieldPropertys = FieldOrPropertyList.ToArray();
        }

        #endregion

        #region 内部方法

        #region Expr_GetValueMethodBody

        /// <summary>
        /// Expr_GetValueMethodBody
        /// </summary>
        /// <param name="type"></param>
        /// <param name="returnType"></param>
        /// <param name="objParameterExpr"></param>
        /// <param name="nameParameterExpr"></param>
        /// <param name="ignoreCaseParameterExpr"></param>
        /// <param name="methodBodyExpr"></param>
        private static void Expr_GetValueMethodBody(Type type, Type returnType, out ParameterExpression objParameterExpr, out ParameterExpression nameParameterExpr, out ParameterExpression ignoreCaseParameterExpr, out BlockExpression methodBodyExpr)
        {
            Type IDataObjectType = type.GetInterface("IDataObject");
            bool IsUseIDataObjectAll = IsIDataObjectGet(type);

            objParameterExpr = Expression.Parameter(type, "obj");
            nameParameterExpr = Expression.Parameter(typeof(string), "name");
            ignoreCaseParameterExpr = Expression.Parameter(typeof(bool), "ignoreCase");

            var methodBody = new List<Expression>();

            methodBodyExpr = null;

            if (IsUseIDataObjectAll)
            {
                if (returnType == typeof(object))
                {
                    methodBody.Add(
                        Expression.Call(objParameterExpr, IDataObjectType.GetMethod("GetValue"),
                            nameParameterExpr, ignoreCaseParameterExpr));
                }
                else if (returnType == typeof(string))
                {
                    methodBody.Add(
                        Expression.Call(
                            Method_ConvertString,
                            Expression.Call(
                                objParameterExpr,
                                IDataObjectType.GetMethod("GetValue"),
                                nameParameterExpr,
                                ignoreCaseParameterExpr
                            )
                        )
                    );
                }

                methodBodyExpr = Expression.Block(returnType, methodBody);
            }
            else
            {
                var v = Expression.Variable(typeof(int));
                var hashCode = Expression.Condition(
                    Expression.IsTrue(ignoreCaseParameterExpr),
                    Expression.Call(Method_GetHashCodeLower, nameParameterExpr),
                    Expression.Call(nameParameterExpr, typeof(string).GetMethod("GetHashCode")));
                var eva = Expression.Assign(v, hashCode);
                methodBody.Add(eva);

                MemberInfo[] memberList = type.GetMembers(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default);
                List<SwitchCase> caseExpressions = new List<SwitchCase>();
                foreach (var member in memberList)
                {
                    if (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field)
                        continue;

                    if (member.MemberType == MemberTypes.Property)
                    {
                        PropertyInfo p = member as PropertyInfo;

                        if (p.CanRead == false) continue;
                    }

                    Expression ConverterExpr = null;

                    if (IDataObjectType != null && IsIDataObjectGet(member))
                    {
                        if (returnType == typeof(object))
                            ConverterExpr =
                                Expression.Call(objParameterExpr, IDataObjectType.GetMethod("GetValue"),
                                nameParameterExpr,
                                ignoreCaseParameterExpr);
                        else
                            ConverterExpr = Expression.Convert(
                                Expression.Call(objParameterExpr, IDataObjectType.GetMethod("GetValue"),
                                nameParameterExpr,
                                ignoreCaseParameterExpr), returnType);
                    }
                    else
                    {
                        Expression memberValue = Expression.PropertyOrField(objParameterExpr, member.Name);
                        ConverterExpr = Expr_GetMemberValue(returnType, memberValue, ConverterExpr);
                    }

                    var caseExpr = Expression.SwitchCase(ConverterExpr, Expr_GetMemberNameHashCodes(member));
                    caseExpressions.Add(caseExpr);
                }

                Expression defaultCaseExpr = null;

                if (returnType == typeof(object))
                    defaultCaseExpr = Expression.Constant(null);
                else
                    defaultCaseExpr = Expression.Constant(string.Empty);

                var switchExpression = Expression.Switch(v,

                    defaultCaseExpr,

                    caseExpressions.ToArray());

                methodBody.Add(switchExpression);
                methodBodyExpr = Expression.Block(returnType, new[] { v }, methodBody);
            }
        }

        #endregion

        #region Expr_SetValueMethodBody

        /// <summary>
        /// Expr_SetValueMethodBody
        /// </summary>
        /// <param name="type"></param>
        /// <param name="valueType"></param>
        /// <param name="objParameterExpr"></param>
        /// <param name="nameParameterExpr"></param>
        /// <param name="ignoreCaseParameterExpr"></param>
        /// <param name="valueParameterExpression"></param>
        /// <param name="methodBodyExpr"></param>
        private static void Expr_SetValueMethodBody(Type type, Type valueType, out ParameterExpression objParameterExpr, out ParameterExpression nameParameterExpr, out ParameterExpression ignoreCaseParameterExpr, out ParameterExpression valueParameterExpression, out BlockExpression methodBodyExpr)
        {
            Type IDataObjectType = type.GetInterface("IDataObject");
            bool IsUseIDataObjectAll = IsIDataObjectGet(type);

            // public void Set(T obj, string name, object value)
            objParameterExpr = Expression.Parameter(type, "obj");
            nameParameterExpr = Expression.Parameter(typeof(string), "name");
            ignoreCaseParameterExpr = Expression.Parameter(typeof(bool), "ignoreCase");
            valueParameterExpression = Expression.Parameter(valueType, "value");

            var methodBody = new List<Expression>();

            methodBodyExpr = null;

            if (IsUseIDataObjectAll)
            {
                if (valueType == typeof(object))
                {
                    methodBody.Add(
                        Expression.Call(objParameterExpr, IDataObjectType.GetMethod("SetValue"),
                            nameParameterExpr, ignoreCaseParameterExpr, valueParameterExpression));
                }
                else if (valueType == typeof(string))
                {
                    methodBody.Add(
                        Expression.Call(
                            objParameterExpr,
                            IDataObjectType.GetMethod("SetValue"),
                            nameParameterExpr,
                            ignoreCaseParameterExpr,
                            Expression.Call(
                                Method_ConvertString,
                                valueParameterExpression
                            )
                        )

                    );
                }

                methodBodyExpr = Expression.Block(typeof(bool), methodBody);
            }
            else
            {

                var v = Expression.Variable(typeof(int));
                var hashCode = Expression.Condition(
                    Expression.IsTrue(ignoreCaseParameterExpr),
                    Expression.Call(Method_GetHashCodeLower, nameParameterExpr),
                    Expression.Call(nameParameterExpr, typeof(string).GetMethod("GetHashCode")));
                var eva = Expression.Assign(v, hashCode);

                methodBody.Add(eva);


                MemberInfo[] memberList = type.GetMembers(BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public);
                List<SwitchCase> caseExpressions = new List<SwitchCase>();
                foreach (var member in memberList)
                {
                    if (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field)
                        continue;

                    if (!IsMemberHaveSet(member))
                        continue;

                    var targetMember = Expression.PropertyOrField(objParameterExpr, member.Name);

                    // 类型正确
                    var SwitchCaseBlock = Expr_SetValue(
                        objParameterExpr, member, IDataObjectType, nameParameterExpr, ignoreCaseParameterExpr,
                        targetMember, valueParameterExpression);
                    SwitchCase caseExpr = Expression.SwitchCase(SwitchCaseBlock, Expr_GetMemberNameHashCodes(member));

                    caseExpressions.Add(caseExpr);
                }
                //var switchExpression = Expression.Switch(eva, Expression.Throw(Expression.New(typeof(ArgumentException))), caseExpressions.ToArray());

                Expression defaultCaseExpr = null;

                if (valueType == typeof(object))
                    defaultCaseExpr = Expression.Constant(null);
                else
                    defaultCaseExpr = Expression.Constant(string.Empty);

                var switchExpression = Expression.Switch(v, Expression.Constant(false), caseExpressions.ToArray());
                methodBody.Add(switchExpression);

                //组装函数, 注意局部变量在第二个参数注册
                methodBodyExpr = Expression.Block(typeof(bool), new[] { v }, methodBody);
            }
        }

        #endregion

        #region GetParameterName

        /// <summary>
        /// 获取IDataParameter的名称
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static string GetParameterName(IDataParameter p)
        {
            string pName = p.SourceColumn;

            if (string.IsNullOrEmpty(pName))
            {
                pName = p.ParameterName.Substring(1);
            }

            if (pName.StartsWith("Original_"))
                pName = pName.Remove(0, "Original_".Length);

            return pName;
        }

        #endregion

        #region Expr_GetMemberValue

        /// <summary>
        /// Expression 获取指定属性的值并返回结果
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="nameParameterExpr"></param>
        /// <param name="memberValue"></param>
        /// <param name="ConverterExpr"></param>
        /// <returns></returns>
        private static Expression Expr_GetMemberValue(Type returnType, Expression memberValue, Expression ConverterExpr)
        {
            if (memberValue.Type == returnType)
                ConverterExpr = memberValue;
            else if (returnType == typeof(object))
            {
                ConverterExpr = Expression.Convert(memberValue, returnType);
            }
            else if (returnType == typeof(string))
            {
                ConverterExpr = Expression.Call(Method_ConvertString, Expression.Convert(memberValue, typeof(object)));
            }
            else if (returnType == typeof(Int32))
            {
                ConverterExpr = Expression.Call(Method_ConvertString, Expression.Convert(memberValue, typeof(Int32)));
            }
            //else
            //{
            //    ConverterExpr = Expression.Call(Method_ConvertObject, nameParameterExpr, Expression.Constant(returnType), Expression.Constant(IsIConvertible(returnType)), memberValue);
            //}
            return ConverterExpr;
        }

        #endregion

        #region Expr_GetMemberNameHashCodes(获取特定属性名称的HashCode和小写的HashCode)

        /// <summary>
        /// 获取特定属性名称的HashCode和小写的HashCode
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private static Expression[] Expr_GetMemberNameHashCodes(MemberInfo member)
        {
            var testCases = new List<Expression>();

            
            // 计算HashCode的过程放在了代码生成阶段，使用Dictionary的方式时HashCode是执行时计算的。
            StringLowerTable.GetLowerHashCode(member.Name);
            StringLowerTable.GetLowerHashCode(member.Name.ToUpperInvariant());
            int a = member.Name.GetHashCode();
            int b = member.Name.ToLowerInvariant().GetHashCode();

            testCases.Add(Expression.Constant(a));
            testCases.Add(Expression.Constant(b));

            foreach (var attribute in member.GetCustomAttributes(true))
            {
                if (attribute is EntityAttribute)
                {
                    var compatibleName = (attribute as EntityAttribute).Name;

                    StringLowerTable.GetLowerHashCode(compatibleName);
                    StringLowerTable.GetLowerHashCode(compatibleName.ToLowerInvariant());

                    int c = compatibleName.GetHashCode();
                    int d = compatibleName.ToLowerInvariant().GetHashCode();

                    testCases.Add(Expression.Constant(c));
                    testCases.Add(Expression.Constant(d));
                }
            }

            return testCases.ToArray();
        }

        #endregion

        #region Expr_SetValue(设置指定属性的值,根据属性类型进行转换)

        /// <summary>
        /// 设置指定属性的值,根据属性类型进行转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="member"></param>
        /// <param name="IDataObjectType"></param>
        /// <param name="NameExpresssion"></param>
        /// <param name="IgnoreCaseExpression"></param>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static Expression Expr_SetValue(Expression obj, MemberInfo member, Type IDataObjectType, Expression NameExpresssion, Expression IgnoreCaseExpression, Expression target, Expression value)
        {
            //数据类型一致
            Expression Convertor = null;

            if (IDataObjectType != null)
            {
                bool IsIDataObject = IsIDataObjectSet(member);

                if (IsIDataObject)
                {
                    return Expression.Block(
                        Expression.Call(obj, IDataObjectType.GetMethod("SetValue"),
                        NameExpresssion,
                        IgnoreCaseExpression, value),
                        Expression.Constant(true));
                }
            }

            //判断target是否字符串
            if (target.Type == value.Type)
            {
                Convertor = value;
            }
            else if (target.Type == typeof(string) && value.Type == typeof(object))
            {

                Convertor = Expression.Condition(
                    Expression.TypeIs(value, typeof(string)),
                    Expression.TypeAs(value, typeof(string)),
                    Expression.Call(Method_ConvertString, value));
            }
            else if (target.Type == typeof(Int32) && value.Type == typeof(object))
            {
                Convertor = Expression.Condition(
                    Expression.TypeIs(value, typeof(Int32)),
                    Expression.Convert(value, typeof(Int32)),
                    Expression.Call(Method_ConvertInt32, value));
            }
            else if (target.Type == typeof(DateTime) && value.Type == typeof(object))
            {
                Convertor = Expression.Condition(
                    Expression.TypeIs(value, typeof(DateTime)),
                    Expression.Convert(value, typeof(DateTime)),
                    Expression.Call(Method_ConvertDateTime, value));
            }
            else
            {
                Convertor = Expression.Call(typeof(EntityToolsInternal), "ConvertObject", new Type[] { target.Type }, Expression.Constant(target.ToString()), Expression.Constant(target.Type), Expression.Constant(IsIConvertible(target.Type)), value);
                //Convertor = Expression.Convert(Expression.Call(Method_ConvertObject, Expression.Constant(target.ToString()), Expression.Constant(target.Type), Expression.Constant(IsIConvertible(target.Type)), value), target.Type);
            }

            //Field则直接转换, 如果是Property则调用方法
            var AssignExpr = (member is FieldInfo) ?
                (Expression.Assign(target, Convertor) as Expression) :
                Expression.Call(obj, (member as PropertyInfo).GetSetMethod(), Convertor);

            return Expression.Block(AssignExpr, Expression.Constant(true));
        }

        #endregion

        #region IsIConvertible(检查指定类型是否实现了IConvertible接口)

        static Type IConvertibleType = typeof(IConvertible);

        /// <summary>
        /// 检查指定类型是否实现了IConvertible接口
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static bool IsIConvertible(Type type)
        {
            foreach (var t in type.GetInterfaces())
                if (t == IConvertibleType) return true;

            return false;
        }

        #endregion

        #region ConvertObject

        /// <summary>
        /// ConvertObject
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="name"></param>
        /// <param name="targetType"></param>
        /// <param name="IsConvertible"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        static TTarget ConvertObject<TTarget>(string name, Type targetType, bool IsConvertible, object o)
        {
            if (o == null)
            {
                if (targetType.IsValueType)
                    throw new FormatException(name + "(" + targetType.Name + ")无法赋值为空");
                return default(TTarget);
            }

            if (o.GetType() == targetType) return (TTarget)o;

            if (IsConvertible)
            {
                if (targetType.IsEnum)
                    return (TTarget)Enum.Parse(targetType, o.ToString(), true);

                if (o is IConvertible)
                    return (TTarget)System.Convert.ChangeType(o, targetType);

                return (TTarget)System.Convert.ChangeType(o, targetType);
            }

            return (TTarget)o;
        }

        #endregion

        #region IsMemberHaveSet(检查指定的Member是否可写)

        /// <summary>
        /// 检查指定的Member是否可写
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        static bool IsMemberHaveSet(MemberInfo m)
        {
            PropertyInfo p = m as PropertyInfo;

            if (p != null)
                return p.CanWrite;

            FieldInfo f = m as FieldInfo;

            return true;
        }

        #endregion

        #region IsIDataObjectGet & IsIDataObjectSet

        /// <summary>
        /// IsIDataObjectGet
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        static bool IsIDataObjectGet(MemberInfo member)
        {
            foreach (var attribute in member.GetCustomAttributes(true))
            {
                EntityAttribute eaAttribute = attribute as EntityAttribute;

                if (eaAttribute != null)
                    return (eaAttribute.DataObjectUsage & EnumDataObjectUsage.GET) == EnumDataObjectUsage.GET;
            }

            return false;
        }

        /// <summary>
        /// IsIDataObjectSet
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        static bool IsIDataObjectSet(MemberInfo member)
        {
            foreach (var attribute in member.GetCustomAttributes(true))
            {
                EntityAttribute eaAttribute = attribute as EntityAttribute;

                if (eaAttribute != null)
                    return (eaAttribute.DataObjectUsage & EnumDataObjectUsage.SET) == EnumDataObjectUsage.SET;

            }

            return false;
        }

        #endregion

        #region DEBUG相关方法

        static MethodInfo Method_ConvertString = typeof(System.Convert).GetMethod("ToString", new Type[] { typeof(object) });
        static MethodInfo Method_ConvertInt32 = typeof(System.Convert).GetMethod("ToInt32", new Type[] { typeof(object) });
        static MethodInfo Method_ConvertInt64 = typeof(System.Convert).GetMethod("ToInt64", new Type[] { typeof(object) });
        static MethodInfo Method_ConvertDateTime = typeof(System.Convert).GetMethod("ToDateTime", new Type[] { typeof(object) });
        static MethodInfo Method_ConvertObject = typeof(EntityToolsInternal).GetMethod("ConvertObject", BindingFlags.NonPublic | BindingFlags.Static);
        static MethodInfo Method_GetHashCodeLower = typeof(StringLowerTable).GetMethod("GetLowerHashCode", BindingFlags.Static | BindingFlags.Public);

        static MethodInfo Method_TraceWriteLineString = typeof(Trace).GetMethod("WriteLine", new[] { typeof(string) });
        static MethodInfo Method_ConsoleWriteLineString = typeof(Console).GetMethod("WriteLine", new[] { typeof(string) });
        static MethodInfo Method_ConsoleWriteLineInt32 = typeof(Console).GetMethod("WriteLine", new[] { typeof(Int32) });
        static MethodInfo Method_ConsoleWriteLineObject = typeof(Console).GetMethod("WriteLine", new[] { typeof(object) });

        #endregion

        #endregion

        /// <summary>
        /// Func_GetValue
        /// </summary>
        /// <returns></returns>
        internal static Func<T, int, TReturn> Func_GetValue2<T, TReturn>()
        {
            var type = typeof(T);
            var returnType = typeof(TReturn);

            ParameterExpression objParameterExpr;
            ParameterExpression hashCodeParameterExpr;
            BlockExpression methodBodyExpr;

            Expr_GetValueMethodBody_NEW(
                type,
                returnType,
                out objParameterExpr,
                out hashCodeParameterExpr,
                out methodBodyExpr);

            return Expression.Lambda<Func<T, int, TReturn>>(
                methodBodyExpr,
                objParameterExpr,
                hashCodeParameterExpr).Compile();
        }

        #region Expr_GetValueMethodBody

        /// <summary>
        /// Expr_GetValueMethodBody
        /// </summary>
        /// <param name="type"></param>
        /// <param name="returnType"></param>
        /// <param name="objParameterExpr"></param>
        /// <param name="hashCodeParameterExpr"></param>
        /// <param name="methodBodyExpr"></param>
        private static void Expr_GetValueMethodBody_NEW(Type type, Type returnType, out ParameterExpression objParameterExpr, out ParameterExpression hashCodeParameterExpr, out BlockExpression methodBodyExpr)
        {
            Type IDataObjectType = type.GetInterface("IDataObject");
            bool IsUseIDataObjectAll = IsIDataObjectGet(type);

            objParameterExpr = Expression.Parameter(type, "obj");
            hashCodeParameterExpr = Expression.Parameter(typeof(Int32), "hashCode");

            var methodBody = new List<Expression>();

            methodBodyExpr = null;

            MemberInfo[] memberList = type.GetMembers(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default);


            List<SwitchCase> caseExpressions = new List<SwitchCase>();
            foreach (var member in memberList)
            {
                if (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field)
                    continue;

                if (member.MemberType == MemberTypes.Property)
                {
                    PropertyInfo p = member as PropertyInfo;

                    if (p.CanRead == false) continue;
                }

                Expression ConverterExpr = null;

                Expression memberValue = Expression.PropertyOrField(objParameterExpr, member.Name);
                ConverterExpr = Expr_GetMemberValue(returnType, memberValue, ConverterExpr);

                var caseExpr = Expression.SwitchCase(ConverterExpr, Expr_GetMemberNameHashCodes(member));
                caseExpressions.Add(caseExpr);

                Expression defaultCaseExpr = null;

                if (returnType == typeof(object))
                    defaultCaseExpr = Expression.Constant(null);
                else
                    defaultCaseExpr = Expression.Constant(string.Empty);

                var switchExpression = Expression.Switch(hashCodeParameterExpr,

                    defaultCaseExpr,

                    caseExpressions.ToArray());

                methodBody.Add(switchExpression);
                methodBodyExpr = Expression.Block(returnType, methodBody);
            }
        }

        #endregion
    }
}
