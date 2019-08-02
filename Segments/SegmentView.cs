using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Segments
{
    public class SegmentView : View, IViewContainer<Segment>, IBorderElement
    {
        private static readonly double _borderWidthDefaultValue = Device.RuntimePlatform == Device.Android ? 3.0 : 1.0;
        private static readonly Color _borderColorDefaultValue = Device.RuntimePlatform == Device.Android ? Color.FromRgb(41, 98, 255) : Color.Blue;
        private static readonly int _cornerRadiusDefaultValue = Device.RuntimePlatform == Device.Android ? 6 : 2;
        private static readonly Color _tintColorDefaultValue = Device.RuntimePlatform == Device.Android ? Color.FromRgb(33, 148, 240) : Color.Blue;

        public IList<Segment> Children { get; set; } = new List<Segment>();
        public event EventHandler<SelectedItemChangedEventArgs> SelectedIndexChanged;
        public static BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(SegmentView), _tintColorDefaultValue);
        public static BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(SegmentView), 0, propertyChanged: OnSegmentSelected);
        public static BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(SegmentView), _cornerRadiusDefaultValue);
        public static BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(SegmentView), _borderColorDefaultValue, propertyChanged:OnBorderColorPropertyChanged);
        public static BindableProperty BorderWidthProperty = BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(SegmentView), _borderWidthDefaultValue);

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
            (bindable as SegmentView)?.OnBorderColorPropertyChanged((Color)oldValue, (Color)newValue);
        }

        private static void OnSegmentSelected(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is SegmentView segment)) return;
            int.TryParse(newValue?.ToString(), out int index);
            segment.SelectedIndexChanged?.Invoke(segment, new SelectedItemChangedEventArgs(segment?.Children[index], index));
        }
    }

    public class Segment : View
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(Segment), string.Empty);

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}
