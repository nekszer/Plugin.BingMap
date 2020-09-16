using System;

namespace Plugin.BingMap
{
    public class Pin
    {

        public Pin()
        {
        }

        public Pin(Location location)
        {
            Latitude = location.Latitude;
            Longitude = location.Longitude;
        }

        public Pin(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public Pin(double latitude, double longitude, Image image)
        {
            Latitude = latitude;
            Longitude = longitude;
            Image = image;
        }

        public Pin(Location location, Image image)
        {
            Latitude = location.Latitude;
            Longitude = location.Longitude;
            Image = image;
        }

        /// <summary>
        /// Representa una imagen para el pin
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Latitud
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitud
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// Titulo para la marca
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Evento click
        /// </summary>
        public event EventHandler<string> Click;

        /// <summary>
        /// Lanza un click sobre el pin
        /// </summary>
        /// <param name="action"></param>
        public void OnClick()
        {
            Click?.Invoke(this, Data);
        }

        /// <summary>
        /// Data es un objeto que puede tener cualquier valor / clase, como parametro para guardar en el pin
        /// </summary>
        public string Data { get; set; }
    }
}
