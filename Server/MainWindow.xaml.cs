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
namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>   
      enum ServerPoint {Start=0,Stop=1}; 
     public partial class MainWindow : Window
    {
        object obj = new object();
        ServerPoint flag = new ServerPoint();
        Socket serv = null;
        List<Client> lstClient = new List<Client>();
        Thread th = null;
        bool Disconn = false;
              
        public MainWindow()
        {
            InitializeComponent();
            this.CreadeSer();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lock (this.obj)
                {
                    this.Stop.IsEnabled = true;
                    this.Start.IsEnabled = false;
                    this.flag = ServerPoint.Start;
                    if (this.serv == null)
                        this.CreadeSer();
                    this.th = new Thread(this.AwaitClient);
                    th.Start();
                    this.ShowMess("Start Server OK");
                }
            }
            catch (ThreadAbortException tae)
            { MessageBox.Show(tae.Message); }
        }

        void CreadeSer()
        {
            try
            {
                this.serv = new Socket(AddressFamily.InterNetwork,
                                         SocketType.Stream,
                                         ProtocolType.IP);
                IPEndPoint enp = new IPEndPoint(IPAddress.Any, 3333);
                this.serv.Bind(enp);
                this.serv.Listen(1000);               
                this.ShowMess("Create Server OK");
                this.ShowMess("Plase Click Start Server");
                this.Disconn = false;
                this.Stop.IsEnabled = false;
                this.Start.IsEnabled = true;
            }
            catch(Exception ex)
            { MessageBox.Show(ex.Message); }
        }
        
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.flag = ServerPoint.Stop;
            this.StopServ();
        }

        void AwaitClient(object obj)
        {
            while (true)
            {
                if (this.flag == ServerPoint.Start)
                {
                    try
                    {
                        Socket client = this.serv.Accept();
                        Thread th = new Thread(this.Recived);
                        th.Start(client);
                        this.ShowMess("Add Client");
                        Client cl = new Client { s = client, th = th };
                        lock (this.obj)
                            this.lstClient.Add(cl);
                    }
                    catch (Exception)
                    { break; }
                }
                else 
                {                    
                    break;
                }
            }
           this.Dispatcher.Invoke(new Action(() => this.StopServ()));
        }           
        
        void Recived(object obj)
        {
            Socket cl = (Socket)obj;
            try
            {
                while (true)
                {
                    byte[] buf = new byte[4096];
                    int iRec = cl.Receive(buf);
                    Array.Resize(ref buf, iRec);
                    buf= this.FindComma(buf, cl);
                    this.Send(buf,cl);                   
                }
            }
            catch (SocketException)
            {
                this.Dispatcher.Invoke(new Action(()=>this.RemoveClient(cl)));               
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }

        }

        void StopServ()
        {
            lock (this.obj)
            {
                if (!this.Disconn)
                {
                    this.Disconn = true;
                    foreach (var i in this.lstClient)
                    {
                        i.s.Close();
                        i.th.Abort();
                    }
                    this.serv.Close();
                    this.serv = null;
                    this.lstClient.Clear();
                    if (this.flag == ServerPoint.Stop)
                    {
                        this.ShowMess("Stop Server OK");
                    }
                    if (this.th != null)
                        this.th.Abort();                  
                }
            }
        }

        void Send(byte[] buf,Socket cl)
        {
            try
            {
                lock (this.lstClient)
                {
                    for (int i = 0; i < this.lstClient.Count; i++)
                    {
                        try
                        {
                            if (this.lstClient[i] == null)
                                continue;
                            this.lstClient[i].s.Send(buf, 0, buf.Length, SocketFlags.None);
                        }
                        catch (SocketException)
                        {
                            this.Dispatcher.Invoke(new Action(() => this.RemoveClient(this.lstClient[i].s)));                                                  
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
         }

        void ShowMess(string mess)
        {
            this.Dispatcher.Invoke( new Action(()=>
                Log.Text += string.Format("{0}\r\n", mess)));
        }
        
        byte [] FindComma(byte[]buf ,Socket cl)
        {
            string s = Encoding.Unicode.GetString(buf);
            int index = this.lstClient.FindIndex(i => i.s == cl);
            int x = s.IndexOf("Login:");
            if(x !=-1)
            {
                this.lstClient[index].Name = s.Substring(x + 6);                
                this.ShowMess(string.Format("Client Name:{0}", this.lstClient[index].Name));
                s = this.SendClients(cl);               
            }
            else
            {
                s = string.Format("{0}:{1}", this.lstClient[index].Name,s);
            }
            return Encoding.Unicode.GetBytes(s);
        }

        string SendClients(Socket cl)
        {
            string s = "Clients:";
            foreach(var i in this.lstClient)
            {
                s += string.Format("{0};", i.Name);
            }
            return s; 
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => this.StopServ()));
        }

        void RemoveClient(Socket cl)
        {
            lock (this.obj)
            {
                foreach (var i in this.lstClient)
                {
                    if (i.s == cl)
                    {
                        this.ShowMess(string.Format("Remove Client:{0}", i.Name));
                        i.s.Close();
                        i.s = null;
                        i.th.Abort();
                        i.Name = null;
                    }
                }
                this.lstClient.RemoveAll(x => x.s == null);
            }
        }
    }
}
