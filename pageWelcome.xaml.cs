using System.Windows.Controls;
using System.Windows.Media;

namespace gouniversalDesktop
{
    /// <summary>
    /// Interaktionslogik für pageWelcome.xaml
    /// </summary>
    public partial class pageWelcome : Page
    {
        public pageWelcome()
        {
            InitializeComponent();
        }

        public void SetBackground(Brush br)
        {
            grid.Background = br;
        }

        public void SetOpacity(double dbl)
        {
            grid.Opacity = dbl;
        }

        public void SetTitle(string s)
        {
            lblTitle.Content = s;
        }

        public void SetTitleForeground(Brush br)
        {
            lblTitle.Foreground = br;
        }
    }
}
