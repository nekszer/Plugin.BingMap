using Plugin.BingMap;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Plugin.BingMap.Behaviors
{
    public class MapSetCenterBehavior : Behavior<Map>
    {

        #region BindableProperty Center
        /// <summary>
        /// Center of map
        /// </summary>
        public static readonly BindableProperty CenterProperty = BindableProperty.Create(nameof(Center), typeof(Center), typeof(MapSetCenterBehavior), default(Center), BindingMode.OneWay);

        /// <summary>
        /// Center of map
        /// </summary>
        public Center Center
        {
            get
            {
                return (Center)GetValue(CenterProperty);
            }

            set
            {
                SetValue(CenterProperty, value);
            }
        }
        #endregion

        private Map CurrentMap { get; set; }

        public MapSetCenterBehavior()
        {
            BindableProperties = new List<BindablePropertyChanged>
            {
                new BindablePropertyChanged(CenterProperty, SetCurrentCenter)
            };
        }

        private void SetCurrentCenter()
        {
            if (CurrentMap == null) return;
            if (Center == null) return;
            CurrentMap.SetCenter(Center);
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
            SetCurrentCenter();
        }

        private void Bindable_LoadComplete(object sender, EventArgs e)
        {
            CurrentMap = sender as Map;
            SetCurrentCenter();
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
