using System;
using System.Collections.Generic;
using System.Text;

namespace Z.Enc
{
    /// <summary>
    /// 密码生成器, 从指定的序列中产生指定长度的无序字符串
    /// </summary>
    public class PasswordMaker
    {
        static int Indexer = 0;
        static Random rnd;
        static object o;
        static readonly char[] CharList = {
'b','2','B','2','a','1','A','1','x','$',
'X','$','y','M','7','4','z','E','8','j',
'n','h','L','v','3','k','Y','@','c','F',
'Z','7','d','H','N','9','R','K','C','#',
'S','3','D','Q','l','4','J','%','9','@',
'[','T','#','G','w',']','5','I',')','(',
'6','!','V','W','g','5','O',')','i','6',
'&',']','m','8','[','(','e','!','P','r',
'f','0','U','0','o','t','s','%','p','q',
'u','&'
        };
        static int iCharListLen;

        static PasswordMaker()
        {
            rnd = new Random();
            iCharListLen = CharList.Length;
            o = new object();
        }

        /// <summary>
        /// 产生一个无序字符串
        /// </summary>
        /// <param name="Max"></param>
        /// <returns></returns>
        public static string Make(UInt16 Max)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Max - 1; i++)
            {
                int x = rnd.Next(iCharListLen - 1) + 1;

                sb.Append(CharList[x]);
            }

            lock (o)
            {
                Indexer = (Indexer + 1) % iCharListLen;

                sb.Append(CharList[Indexer]);

            }

            return sb.ToString();
        }
    }
}
