using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using System.Linq;
using Segments;
using Segments.iOS.Renderers;
using System;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Forms.Internals;
using Foundation;

#if __MOBILE__
using UIKit;
using NativeImage = UIKit.UIImage;

[assembly: ExportRenderer(typeof(Segments.Segments), typeof(SegmentControlRenderer))]
namespace Segments.iOS.Renderers
#else
using AppKit;
using CoreAnimation;
using NativeImage = AppKit.NSImage;
namespace Xamarin.Forms.Platform.MacOS
#endif
{
    public class SegmentControlRenderer : ViewRenderer<Segments, UISegmentedControl>
    {
        UISegmentedControl _control;

        protected async override void OnElementChanged(ElementChangedEventArgs<Segments> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (Control == null)
            {
                _control = new UISegmentedControl();

                for (int i = 0; i < Element.Items.Count; i++)
                {
                    var segment = Element.Items.ElementAt(i);
                    var img = await GetImage(segment);
                    if (img == null)
                        _control.InsertSegment(segment, i, false);
                    else
                    {
                        img = img.Scale(new CoreGraphics.CGSize(17, 17)); // Apple docs
                        _control.InsertSegment(img, i, false);
                    }
                }

                _control.ClipsToBounds = true;
                _control.TintColor = Element.TintColor.ToUIColor();
                _control.SelectedSegment = Element.SelectedIndex;
                _control.BackgroundColor = Element.BackgroundColor.ToUIColor();                
                _control.Layer.MasksToBounds = true;

                _control.Layer.BorderColor = Element.IsBorderColorSet()
                    ? Element.BorderColor.ToCGColor()
                    : Element.TintColor.ToCGColor();

                if (Element.IsCornerRadiusSet())
                {
                    _control.Layer.CornerRadius = Element.CornerRadius;
                    _control.Layer.BorderWidth = (nfloat)Element.BorderWidth;
                }

                SetSelectedTextColor();
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

            if (e.PropertyName == Segments.SelectedIndexProperty.PropertyName)
            {
                _control.SelectedSegment = Element.SelectedIndex;
                SetSelectedTextColor();
            }
            else if(e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
            {
                _control.Enabled = Element.IsEnabled;
                _control.TintColor = Element.IsEnabled ? Element.TintColor.ToUIColor() : Color.Gray.ToUIColor();// Element.UnselectedTintColor.ToUIColor()                    
            }
        }

        private Task<NativeImage> GetImage(ImageSource imageSource)
        {
            IImageSourceHandler handler = null;

            if (imageSource is FileImageSource)
            {
                handler = new FileImageSourceHandler();
            }
            else if (imageSource is StreamImageSource)
            {
                handler = new StreamImagesourceHandler();
            }
            else if (imageSource is UriImageSource)
            {
                handler = new ImageLoaderSourceHandler();
            }
            else if (imageSource is FontImageSource)
            {
                handler = new FontImageSourceHandler();
            }
            return handler.LoadImageAsync(imageSource);
        }

        void SetSelectedTextColor()
        {
            // Don't change the color on iOS 13
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                return;

            var attr = new UITextAttributes
            {
                TextColor = Element.IsEnabled ? Color.White.ToUIColor() : Element.TintColor.ToUIColor()
            };
            _control.SetTitleTextAttributes(attr, Element.IsEnabled ? UIControlState.Selected : UIControlState.Normal);
        }

        void OnSelectedIndexChanged(object sender, System.EventArgs e)
        {
            Element.SelectedIndex = (int)_control.SelectedSegment;
        }

        protected override void Dispose(bool disposing)
        {
            _control.ValueChanged -= OnSelectedIndexChanged;
            _control.Dispose();
            base.Dispose(disposing);
        }
    }
}
