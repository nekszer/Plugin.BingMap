using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BingMapsTest.Services
{
    public interface IStorageService<K>
    {
        Task<T> Get<T>(Expression<Func<K, T>> selector);
        Task<bool> Set<T>(Expression<Func<K, T>> selector, T value);
    }
}