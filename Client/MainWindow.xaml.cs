using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        object obj = new object();
        Socket client = null;
        string Login = "";
        Thread th = null;
        bool FlagCon = true;
        bool Disconn = false;

        public MainWindow()
        {
            InitializeComponent();
            this.StartProg();
        }

        void StartProg()
        {
            if (this.client == null)
                this.client = new Socket(AddressFamily.InterNetwork,
                                         SocketType.Stream,
                                         ProtocolType.IP);
            this.btConn.IsEnabled = true;
            this.btDisConn.IsEnabled = false;
            this.Send.IsEnabled = false;
        }
        IPAddress GetIp(string s)
        {
            IPAddress ip = null;
            try
            {
                ip = IPAddress.Parse(s);
            }
            catch (Exception)
            {
                IPAddress[] ar = Dns.GetHostAddresses(s);
                ip = ar[0];
            }
            return ip;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window1 f = new Window1(this.ConnectServ);
            f.Show();
        }

        void ConnectServ(string ip, int port, string Login)
        {
            try
            {
                IPEndPoint endP = new IPEndPoint(this.GetIp(ip), port);
                this.Login = Login;
                this.client.Connect(endP);
                this.TextSend(string.Format("Login:{0}",Login));
                this.th = new Thread(this.Receive);
                th.Start();
                this.btDisConn.IsEnabled = true;
                this.Send.IsEnabled = true;
                this.btConn.IsEnabled = false;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        void Receive(object obj)
        {
            try
            {
                while (true)
                {
                    byte[] buf = new byte[4096];
                    int iRec = this.client.Receive(buf);
                    Array.Resize(ref buf, iRec);
                    buf = this.FindComma(buf);
                    this.Dispatcher.Invoke(new Action(() => this.ChatText.Text +=
                                           string.Format("{0}\r\n", Encoding.Unicode.GetString(buf,0, buf.Length))));
                }
            }
            catch (SocketException)
            {
                this.Dispatcher.Invoke(new Action(()=> this.btDisConn_Click(null, null)));
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.TextSend(this.SendText.Text);
        }

        void TextSend(string s)
        {
            try
            {                
                byte[] buf = Encoding.Unicode.GetBytes(s);
                this.client.Send(buf, 0, buf.Length, SocketFlags.None);
                this.SendText.Text = "";
            }
            catch (SocketException)
            {
                this.Dispatcher.Invoke(new Action(() => this.btDisConn_Click(null, null)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        byte[] FindComma(byte [] buf)
        {
            string s = Encoding.Unicode.GetString(buf);
            int x = s.IndexOf("Clients:");
            if(x!=-1)
            {
                this.Dispatcher.Invoke(new Action(() => this.ListView.Items.Clear()));
                string[] str = s.Substring(x + 8).Split(';');
                foreach (var i in str)
                    this.Dispatcher.Invoke(new Action(()=>this.ListView.Items.Add(i)));
                if(this.FlagCon)
                    buf = Encoding.Unicode.GetBytes("Вы вошли в чат.");
                else
                    buf = Encoding.Unicode.GetBytes("Ввошол новый клиент");
            }
            this.FlagCon = false;
            return buf;
        }

        private void btDisConn_Click(object sender, RoutedEventArgs e)
        {
            lock (this.obj)
            {
                if (!this.Disconn)
                {
                    this.Disconn = true;
                    this.client.Close();
                    this.client = null;
                    if(this.th!=null)
                         this.th.Abort();
                    this.FlagCon = true;
                    this.Login = "";
                    this.ListView.Items.Clear();
                    this.ChatText.Text = "";
                    this.StartProg();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.btDisConn_Click(null, null);
        }
    }
}
