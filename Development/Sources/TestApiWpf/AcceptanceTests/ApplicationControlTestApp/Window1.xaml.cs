// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Microsoft.Test.AcceptanceTests.WpfTestApplication
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            MyListView.ItemsSource = new PeopleCollection();
        }

        private void btn_Open_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new TestDialog();
            dlg.Show();
        }

        private void btn_Animation_Click(object sender, RoutedEventArgs e)
        {
            var doubleAnimation = new DoubleAnimation(
                btn_Animation.ActualWidth,
                btn_Animation.ActualWidth + 100.0,
                new Duration(TimeSpan.FromMilliseconds(5000)));
            btn_Animation.BeginAnimation(Control.WidthProperty, doubleAnimation);
        }

        private void btn_Debug_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
