using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAdmin
{
    class Functions
    {
        /// <summary>
        /// Переводит название папки \\mssrv1c01\Obmen\Node\**-*** в ip аптеки 10.**.***.10
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static string translateFromTheNameOfTheFolderToThePharmacyNumber(string dir)
        {
            //Char ch = '\\';
            int indexForSlash = dir.LastIndexOf(@"\");
            dir = dir.Substring(indexForSlash + 1, dir.Length - indexForSlash - 1);
            string[] splitDir = dir.Split('-');
            try
            {
                if (splitDir[1].Length == 3) return $"10.{Convert.ToInt32(splitDir[0]).ToString()}.{Convert.ToInt32(splitDir[1]).ToString()}.10";
                else
                {
                    return $"10.{(Convert.ToInt32(splitDir[0]) + 10).ToString()}.{Convert.ToInt32(splitDir[1].Substring(3, splitDir[1].Length - 3)).ToString()}.10";
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Возвращает массив папок всех аптек
        /// </summary>
        /// <param name="folder">Папка, с которой беруться все атпеки</param>
        /// <returns></returns>
        private static DirectoryInfo[] getAllFolderForTranslate(string folder)
        {
            return new DirectoryInfo(folder).GetDirectories();
        }

        /// <summary>
        /// Формирует список всех IP адресов по названию папок, в указанной папке.
        /// </summary>
        /// <returns>Возвращает список IP адресов всех аптек по названию папок</returns>
        public static List<string> getIPforAllAptFromTheFolder(string folder)
        {
            //FileInfo[] files = getAllFolderForTranslate(@"\\mssrv1c01\Obmen\Node"); 
            DirectoryInfo[] directories = getAllFolderForTranslate(folder);
            List<string> result = new List<string>();
            foreach (DirectoryInfo directory in directories)
            {
                string temp = translateFromTheNameOfTheFolderToThePharmacyNumber(directory.FullName);
                if (temp == null) continue;
                result.Add(temp);
            }
            return result;
        }

        /// <summary>
        /// Пингует асинхронно одну машину с адресом adress
        /// </summary>
        /// <param name="IP машины для пинга"></param>
        /// <returns>Task<IPStatus></IPStatus></returns>
        public static async Task<IPStatus> pingAsync(string address)
        {
            IPAddress IP = null;
            IPAddress.TryParse(address, out IP);
            PingReply pr = await new Ping().SendPingAsync(IP);
            return pr.Status;
        }

        /// <summary>
        /// Получает последнюю версию программы
        /// </summary>
        /// <param name="urlRequest">Адрес для запроса</param>
        /// <returns>Возвращает последнюю версию программы</returns>
        public static string getLastVersion(Uri urlRequest)
        {
            return string.Empty;
        }

        //public static void formatComand(string comand)
        //{

        //}
    }
}
