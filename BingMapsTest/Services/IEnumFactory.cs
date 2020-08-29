namespace BingMapsTest.Services
{
    public interface IEnumFactory<TEnum, KInterface>
    {
        KInterface Resolve(TEnum action);
    }
}