using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Segments
{
    public class SegmentView : View, IViewContainer<Segment>, IBorderElement
    {
        public IList<Segment> Children { get; set; } = new List<Segment>();
        public event EventHandler<SelectedItemChangedEventArgs> SelectedIndexChanged;
        public static BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(SegmentView), Color.Blue);
        public static BindableProperty UnselectedTintColorProperty = BindableProperty.Create(nameof(UnselectedTintColor), typeof(Color), typeof(SegmentView), default(Color));
        public static BindableProperty SelectedTextColorProperty = BindableProperty.Create(nameof(SelectedTextColor), typeof(Color), typeof(SegmentView), Color.White);
        public static BindableProperty UnselectedTextColorProperty = BindableProperty.Create(nameof(UnselectedTextColor), typeof(Color), typeof(SegmentView), Color.Silver);
        public static BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(SegmentView), defaultValue: 0, propertyChanged: OnSegmentSelected);
        public static BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(SegmentView));
        public static BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(SegmentView), Color.Blue, propertyChanged:OnBorderColorPropertyChanged);

        public static BindableProperty BorderWidthProperty = BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(SegmentView), defaultValue: 1.0);

        public Color TintColor
        {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }

        public Color UnselectedTintColor
        {
            get => (Color)GetValue(UnselectedTintColorProperty);
            set => SetValue(UnselectedTintColorProperty, value);
        }

        public Color SelectedTextColor
        {
            get => (Color)GetValue(SelectedTextColorProperty);
            set => SetValue(SelectedTextColorProperty, value);
        }

        public Color UnselectedTextColor
        {
            get => (Color)GetValue(UnselectedTextColorProperty);
            set => SetValue(UnselectedTextColorProperty, value);
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

        public int CornerRadiusDefaultValue => 2;
        public Color BorderColorDefaultValue => Device.RuntimePlatform == Device.Android ? Color.FromRgb(41, 98, 255) : Color.Blue;
        public double BorderWidthDefaultValue => 1.0;

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
