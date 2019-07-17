using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using UIKit;
using System.Linq;
using Segments;
using Segments.iOS.Renderers;

[assembly: ExportRenderer(typeof(SegmentView), typeof(SegmentControlRenderer))]
namespace Segments.iOS.Renderers
{
    public class SegmentControlRenderer : ViewRenderer<SegmentView, UISegmentedControl>
    {
        UISegmentedControl _control;

        protected override void OnElementChanged(ElementChangedEventArgs<SegmentView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (Control == null)
            {

                _control = new UISegmentedControl();

                for (int i = 0; i < Element.Children.Count; i++)
                {
                    _control.InsertSegment(Element.Children.ElementAt(i).Title, i, false);
                }

                _control.TintColor = Element.TintColor.ToUIColor();
                _control.SelectedSegment = Element.SelectedIndex;
                _control.ClipsToBounds = true;
                _control.BackgroundColor = Element.BackgroundColor.ToUIColor();
                
                SetSelectedTextColor(false);
                SetNativeControl(_control);
            }

            if (e.OldElement != null && _control != null)
                _control.ValueChanged -= OnSelectedIndexChanged;

            if (e.NewElement != null)
                _control.ValueChanged += OnSelectedIndexChanged;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (_control == null || Element == null) return;

            switch (e.PropertyName)
            {
                case "SelectedSegment":
                    _control.SelectedSegment = Element.SelectedIndex;
                    break;
                case "TintColor":
                    _control.TintColor = Element.IsEnabled ? Element.TintColor.ToUIColor() : Element.UnselectedTintColor.ToUIColor();
                    break;
                case "IsEnabled":
                    _control.Enabled = Element.IsEnabled;
                    _control.TintColor = Element.IsEnabled ? Element.TintColor.ToUIColor() : Element.UnselectedTintColor.ToUIColor();
                    break;
                case "SelectedTextColor":
                    SetSelectedTextColor(true);
                    break;
            }
        }

        void SetSelectedTextColor(bool isEnabled)
        {
            var attr = new UITextAttributes
            {
                TextColor = isEnabled ? Element.SelectedTextColor.ToUIColor() : Element.TintColor.ToUIColor()
            };
            _control.SetTitleTextAttributes(attr, isEnabled ? UIControlState.Selected : UIControlState.Normal);
        }

        void OnSelectedIndexChanged(object sender, System.EventArgs e)
        {
            Element.SelectedIndex = (int)_control.SelectedSegment;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _control.ValueChanged -= OnSelectedIndexChanged;
        }
    }
}
