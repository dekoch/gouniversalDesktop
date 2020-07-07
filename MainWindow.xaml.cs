using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace gouniversalDesktop
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string JSONCONFIG = "desktopconfig.json";
        private const int INIT = 0;
        private const int LOADING = 1;
        private const int LOADED = 2;
        private const int RUNNING = 3;
        private const int EXIT = 4;

        private runprocess gou;

        private pageWelcome pWelcome;
        private pageContent pContent;

        private readonly object stateLock = new object();
        private int intCurrentState;
        private int intNewState;

        DispatcherTimer timerState;

        private string strTitle;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void setNewState(int state)
        {
            lock (stateLock)
            {
                intNewState = state;
            }
        }

        private int getNewState()
        {
            lock (stateLock)
            {
                return intNewState;
            }
        }

        private void setCurrentState(int state)
        {
            lock (stateLock)
            {
                intCurrentState = state;
            }
        }

        private int getCurrentState()
        {
            lock (stateLock)
            {
                return intCurrentState;
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            strTitle = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            strTitle = strTitle.Replace(".exe", "");

            this.Title = strTitle;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pWelcome = new pageWelcome();
            pWelcome.SetTitle(strTitle);
            frameWelcome.Navigate(pWelcome);

            pContent = new pageContent();
            frameContent.Navigate(pContent);

            gou = new runprocess();

            setCurrentState(INIT);
            setNewState(LOADING);

            timerState = new DispatcherTimer();
            timerState.Interval = TimeSpan.FromMilliseconds(50);
            timerState.Tick += timerState_Tick;
            timerState.Start();
        }

        void timerState_Tick(object sender, EventArgs e)
        {
            int intCurrentState = getCurrentState();
            int intNewState = getNewState();

            if (intNewState != intCurrentState)
            {

                switch (intNewState)
                {
                    case LOADING:
                        config.ConfigFile configFile = new config.ConfigFile();
                        configFile.Binary = "gouniversal_amd64.exe";
                        configFile.StartPath = "";

                        config conf = new config();

                        if (File.Exists(JSONCONFIG) == false)
                        {
                            conf.Write(JSONCONFIG, configFile);
                        }
                        else
                        {
                            configFile = conf.Read(JSONCONFIG);
                        }


                        if (File.Exists(configFile.Binary))
                        {
                            gou.Start(configFile.Binary);

                            if (configFile.StartPath == "")
                            {
                                pContent.Start("http://127.0.0.1:8080");
                            }
                            else
                            {
                                pContent.Start("http://127.0.0.1:8080?path=" + configFile.StartPath);
                            }
                            
                            pContent.SetOpacity(0.0);
                            frameContent.Visibility = Visibility.Visible;

                            timerState.Interval = TimeSpan.FromSeconds(5);
                            setNewState(LOADED);
                        }
                        break;

                    case LOADED:
                        pWelcome.SetBackground(Brushes.Transparent);
                        pWelcome.SetTitleForeground(Brushes.Black);
                        pWelcome.SetOpacity(0.8);

                        pContent.SetOpacity(1.0);

                        timerState.Interval = TimeSpan.FromSeconds(2);
                        setNewState(RUNNING);
                        break;

                    case RUNNING:
                        frameWelcome.Visibility = Visibility.Hidden;

                        timerState.Interval = TimeSpan.FromMilliseconds(50);
                        break;

                    case EXIT:
                        this.Close();
                        break;
                }

                setCurrentState(intNewState);
            }


            if (intCurrentState == RUNNING)
            {
                if (gou.IsRunning() == false)
                {
                    setNewState(EXIT);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (gou.IsRunning())
            {
                gou.Stop();
            }
        }
    }
}
