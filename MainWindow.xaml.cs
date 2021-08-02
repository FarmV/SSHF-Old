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
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

using Point = System.Windows.Point;
using System.Diagnostics;
using WebDriverManager.Helpers;
using Path = System.IO.Path;

[assembly: DisableDpiAwareness]

namespace WPF_Traslate_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            cmd("taskkill /f /im chromedriver.exe");
            _notifyIcon.Dispose();
        }



        public void myTestKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                ButtonTanslate.Content = "true";
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            SetIconToMainApplication();
        }
        //private System.Windows.Forms.NotifyIcon _notifyIcon;

        public static void Method_Name()
        {
            
        }

        private static volatile Queue<Point> myPoints = new Queue<Point>();
        private static volatile bool startfor = true;
        //  public static CancellationTokenSource cts = new CancellationTokenSource();
        //  CancellationToken ct = cts.Token;
        public static volatile bool MyEnetrForm;
        
        public async void RefreshWindow()
        {
            
            //double screenRealWidth = SystemParameters.PrimaryScreenWidth * (dpiBase * getScalingFactor()) / dpiBase;
            //double screenRealHeight = SystemParameters.PrimaryScreenHeight * (dpiBase * getScalingFactor()) / dpiBase;

            //Matrix m =
            //PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice;
            //double dx = m.M11;
            //double dy = m.M22;
;

            await Task.Run(() =>
            {
               
                Dispatcher.Invoke(async () =>
                {
                    for (; ; )
                    {
                        if (MyEnetrForm)
                        {
                            break;
                        }
                        
                        //  248/24 = 10.333
                        // 100% / 10.333 = 9.677 %
                        if (startfor)
                        {
                            Point startpoint = GetCursorPosition();
                            myPoints.Enqueue(startpoint);
                            startfor = false;
                        }
                        //                 50          40        1.25          80  100-80    20
                        //                 100          120      -20               120 /10      /20                          
                        else
                        {
                            //Point startpoint = GetCursorPosition();
                            //myPoints.Enqueue(startpoint);
                            Point oldpoint = myPoints.Dequeue();      // 999   1231                                                                   // 458   67
                            Point point = GetCursorPosition();
                            await Task.Delay(10);
                            double res1 = oldpoint.X;
                            double res2 = oldpoint.Y;
                            double res3 = point.X;
                            double res4 = point.Y;
                            double totalresX = 100.00 / (res1 / res3); //3000

                            double a1 = (oldpoint.X - point.X) / 10;                //192
                            double a2 = (oldpoint.Y - point.Y) / 10;                // 108

                            // 10
                            var timedelay = 2 * a1;
                            if (timedelay == 0)
                            {
                                timedelay = 170;
                            }
                            var timedelay2 = 0.6 * a2;
                            if (timedelay2 == 0)
                            {
                                timedelay2 = 170;
                            }
                            if (timedelay < 0)
                            {
                                timedelay = timedelay * -1;
                            }
                            if (timedelay2 < 0)
                            {
                                timedelay2 = timedelay2 * -1;
                            }
                            var totaltimedealy = (timedelay + timedelay2) / 2;
                            await Task.Delay((int)totaltimedealy);
                            One.Left = point.X + 17;
                            // await Task.Delay((int)timedelay2);
                            One.Top = point.Y + 17;
                            myPoints.Enqueue(point);
                            //point.Y = SystemParameters.PrimaryScreenWidth*(96 * 1.25) / 96;
                            //point.X = SystemParameters.PrimaryScreenHeight * (96 * 1.25) / 96;
                            //  Point point = myPoints.Dequeue();

                        }
                    }


                });

            });


           

        }

        private static readonly string PathOriginalScreenshot = @"mytest\origianal.PNG";
        private static readonly string PathModifiedScreenshot = @"mytest\forbackgrund.PNG";
        public bool myHideWindow;
        private Task GetTranslate()
        {
            BitmapSource imageFromBuffer = GetBuferImage();
            if (imageFromBuffer == null) throw new ArgumentNullException("Буфер пустой", new Exception("imageFromBuffer"));


            using (FileStream createFileFromImageBuffer = new FileStream(PathOriginalScreenshot, FileMode.OpenOrCreate))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageFromBuffer));
                encoder.Save(createFileFromImageBuffer);

            }
           
            
            Tuple<ChromeDriver, WebDriverWait, Actions> configurateWebDriver = MyClassTranslateText.ConfigurateWebDrive(myHideWindow);
                                 
            

            string scanTextImage = MyClassTranslateText.ScanImageToText(ref configurateWebDriver).Result;
            if (scanTextImage == null) throw new ArgumentNullException("Ошибка Скан", new Exception("scanTextImage"));

            string resultTranslate = MyClassTranslateText.Translate(scanTextImage, ref configurateWebDriver).Result;
            if (resultTranslate == null) throw new ArgumentNullException("Ошибка Перевод", new Exception("resultTranslate")); ;

            configurateWebDriver.Item1.Dispose();
            cmd("taskkill /f /im chromedriver.exe");// chromedriver.exe

            Tuple<string, int> changed = MyClassTranslateText.ReplaceTextForImage(resultTranslate).Result;
            string changedTextforImageConstructor = changed.Item1;
            int stingrCountForImageConstructor = changed.Item2;

            Bitmap imageForBackground = new Bitmap(1400, (stingrCountForImageConstructor + 4) * 62);

            One.Width = 1400;
            One.Height = (stingrCountForImageConstructor + 4) * 62;

            Graphics graphicTextForBackgroundImage = Graphics.FromImage(imageForBackground);
            graphicTextForBackgroundImage.Clear(System.Drawing.Color.FromArgb(30, 30, 30));
            graphicTextForBackgroundImage.DrawString(changedTextforImageConstructor, new Font("Arial", 30), new SolidBrush(System.Drawing.Color.Goldenrod), 0f, 0f);
            graphicTextForBackgroundImage.Dispose();

            FileStream streamBackgroundIamage = new FileStream(PathModifiedScreenshot, FileMode.OpenOrCreate);
            imageForBackground.Save(streamBackgroundIamage, System.Drawing.Imaging.ImageFormat.Png);
            imageForBackground.Dispose();
            streamBackgroundIamage.Dispose();


            //Dispatcher.Invoke(new Action(RefreshWindow));

            //object asdsd = this.Resources["back"];
            //Uri bbbb = new Uri("pack://application:,,,/mytest/forbackgrund.PNG");
            //string ccc = bbbb.AbsolutePath;
            Background = new ImageBrush(new BitmapImage(new Uri("mytest/forbackgrund.PNG", UriKind.Relative)));


            return Task.CompletedTask;
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            
            try
            {
                //Dispatcher.Invoke(new Action(RefreshWindow));
                 GetTranslate().Wait();
                 ButtonTanslate.Visibility = Visibility.Hidden;

            }
            catch (ArgumentNullException ex)
            {
                ButtonTanslate.Content = $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}";
                return;
            }
            catch
            {
                return;
            }
        }

        public static void cmd(string line)
        {
            Process.Start(new ProcessStartInfo { FileName = "cmd", Arguments = $"/c {line}", WindowStyle = ProcessWindowStyle.Hidden }).WaitForExit();
        }

        private static BitmapSource GetBuferImage()
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
        private static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            //bool success = User32.GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }

        private void MyDoubleFormClick(object sender, MouseButtonEventArgs e)
        {

            this.Close();
           



        }
        public System.Windows.Forms.NotifyIcon _notifyIcon;
       // private System.Windows.Forms.ContextMenu contextMenu;
      //  private System.Windows.Forms.MenuItem menuItem;
      //  private System.ComponentModel.IContainer components;
        private void LoadMyWindow(object sender, RoutedEventArgs e)
        {
           // _notifyIcon = new System.Windows.Forms.NotifyIcon();
           //// _notifyIcon.Icon = Properties.Resources.ResourceManager.GetObject("AppIcon") as Icon;
           // _notifyIcon.Visible = true;
        }
        private void SetIconToMainApplication()
        {
           // var col = Resources.Keys;
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            //_notifyIcon.Icon = (Icon)this.FindResource("foricon.ico");

            _notifyIcon.Icon= new Icon(@"Tic.ico");
            _notifyIcon.Visible = true;
            _notifyIcon.Click += ClickNotifyIcon;
            




        }
        public bool MenuIsOpen = default;
        public void ClickNotifyIcon(object sender, EventArgs e)
        {
            if (MenuIsOpen)
            {
                var adssad = App.Current.Windows;
                object bbb = adssad.SyncRoot;
                foreach (var item in App.Current.Windows)
                {
                    if (item is WPF_Traslate_Test.MenuContent)
                    {
                        MenuContent menu = (MenuContent)item;
                        menu.Close();
                        MenuIsOpen = false;
                    }
                }
                return;
            }
            MenuIsOpen = true;
            App.Current.ShutdownMode =  System.Windows.ShutdownMode.OnExplicitShutdown;
            One.Visibility = Visibility.Collapsed;
            One.Hide();
            MenuContent menuContent = new MenuContent(this);
            Point positionCursor = GetCursorPosition();
            menuContent.Top = positionCursor.Y-80.00;
            menuContent.Left = positionCursor.X+5;
            menuContent.Show();
            menuContent.Topmost = true;
            

            

        }

        private void NotClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            One.Hide();
        }

        private void EnterForm(object sender, MouseEventArgs e)
        {
            //CancellationTokenSource cts = new CancellationTokenSource();
            //CancellationToken ct = cts.Token;
            //cts.Cancel();
            //Dispatcher.Invoke(new Action<CancellationTokenSource, CancellationToken>(RefreshWindow), cts, ct);
            // cts.Cancel();
            MyEnetrForm = true;
        }

        private void LevaeForm(object sender, MouseEventArgs e)
        {
            MyEnetrForm = false;
            //CancellationTokenSource cts = new CancellationTokenSource();
            //CancellationToken ct = cts.Token;
            Dispatcher.Invoke(new Action(RefreshWindow));
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

    private static string urlbody = $"https://www.deepl.com/translator#en/ru/";





    public static Tuple<ChromeDriver, WebDriverWait, Actions> ConfigurateWebDrive(bool hide = false)
    {

        // ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();//скрывает батник

        ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser).Replace("\\chromedriver.exe", ""));
        chromeDriverService.HideCommandPromptWindow = true;//
        ChromeOptions option = new ChromeOptions();
        option.AddArguments("--window-size=1920,1080");
        if (hide)
        {
            option.AddArgument("--headless");
        }
        
        //option.AddArgument("--headless");  //скрывает окно
        ChromeDriver driver = new ChromeDriver(chromeDriverService, option);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15.00);
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10.00);
        driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(15.00);
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
        Actions actions = new Actions(driver);

        //driver.Navigate().GoToUrl("chrome://settings/");
        //driver.ExecuteScript($"chrome.settingsPrivate.setDefaultZoom({zoom});");

        return Tuple.Create(driver, wait, actions);
    }
    public static Task<string> Translate(string textForTranslate, ref Tuple<ChromeDriver, WebDriverWait, Actions> configurate)
    {//  ((IJavaScriptExecutor)driver).ExecuteScript("document.body.style.transform='scale(0.5)';");
     // Tuple<ChromeDriver, WebDriverWait, Actions> config = configurate;
        ChromeDriver driver = configurate.Item1;
        WebDriverWait wait = configurate.Item2;
        Actions actions = configurate.Item3;

        driver.Navigate().GoToUrl(urlbody);

        IWebElement restagrget = driver.FindElement(By.XPath("html/body"));

        restagrget.SendKeys(textForTranslate);


        IWebElement results = driver.FindElement(By.XPath("//*[@id='target-dummydiv']"));



        //IWebElement myrestranslate = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//*[@class='results-text result']")));
        string translate = null;
        IWebElement myrestranslate = wait.Until(x =>
        {
            int strlenght = textForTranslate.Length;

            for (; ; )
            {
                
                string aaa = results.GetAttribute("textContent");
                

                double res1 = strlenght / aaa.Length;  //  248/24 = 10.333
                double res2 = 100 / res1;              // 100% / 10.333 = 9.677 %
                if (res2 > 90)
                {
                    aaa = aaa.Replace(Environment.NewLine, string.Empty);
                    translate = aaa;
                    break;
                }
                Thread.Sleep(30);

            }

            //string res;
            //for (int i = 0; ; i++)
            //{
            //    string aaa = results.GetAttribute("innerHTML");

            //    if (aaa != "" && aaa != "\r\n")
            //    {
            //        res = aaa.Replace("\r", "").Replace("\n", "");
            //        break;
            //    }
            //}

                //string aaa = results.GetAttribute("textContent");

                //if (aaa != string.Empty && aaa != "\r\n")
                //{
                //    res = aaa.Replace("\r", "").Replace("\n", "");
                //    break;
                //}
         





            return results;

        });

 
        //HtmlDocument doc = new HtmlDocument();
        //doc.LoadHtml(results.GetAttribute("innerHTML"));
        //string translate = doc.Text;

        return Task.FromResult(translate) ?? null;

    }

    public static Task<string> ScanImageToText(ref Tuple<ChromeDriver, WebDriverWait, Actions> configurate)
    {
        //ChromeOptions option = new ChromeOptions();
        //option.AddArguments("--window-size=1920,1080");
        //ChromeDriver driver = new ChromeDriver(@"C:\Текущие проэкты 2.0", option);

        //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15.00);
        //driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10.00);
        //driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(15.00);
        //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
        //Actions actions = new Actions(driver);

        ChromeDriver driver = configurate.Item1;
        WebDriverWait wait = configurate.Item2;
        Actions actions = configurate.Item3;

        TimeSpan ssddddwww = driver.Manage().Timeouts().ImplicitWait;
        driver.Navigate().GoToUrl("https://img2txt.com/ru");
       // string filePath = @"C:\Users\user\source\repos\WPF_Traslate_Test\bin\Debug\net5.0-windows\mytest\origianal.PNG";
        IWebElement langMenu = driver.FindElement(By.XPath("//*[@class='select2-selection__rendered']"));
        IWebElement cooce = driver.FindElement(By.XPath("//*[@aria-label='dismiss cookie message']"));
        actions.MoveToElement(cooce);
        cooce.Click();
        ((IJavaScriptExecutor)driver).ExecuteScript("document.body.style.transform='scale(0.5)';");
        //actions.MoveToElement(langMenu);
        IWebElement element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(langMenu));
        langMenu.Click();
        IWebElement contexLangMenu = driver.FindElement(By.XPath("//*[@class='select2-selection select2-selection--single']"));

        contexLangMenu.SendKeys(Keys.Down);
        contexLangMenu.SendKeys(Keys.Down);
        contexLangMenu.SendKeys(Keys.Enter);




        IWebElement results = driver.FindElement(By.XPath("//*[@class='dz-hidden-input']"));
        results.SendKeys(Path.GetFullPath("mytest/origianal.PNG"));
        Thread.Sleep(440); // Ожидание
        IWebElement downoad = driver.FindElement(By.XPath("//*[@class='form-bottom']"));
        downoad.Click();
        IWebElement myrestranslate = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//*[@class='results-text result']")));
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(myrestranslate.GetAttribute("innerHTML"));
        string Mytransresult = doc.DocumentNode.FirstChild.EndNode.NextSibling.InnerHtml.Replace("", string.Empty);




        // driver.Dispose();
        return Task.FromResult(Mytransresult) ?? null;




    }
    public static Task<Tuple<string, int>> ReplaceTextForImage(string rep)
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
