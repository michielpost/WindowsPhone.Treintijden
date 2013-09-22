using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Shell;
using System.Linq;
using ActueelNS.Services.Interfaces;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;
using ActueelNS.Tile;
using ActueelNS.Services.Models;
using ActueelNS.Resources;
using System.Globalization;

namespace ActueelNS.Services
{
    public class LiveTileService : ILiveTileService
    {

        public bool ExistsStation(string name)
        {
            string uri = string.Format("/Views/StationTijden.xaml?id={0}", name);

            return Exists(uri);
        }

        public void CreateStation(string name, string code)
        {
            string uri = string.Format("/Views/StationTijden.xaml?id={0}", name);

            if (!Exists(uri))
            {

                var filenameInput = string.Format("/Shared/ShellContent/{0}.jpg", code);
                var filenameInputS = string.Format("/Shared/ShellContent/{0}_s.jpg", code);
                var filenameInputW = string.Format("/Shared/ShellContent/{0}_w.jpg", code);

                var filenameW = GenerateTileWide(name, code, filenameInputW);
                var filenameS = GenerateTileSmall(name, code, filenameInputS);
                var filename = GenerateTile(name, code, filenameInput);

                var isoStoreTileImage = string.Format("isostore:{0}", filename);
                var isoStoreTileImageS = string.Format("isostore:{0}", filenameS);
                var isoStoreTileImageW = string.Format("isostore:{0}", filenameW);

                // Create the Tile object and set some initial properties for the Tile.
                // The Count value of 12 shows the number 12 on the front of the Tile. Valid values are 1-99.
                // A Count value of 0 indicates that the Count should not be displayed.
                FlipTileData NewTileData = new FlipTileData
                {
                    BackgroundImage = new Uri(isoStoreTileImage, UriKind.Absolute),
                    SmallBackgroundImage = new Uri(isoStoreTileImageS, UriKind.Absolute),
                     WideBackgroundImage = new Uri(isoStoreTileImageW, UriKind.Absolute),
                    //Title = name,
                    //Count = 12,
                    //BackTitle = "Back of Tile",
                    //BackContent = "Welcome to the back of the Tile",
                    //BackBackgroundImage = new Uri("Blue.jpg", UriKind.Relative)
                };

              
                // Create the Tile and pin it to Start. This will cause a navigation to Start and a deactivation of our application.
                ShellTile.Create(new Uri(uri, UriKind.Relative), NewTileData, true);
            }
        }

        private string GenerateTile(string name, string code, string filename)
        {
            //FrameworkElement reflectedFE = ReflectedElement as FrameworkElement
            TileControl tileControl = new TileControl(name, code);
            tileControl.Measure(new Size(336, 336));
            tileControl.Arrange(new Rect(0, 0, 336, 336));
            tileControl.UpdateLayout();
            tileControl.Measure(new Size(336, 336));
            tileControl.Arrange(new Rect(0, 0, 336, 336));
            tileControl.UpdateLayout();

            WriteableBitmap sourceBitmap = new WriteableBitmap(tileControl, null);

            sourceBitmap.Invalidate();

           

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var st = new IsolatedStorageFileStream(filename, FileMode.Create, FileAccess.Write, store))
                {
                    sourceBitmap.SaveJpeg(st, 336, 336, 0, 100);
                }
            }
            return filename;
        }

        private string GenerateTileSmall(string name, string code, string filename)
        {
            //FrameworkElement reflectedFE = ReflectedElement as FrameworkElement
            TileSmallControl tileControl = new TileSmallControl(name, code);
            tileControl.Measure(new Size(159, 159));
            tileControl.Arrange(new Rect(0, 0, 159, 159));
            tileControl.UpdateLayout();
            tileControl.Measure(new Size(159, 159));
            tileControl.Arrange(new Rect(0, 0, 159, 159));
            tileControl.UpdateLayout();

            WriteableBitmap sourceBitmap = new WriteableBitmap(tileControl, null);

            sourceBitmap.Invalidate();



            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var st = new IsolatedStorageFileStream(filename, FileMode.Create, FileAccess.Write, store))
                {
                    sourceBitmap.SaveJpeg(st, 159, 159, 0, 100);
                }
            }
            return filename;
        }

        private string GenerateTileWide(string name, string code, string filename)
        {
            //FrameworkElement reflectedFE = ReflectedElement as FrameworkElement
            TileWideControl tileControl = new TileWideControl(name, code);
            tileControl.Measure(new Size(691, 336));
            tileControl.Arrange(new Rect(0, 0, 691, 336));
            tileControl.UpdateLayout();
            tileControl.Measure(new Size(691, 336));
            tileControl.Arrange(new Rect(0, 0, 691, 336));
            tileControl.UpdateLayout();

            WriteableBitmap sourceBitmap = new WriteableBitmap(tileControl, null);

            sourceBitmap.Invalidate();



            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var st = new IsolatedStorageFileStream(filename, FileMode.Create, FileAccess.Write, store))
                {
                    sourceBitmap.SaveJpeg(st, 691, 336, 0, 100);
                }
            }
            return filename;
        }

        private bool Exists(string name)
        {
            return ShellTile.ActiveTiles.Any(x => x.NavigationUri.ToString().Contains(name));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        

        public void CreateAdvies(PlannerSearch search, int index, DateTime date)
        {
            if (!Exists(string.Format("/Views/Reisadvies.xaml?id={0}&index={1}", search.Id, index)))
            {

                var filenameInput = string.Format("/Shared/ShellContent/{0}_{1}.jpg", search.Id, index);

                var filename = GenerateAdviesTile(search.VanStation.Name, search.NaarStation.Name, date.ToString("dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture), filenameInput);
                var isoStoreTileImage = string.Format("isostore:{0}", filename);

                // Create the Tile object and set some initial properties for the Tile.
                // The Count value of 12 shows the number 12 on the front of the Tile. Valid values are 1-99.
                // A Count value of 0 indicates that the Count should not be displayed.
                StandardTileData NewTileData = new StandardTileData
                {
                    BackgroundImage = new Uri(isoStoreTileImage, UriKind.Absolute),
                    //Title = name,
                    //Count = 12,
                    //BackTitle = "Back of Tile",
                    //BackContent = "Welcome to the back of the Tile",
                    //BackBackgroundImage = new Uri("Blue.jpg", UriKind.Relative)
                };

                // Create the Tile and pin it to Start. This will cause a navigation to Start and a deactivation of our application.
                ShellTile.Create(new Uri(string.Format("/Views/Reisadvies.xaml?id={0}&index={1}", search.Id, index), UriKind.Relative), NewTileData);
            }
        }

        public bool ExistsCreateAdvies(PlannerSearch search, int index, DateTime date)
        {
            return Exists(string.Format("/Views/Reisadvies.xaml?id={0}&index={1}", search.Id, index));
        }


        private string GenerateAdviesTile(string from, string to, string date, string filename)
        {
            //FrameworkElement reflectedFE = ReflectedElement as FrameworkElement
            TileAdviesControl tileControl = new TileAdviesControl(from, to, date);
            tileControl.Measure(new Size(336, 336));
            tileControl.Arrange(new Rect(0, 0, 336, 336));
            tileControl.UpdateLayout();
            tileControl.Measure(new Size(336, 336));
            tileControl.Arrange(new Rect(0, 0, 336, 336));
            tileControl.UpdateLayout();

            WriteableBitmap sourceBitmap = new WriteableBitmap(tileControl, null);

            sourceBitmap.Invalidate();


            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var st = new IsolatedStorageFileStream(filename, FileMode.Create, FileAccess.Write, store))
                {
                    sourceBitmap.SaveJpeg(st, 336, 336, 0, 100);
                }
            }
            return filename;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////



        public void CreatePlanner(string from, string to, string via, string fromCode, string toCode, string viaCode)
        {
            string uri = string.Format("/Views/Planner.xaml?from={0}&to={1}&via={2}", from, to, via);

            if (!Exists(uri))
            {
                string filename = string.Empty;
                string filenameInput = string.Format("/Shared/ShellContent/Planner_{0}_{1}_{2}.jpg", fromCode, toCode, viaCode);

                if (string.IsNullOrEmpty(from)
                && string.IsNullOrEmpty(to))
                {
                    filename = GenerateTile(AppResources.TileReisplanner, AppResources.TileAppTitle, filenameInput);
                }
                else
                {
                    filename = GenerateAdviesTile(from, to, AppResources.TileReisplanner, filenameInput);
                }

                var isoStoreTileImage = string.Format("isostore:{0}", filename);

                // Create the Tile object and set some initial properties for the Tile.
                // The Count value of 12 shows the number 12 on the front of the Tile. Valid values are 1-99.
                // A Count value of 0 indicates that the Count should not be displayed.
                StandardTileData NewTileData = new StandardTileData
                {
                    BackgroundImage = new Uri(isoStoreTileImage, UriKind.Absolute),
                    //Title = name,
                    //Count = 12,
                    //BackTitle = "Back of Tile",
                    //BackContent = "Welcome to the back of the Tile",
                    //BackBackgroundImage = new Uri("Blue.jpg", UriKind.Relative)
                };

                // Create the Tile and pin it to Start. This will cause a navigation to Start and a deactivation of our application.
                ShellTile.Create(new Uri(uri, UriKind.Relative), NewTileData);
            }
        }

        public bool ExistsCreatePlanner(string from, string to, string via)
        {
            string uri = string.Format("/Views/Planner.xaml?from={0}&to={1}&via={2}", from, to, via);

            return Exists(uri);

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////


        public void CreateStoringen()
        {
            string uri = "/Views/Storingen.xaml?";

            if (!Exists(uri))
            {
                var filenameInput = string.Format("/Shared/ShellContent/Storingen.jpg");
                //var filenameInputS = string.Format("/Shared/ShellContent/Storingen_s.jpg");
                var filenameInputW = string.Format("/Shared/ShellContent/Storingen_w.jpg");

                var filenameW = GenerateTileWide(AppResources.TileStoringen, string.Empty, filenameInputW);
                //var filenameS = GenerateTileSmall(AppResources.TileStoringen, string.Empty, filenameInputS);
                var filename = GenerateTile(AppResources.TileStoringen, string.Empty, filenameInput);

                var isoStoreTileImage = string.Format("isostore:{0}", filename);
                //var isoStoreTileImageS = string.Format("isostore:{0}", filenameS);
                var isoStoreTileImageW = string.Format("isostore:{0}", filenameW);


                // Create the Tile object and set some initial properties for the Tile.
                // The Count value of 12 shows the number 12 on the front of the Tile. Valid values are 1-99.
                // A Count value of 0 indicates that the Count should not be displayed.
                FlipTileData NewTileData = new FlipTileData
                {
                    BackgroundImage = new Uri(isoStoreTileImage, UriKind.Absolute),
                    //SmallBackgroundImage = new Uri(isoStoreTileImageS, UriKind.Absolute),
                    WideBackgroundImage = new Uri(isoStoreTileImageW, UriKind.Absolute),
                    //Title = name,
                    //Count = 12,
                    //BackTitle = "Back of Tile",
                    //BackContent = "Welcome to the back of the Tile",
                    //BackBackgroundImage = new Uri("Blue.jpg", UriKind.Relative)
                };

                // Create the Tile and pin it to Start. This will cause a navigation to Start and a deactivation of our application.
                ShellTile.Create(new Uri(uri, UriKind.Relative), NewTileData, true);
            }
        }

        public bool ExistsCreateStoringen()
        {
            string uri = "/Views/Storingen.xaml";

            return Exists(uri);

        }


       


        ////////////////////////////////////////////////////////////////////////////////////////////////
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        public static void ResetTiles(int number)
        {
            //Update tiles
            foreach (var foundTile in ShellTile.ActiveTiles)
            {
                var liveTile = new StandardTileData
                {
                    Count = number
                };

                foundTile.Update(liveTile);

            }
        }

       
    }
}
