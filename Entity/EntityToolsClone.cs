using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using Z.Entity.Extensions;

namespace Z.Entity
{
    /// <summary>
    /// 实体拷贝工具, 用于在两个实体对象之间实现拷贝
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTO"></typeparam>
    public class EntityTools<TFrom, TTO>
    {
        private static Action<TFrom, TTO> copyTo;
        private static Action<TFrom, TTO> copyToIgnoreCase;
        private static Action<TFrom, TTO> emtptyAction = new Action<TFrom, TTO>((s, d) => { });

        static EntityTools()
        {
            copyTo = CreateCopyToDelegate();
            copyToIgnoreCase = CreateCopyToDelegate(true);
        }

        /// <summary>
        /// 从a对象拷贝到b对象
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="ignoreCase"></param>
        public static void CopyTo(TFrom a, TTO b, bool ignoreCase = false)
        {
            (ignoreCase ? copyToIgnoreCase : copyTo)(a, b);
        }

        internal static Action<TFrom, TTO> CreateCopyToDelegate(bool ignoreCase = false)
        {
            var aType = typeof(TFrom);
            var bType = typeof(TTO);

            var a = Expression.Parameter(aType, "a");
            var b = Expression.Parameter(bType, "b");
            var methodBody = new List<Expression>();

            var aMemberList = aType.GetMembers(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default);
            var bMemberList = bType.GetMembers(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default);
            foreach (var memberInfo in aMemberList)
            {
                if (memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property)
                    continue;

                var bMember = FindCompatibleMember(bMemberList, memberInfo.Name, memberInfo.MemberType, ignoreCase);
                if (bMember == null)
                    continue;

                if (bMember.MemberType == MemberTypes.Property)
                {
                    PropertyInfo p = bMember as PropertyInfo;

                    if (p.CanWrite == false) continue;
                }

                var retType = memberInfo.GetReturnType();
                if (retType != bMember.GetReturnType())
                    continue;

                var aField = Expression.PropertyOrField(a, memberInfo.Name);
                var bField = Expression.PropertyOrField(b, bMember.Name);
                // 值类型直接拷贝
                if (retType.IsValueType || retType == typeof(string))
                {
                    methodBody.Add(Expression.Assign(bField, aField));
                }
                // 引用类型优先使用IClonable接口进行对象拷贝，如果没有实现IClonable接口，则执行浅拷贝。
                else
                {
                    if (retType.GetInterface("ICloneable") != null)
                    {
                        var aClone = Expression.Convert(Expression.Call(aField, retType.GetMethod("Clone")), retType);
                        methodBody.Add(Expression.Assign(bField, aClone));
                    }
                    else
                        methodBody.Add(Expression.Assign(bField, aField));
                }
            }

            if (methodBody.Count > 0)
            {
                var methodBodyExpr = Expression.Block(typeof(void), null, methodBody);

                return Expression.Lambda<Action<TFrom, TTO>>(methodBodyExpr, a, b).Compile();
            }
            else
                return emtptyAction;
        }

        private static MemberInfo FindCompatibleMember(MemberInfo[] memberList, string targetName, MemberTypes types, bool ignoreCase)
        {
            var bMember = memberList.FirstOrDefault(m => IsMemberMatchName(m, targetName, ignoreCase) && m.MemberType == types);
            if (bMember == null)
                memberList.FirstOrDefault(m => IsMemberMatchName(m, targetName, ignoreCase));

            return bMember;
        }

        private static bool IsMemberMatchName(MemberInfo member, string targetName, bool ignoreCase)
        {
            var compatibleNames = new List<string>() { member.Name };
            compatibleNames.AddRange(from a in member.GetCustomAttributes(true)
                                     where a is EntityAttribute
                                     select (a as EntityAttribute).Name);

            return compatibleNames.Any(name => String.Compare(name, targetName, ignoreCase) == 0);
        }
    }
}
