using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Z.Util
{
    /// <summary>
    /// 网络连通性工具
    /// </summary>
    public static class NetTools
    {
        /// <summary>
        /// Ping
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Timeout"></param>
        public static void Ping(string Address, int Timeout)
        {
            using (System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping())
            {
                if (Timeout == 0) Timeout = 5000;
                if (p.Send(Address, Timeout).Status != System.Net.NetworkInformation.IPStatus.Success)
                {
                    throw new Exception("Ping " + Address + " Error");
                }
            }
        }

        /// <summary>
        /// Ping 不通则抛出异常
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Timeout"></param>
        /// <returns></returns>
        public static bool PingWithoutException(string Address, int Timeout)
        {
            using (System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping())
            {
                if (Timeout == 0) Timeout = 5000;
                if (p.Send(Address, Timeout).Status != System.Net.NetworkInformation.IPStatus.Success)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Telnet 端口
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Port"></param>
        /// <param name="Timeout"></param>
        public static void TelnetWithPing(string Address, int Port, int Timeout)
        {
            Ping(Address, Timeout);

            Telnet(Address, Port, Timeout);
        }

        /// <summary>
        /// 检查端口是否存在, 如果IP不存在, 会导致高达20秒的超时
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Port"></param>
        /// <param name="Timeout"></param>
        public static void Telnet(string Address, int Port, int Timeout)
        {
            using (TcpClient tcp = new TcpClient())
            {
                if (Timeout == 0) Timeout = 5000;
                tcp.SendTimeout = Timeout;
                tcp.ReceiveTimeout = Timeout;

                tcp.Connect(Address, Port);

                if (!tcp.Connected)
                {
                    throw new Exception(Address + ":" + Port + "端口连接失败");
                }

                tcp.Close();
            }
        }
    }
}
