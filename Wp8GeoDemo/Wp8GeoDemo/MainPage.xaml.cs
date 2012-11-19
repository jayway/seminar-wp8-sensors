using System;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Wallet;
using Windows.Devices.Geolocation;

namespace Wp8GeoDemo
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            StartGeoListening();
        }

        private void StartGeoListening()
        {
            var geolocator = new Geolocator
            {
                //ReportInterval = 5000,  //in ms
                MovementThreshold = 20,
                DesiredAccuracy = PositionAccuracy.High
            };

            geolocator.StatusChanged += geolocator_StatusChanged;
            //geolocator.PositionChanged += HandlePositionChangedSimple;
            geolocator.PositionChanged += HandlePositionChangedWithReverseGeo;
        }

        private void HandlePositionChangedSimple(Geolocator sender, PositionChangedEventArgs args)
        {
            DebugWrite("position changed");

            var geoposition = args.Position;
            var geocoordinate = geoposition.Coordinate;
            var text = string.Format("You are at: {0}, {1}", geocoordinate.Latitude, geocoordinate.Longitude);
            
            Dispatcher.BeginInvoke(() => DisplayTextOrToast(text));
        }

        private void DisplayTextOrToast(string text)
        {
            if (App.IsRunningInBackground)
            {
                ShowMovementToast(text);
            }
            else
            {
                Dispatcher.BeginInvoke(() => DisplayTimedStatus(text));
            }
        }

        private static void ShowMovementToast(string text)
        {
            var toast = new ShellToast
                            {
                                Title = "movement!",
                                Content = text
                            };
            toast.Show();
        }

        private void DisplayTimedStatus(string text)
        {
            string textWithDate = string.Format("{0:HH:mm.ss.f}{1}{2}",
                                                DateTime.Now, Environment.NewLine, text);
            StatusText.Text = textWithDate;
        }

        private void HandlePositionChangedWithReverseGeo(Geolocator sender, PositionChangedEventArgs args)
        {
            try
            {
                DebugWrite("position changed");
                var geoposition = args.Position;

                Dispatcher.BeginInvoke(() =>
                {
                    var geoCoordinate = new GeoCoordinate(
                        geoposition.Coordinate.Latitude,
                        geoposition.Coordinate.Longitude);

                    var query = new ReverseGeocodeQuery
                                    {
                                        GeoCoordinate = geoCoordinate
                                    };
                    query.QueryCompleted += (s, e) =>
                    {
                        var location = e.Result.FirstOrDefault();
                        if (location == null) return;

                        MapAddress address =
                            location.Information.Address;
                        string text = "You are at: " +
                                        address.Street + ", " +
                                        address.City;

                        DisplayTextOrToast(text);
                    };

                    query.QueryAsync();
                });

            }
            catch (Exception e)
            {
                DebugWrite(e.ToString());
            }
        }

        void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            Debug.WriteLine("geo status: {0}", args.Status);
        }

        private static void DebugWrite(string text)
        {
            Debug.WriteLine("{0:HH:mm.ss.fff}: {1}", DateTime.Now, text);
        }


        // Sample code for building a localized ApplicationBar

        //private void BuildLocalizedApplicationBar()

        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        private async void AddWalletItem_OnTap(object sender, GestureEventArgs e)
        {
            var dealId = "dealId";
            var walletItem = Wallet.FindItem(dealId);
            if (walletItem != null)
            {
                Wallet.Remove(walletItem);
            }

            var deal = new Deal(dealId)
                           {
                               Code = "872394875",
                               Description = "Really cheap turn-by-turn directions to our office.",
                               ExpirationDate = DateTime.Now.AddDays(7),
                               MerchantName = "Jayway",
                               DisplayName = "Directions"
                           };
            deal.NavigationUri = new Uri("/DealPage.xaml?DealName=" + deal.DisplayName, UriKind.Relative);
            await deal.SaveAsync();
        }
    }
}