using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Wp8GeoDemo
{
    public partial class DealPage : PhoneApplicationPage
    {
        public DealPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string dealName;
            NavigationContext.QueryString.TryGetValue("DealName", out dealName);
            if (string.IsNullOrEmpty(dealName)) return;

            DealName.Text = dealName;
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/DirectionsPage.xaml", UriKind.Relative));
        }
    }
}