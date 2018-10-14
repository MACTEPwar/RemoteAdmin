using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteAdmin_Client_
{
    public static class Functions
    {
        public static string pingAllChildren(string myIp)
        {
            object locker = new object();
            string res = string.Empty;
            for (int i = 1; i < 5; i++)
            {
                int j = i;
                Task.Run(async () =>
                {
                    string ip = myIp.Remove(myIp.LastIndexOf('.'), myIp.Length - myIp.LastIndexOf('.')) + $".{j + 10}";

                    IPStatus t = await pingAsync($"{ip}");
                    lock (locker)
                    {
                        res += $"{ip} : {t.ToString()}\n";
                    }
                }).Wait();
                //string ip = myIp.Remove(myIp.LastIndexOf('.'), myIp.Length - myIp.LastIndexOf('.')) + $".{j + 10}";
                //ThreadPool.QueueUserWorkItem(pingAsync(ip));
            }
            return res;
        }

        /// <summary>
        /// Пингует асинхронно одну машину с адресом adress
        /// </summary>
        /// <param name="IP машины для пинга"></param>
        /// <returns>Task<IPStatus></IPStatus></returns>
        private static async Task<IPStatus> pingAsync(string address)
        {
            IPAddress IP = null;
            IPAddress.TryParse(address, out IP);
            PingReply pr = await new Ping().SendPingAsync(IP);
            return pr.Status;
        }
    }
}
