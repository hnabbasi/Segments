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
            ColorPicker.ItemsSource = _colors;

            // go ahead, play with it
            //AddTime();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CornerRadiusSlider.Maximum = top.Height / 2;
        }

        public void CornerRadiusChanged(object sender, ValueChangedEventArgs e)
        {
            top.CornerRadius = (int)e.NewValue;
        }

        public void TintChanged(object sender, EventArgs e)
        {
            top.TintColor = _colors[ColorPicker.SelectedIndex].Item2;
        }

        public void OnSegmentSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ViewSelected.Text = $"Selected '{((Segment)e.SelectedItem).Title}' at index {e.SelectedItemIndex}";
        }

        void AddTime()
        {
            MainStack.Children.Add(new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new Label
                    {
                        Text = "Time",
                        VerticalTextAlignment = TextAlignment.Center
                    },
                    new SegmentView
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children = new List<Segment>
                        {
                            { new Segment { Title = "Morning" } },
                            { new Segment { Title = "Evening" } },
                            { new Segment { Title = "Night" } }
                        }
                    }
                }
            });
        }
    }
}
