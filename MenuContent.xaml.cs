using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для MenuContent.xaml
    /// </summary>
    public partial class MenuContent : Window
    {
        private static MouseHook mouseHook;
        private static MainWindow GlobalWindow;
        private static bool MyClickBoard;
        public MenuContent(MainWindow window)
        {
            InitializeComponent();
            GlobalWindow = window;
            this.ShowInTaskbar = false;
            if (GlobalWindow.myHideWindow)
            {
                buttonHide.Content = "Процес скрыт";
            }
            mouseHook = new MouseHook();


            mouseHook.LeftButtonDown += ClickMouseOutside;
            mouseHook.Install();

        }

        private async void ClickMouseOutside(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            bool myReturn = false;
            void ret(object sender, EventArgs e)
            {
               // Thread.Sleep(300);
                myReturn = true;
            }
            GlobalWindow._notifyIcon.Click += ret;
            await Task.Delay(200);
            if (!MyClickBoard)
            {
                if (myReturn)
                {
                    GlobalWindow._notifyIcon.Click -= ret;
                    return;
                }
                else
                {
                    if (GlobalWindow.MenuIsOpen)
                    {
                        var adssad = App.Current.Windows;
                        //  object bbb = adssad.SyncRoot;
                        foreach (var item in App.Current.Windows)
                        {
                            if (item is WPF_Traslate_Test.MenuContent)
                            {
                                MenuContent menu = (MenuContent)item;
                                //menu.Dispose();
                                mouseHook.Uninstall();
                                menu.Close();
                                GlobalWindow.MenuIsOpen = false;
                            }
                        }
                        return;
                    }
                }
                //this.Dispose();
               
               // MyClickBoard = !MyClickBoard;
            }
            


        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            
            GlobalWindow.Dispose();
            App.Current.Shutdown();


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            if (GlobalWindow.One.Width == GlobalWindow.ButtonTanslate.ActualWidth && GlobalWindow.One.Height == GlobalWindow.ButtonTanslate.ActualHeight)
            {
                return;
            }

            GlobalWindow.One.Show();
            GlobalWindow.One.Visibility = Visibility.Visible;
            GlobalWindow.MenuIsOpen = false;
            this.Close();
        }

        private void MyMouseLevae(object sender, MouseEventArgs e)
        {

            
        }

        public void Dispose()
        {
            mouseHook.Uninstall();
        }

        private void ClearFormClick(object sender, RoutedEventArgs e)
        {
          //  GlobalWindow.One.Background = default;
           // GlobalWindow.ButtonTanslate.Visibility = Visibility.Visible;
            GlobalWindow.One.Width = GlobalWindow.ButtonTanslate.ActualWidth;
            GlobalWindow.One.Height = GlobalWindow.ButtonTanslate.ActualHeight;
            GlobalWindow.One.Background = Brushes.Transparent;
            GlobalWindow.Hide();


        }

        private void Window_Activated(object sender, EventArgs e)
        {
            

        }
        
        private void ShowPocces(object sender, RoutedEventArgs e)
        {
            if (GlobalWindow.myHideWindow)
            {
              GlobalWindow.myHideWindow = false;
              
                if (sender is Button)
                {
                    Button but = (Button)sender;
                    but.Content = "Показывать процес 🗸";
                }
            }
            else
            {
                GlobalWindow.myHideWindow = true;
               
                if (sender is Button)
                {
                    Button but = (Button)sender;
                    but.Content = "Процес скрыт";
                }
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            MyClickBoard = true;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            MyClickBoard = false;
        }
    }
}
