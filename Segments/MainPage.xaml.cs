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
        private List<string> _source;

        public MainPage()
        {
            InitializeComponent();
            //textSegments.ItemsSource = new List<string> {
            //    "Mon",
            //    "Tue",
            //    "Wed"
            //};
            BindingContext = new ViewModel();
        }
        public List<string> Source
        {
            get => _source;
            set { _source = value; OnPropertyChanged(); }
        }
        public void OnSegmentSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ViewSelected.Text = $"Selected '{e.SelectedItem}' at index {e.SelectedItemIndex}";
        }
    }

    public class ViewModel : INotifyPropertyChanged
    {
        private IList<Segment> _objectSource;
        public IList<Segment> ObjectSource
        {
            get => _objectSource;
            set
            {
                _objectSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ObjectSource)));
            }
        }

        private IList<string> _source;
        public IList<string> StringSource
        {
            get => _source;
            set
            {
                _source = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StringSource)));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            StringSource = new List<string>
            {
                "VM STR A",
                "VM STR B",
                "VM STR C"
            };

            ObjectSource = new List<Segment>
            {
                new Segment {Name = "VM OBJ A", Age = 20 },
                new Segment {Name = "VM OBJ B", Age = 30 },
                new Segment {Name = "VM OBJ C", Age = 40 },
            };
        }
    }

    public class Segment
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
