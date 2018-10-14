using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteAdmin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Клиент UDP
        /// </summary>
        UdpUser client;

        /// <summary>
        /// Начальные настройки программы
        /// </summary>
        /// <param name="intervalForTimerPing">Интервал таймера для пинга в секундах</param>
        private void startProgram(int intervalForTimerPing)
        {
            timer_tick();
            //создаем и запускаем таймер для пинга
            System.Windows.Forms.Timer timerForPing = new System.Windows.Forms.Timer();
            //timerForPing(null, null);
            timerForPing.Interval = intervalForTimerPing * 1000;
            timerForPing.Tick += (_event, _object) => {
                timer_tick();
            };
            timerForPing.Start();
        }

        private void timer_tick()
        {
            object locker = new object();
            label1.Text = label2.Text = 0.ToString();
            //хожу по всем строкам dataGridView
            foreach (DataGridViewRow dgvR in dataGridView1.Rows)
            {
                Task.Run(async () =>
                {
                    //запускаю асинхронный пинг
                    IPStatus t = await Functions.pingAsync(dataGridView1[0, dgvR.Index].Value.ToString());
                    if (t == IPStatus.Success)
                    {
                        //записую положительный результат пинга
                        lock (locker)
                        {
                            dataGridView1.Rows[dgvR.Index].DefaultCellStyle.BackColor = Color.Green;
                            label1.Text = (Convert.ToInt32(label1.Text) + 1).ToString();
                        }
                    }
                    else
                    {
                        //записую отрицательный результат пинга
                        lock (locker)
                        {
                            dataGridView1.Rows[dgvR.Index].DefaultCellStyle.BackColor = Color.Red;
                            label2.Text = (Convert.ToInt32(label2.Text) + 1).ToString();
                        }
                    }
                    //записую значение пинга
                    dataGridView1[1, dgvR.Index].Value = t;
                });
            }
        }

        /// <summary>
        /// Кнопка "Пинг"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            //Заполняем аптеками из папки обменов
            List<string> ipAllApt = Functions.getIPforAllAptFromTheFolder(@"\\mssrv1c01\Obmen\Node");
            foreach(string IP in ipAllApt)
            {
                dataGridView1.Rows.Add(IP);
            }
            //запускаем функцию для сарта программы
            startProgram(10);
        }

        CookieContainer cookies = new CookieContainer();
        private string get_http(string url)
        {
            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:17.0) Gecko/20100101 Firefox/17.0";
            //req.CookieContainer = cookies;
            //req.Headers.Add("DNT", "1");
            //req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            //Stream stream = resp.GetResponseStream();
            //StreamReader sr = new StreamReader(stream);
            //resp.Close();
            //string text = sr.ReadToEnd();
            //sr.Close();
            //return text;
            //using (var sw = new StreamWriter("page1.html"))
            //    sw.Write(text);
            WebRequest webr = WebRequest.Create(url);
            HttpWebResponse resp = null;

            try
            {
                resp = (HttpWebResponse)webr.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream, Encoding.GetEncoding(resp.CharacterSet));

            return sr.ReadToEnd();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(translateFromTheNameOfTheFolderToThePharmacyNumber(@"\\mssrv1c01\Obmen\Node\02-0228"));
            MessageBox.Show(get_http("https://raw.githubusercontent.com/MACTEPwar/blog/master/requests.php"));
            
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // Проверить (нумерация строк)
            int index = e.RowIndex;
            string indexStr = (index + 1).ToString();
            object header = this.dataGridView1.Rows[index].HeaderCell.Value;
            if (header == null || !header.Equals(indexStr))
                this.dataGridView1.Rows[index].HeaderCell.Value = indexStr;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(e.ToString());
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendComand();
            }
        }

        private void sendComand()
        {
            textBox1.Text.LastIndexOf("c>");
            // textBox1.Text.Substring(textBox1.Text.LastIndexOf("c>")+2); - команда
            client.Send(textBox1.Text.Substring(textBox1.Text.LastIndexOf("c>") + 2));
            //UDPSocket _socket = new UDPSocket();
            //_socket.Client(getLocalIP().ToString(), 27000);
            // для следующей командa
            textBox1.Text += "c>";
            textBox1.SelectionStart = textBox1.Text.Length;
        }

        /// <summary>
        /// Получает локальный IP адрес
        /// </summary>
        /// <returns></returns>
        private IPAddress getLocalIP()
        {
            IPAddress ip = null;
            for (int i = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Length - 1;i>=0;i--)
            {
                ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[i];
                if (Dns.GetHostEntry(Dns.GetHostName()).AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    break;
                }
            }
            return ip;
        }

        /// <summary>
        /// Фильр для IP
        /// </summary>
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // возвращаю все поля
            TextBox t = sender as TextBox;
            var tasks2 = new List<Task>();
            int i = 0;
            foreach (DataGridViewRow tt in dataGridView1.Rows)
            {
                DataGridViewRow ss = tt;
                tasks2.Add(Task.Run(() =>
                {
                    ss.Visible = true;
                }));
            }
            Task.WhenAll(tasks2);
            // фильтрую поля
            var s = from DataGridViewRow tt in dataGridView1.Rows
                    where tt.Cells[0].Value != null
                    let isMatch = Regex.IsMatch(tt.Cells["IP"].Value.ToString(), "(" + t.Text + ")")
                    where isMatch == false 
                    select tt;
            var tasks = new List<Task>();
            foreach (DataGridViewRow tt in s)
            {
                DataGridViewRow ss = tt;
                tasks.Add(Task.Run(() =>
                {
                    ss.Visible = false;
                }));
            }
            Task.WhenAll(tasks);
        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {

        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show(e.RowIndex.ToString());
            //create a new client
            try
            {
                client = UdpUser.ConnectTo(dataGridView1["IP", e.RowIndex].Value.ToString(), 32123);
            }
            catch
            {

            }
            //wait for reply messages from server and send them
            Task.Factory.StartNew(async () => {
                while (true)
                {
                    try
                    {
                        var received = await client.Receive();
                        
                        textBox1.Text += received.Message;
                        textBox1.Text += Environment.NewLine+"c>";
                        textBox1.SelectionStart = textBox1.Text.Length;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            });
        }
    }
}