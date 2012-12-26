using System;
using System.Collections.Generic;

namespace Z.Injection
{
    /// <summary>
    /// 
    /// </summary>
    public class Injector
    {
        private static Dictionary<Type, Type> registedInterfaces = new Dictionary<Type, Type>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        public static void Regist<I, T>()
            where T : I
        {
            registedInterfaces[typeof(I)] = typeof(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static I Resolve<I>(params object[] args)
        {
            Type type;
            if (registedInterfaces.TryGetValue(typeof(I), out type))
            {
                return (I)Activator.CreateInstance(type, args);
            }
            else
            {
                return default(I);
            }
        }
    }
}
