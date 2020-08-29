using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace BingMapsTest.Services
{
    public class StorageService<K> : IStorageService<K>
    {
        public async Task<T> Get<T>(Expression<Func<K, T>> selector)
        {
            try
            {
                var prop = (PropertyInfo)((MemberExpression)selector.Body).Member;
                var response = await SecureStorage.GetAsync(prop.Name);
                return (T)Convert.ChangeType(response, typeof(T));
            }
            catch { }
            return default;
        }

        public async Task<bool> Set<T>(Expression<Func<K, T>> selector, T value)
        {
            try
            {
                var prop = (PropertyInfo)((MemberExpression)selector.Body).Member;
                SecureStorage.Remove(prop.Name);
                await SecureStorage.SetAsync(prop.Name, value.ToString());
                return true;
            }
            catch { }
            return false;
        }
    }
}