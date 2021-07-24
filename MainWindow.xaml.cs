using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;

[assembly: DisableDpiAwareness]

namespace WPF_Traslate_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


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
        }

        public async void MyFolow()
        {
            //double screenRealWidth = SystemParameters.PrimaryScreenWidth * (dpiBase * getScalingFactor()) / dpiBase;
            //double screenRealHeight = SystemParameters.PrimaryScreenHeight * (dpiBase * getScalingFactor()) / dpiBase;

            //Matrix m =
            //PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice;
            //double dx = m.M11;
            //double dy = m.M22;

            await Task.Run(() =>
            {
                Dispatcher.Invoke(async () =>
                {
                    for (; ; )
                    {
                        Point point = GetCursorPosition();

                        //point.Y = SystemParameters.PrimaryScreenWidth*(96 * 1.25) / 96;
                        //point.X = SystemParameters.PrimaryScreenHeight * (96 * 1.25) / 96;
                        await Task.Delay(10);
                        One.Top = point.Y + 10;
                        One.Left = point.X + 10;
                    }


                });

            });




        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource imagebufer = BuferImage();
            if (imagebufer == null)
            {
                return;
            }
            using (FileStream fileStream = new FileStream(@"mytest\mytest.PNG", FileMode.OpenOrCreate))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imagebufer));
                encoder.Save(fileStream);

            }

            string scantotext = MyClassTranslateText.MyTrasletImage().Result;
            if (scantotext == null)
            {
                return;
            }
            MyClassTranslateText.Query = scantotext;
            string resulttranslate = MyClassTranslateText.MyTranslate(MyClassTranslateText.Query).Result;
            if (resulttranslate == null)
            {
                return;
            }
            string resulttranslate1 = MyClassTranslateText.MyReplaceforpng(resulttranslate).Result.Item1;
            int strcount = MyClassTranslateText.MyReplaceforpng(resulttranslate).Result.Item2;
            Bitmap a = new Bitmap(1250, (strcount + 4) * 62);
            One.Width = 1250;
            One.Height = (strcount + 4) * 62;
            Graphics g = Graphics.FromImage(a);
            g.Clear(System.Drawing.Color.FromArgb(30, 30, 30));
            g.DrawString(resulttranslate1, new Font("Arial", 30), new SolidBrush(System.Drawing.Color.Goldenrod), 0f, 0f);
            g.Dispose();
            FileStream fileS = new FileStream(@"mytest\testo.PNG", FileMode.OpenOrCreate);
            a.Save(fileS, System.Drawing.Imaging.ImageFormat.Png);
            a.Dispose();
            fileS.Dispose();
            Dispatcher.Invoke(new Action(MyFolow));
            Bot.Visibility = Visibility.Hidden;
            Background = new ImageBrush(new BitmapImage(new Uri(@"C:\Users\user\source\repos\WPF_Traslate_Test\bin\Debug\net5.0-windows\mytest\testo.PNG")));


        }


        private static BitmapSource BuferImage()
        {
            // var image1 = new BitmapImage(new Uri(@"C:\Users\user\Pictures\2.png"));
            BitmapSource image = Clipboard.GetImage();
            return image ?? null;
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


    public static void SaveClipboardImageToFile(string filePath)
    {
        BitmapSource image = Clipboard.GetImage();
        if (image == null)
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
public class MyClassTranslateText
{
    public static string Query = $"Client Packs — Module makers can now create Client Packs in the toolset, which can be placed in clients My Documents\u005Cpwc\u005C folder to allow users to connect to Multiplayer Games for which they do not have the module.These.pwc files contain only the dataabsolutely necessary to run the module on the client side are useful if, for instance, youare running a persistent world and do not wish to allow clients to open your module withall of its areas, creatures, and scripts visible.";

    private static string SearchQuery = "Hello World! I am not a programmer";
    //private static string urlbody = $"https://www.deepl.com/translator#en/ru/{SearchQuery}";
    //public static void myReplayser()
    //{

    //    Query = Query.Replace(@"\r\n","").Replace(" ", "%20").Replace("#", "%23");

    //    SearchQuery = Query;
    //    urlbody = $"https://www.deepl.com/translator#en/ru/{SearchQuery}";

    //}
    private static string urlbody = $"https://www.deepl.com/translator#en/ru/";
    public static Task<string> MyTranslate(string transbody)
    {//  ((IJavaScriptExecutor)driver).ExecuteScript("document.body.style.transform='scale(0.5)';");
        
      

        ChromeDriver driver = new ChromeDriver(@"C:\Текущие проэкты 2.0");
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15.00);
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10.00);
        driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(15.00);
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
        driver.Navigate().GoToUrl("chrome://settings/");
        driver.ExecuteScript("chrome.settingsPrivate.setDefaultZoom(0.7);");
        
        driver.Navigate().GoToUrl(urlbody); 

        IWebElement restagrget = driver.FindElement(By.XPath("html/body"));

        restagrget.SendKeys(transbody);


        IWebElement results = driver.FindElement(By.XPath("//*[@id='target-dummydiv']"));



        //IWebElement myrestranslate = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//*[@class='results-text result']")));
        IWebElement myrestranslate = wait.Until(x =>
        {
            int strlenght = transbody.Length;

            for (;;)
            {
                
                string aaa = results.GetAttribute("innerHTML");
                double res = strlenght / aaa.Length;  //  248/24 = 10.333
                double res2 = 100 / res;              // 100% / 10.333 = 9.677 %
                if (res2>90)          
                {
                    break;
                }
                Thread.Sleep(30);

            }

            




            return driver.FindElement(By.XPath("//*[@id='target-dummydiv']"));

        });
  
        string res;
        for (int i = 0; ; i++)
        {   string aaa = results.GetAttribute("innerHTML");
                       
            if (aaa != "" && aaa != "\r\n")
            {
                res = aaa.Replace("\r", "").Replace("\n", "");
                break;
            }
        }
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(results.GetAttribute("innerHTML"));
        string translate = doc.Text;
        driver.Dispose();
        return Task.FromResult(translate) ?? null;

    }

    public static Task<string> MyTrasletImage()
    {     
        ChromeOptions option = new ChromeOptions();
        option.AddArguments("--window-size=1920,1080");
        ChromeDriver driver = new ChromeDriver(@"C:\Текущие проэкты 2.0", option);

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15.00);
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10.00);
        driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(15.00);
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
        Actions actions = new Actions(driver);
        driver.Navigate().GoToUrl("https://img2txt.com/ru");
        // driver.GetDevToolsSession(); 
        string filePath = @"C:\Users\user\source\repos\WPF_Traslate_Test\bin\Debug\net5.0-windows\mytest\mytest.PNG";
        IWebElement langMenu = driver.FindElement(By.XPath("//*[@class='select2-selection__rendered']"));
       // WebElement dynamicElement = new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until();
        IWebElement cooce = driver.FindElement(By.XPath("//*[@aria-label='dismiss cookie message']"));
        cooce.Click();
        ((IJavaScriptExecutor)driver).ExecuteScript("document.body.style.transform='scale(0.5)';");
        actions.MoveToElement(langMenu);
        langMenu.Click();
        IWebElement contexLangMenu = driver.FindElement(By.XPath("//*[@class='select2-selection select2-selection--single']")); ;
        contexLangMenu.SendKeys("webdriver" + Keys.Down);
        contexLangMenu.SendKeys("webdriver" + Keys.Down);
        contexLangMenu.SendKeys("webdriver" + Keys.Enter);
        IWebElement results = driver.FindElement(By.XPath("//*[@class='dz-hidden-input']"));
        results.SendKeys(filePath);
        Thread.Sleep(500);
        IWebElement downoad = driver.FindElement(By.XPath("//*[@class='form-bottom']"));
        downoad.Click();
       // Thread.Sleep(15000);
        IWebElement myrestranslate = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//*[@class='results-text result']")));

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(myrestranslate.GetAttribute("innerHTML"));
        string Mytransresult = doc.DocumentNode.FirstChild.EndNode.NextSibling.InnerHtml.Replace("", "");




        driver.Dispose();
        return Task.FromResult(Mytransresult) ?? null;




    }
    public static Task<Tuple<string,int>> MyReplaceforpng(string rep)
    {
        List<int> vs = new List<int>();
        for (int i = 0; ; i++)
        {
            int mystr = rep.IndexOf(' ', i);
            if (mystr == -1)
            {
                break;
            }
            vs.Add(mystr);

        }
        IEnumerable<int> res = vs.Distinct();
        List<int> asd = res.ToList();
        List<int> myrescount = new List<int>();
        int count = default;
        for (int i = 0; i < asd.Count(); i++)
        {
            if (count == default)
            {
                if (asd[i] > 60)
                {
                    count = asd[i - 1];
                    myrescount.Add(asd[i - 1]);
                }

            }
            else
            {
                if (asd[i] - count > 60)
                {
                    myrescount.Add(asd[i - 1]);
                    count = asd[i - 1];
                }
                else
                {

                }
            }


        }
        string myASSSS = rep;
        for (int i = 0; i < myrescount.Count; i++)
        {          
                myASSSS = myASSSS.Remove(myrescount[i] + 1 * i, 1).Insert(myrescount[i] + 1 * i, Environment.NewLine);         
        }
        return Task.FromResult(Tuple.Create(myASSSS, myrescount.Count));
    }
}
