using LightForms;
using LightForms.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BingMapsTest.Resources.Strings
{
    public class JsonLocalizationManager : BaseResource, ILocalizationManager
    {
        public string FileName { get; set; }

        public JsonLocalizationManager(string filename)
        {
            FileName = filename;
        }

        public T Localize<T>(string key)
        {
            return Localize(key, LightFormsApplication.Instance.Culture ?? new CultureInfo("en"), default(T));
        }

        public T Localize<T>(string key, T @default = default)
        {
            return Localize(key, LightFormsApplication.Instance.Culture ?? new CultureInfo("en"), default(T));
        }

        public T Localize<T>(string key, CultureInfo culture)
        {
            return Localize(key, culture, default(T));
        }

        public T Localize<T>(string key, CultureInfo culture, T @default = default)
        {
            var data = GetLanguageStrings();
            if (!data.ContainsKey(key))
            {
                System.Diagnostics.Debug.WriteLine($"{key} no se encuentra", "Localization");
                return (T)Convert.ChangeType(key, typeof(T));
            }

            var dic = data[key];
            if (!dic.ContainsKey(culture.TwoLetterISOLanguageName))
                throw new NullReferenceException($"{culture.TwoLetterISOLanguageName} no se encontrol en el archivo json");

            var value = dic[culture.TwoLetterISOLanguageName];
            return (T)Convert.ChangeType(value, typeof(T));
        }

        private Dictionary<string, Dictionary<string, string>> Data { get; set; }
        private Dictionary<string, Dictionary<string, string>> GetLanguageStrings()
        {
            if (Data != null) return Data;
            var dictionary = GetDictionary(FileName);
            if (dictionary == null) throw new NullReferenceException($"{FileName} no se encontro el archivo o el archivo no tiene");
            var commondictionary = GetDictionary("Common.json");
            if (commondictionary != null)
                foreach (var item in commondictionary)
                    dictionary.Add(item.Key, item.Value);
            Data = dictionary;
            return Data;
        }

        private Dictionary<string, Dictionary<string, string>> GetDictionary(string filename)
        {
            Dictionary<string, Dictionary<string, string>> dictionary = null;
            Stream stream = GetStream(typeof(JsonLocalizationManager), filename);
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                try
                {
                    dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
                }
                catch { }
            }
            return dictionary;
        }
    }
}
