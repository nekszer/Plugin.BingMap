using System;
using System.IO;
using Xamarin.Forms;

namespace Plugin.BingMap
{
    public class Image
    {

        /// <summary>
        /// Constructor por defaul
        /// </summary>
        public Image()
        {

        }

        internal byte[] ByteArray { get; set; }

        /// <summary>
        /// Crea un icono basado en el flujo de datos de un archivo
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format">image/png o image/jpg</param>
        /// <param name="point"></param>
        public Image(Stream image, string format, Point? point)
        {
            if (image is null) throw new NullReferenceException("Image no puede ser null debido a que asigna la imagen por defecto del pin");
            if (string.IsNullOrEmpty(format)) throw new NullReferenceException("El formato no es válida");
            if (point is null) throw new NullReferenceException("Point no puede ser null, point define la posicion del icono");
            byte[] buffer = new byte[16 * 1024];
            byte[] bytearray = null;
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = image.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                bytearray = ms.ToArray();
            }
            ByteArray = bytearray;
            Source = "data:" + format + ";base64," + Convert.ToBase64String(bytearray);
            X = point.HasValue ? point.Value.X : 0;
            Y = point.HasValue ? point.Value.Y : 0;
        }


        public Image(string source, Point? point)
        {
            if (string.IsNullOrEmpty(source)) throw new NullReferenceException("Source no es válida");
            if (point == null) throw new NullReferenceException("La uri no es válida");
            if (source.Contains("http"))
            {
                Source = source;
            }
            else
            {
                throw new ArgumentException("El parametro source debe hacer referencia a un archivo en la web... http....png");
            }
            X = point.HasValue ? point.Value.X : 0;
            Y = point.HasValue ? point.Value.Y : 0;
        }

        public Image(string base64image, string format, Point? point)
        {
            if (string.IsNullOrEmpty(base64image)) throw new NullReferenceException("La imagen no es válida");
            if (string.IsNullOrEmpty(format)) throw new NullReferenceException("El formato no es válida");
            if (point == null) throw new NullReferenceException("La uri no es válida");
            Source = "data:" + format + ";base64," + base64image;
            ByteArray = Convert.FromBase64String(base64image);
            X = point.HasValue ? point.Value.X : 0;
            Y = point.HasValue ? point.Value.Y : 0;
        }

        public string Source { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public byte[] ToByteArray()
        {
            if (ByteArray != null) return ByteArray;
            return null;
        }
    }
}
