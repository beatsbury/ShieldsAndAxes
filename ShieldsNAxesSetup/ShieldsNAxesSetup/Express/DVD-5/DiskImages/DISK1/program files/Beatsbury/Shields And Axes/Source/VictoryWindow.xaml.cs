using System.Windows;
using System.Windows.Controls;

namespace ShieldsNAxes
{
    /// <summary>
    /// Interaction logic for VictoryWindow.xaml
    /// </summary>
    public partial class VictoryWindow : Window
    {
        public TextBox text { get; set; }

        public VictoryWindow()
        {
            InitializeComponent();
        }

        public void skolButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
