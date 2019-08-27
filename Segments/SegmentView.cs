using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Platform;

namespace Segments
{
    public class Segments : View, IBorderElement
    {
        private static readonly double _borderWidthDefaultValue = Device.RuntimePlatform == Device.Android ? 3.0 : 1.0;
        private static readonly Color _borderColorDefaultValue = Device.RuntimePlatform == Device.Android ? Color.FromRgb(41, 98, 255) : Color.Blue;
        private static readonly int _cornerRadiusDefaultValue = Device.RuntimePlatform == Device.Android ? 6 : 2;
        private static readonly Color _tintColorDefaultValue = Device.RuntimePlatform == Device.Android ? Color.FromRgb(33, 148, 240) : Color.Blue;

        public event EventHandler<SelectedItemChangedEventArgs> SelectedIndexChanged;

        public static BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(Segments), _tintColorDefaultValue);
        public static BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(Segments), 0, propertyChanged: OnSegmentSelected);
        public static BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(Segments), _cornerRadiusDefaultValue);
        public static BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(Segments), _borderColorDefaultValue, propertyChanged:OnBorderColorPropertyChanged);
        public static BindableProperty BorderWidthProperty = BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(Segments), _borderWidthDefaultValue);

        #region With ItemSource

        public IList<string> Items { get; } = new LockableObservableListWrapper();

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(Segments), default(IList),
                                    propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(Segments), null, BindingMode.TwoWay,
                                    propertyChanged: OnSelectedItemChanged);

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        BindingBase _itemDisplayBinding;
        public BindingBase ItemDisplayBinding
        {
            get { return _itemDisplayBinding; }
            set
            {
                if (_itemDisplayBinding == value)
                    return;

                OnPropertyChanging();
                _itemDisplayBinding = value;
                ResetItems();
                OnPropertyChanged();
            }
        }

        static readonly BindableProperty s_displayProperty =
            BindableProperty.Create("Display", typeof(string), typeof(Segments), default(string));

        static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((Segments)bindable)?.OnItemsSourceChanged((IList)oldValue, (IList)newValue);
        }

        void OnItemsSourceChanged(IList oldValue, IList newValue)
        {
            var oldObservable = oldValue as INotifyCollectionChanged;
            if (oldObservable != null)
                oldObservable.CollectionChanged -= CollectionChanged;

            var newObservable = newValue as INotifyCollectionChanged;
            if (newObservable != null)
            {
                newObservable.CollectionChanged += CollectionChanged;
            }

            if (newValue != null)
                ResetItems();
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItems(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(e);
                    break;
                default: //Move, Replace, Reset
                    ResetItems();
                    break;
            }
        }
        void AddItems(NotifyCollectionChangedEventArgs e)
        {
            int index = e.NewStartingIndex < 0 ? Items.Count : e.NewStartingIndex;
            foreach (object newItem in e.NewItems)
                ((LockableObservableListWrapper)Items).InternalInsert(index++, GetDisplayMember(newItem));
        }

        void RemoveItems(NotifyCollectionChangedEventArgs e)
        {
            int index = e.OldStartingIndex < Items.Count ? e.OldStartingIndex : Items.Count;
            foreach (object _ in e.OldItems)
                ((LockableObservableListWrapper)Items).InternalRemoveAt(index--);
        }

        void ResetItems()
        {
            if (ItemsSource == null)
                return;
            ((LockableObservableListWrapper)Items).InternalClear();
            foreach (object item in ItemsSource)
                ((LockableObservableListWrapper)Items).InternalAdd(GetDisplayMember(item));
            UpdateSelectedItem(SelectedIndex);
        }

        string GetDisplayMember(object item)
        {
            if (ItemDisplayBinding == null)
                return item.ToString();

            //ItemDisplayBinding.Apply(item, this, s_displayProperty);
            //ItemDisplayBinding.Unapply();
            return (string)GetValue(s_displayProperty);
        }

        static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var segments = (Segments)bindable;
            segments.SelectedItem = newValue;
        }

        void UpdateSelectedItem(int index)
        {
            if (index == -1)
            {
                SelectedItem = null;
                return;
            }

            if (ItemsSource != null)
            {
                SelectedItem = ItemsSource[index];
                return;
            }

            SelectedItem = Items[index];
        }
        #endregion

        #region Can't control these on iOS elegantly. Maybe move it to On<Android>

        //public static BindableProperty UnselectedTintColorProperty = BindableProperty.Create(nameof(UnselectedTintColor), typeof(Color), typeof(SegmentView), default(Color));
        //public static BindableProperty SelectedTextColorProperty = BindableProperty.Create(nameof(SelectedTextColor), typeof(Color), typeof(SegmentView), Color.White);
        //public static BindableProperty UnselectedTextColorProperty = BindableProperty.Create(nameof(UnselectedTextColor), typeof(Color), typeof(SegmentView), Color.Silver);

        //public Color UnselectedTintColor
        //{
        //    get => (Color)GetValue(UnselectedTintColorProperty);
        //    set => SetValue(UnselectedTintColorProperty, value);
        //}

        //public Color SelectedTextColor
        //{
        //    get => (Color)GetValue(SelectedTextColorProperty);
        //    set => SetValue(SelectedTextColorProperty, value);
        //}

        //public Color UnselectedTextColor
        //{
        //    get => (Color)GetValue(UnselectedTextColorProperty);
        //    set => SetValue(UnselectedTextColorProperty, value);
        //}

        //public bool IsUnselectedTextColorSet() => IsSet(UnselectedTextColorProperty);
        //public bool IsUnselectedTintColorSet() => IsSet(UnselectedTintColorProperty);
        #endregion

        public Color TintColor
        {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public int CornerRadius
        {
            get => (int)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public double BorderWidth
        {
            get => (double)GetValue(BorderWidthProperty);
            set => SetValue(BorderWidthProperty, value);
        }

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public int CornerRadiusDefaultValue => _cornerRadiusDefaultValue;
        public Color BorderColorDefaultValue => _borderColorDefaultValue;
        public double BorderWidthDefaultValue => _borderWidthDefaultValue;


        public void OnBorderColorPropertyChanged(Color oldValue, Color newValue)
        {
            // what's this for?
        }

        public bool IsCornerRadiusSet() => IsSet(CornerRadiusProperty);
        public bool IsBackgroundColorSet() => IsSet(BackgroundColorProperty);
        public bool IsBorderColorSet() => IsSet(BorderColorProperty);
        public bool IsBorderWidthSet() => IsSet(BorderWidthProperty);
        
        private static void OnBorderColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as Segments)?.OnBorderColorPropertyChanged((Color)oldValue, (Color)newValue);
        }

        private static void OnSegmentSelected(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is Segments segment)) return;
            int.TryParse(newValue?.ToString(), out int index);
            segment.SelectedIndexChanged?.Invoke(segment, new SelectedItemChangedEventArgs(segment?.ItemsSource[index], index));
        }
    }
}
