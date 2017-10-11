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
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        Action<string, int, string> Connect { set; get; }

        public Window1(Action<string, int, string> act)
        {
            InitializeComponent();
            this.Connect = act;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Connect(this.IpName.Text,
                         int.Parse(this.Port.Text),
                         this.Login.Text);
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
