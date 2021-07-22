using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace WPF_Traslate_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftAlt && e.Key == Key.F)
            {
                Bot.Content = "true";
            }    
        }

        public void myTestKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                Bot.Content = "true";
            }
        }  
        public MainWindow()
        {
            InitializeComponent();
           // One.ShowInTaskbar = true;


        }


    
        public void MySize(int x, int y)
        {
            One.Width = x;
            One.Height = y;
        }

        [System.ComponentModel.TypeConverter("System.Windows.LengthConverter," +
        " PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, Custom=null")]
        public async void MyFolow()
        {
            //One.Top = 0;
            //One.Left = 0;
            await Task.Run(() =>
            {
                Dispatcher.Invoke(async() => 
                {
                    for (; ; )
                    {
                        Point point = GetCursorPosition();
                        await Task.Delay(10);
                        One.Top = point.Y;
                        One.Left = point.X;
                    }


                });

            });
            //await Task.Run(() =>
            //{
            //    Dispatcher.Invoke(() =>
            //    {

            //        One.Top = 300;
            //        One.Left = 500;


            //    });
                
            //});

        }
       
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // MySize(400,200);
            // App.Current.Dispatcher.Invoke(() => { MyFolow(); });
            Dispatcher.Invoke(new Action(MyFolow));

            //  RenderTargetBitmap bitmap = new RenderTargetBitmap();
            //Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            //Graphics graphics = Graphics.FromImage(printscreen as Image);
            //graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
            //printscreen.Save("D:\\1.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
            // MyTest.MyMethod();
            // MyTest.SaveClipboardImageToFile(@"C:\Текущие проэкты 2.0\test\Test.PNG");
        }

       

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new System.Windows.Point(point.X, point.Y);
            }
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            //bool success = User32.GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }


    }
}
class MyTest
{
    public static void MyMethod()
    {
       


       

        Console.WriteLine();
    }

    public static void SaveClipboardImageToFile(string filePath)
    {
        BitmapSource image = Clipboard.GetImage();
        if (image== null)
        {
            return;
        }
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(fileStream);
        }
    }

    public static void CreateBitmapFromVisual(Visual target, string fileName)
    {
        Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
        RenderTargetBitmap renderTarget = new RenderTargetBitmap(
            (int)bounds.Width,
            (int)bounds.Height,
            96,
            96,
            PixelFormats.Pbgra32);

        DrawingVisual visual = new DrawingVisual();

        using (var context = visual.RenderOpen())
        {
            var visualBrush = new VisualBrush(target);
            context.DrawRectangle(visualBrush, null, new Rect(new System.Windows.Point(), bounds.Size));
        }

        renderTarget.Render(visual);
        var bitmapEncoder = new BmpBitmapEncoder();
        bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTarget));
        using (var stm = File.Create(fileName))
            bitmapEncoder.Save(stm);
    }


}
