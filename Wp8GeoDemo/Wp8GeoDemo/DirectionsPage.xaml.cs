using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;

namespace Wp8GeoDemo
{
    public partial class DirectionsPage : PhoneApplicationPage
    {
        public DirectionsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var query = new GeocodeQuery
                {
                    SearchTerm = "Hans Michelsensgatan 9, Malmö, Sweden",
                    GeoCoordinate = new GeoCoordinate(55.36, 13.00)
                };
            query.QueryCompleted += async (sender, result) =>
                {
                    var destination = result.Result.FirstOrDefault();
                    if (destination == null)
                        return;

                    var locator = new Geolocator
                                      {
                                          DesiredAccuracyInMeters = 20
                                      };

                    var position = await locator.GetGeopositionAsync(
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(10));

                    var origin = new GeoCoordinate(position.Coordinate.Latitude, position.Coordinate.Longitude);
                    var routeQuery = new RouteQuery();
                    var waypoints = new[] {origin, destination.GeoCoordinate};
                    routeQuery.Waypoints = waypoints;
                    routeQuery.QueryCompleted += routeQuery_QueryCompleted;
                    routeQuery.QueryAsync();
                };
            query.QueryAsync();
        }

        void routeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            var route = e.Result;
            if (route == null) return;

            DirectionsMap.SetView(route.BoundingBox, new Thickness(10), MapAnimationKind.Parabolic);
            DirectionsMap.LandmarksEnabled = true;
            DirectionsMap.AddRoute(new MapRoute(route));
        }
    }
}