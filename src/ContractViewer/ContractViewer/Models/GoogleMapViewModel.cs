using System.Globalization;
using Jmelosegui.Mvc.GoogleMap;

namespace ContractViewer.Models
{
    /// <summary>
    /// Class represents viewmodel for google map, include base settings 
    /// </summary>
    public class GoogleMapViewModel
    {
        public GoogleMapViewModel()
        {
            MapName = "map";
            Address = "Czech republic";
            Culture = CultureInfo.GetCultureInfo("cs");
            Height = 650;
            Zoom = 8;
            ShowScaleControl = true;
            ShowPanControl = true;
            PanControlPosition = ControlPosition.TopLeft;
            ShowZoomControl = true;
            ZoomControlPosition = ControlPosition.TopLeft;
            ZoomControlStyle = ZoomControlStyle.Default;
            ShowStreetViewControl = true;
            StreetViewControlPosition = ControlPosition.TopLeft;
            ShowOverviewMapControl = true;
            OverviewMapControlOpened = true;
            ShowMapType = false;
            MapTypeControl = MapType.Roadmap;
            MapTypeControlStyle = MapTypeControlStyle.Default;
            MapTypeControlPosition = ControlPosition.TopRight;
        }
        public string MapName { get; set; }
        public string Address { get; set; }
        public CultureInfo Culture { get; set; }
        public int Height { get; set; }
        public int Zoom { get; set; }
        public bool ShowScaleControl { get; set; }
        public bool ShowPanControl { get; set; }
        public ControlPosition PanControlPosition { get; set; }
        public bool ShowMapType { get; set; }
        public MapType MapTypeControl { get; set; }
        public MapTypeControlStyle MapTypeControlStyle { get; set; }
        public ControlPosition MapTypeControlPosition { get; set; }
        public bool ShowZoomControl { get; set; }
        public ControlPosition ZoomControlPosition { get; set; }
        public ZoomControlStyle ZoomControlStyle { get; set; }
        public bool ShowStreetViewControl { get; set; }
        public ControlPosition StreetViewControlPosition { get; set; }
        public bool OverviewMapControlOpened { get; set; }
        public bool ShowOverviewMapControl { get; set; }
    }
}