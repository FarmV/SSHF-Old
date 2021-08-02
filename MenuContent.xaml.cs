using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Логика взаимодействия для MenuContent.xaml
    /// </summary>
    public partial class MenuContent : Window, IDisposable
    {
        private static MainWindow GlobalWindow;
        public MenuContent(MainWindow window)
        {
            InitializeComponent();
            GlobalWindow = window;
            this.ShowInTaskbar = false;
            if (GlobalWindow.myHideWindow)
            {
                buttonHide.Content = "Процес скрыт";
            }

            
        }
        
        //[DllImport("user32.dll")]
        //static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        //[DllImport("user32.dll")]
        //static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        //[DllImport("user32.dll")]
        //static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        //[DllImport("kernel32.dll")]
        //static extern IntPtr LoadLibrary(string lpFileName);

        //private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        //const int WH_KEYBOARD_LL = 13; // Номер глобального LowLevel-хука на клавиатуру
        //const int WM_KEYDOWN = 0x100; // Сообщения нажатия клавиши

        //private LowLevelKeyboardProc _proc = hookProc;

        //private static IntPtr hhook = IntPtr.Zero;

        //public void SetHook()
        //{
        //    IntPtr hInstance = LoadLibrary("User32");
        //    hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        //}

        //public static void UnHook()
        //{
        //    UnhookWindowsHookEx(hhook);
        //}

        //public static IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
        //{
        //    if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        //    {
        //        int vkCode = Marshal.ReadInt32(lParam);
        //        //////ОБРАБОТКА НАЖАТИЯ
        //        myfunc(); // ошибка
        //        return (IntPtr)1;
        //    }
        //    else
        //        return CallNextHookEx(hhook, code, (int)wParam, lParam);
        //}

        //private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    // убираем хук
        //    UnHook();

        //}

        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    SetHook();
        //}

        //public void myfunc()
        //{

        //    MessageBox.Show("Hello!");

        //}




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            GlobalWindow._notifyIcon.Dispose();
            App.Current.Shutdown();


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
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
            throw new NotImplementedException();
        }

        private void ClearFormClick(object sender, RoutedEventArgs e)
        {
          //  GlobalWindow.One.Background = default;
            GlobalWindow.ButtonTanslate.Visibility = Visibility.Visible;
            GlobalWindow.One.Width = GlobalWindow.ButtonTanslate.ActualWidth;
            GlobalWindow.One.Height = GlobalWindow.ButtonTanslate.ActualHeight;
            
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


    }
}
