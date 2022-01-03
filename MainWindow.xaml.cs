using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

using Point = System.Windows.Point;
using System.Diagnostics;
using WebDriverManager.Helpers;
using Path = System.IO.Path;
using System.Windows.Controls;
using System.Windows.Interop;
using Microsoft.Win32;

[assembly: DisableDpiAwareness]

namespace WPF_Traslate_Test
{
    public partial class MainWindow : Window, IDisposable
    {
        public static readonly string PathOriginalScreenshot = $"{System.IO.Path.GetTempPath()}myAPP\\Original.png";
        public static readonly string PathModifiedScreenshot = $"{System.IO.Path.GetTempPath()}myAPP\\forbackground.png";

        public static readonly string G1 = @$"{System.IO.Path.GetTempPath()}myAPP\g1";
        public static readonly string G2 = @$"{System.IO.Path.GetTempPath()}myAPP\g2";
        public static readonly string G3 = @$"{System.IO.Path.GetTempPath()}myAPP\g3";
        public static readonly string G4 = @$"{System.IO.Path.GetTempPath()}myAPP\g4";
        public static readonly string G5 = @$"{System.IO.Path.GetTempPath()}myAPP\g5";



        private static volatile Queue<Point> myPoints = new Queue<Point>();

        public static volatile bool MyEnetrForm;

        public bool myHideWindow;

        //  public static CancellationTokenSource cts = new CancellationTokenSource();    
        //  CancellationToken ct = cts.Token;
        #region Функция FastTranslate
        void StartFastTranslate()
        {
            Process.Start(new ProcessStartInfo(@"C:\TEK_PROJECT\Visual Studio project\TranlateToDepl\bin\Debug\net48\TranlateToDepl.exe"));
        }
        #endregion
        #region Инициализация Конструктор



        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string reason);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);
        [DllImport("kernel32.dll")]
        static extern bool SetProcessShutdownParameters(uint dwLevel, uint dwFlags);
        public MainWindow()
        {//todo Найти прогрессбар скчаивания хромдрайвера

            InitializeComponent();
            if (!SingleProgramCheck())
            {
                App.Current.Shutdown();
                this.Dispose();
                return;
            }
            SetIconToMainApplication();
            cmd("taskkill /f /im chromedriver.exe");
            HookIntiallize();
            CheckTempFiles();
            MyMouseHook = new MouseHook();
            MyMouseHook.Install();
            MyMouseHook.MouseWheel += MyMouseSize;
            ButtonTanslate.Visibility = Visibility.Hidden;
            this.Visibility = Visibility.Hidden;
            
            
            // Объявить важность этого приложения при выключении (0x3FF = Максимальная важность!)
            SetProcessShutdownParameters(0x3FF, SHUTDOWN_NORETRY);

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            SystemEvents.SessionEnded += SystemEvents_Power;
           // NotSpleep();
        }

        protected  override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        const int WM_QUERYENDSESSION = 0x0011;
        const int WM_ENDSESSION = 0x0016;
        const uint SHUTDOWN_NORETRY = 0x00000001;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            if (msg == WM_QUERYENDSESSION)
            {
                ShutdownBlockReasonCreate(hwnd, "Никакого выключения пока программа работает!");
                Thread.Sleep(2000);
                ShutdownBlockReasonCreate(hwnd, lParam.ToString());
                Thread.Sleep(10_000);
                // Handle messages...
                // Осторожней с MessageBox - он блокирует главный Поток, так что всё, что после него НЕ ВЫПОЛНИТСЯ!
                //MessageBox.Show("Винда собиралась выключиться!");

                // Тут выполняем всё, что мы хотели сделать до выключения/перезагрузки
                // Симулируем работу просто усыпив Поток на 10 секунд
                Thread.Sleep(10_000);
                ShutdownBlockReasonCreate(hwnd, "Теперь можно выключаться!");

                // Чтобы пользователь увидел надпись - усыпим Поток ещё раз, но уже на 2 секунды
                Thread.Sleep(2_000);
                ShutdownBlockReasonDestroy(hwnd);
            }

            return IntPtr.Zero;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint SetThreadExecutionState(uint esFlagsl, uint esFlagsl2);
        //private async void NotSpleep()
        //{
        //    while (true)
        //    {
        //        await Task.Delay(200);
        //        uint a = SetThreadExecutionState(0x00000002, 0x00000001);
        //    }

        //}







        static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            
            Console.WriteLine(e.Mode.ToString());    // спящий режим
        }

        static void SystemEvents_Power(object sender, SessionEndedEventArgs e)
        {
            Console.WriteLine(e.Reason.ToString()); // выкл.
        }
        #endregion

        #region Хлам Инициализация хука мыши
        private MouseHook MyMouseHook;

        private void MyMouseSize(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {

            if (CapitalDown)
            {

                try
                {
                    this.Width = this.Width + (this.Width / 20);
                    this.Height = this.Height + (this.Height / 20);


                }
                catch (Exception)
                {


                }
            }
            if (LSHIFTDown)
            {
                try
                {
                    this.Width = this.Width - (this.Width / 20);
                    this.Height = this.Height - (this.Height / 20);

                }
                catch (Exception)
                {


                }
            }
        }
        #endregion

        #region Функция вызова CMD и Функция запрещающая вызов более одной копии приложения
        public static void cmd(string line)
        {
            Process.Start(new ProcessStartInfo { FileName = "cmd", Arguments = $"/c {line}", WindowStyle = ProcessWindowStyle.Hidden }).WaitForExit();
        }

        private static volatile Mutex InstanceCheckMutex;
        private static bool SingleProgramCheck()
        {
            bool isNew = true;
            InstanceCheckMutex = new Mutex(true, "MyMutexSingleProgramCheck", out isNew);
            return isNew;

        }
        #endregion

        #region Инициализация иконки для трея, обработка нажатя по иконке, функция итерации визуальных элеметов
        public System.Windows.Forms.NotifyIcon _notifyIcon;
        public bool NotificationMenuIsOpen = default;
        private void SetIconToMainApplication()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon($"{AppContext.BaseDirectory}{Process.GetCurrentProcess().ProcessName}.exe");
            _notifyIcon.Visible = true;
            _notifyIcon.Click += ClickNotifyIcon;
        }
       
        
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject /// невозможно перебрать элементы если окно не отображется
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }                    

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        


        public void ClickNotifyIcon(object sender, EventArgs e)
        {
            if (NotificationMenuIsOpen)
            {
                WindowCollection adssad = App.Current.Windows;
                //  object bbb = adssad.SyncRoot;
                foreach (var item in App.Current.Windows)
                {
                    if (item is WPF_Traslate_Test.MenuContent)
                    {
                        MenuContent menu = (MenuContent)item;
                        //menu.Dispose();
                        MenuCheckedButton.Clear();
                        foreach (RadioButton radioButton in MainWindow.FindVisualChildren<RadioButton>(menu))
                        {
                            if (radioButton.IsChecked == true)
                            {
                               MenuCheckedButton.Add(radioButton.Name, true);
                                
                            }
                        }
                        menu.Close();
                        NotificationMenuIsOpen = false;
                    }
                }
                return;
            }
            NotificationMenuIsOpen = true;
            App.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            One.Visibility = Visibility.Collapsed;
            One.Hide();
            MenuContent menuContent = new MenuContent(this);
            Point positionCursor = GetCursorPosition();
            menuContent.Topmost = true;
            double resolutionWidth = SystemParameters.PrimaryScreenWidth;
            double resolutionHeight = SystemParameters.PrimaryScreenHeight;

            WindowCollection windowsMyApp = App.Current.Windows;

            double posT = menuContent.Top = positionCursor.Y - 200.00;
            double posL = menuContent.Left = positionCursor.X + 5;
            menuContent.Show();
            double menuWidth = default;
            double menuHeight = default;
            foreach (var item in App.Current.Windows)
            {
                if (item is WPF_Traslate_Test.MenuContent)
                {
                    MenuContent menu = (MenuContent)item;

                    menuWidth = menu.ActualWidth;
                    menuHeight = menu.ActualHeight;
                }
            }

            if (menuWidth + positionCursor.X > resolutionWidth)
            {
                menuContent.Left = resolutionWidth - menuWidth;
                menuContent.Left = positionCursor.X - (menuWidth + 5);
            }
            if (menuHeight + positionCursor.Y < resolutionHeight)
            {
                menuContent.Top = resolutionHeight - menuHeight;
                menuContent.Top = positionCursor.Y - (menuHeight - 200.00);
            }


        }
        #endregion

        #region Проверка временных файлов и их очиста
        private static void CheckTempFiles(bool delete = false)
        {
            string pathTemporary = $"{System.IO.Path.GetTempPath()}myAPP";
            DirectoryInfo directory = new DirectoryInfo(pathTemporary);
            if (Directory.Exists(pathTemporary) & delete == false)
            {
                // MessageBox.Show("true");
                // directory.Delete();
            }
            if (!Directory.Exists(pathTemporary) & delete == false)
            {
                directory.Create();
            }
            if (Directory.Exists(pathTemporary) & delete == true)
            {
                directory.Delete(true); //todo Изменить алгоритм очистки с изменнением папкки умолчания хромдрайвера
            }

            List<DirectoryInfo> patchGroups = new List<DirectoryInfo>();
                      
            DirectoryInfo directoryg1 = new DirectoryInfo(G1);
            patchGroups.Add(directoryg1);
           
            DirectoryInfo directoryg2 = new DirectoryInfo(G2);
            patchGroups.Add(directoryg2);
            
            DirectoryInfo directoryg3 = new DirectoryInfo(G3);
            patchGroups.Add(directoryg3);
            
            DirectoryInfo directoryg4 = new DirectoryInfo(G4);
            patchGroups.Add(directoryg4);
            
            DirectoryInfo directoryg5 = new DirectoryInfo(G5);
            patchGroups.Add(directoryg5);

            foreach (var item in patchGroups)
            {
                if (!Directory.Exists(item.FullName) & delete == false)
                {
                    item.Create();
                }
            }


        }
        #endregion

        #region Освобождение ресурсов
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (!SingleProgramCheck())
            {
            }
            if (!SingleProgramCheck())
            {
                cmd("taskkill /f /im chromedriver.exe");
            }

            try
            {
                try
                {
                    CheckTempFiles(false);// delete off директории
                }
                catch (Exception)
                {
                }
                _notifyIcon.Dispose();
            }
            catch (Exception)
            {
            };
        }
        #endregion

        #region Функция инициализации хука клавиатуры
        private KeyboardHook keyboardHook;
        private void HookIntiallize()
        {
            keyboardHook = new KeyboardHook();
            keyboardHook.KeyUp += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);
            keyboardHook.KeyDown += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);
            keyboardHook.Install();
        }
        #endregion

        #region Функция обновления положения окна
        public async void RefreshWindow() =>
        //double screenRealWidth = SystemParameters.PrimaryScreenWidth * (dpiBase * getScalingFactor()) / dpiBase;
        //double screenRealHeight = SystemParameters.PrimaryScreenHeight * (dpiBase * getScalingFactor()) / dpiBase;

        //Matrix m =
        //PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice;
        //double dx = m.M11;
        //double dy = m.M22;          


        await Task.Run(() =>//todo Переработать алгоритм захвата окна(следования за курсором)
            {


                Dispatcher.Invoke(async () =>
                {

                    for (int i = 0; ; i++)
                    {
                        if (MyCancelRefreshWindows)
                        {
                            break;
                        }
                        if (!MyCancelRefreshWindows)
                        {
                            //if (MyEnetrForm)
                            //{
                            //    break;
                            //}
                            if (OneLeave)
                            {
                                Point point1 = GetCursorPosition();
                                await Task.Delay(2);
                                One.Left = point1.X + 24;
                                One.Top = point1.Y + 24;
                            }
                            else
                            {
                                //Point leavePos = MyPosCursorLeaveFormLast;
                                //Point point2 = GetCursorPosition();

                                Point point1 = GetCursorPosition();
                                await Task.Delay(2);
                                One.Left = point1.X + 24;
                                One.Top = point1.Y + 24;









                            }
                        }
                    }
                });

            });

        #endregion

        #region Функция по добавлению дополнительных окок в общий список
        public Task ConstuctorGroupWinodws(MyCollectionWindows[] windows)
        {
            for (int i = 0; i < windows.Length; i++)
            {
                MyCollectionWindows window = windows[i];
                window.Show();
                window.Topmost = true;

            }
            return Task.CompletedTask;
        }
        public Dictionary<string, bool> MenuCheckedButton = new Dictionary<string, bool>();
        public Task<MyCollectionWindows[]> GetWinowsG()
        {
            if (true)
            {

            }
            var a = ListGroupsWindows.Where( Win => Win.Value.Item1  == 1 );
            
            MyCollectionWindows[] wincCollection = new MyCollectionWindows[a.Count()];
            int cc = 0;
            foreach (KeyValuePair<MyCollectionWindows, Tuple<int, int, int>> item in a)
            {
                wincCollection[cc] = item.Key;
                cc++;
            }
 

            return Task.FromResult(wincCollection);
        }
        /// <summary> 
        /// <para>
        /// <param name="MyCollectionWindows"><see cref="MyCollectionWindows"/> Представляет одно окно в коллекции</param>
        /// </para>  
        /// <example>
        /// This shows how to increment an integer.
        /// <code>
        /// var index = 5;
        /// index++;
        /// </code>
        /// </example>
        /// </summary>
        
        private Dictionary<MyCollectionWindows,Tuple<int,int,int>> ListGroupsWindows = new Dictionary<MyCollectionWindows, Tuple<int, int, int>>();
        // Tuple  1. Группа от G1 до G5  2. Видимость окна? 3. 0
        public Task SetWindowtoPool(MyCollectionWindows win)
        {
            ListGroupsWindows.Add(win, Tuple.Create(1,0,0));


            return Task.CompletedTask;
        }


        #endregion

        #region Функция получения перевода для формы
        private Task GetTranslate()
        {
            BitmapSource imageFromBuffer = GetBuferImage();
            if (imageFromBuffer == null) throw new ArgumentNullException("Буфер пустой", new Exception("imageFromBuffer"));

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    BitmapEncoder encoder = new PngBitmapEncoder();
            //    encoder.Frames.Add(BitmapFrame.Create(imageFromBuffer));
            //    encoder.Save(ms);
            //    var path2 = ms.
            //}

            using (FileStream createFileFromImageBuffer = new FileStream(PathOriginalScreenshot, FileMode.OpenOrCreate))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageFromBuffer));
                encoder.Save(createFileFromImageBuffer);

            }

            Tuple<ChromeDriver, WebDriverWait, Actions, ChromeOptions> configurateWebDriver = MyClassTranslateText.ConfigurateWebDrive(myHideWindow);

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

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(PathModifiedScreenshot);
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.EndInit();

            Background = new ImageBrush(bi);

            if (this.One.Visibility == Visibility.Hidden)
            {
                this.Show();
                // HookIntiallize();
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Обработка нажатия клавиш, вызов соответствующих функций
        private void keyboardHook_KeyUp(KeyboardHook.VKeys key)
        {
            if (key == KeyboardHook.VKeys.KEY_A)
            {
                KeyADown = false;
            }
            if (key == KeyboardHook.VKeys.LWIN)
            {
                LWINDown = false;
            }
            if (key == KeyboardHook.VKeys.LSHIFT)
            {
                LSHIFTDown = false;

            }
            if (key == KeyboardHook.VKeys.CAPITAL)
            {
                CapitalDown = false;

            }
            if (key == KeyboardHook.VKeys.LCONTROL)
            {
                LcontrolDown = false;

            }

        }

        bool KeyADown = false;
        bool LWINDown = false;
        bool LSHIFTDown = false;
        bool CapitalDown = false;
        bool LcontrolDown = false;

        private void keyboardHook_KeyDown(KeyboardHook.VKeys key)
        {

            if (key == KeyboardHook.VKeys.KEY_A)
            {
                KeyADown = true;
            }
            if (key == KeyboardHook.VKeys.LWIN)
            {
                LWINDown = true;

            }
            if (key == KeyboardHook.VKeys.LSHIFT)
            {
                LSHIFTDown = true;

            }
            if (key == KeyboardHook.VKeys.CAPITAL)
            {
                CapitalDown = true;

            }
            if (key == KeyboardHook.VKeys.LCONTROL)
            {
                LcontrolDown = true;
            }



            if (KeyADown & LWINDown & LSHIFTDown)
            {

                KeyADown = false;
                LWINDown = false;
                LSHIFTDown = false;
                CapitalDown = false;

                try
                {

                    BitmapSource imageFromBuffer = GetBuferImage();

                    if (imageFromBuffer == null)
                    {
                        if (System.IO.File.Exists(PathOriginalScreenshot))
                        {
                            BitmapImage bi = new BitmapImage();
                            bi.BeginInit();
                            bi.UriSource = new Uri(PathOriginalScreenshot);
                            bi.CacheOption = BitmapCacheOption.OnLoad;
                            bi.EndInit();

                            imageFromBuffer = bi;
                        }
                    }
                    else
                    {
                        try
                        {
                            //BitmapImage bi = new BitmapImage();
                            //bi.BeginInit();
                            //bi.UriSource = new Uri(PathModifiedScreenshot);
                            //bi.CacheOption = BitmapCacheOption.OnLoad;
                            //bi.EndInit();
                            //Background = new ImageBrush(bi);
                            using (FileStream createFileFromImageBuffer = new FileStream(PathOriginalScreenshot, FileMode.OpenOrCreate))
                            {
                                BitmapEncoder encoder = new PngBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(imageFromBuffer));
                                encoder.Save(createFileFromImageBuffer);

                            }
                        }
                        catch (Exception)
                        {

                            throw new ArgumentNullException("Буфер пустой, нет изображения", new Exception("imageFromBuffer"));
                        }
                    }

                    this.One.Width = imageFromBuffer.Width;
                    this.One.Height = imageFromBuffer.Height;

                    Background = new ImageBrush(imageFromBuffer);

                    this.Focus();
                    this.Topmost = true;
                    if (this.Visibility == Visibility.Hidden)
                    {
                        this.Show();
                    }
                    Dispatcher.Invoke(new Action(RefreshWindow));
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
            if (CapitalDown & LcontrolDown)
            {
                MyCancelRefreshWindows = !MyCancelRefreshWindows;
                Dispatcher.Invoke(new Action(RefreshWindow));
            }
            if (LWINDown & LSHIFTDown & CapitalDown)
            {
                KeyADown = false;
                LWINDown = false;
                LSHIFTDown = false;
                CapitalDown = false;
                keyboardHook.Uninstall();
                if (System.IO.File.Exists(MainWindow.PathModifiedScreenshot))
                {
                    System.IO.File.Delete(MainWindow.PathModifiedScreenshot);
                }
                this.Hide();
                Button_Click(new object(), new RoutedEventArgs());
            }
            else
            {
                ButtonTanslate.Content = key;
            }
        }
        #endregion

        #region Получение изображения из буфера обмена
        private static BitmapSource GetBuferImage()
        {
            // var image1 = new BitmapImage(new Uri(@"C:\Users\user\Pictures\2.png"));
            BitmapSource image = Clipboard.GetImage();
            try
            {
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            }
            catch (Exception)
            {


            }
            return image ?? null;
        }
        #endregion

        #region Функционал получения позиции курсора

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

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }
        #endregion

        #region Обработка событий формы
        private void MyDoubleFormClick(object sender, MouseButtonEventArgs e)
        {
            MyCancelRefreshWindows = false;
            this.Close();
        }

        private void NotClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            One.Hide();
        }

        private void EnterForm(object sender, MouseEventArgs e)
        {
            MyEnetrForm = true;
        }
        bool MyCancelRefreshWindows = false;

        bool OneLeave = false;
        Point MyPosCursorLeaveFormLast = new Point();
        private void LevaeForm(object sender, MouseEventArgs e)
        {


            //if (massPoint.Length == 0)
            //{
            //    massPoint[0] = MyPosCursorLeaveFormPreLast;
            //    massPoint[1] = MyPosCursorLeaveFormLast; 
            //}
            //if (massPoint.Length > 0)
            //{
            //    massPoint[1] = MyPosCursorLeaveFormLast;
            //}

            MyEnetrForm = false;
            if (MyCancelRefreshWindows)
            {
                return;
            }
            MyPosCursorLeaveFormLast = GetCursorPosition();
            Dispatcher.Invoke(new Action(RefreshWindow));
        }
        private void MyMouseRight(object sender, MouseButtonEventArgs e)
        {
            MyCancelRefreshWindows = !MyCancelRefreshWindows;
            Dispatcher.Invoke(new Action(RefreshWindow));
        }

        private void One_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.Source != null)
                {
                    string[] arrayDrops = new string[] { MainWindow.PathOriginalScreenshot };
                    DataObject dataObject = new DataObject(DataFormats.FileDrop, arrayDrops);
                    dataObject.SetData(DataFormats.StringFormat, dataObject);

                    DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
                    this.Topmost = true;
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                //Dispatcher.Invoke(new Action(RefreshWindow));
                GetTranslate().Wait();
                // ButtonTanslate.Visibility = Visibility.Hidden;
                keyboardHook.Install();
            }
            catch (ArgumentNullException ex)
            {
                ButtonTanslate.Content = $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}";
                keyboardHook.Install();
                return;
            }
            catch
            {
                return;
            }
        }

        private void Mouse3Wheel(object sender, MouseWheelEventArgs e)
        {
            //MyCollectionWindows window = new MyCollectionWindows(this);
            //window.Background = this.Background;
            //window.Height = this.ActualHeight;
            //window.Width = this.ActualWidth;
            //window.Show();
            //window.Topmost = true;
            //Aggregate window = new Aggregate();
            //window.Show();
            //window.Topmost = true;
            //StartFastTranslate();

        }

        //private void Form1_Drag(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent("FileDrop", false))
        //    {
        //        string[] paths = (string[])(e.Data.GetData("FileDrop", false));
        //        foreach (string path in paths)
        //        {
        //            string path12 = path;
        //        }
        //    }
        //}
        #endregion


    }

}
#region Реализация логики получения перевода
public class MyClassTranslateText
{
    public static string Query = $"Client Packs — Module makers can now create Client Packs in the toolset, which can be placed in clients My Documents\u005Cpwc\u005C folder to allow users to connect to Multiplayer Games for which they do not have the module.These.pwc files contain only the dataabsolutely necessary to run the module on the client side are useful if, for instance, youare running a persistent world and do not wish to allow clients to open your module withall of its areas, creatures, and scripts visible.";

    private static string urlbody = $"https://www.deepl.com/translator#en/ru/";


    public static Tuple<ChromeDriver, WebDriverWait, Actions, ChromeOptions> ConfigurateWebDrive(bool hide = false)
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
        driver.Manage().Window.Maximize();

        //driver.Navigate().GoToUrl("chrome://settings/");
        //driver.ExecuteScript($"chrome.settingsPrivate.setDefaultZoom({zoom});");

        return Tuple.Create(driver, wait, actions, option);
    }
    public static Task<string> Translate(string textForTranslate, ref Tuple<ChromeDriver, WebDriverWait, Actions, ChromeOptions> configurate)
    {//  ((IJavaScriptExecutor)driver).ExecuteScript("document.body.style.transform='scale(0.5)';");
     // Tuple<ChromeDriver, WebDriverWait, Actions> config = configurate;
        ChromeDriver driver = configurate.Item1;
        WebDriverWait wait = configurate.Item2;
        Actions actions = configurate.Item3;
        ChromeOptions option = configurate.Item4;
        option.PageLoadStrategy = PageLoadStrategy.Default;//??
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
            return results;
        });
        return Task.FromResult(translate) ?? null;
    }

    public static Task<string> ScanImageToText(ref Tuple<ChromeDriver, WebDriverWait, Actions, ChromeOptions> configurate)
    {
        ChromeDriver driver = configurate.Item1;
        WebDriverWait wait = configurate.Item2;
        Actions actions = configurate.Item3;
        ChromeOptions option = configurate.Item4;
        option.PageLoadStrategy = PageLoadStrategy.Eager;//??
        TimeSpan ssddddwww = driver.Manage().Timeouts().ImplicitWait;
        driver.Navigate().GoToUrl("https://img2txt.com/ru");
        // string filePath = @"C:\Users\user\source\repos\WPF_Traslate_Test\bin\Debug\net5.0-windows\mytest\origianal.PNG";
        IWebElement langMenu = driver.FindElement(By.XPath("//*[@class='select2-selection__rendered']"));
        IWebElement cooce = driver.FindElement(By.XPath("//*[@aria-label='dismiss cookie message']"));
        actions.MoveToElement(cooce);
        cooce.Click();
        ((IJavaScriptExecutor)driver).ExecuteScript("document.body.style.transform='scale(0.5)';");
        IWebElement element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(langMenu));
        langMenu.Click();
        IWebElement contexLangMenu = driver.FindElement(By.XPath("//*[@class='select2-selection select2-selection--single']"));

        contexLangMenu.SendKeys(Keys.Down);
        contexLangMenu.SendKeys(Keys.Down);
        contexLangMenu.SendKeys(Keys.Enter);

        IWebElement results = driver.FindElement(By.XPath("//*[@class='dz-hidden-input']"));
        results.SendKeys(Path.GetFullPath(WPF_Traslate_Test.MainWindow.PathOriginalScreenshot));
        Thread.Sleep(440); // Ожидание
        IWebElement downoad = driver.FindElement(By.XPath("//*[@class='form-bottom']"));
        downoad.Click();
        IWebElement myrestranslate = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//*[@class='results-text result']")));
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(myrestranslate.GetAttribute("innerHTML"));
        string Mytransresult = doc.DocumentNode.FirstChild.EndNode.NextSibling.InnerHtml.Replace("", string.Empty);

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
        string myReplace = rep;
        for (int i = 0; i < myrescount.Count; i++)
        {
            myReplace = myReplace.Remove(myrescount[i] + 1 * i, 1).Insert(myrescount[i] + 1 * i, Environment.NewLine);
        }
        return Task.FromResult(Tuple.Create(myReplace, myrescount.Count));
    }
}
#endregion