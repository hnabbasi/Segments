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
        public MainPage()
        {
            InitializeComponent();

            // go ahead, play with it
            //AddTime();
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
