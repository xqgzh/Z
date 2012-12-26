using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Z.Util
{
    /// <summary>
    /// IP掩码比较工具
    /// 如果不指定掩码， 则自动根据IP地址生成掩码， 0=0， 非0=255
    /// </summary>
    /// <example>
    /// 方法1
    /// IPMaskCalculator imc = IPMaskCalculator.Parse("61.183.13.0", "255.255.255.0");
    /// bool result = imc.Compare("61.183.13.156");
    /// 方法2
    /// List&lt;IPMaskCalculator&gt; imc = IPMaskCalculator.Parse("61.183.13.0,255.255.255.0;61.183.0.0;");
    /// bool result = imc[0].Compare("61.183.13.156");
    /// 方法3
    /// 
    /// </example>
    public class IPMaskCalculator
    {
        #region 不允许直接创建

        private byte[] MaskBytes;
        private byte[] IpBytes;

        private IPMaskCalculator()
        {
        }

        #endregion

        #region 初始化掩码和比较值， 如果掩码为空， 则自动识别

        /// <summary>
        /// 解析输入的字符串为一个IPMaskCalculator列表, 格式: IP1,Mask1;IP2,Mask2
        /// </summary>
        /// <param name="IpListString"></param>
        /// <returns>IPMask列表</returns>
        public static List<IPMaskCalculator> Parse(string IpListString)
        {
            List<IPMaskCalculator> list = new List<IPMaskCalculator>();

            string[] IpListArray = IpListString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string iplist in IpListArray)
            {
                string[] ip = Split(iplist);

                IPMaskCalculator imc = null;
                if (ip.Length == 1)
                    IPMaskCalculator.TryParse(ip[0], "", out imc);
                else if (ip.Length > 1)
                    IPMaskCalculator.TryParse(ip[0], ip[1], out imc);

                if (list != null)
                    list.Add(imc);
            }

            return list;
        }

        #region 通用格式识别

        /// <summary>
        /// 从字符串中拆分IP地址, 支持,或者^$^或者^, 自动过滤空格
        /// </summary>
        /// <param name="iplist">IP地址和掩码列表</param>
        /// <returns></returns>
        private static string[] Split(string iplist)
        {
            string[] ip = null;

            if (iplist.IndexOf("^$^") >= 0)
                ip = iplist.Split(new string[] { "^$^" }, StringSplitOptions.RemoveEmptyEntries);
            else if (iplist.IndexOf('(') >= 0)
                ip = iplist.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            else if (iplist.IndexOf('^') >= 0)
                ip = iplist.Split(new char[] { '^' }, StringSplitOptions.RemoveEmptyEntries);
            else if (iplist.IndexOf(',') >= 0)
                ip = iplist.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            else
                ip = new string[] { iplist };
            return ip;
        }

        #endregion

        #endregion

        #region 初始化掩码和比较值, 掩码可以为空， 如果掩码为空， 则自动识别

        /// <summary>
        /// 初始化掩码和比较值, 掩码可以为空， 如果掩码为空， 则自动识别
        /// </summary>
        /// <param name="Mask"></param>
        /// <param name="IpCompare"></param>
        /// <returns></returns>
        public static IPMaskCalculator Parse(string IpCompare, string Mask)
        {
            IPAddress ipCompare = IPAddress.Parse(IpCompare);

            IPMaskCalculator imc = new IPMaskCalculator();
            imc.IpBytes = ipCompare.GetAddressBytes();

            IPAddress ipMask;
            if (IPAddress.TryParse(Mask, out ipMask))
            {
                imc.MaskBytes = ipMask.GetAddressBytes();
            }
            else
            {
                imc.MaskBytes = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    if (imc.IpBytes[i] == 0) imc.MaskBytes[i] = 0;
                    else imc.MaskBytes[i] = 255;
                }
            }

            return imc;
        }

        /// <summary>
        /// 初始化掩码和比较值，如果异常则返回false， 如果掩码为空， 则自动识别
        /// </summary>
        /// <param name="Mask"></param>
        /// <param name="IpCompare"></param>
        /// <param name="caculater"></param>
        /// <returns></returns>
        public static bool TryParse(string IpCompare, string Mask, out IPMaskCalculator caculater)
        {
            caculater = null;
            try
            {
                caculater = Parse(IpCompare, Mask);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 比较ipString与结果是否一致

        /// <summary>
        /// 比较ipString与结果是否一致
        /// </summary>
        /// <param name="ipString"></param>
        /// <returns></returns>
        public bool Compare(string ipString)
        {
            if (MaskBytes == null || IpBytes == null)
            {
                throw new NullReferenceException("IPMaskCaculater没有初始化");
            }

            IPAddress ipAddress = IPAddress.Parse(ipString);

            byte[] ip = ipAddress.GetAddressBytes();

            byte[] result = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                result[i] = Convert.ToByte(ip[i] & MaskBytes[i]);

                if (result[i] != IpBytes[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// 比较ipString与结果是否一致
        /// </summary>
        /// <param name="ip">4个byte字段表示的IP地址</param>
        /// <returns></returns>
        public bool Compare(byte[] ip)
        {
            if (MaskBytes == null || IpBytes == null)
            {
                throw new NullReferenceException("IPMaskCaculater没有初始化");
            }

            byte[] result = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                result[i] = Convert.ToByte(ip[i] & MaskBytes[i]);

                if (result[i] != IpBytes[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// 计算ip与IPMaskCalculator的掩码计算结果
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>与掩码进行与运算之后的结果</returns>
        public string Compute(string ip)
        {
            byte[] IP1 = IPAddress.Parse(ip).GetAddressBytes();

            byte[] bytes = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                bytes[i] = Convert.ToByte(IP1[i] & MaskBytes[i]);
            }

            return bytes[0] + "." + bytes[1] + "." + bytes[2] + "." + bytes[3];
        }

        #endregion
        
        #region 计算两个IP地址的与结果

        /// <summary>
        /// 计算两个IP地址的与结果
        /// </summary>
        /// <param name="ip1"></param>
        /// <param name="ip2"></param>
        /// <returns></returns>
        public static string Compute(string ip1, string ip2)
        {
            byte[] IP1 = IPAddress.Parse(ip1).GetAddressBytes();
            byte[] IP2 = IPAddress.Parse(ip2).GetAddressBytes();

            byte[] bytes = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                bytes[i] = Convert.ToByte(IP1[i] & IP2[i]);
            }

            return bytes[0] + "." + bytes[1] + "." + bytes[2] + "." + bytes[3];
        }

        #endregion

        #region 计算IP与IPMaskCalculator列表是否吻合， 不吻合返回false

        /// <summary>
        /// 计算IP与IPMaskCalculator列表是否吻合, 找到一个吻合的就返回
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool CompareTrue(string ip, List<IPMaskCalculator> list)
        {
            byte[] ipBytes = IPAddress.Parse(ip).GetAddressBytes();
            foreach (IPMaskCalculator imc in list)
                if (imc.Compare(ipBytes) == true) return true;
            return true;
        }

        /// <summary>
        /// 计算IP与IPMaskCalculator列表是否吻合, 找到一个不吻合的就返回
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool CompareFalse(string ip, List<IPMaskCalculator> list)
        {
            byte[] ipBytes = IPAddress.Parse(ip).GetAddressBytes();
            foreach (IPMaskCalculator imc in list)
                if (imc.Compare(ipBytes) == false) return false;
            return true;
        }

        /// <summary>
        /// 按照拒绝优先的规则，比较允许列表和拒绝列表
        /// 如果IP地址不合法, 除非允许列表和禁止列表都为空， 否则全部为禁止
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="AllowList"></param>
        /// <param name="DenyList"></param>
        /// <returns></returns>
        public static bool Compare(string ip, List<IPMaskCalculator> AllowList, List<IPMaskCalculator> DenyList)
        {
            //没做限制直接返回
            if (AllowList.Count == 0 && DenyList.Count == 0)
                return true;

            IPAddress ipAddress;
            
            //如果IP地址不合法
            if (IPAddress.TryParse(ip, out ipAddress) == false)
                return false;

            byte[] ipBytes = ipAddress.GetAddressBytes();

            return Compare(ipBytes, AllowList, DenyList);

        }

        /// <summary>
        /// 按照拒绝优先的规则，比较允许列表和拒绝列表
        /// 如果IP地址不合法, 除非允许列表和禁止列表都为空， 否则全部为禁止
        /// 禁止返回false;
        /// 允许返回true
        /// </summary>
        /// <param name="ipBytes"></param>
        /// <param name="AllowList"></param>
        /// <param name="DenyList"></param>
        /// <returns>禁止返回false, 允许返回true</returns>
        public static bool Compare(byte[] ipBytes, List<IPMaskCalculator> AllowList, List<IPMaskCalculator> DenyList)
        {
            //没做限制直接返回
            if (AllowList.Count == 0 && DenyList.Count == 0)
                return true;

            //拒绝优先， 一个满足条件都返回false
            foreach (IPMaskCalculator imc in DenyList)
            {
                if (imc.Compare(ipBytes) == true)
                    return false;
            }

            //允许列表， 满足条件都返回true
            foreach (IPMaskCalculator imc in AllowList)
            {
                if (imc.Compare(ipBytes) == true)
                    return true;
            }

            //都过了， 返回true;
            if (AllowList.Count > 0)
                return false; //有允许列表，没有符合条件的算是拒绝

            return true;
        }

        #endregion
    }
}
