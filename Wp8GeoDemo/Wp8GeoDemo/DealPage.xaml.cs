using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

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
    }
}