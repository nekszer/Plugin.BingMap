namespace Plugin.BingMap
{
    public class ViewChanged
    {
        public Location Location { get; }

        public Type EventType { get; }

        public ViewChanged(Type type, Location location)
        {
            Location = location;
            EventType = type;
        }

        public enum Type
        {
            ViewChangeStart,
            ViewChange,
            ViewChangeEnd,
            ViewRendered,
            MapTypeChanged,
        }
    }
}