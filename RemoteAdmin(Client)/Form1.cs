using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteAdmin_Client_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var server = new UdpListener();

            //start listening for messages and copy the messages back to the client
            Task.Factory.StartNew(async () => {
                while (true)
                {
                    var received = await server.Receive();

                    // Здесь нужно разобрать строку, которая пришла ( и выполнять соответствующую функцию

                    if (received.Message == "getIP\r\n")
                    {
                        server.Reply("My ip is " + getLocalIP().ToString(), received.Sender);
                    }
                    else if(received.Message == "pingAllChildren\r\n")
                    {
                        server.Reply(Functions.pingAllChildren(getLocalIP().ToString()), received.Sender);
                    }
                    else
                    {
                        server.Reply("unknown comand, please, try again", received.Sender);
                    }
                    //server.Reply("copy " + received.Message, received.Sender);
                    if (received.Message == "quit")
                        break;
                }
            });
        }

        private IPAddress getLocalIP()
        {
            IPAddress ip = null;
            for (int i = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Length - 1; i >= 0; i--)
            {
                ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[i];
                if (Dns.GetHostEntry(Dns.GetHostName()).AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    break;
                }
            }
            return ip;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Functions.pingAllChildren("10.1.10.10"));
        }
    }
}
