using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace Segments
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private Tuple<string, Color>[] _colors;

        Segment[] _names;
        public Segment[] Names 
        { 
            get => _names;
            set {
                _names = value;
                OnPropertyChanged();
            }
        }

        public MainPage()
        {
            InitializeComponent();
            _colors = new Tuple<string, Color>[]
            {
                new Tuple<string, Color>("Red", Color.Red),
                new Tuple<string, Color>("Blue", Color.Blue),
                new Tuple<string, Color>("Green", Color.Green),
                new Tuple<string, Color>("Purple", Color.Purple),
            };

            Names = new Segment[] {
                new Segment { Title = "A-F" },
                new Segment { Title = "G-R" },
                new Segment { Title = "S-Z" }
            };
            // ColorPicker.ItemsSource = _colors;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // var max = Device.RuntimePlatform == Device.iOS ? top.HeightRequest / 2 : 20;
            // CornerRadiusSlider.Maximum = max < 10 ? 20 : max;
        }

        public void CornerRadiusChanged(object sender, ValueChangedEventArgs e)
        {
            //top.CornerRadius = (int)e.NewValue;
            // CornerRadiusValue.Text = ((int)e.NewValue).ToString();
        }

        protected void OnBorderWidthChanged(object sender, ValueChangedEventArgs e)
        {
            //top.BorderWidth = e.NewValue;
            // BorderWidthValue.Text = ((int)e.NewValue).ToString();
        }

        public void TintChanged(object sender, EventArgs e)
        {
            //top.TintColor = _colors[ColorPicker.SelectedIndex].Item2;
        }

        public void OnSegmentSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ViewSelected.Text = $"Selected '{((Segment)e.SelectedItem).Title}' at index {e.SelectedItemIndex}";
        }
    }
}
