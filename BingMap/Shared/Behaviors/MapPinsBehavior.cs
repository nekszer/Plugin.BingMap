using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace Plugin.BingMap.Behaviors
{
    public class MapPinsBehavior : Behavior<Map>
    {
        private Map CurrentMap { get; set; }

        #region BindableProperty Pins
        /// <summary>
        /// Description of property
        /// </summary>
        public static readonly BindableProperty PinsProperty = BindableProperty.Create(nameof(Pins), typeof(ObservableCollection<Pin>), typeof(MapPinsBehavior), null, BindingMode.OneWay);

        /// <summary>
        /// Description of property
        /// </summary>
        public ObservableCollection<Pin> Pins
        {
            get
            {
                return (ObservableCollection<Pin>)GetValue(PinsProperty);
            }

            set
            {
                SetValue(PinsProperty, value);
            }
        }
        #endregion

        public MapPinsBehavior()
        {
            BindableProperties = new List<BindablePropertyChanged>
            {
                new BindablePropertyChanged(PinsProperty, SetCurrentPins)
            };
        }

        private void Bindable_LoadComplete(object sender, EventArgs e)
        {
            CurrentMap = sender as Map;
            SetCurrentPins();
        }

        private void SetCurrentPins()
        {
            SetCollection();
        }

        private void SetCollection()
        {
            if (CurrentMap == null) return;
            if (Pins == null) return;
            Pins.CollectionChanged -= Pins_CollectionChanged;
            Pins.CollectionChanged += Pins_CollectionChanged;
            CurrentMap.Pins.Clear();
            foreach (var pin in Pins)
                CurrentMap.Pins.Add(pin);
        }

        private void Pins_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetCollection();
        }

        public Map AssociatedObject { get; private set; }

        protected override void OnAttachedTo(Map bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;
            bindable.BindingContextChanged += Bindable_BindingContextChanged;
            bindable.LoadComplete += Bindable_LoadComplete;
        }

        private void Bindable_BindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
            SetCollection();
        }

        protected override void OnDetachingFrom(Map bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.LoadComplete -= Bindable_LoadComplete;
        }

        #region Bindable Property Changed
        public List<BindablePropertyChanged> BindableProperties { get; set; }

        protected override void OnPropertyChanged(string propertyname = null)
        {
            base.OnPropertyChanged(propertyname);
            if (BindableProperties == null) return;
            BindableProperties.FirstOrDefault(b => b.PropertyChanged(propertyname));
        }

        public class BindablePropertyChanged
        {
            protected string PropetyName
            {
                get
                {
                    return SourceProperty?.PropertyName;
                }
            }
            protected BindableProperty SourceProperty;
            protected System.Action Action { get; set; }

            public BindablePropertyChanged(BindableProperty sourceproperty, System.Action action)
            {
                SourceProperty = sourceproperty;
                Action = action;
            }

            public bool PropertyChanged(string propertyname)
            {
                if (SourceProperty.PropertyName != propertyname) return false;
                Action?.Invoke();
                return true;
            }
        }
        #endregion
    }
}
