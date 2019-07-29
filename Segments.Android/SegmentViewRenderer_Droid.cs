using System;
using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Segments;
using Segments.Droid.Renderers;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(SegmentView), typeof(SegmentControlRenderer))]
namespace Segments.Droid.Renderers
{
    public class SegmentControlRenderer : ViewRenderer<SegmentView, RadioGroup>
    {
        readonly Context _context;

        RadioGroup _radioGroup;
        RadioButton _currentRadioButton;

        Color _tintColor;
        Color _unselectedTintColor;
        Color _selectedTextColor;
        Color _unselectedTextColor;
        Color _backgroundColor;
        Color _disabledColor = Color.Gray;

        int _buttonHeight = 40;
        int _strokeWidth = 12;

        public SegmentControlRenderer(Context context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Whent the element is changed.
        /// </summary>
        /// <param name="e">Xamarin.Forms Elements</param>
        protected override void OnElementChanged(ElementChangedEventArgs<SegmentView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                InitializeColors();
                PopulateSegments();
                SetNativeControl(_radioGroup);
            }

            if (e.OldElement != null && _radioGroup != null)
                _radioGroup.CheckedChange -= OnRadioGroupCheckedChanged;

            if (e.NewElement != null)
            {
                _radioGroup.CheckedChange += OnRadioGroupCheckedChanged;
                var selected = (RadioButton)_radioGroup.GetChildAt(Element.SelectedIndex);
                selected.Checked = true;
            }
        }
        
        /// <summary>
        /// When a property of an element changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Xamarin.Forms Elements</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (_radioGroup == null || Element == null)
                return;

            switch (e.PropertyName)
            {
                case "SelectedIndex":
                    var option = (RadioButton)_radioGroup.GetChildAt(Element.SelectedIndex);

                    if (option != null)
                        option.Checked = true;
                    break;
            }
        }

        void PopulateSegments()
        {
            if (_radioGroup != null)
            {
                _radioGroup.RemoveAllViews();
                _radioGroup = null;
                _radioGroup.Dispose();
            }

            _radioGroup = new RadioGroup(_context)
            {
                Orientation = Android.Widget.Orientation.Horizontal,
                LayoutParameters = new RadioGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent)
            };
            _radioGroup.SetBackgroundColor(_backgroundColor);
            _radioGroup.SetMinimumHeight(_buttonHeight);

            for (var i = 0; i < Element.Children.Count; i++)
            {
                var rb = GetRadioButton(Element.Children[i].Title);
                ConfigureRadioButton(i, rb);
                _radioGroup.AddView(rb);
            }
        }

        void ConfigureRadioButton(int i, RadioButton rb)
        {
            if (i == Element.SelectedIndex)
            {
                rb.SetTextColor(_tintColor);
                _currentRadioButton = rb;
            }
            else
            {
                rb.SetTextColor(_unselectedTextColor);
            }

            UpdateButtonColors(rb);

            rb.Enabled = Element.IsEnabled;
        }

        void InitializeColors()
        {
            _backgroundColor = Element.BackgroundColor.ToAndroid();
            _tintColor = Element.TintColor.ToAndroid();
            _unselectedTintColor = Element.UnselectedTintColor.ToAndroid();
            _unselectedTextColor = Element.SelectedTextColor.ToAndroid();
            _selectedTextColor = Element.UnselectedTextColor.ToAndroid();
        }

        void UpdateButtonColors(RadioButton rb)
        {
            var gradientDrawable = (StateListDrawable)rb.Background;
            var drawableContainerState = (DrawableContainer.DrawableContainerState)gradientDrawable.GetConstantState();
            var children = drawableContainerState.GetChildren();

            var color = Element.IsEnabled ? _tintColor : _disabledColor;

            GradientDrawable _selectedShape;
            GradientDrawable _unselectedShape;

            if (children[0] is InsetDrawable inner)
            {
                //layers = (LayerDrawable)children[0];
                //var outer = layers.GetDrawable(0);
                //var outerInset = outer as InsetDrawable;
                //_selectedShape = outerInset.Drawable as GradientDrawable;

                ////var inner = layers.GetDrawable(1);
                //var inner = layers.GetDrawable(0);
                var innerInset = inner as InsetDrawable;

                _selectedShape = innerInset.Drawable as GradientDrawable;
                //_selectedShape.SetStroke(_strokeWidth, color);
            }
            else
            {
                // Doesnt works on API < 18
                _selectedShape = children[0] is GradientDrawable ? (GradientDrawable)children[0] : (GradientDrawable)((InsetDrawable)children[0]).Drawable;
            }
            //_selectedShape.SetStroke(_strokeWidth, _backgroundColor);
            _selectedShape.SetStroke(_strokeWidth, color);
            _selectedShape.SetColor(color);

            _unselectedShape = children[1] is GradientDrawable ? (GradientDrawable)children[1] : (GradientDrawable)((InsetDrawable)children[1]).Drawable;
            _unselectedShape.SetColor(_unselectedTintColor);
            _unselectedShape.SetStroke(0, color);
        }

        void OnRadioGroupCheckedChanged(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            var rg = (RadioGroup)sender;
            if (rg.CheckedRadioButtonId == -1)
                return;

            var id = rg.CheckedRadioButtonId;
            var radioButton = rg.FindViewById(id);
            var radioId = rg.IndexOfChild(radioButton);

            var rb = (RadioButton)rg.GetChildAt(radioId);

            _currentRadioButton?.SetTextColor(_tintColor);
            rb.SetTextColor(_selectedTextColor);
            UpdateButtonColors(rb);

            _currentRadioButton = rb;
            Element.SelectedIndex = radioId;
        }

        #region Drawable Resources

        RadioButton GetRadioButton(string title)
        {
            var rb = new RadioButton(_context)
            {
                Text = title,
                Gravity = GravityFlags.Center,
                TextAlignment = TextAlignment.Center,
            };
            rb.SetButtonDrawable(null);
            rb.SetBackground(GetRadioButtonDrawable());
            rb.SetTextColor(GetColorList());
            rb.LayoutParameters = new RadioGroup.LayoutParams(0, LayoutParams.MatchParent, 1.0f);
            rb.SetMinimumHeight(_buttonHeight);
            rb.SetTextSize(Android.Util.ComplexUnitType.Sp, 14.0f);
            return rb;
        }

        StateListDrawable GetRadioButtonDrawable()
        {
            var drawable = new StateListDrawable();
            drawable.AddState(new int[] { Android.Resource.Attribute.StateChecked }, GetCheckedDrawable());
            drawable.AddState(new int[] { -Android.Resource.Attribute.StateChecked }, GetUncheckedDrawable());
            return drawable;
        }

        InsetDrawable GetCheckedDrawable()
        {
            var rect = new GradientDrawable();
            rect.SetShape(ShapeType.Rectangle);
            rect.SetColor(GetColorList());
            //rect.SetStroke(0, _tintColor);
            rect.SetStroke(1, _tintColor);
            rect.SetCornerRadius(0);

            //var line = new GradientDrawable();
            //line.SetShape(ShapeType.Rectangle);
            //line.SetStroke(_strokeWidth, _tintColor);

            return new InsetDrawable(rect, 0);
            //return new LayerDrawable(new Drawable[] {
            //    new InsetDrawable(rect, 0),
            //    //new InsetDrawable(line, -_strokeWidth, -_strokeWidth, -_strokeWidth, 0)
            //});
        }

        InsetDrawable GetUncheckedDrawable()
        {
            var rect = new GradientDrawable();
            rect.SetShape(ShapeType.Rectangle);
            rect.SetColor(GetColorList());
            rect.SetStroke(0, _unselectedTintColor);
            
            return new InsetDrawable(rect, 0);
        }

        ColorStateList GetColorList()
        {
            return new ColorStateList(new int[][] {
                new int[] { Android.Resource.Attribute.StateChecked,
                            -Android.Resource.Attribute.StateChecked }},
                new int[] { _unselectedTintColor,
                            _tintColor });
        }

        #endregion

        /// <summary>
        /// Dispose the specified disposing.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (_radioGroup != null)
                _radioGroup.CheckedChange -= OnRadioGroupCheckedChanged;
            base.Dispose(disposing);
        }
    }
}
