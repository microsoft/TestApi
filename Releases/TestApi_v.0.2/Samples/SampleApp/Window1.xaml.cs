using System;
using System.Windows;
using System.Windows.Controls;

namespace SampleApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        void CheckStyleBox(object sender, RoutedEventArgs e)
        {
            CheckBox box=(CheckBox)sender;
            if (box.IsChecked==true)
            {
                BackgroundLayer.Visibility = Visibility.Visible;
            }
            else
            {
                BackgroundLayer.Visibility = Visibility.Hidden;
            }
        }

        private void AppendText(object sender, RoutedEventArgs e)
        {
            outputTextBox.Text += inputTextBox.Text + "\n";
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            // This code allows the content to be resized by the user
            // The app loads with a fixed grid size initially to allow
            // for visual verification testing
            this.SizeToContent = SizeToContent.Manual;
            mainGrid.Height = Double.NaN;
            mainGrid.Width = Double.NaN;
        }
        
    }
}
