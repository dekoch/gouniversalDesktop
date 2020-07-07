using System.Windows;
using System.Windows.Controls;

namespace gouniversalDesktop
{
    /// <summary>
    /// Interaktionslogik für pageGOU.xaml
    /// </summary>
    public partial class pageContent : Page
    {
        public pageContent()
        {
            InitializeComponent();

            webv.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
        }

        public void Start(string url)
        { 
            if (webv.IsBrowserInitialized)
            {
                webv.Load(url);
            }
        }

        void OnIsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
         
        }

        public void SetOpacity(double dbl)
        {
            grid.Opacity = dbl;
        }
    }
}
