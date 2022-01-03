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

namespace WPF_Traslate_Test
{
    /// <summary>
    /// Логика взаимодействия для MyCollectionWindows.xaml
    /// </summary>
    public partial class MyCollectionWindows : Window
    {         
        private static MainWindow GlobalWindow;

        public MyCollectionWindows(MainWindow window)
        {
            InitializeComponent();
            GlobalWindow = window;

        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }















    }
}
