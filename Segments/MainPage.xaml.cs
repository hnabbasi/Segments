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
        public MainPage()
        {
            InitializeComponent();
        }

        public void OnSegmentSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ViewSelected.Text = $"Selected '{e.SelectedItem}' at index {e.SelectedItemIndex}";
        }
    }
}
