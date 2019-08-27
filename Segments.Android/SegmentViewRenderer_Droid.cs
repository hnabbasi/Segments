using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using Segments;
using Segments.Droid.Renderers;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRendererAttribute(typeof(Segments.Segments), typeof(SegmentControlRenderer))]
namespace Segments.Droid.Renderers
{
    public class SegmentControlRenderer : ViewRenderer<Segments, RadioGroup>
    {
        readonly Context _context;
        readonly float _defaultControlHeight = 30.0f;
        readonly float _defaultTextSize = 15.0f;

        RadioGroup _radioGroup;
        RadioButton _currentRadioButton;

        Color _tintColor;
        Color _unselectedTintColor;
        Color _unSelectedTextColor;
        Color _selectedTextColor;
        Color _backgroundColor;
        Color _disabledColor = Color.Gray;

        int _buttonHeight;
        int _strokeWidth;
        int _cornerRadius;
        Color _strokeColor;

        public SegmentControlRenderer(Context context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Whent the element is changed.
        /// </summary>
        /// <param name="e">Xamarin.Forms Elements</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Segments> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(BuildControl());
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

            if (e.PropertyName.Equals(nameof(Element.SelectedIndex)))
            {
                var selectedRadioButton = (RadioButton)_radioGroup.GetChildAt(Element.SelectedIndex);
                
                if (selectedRadioButton != null)
                    selectedRadioButton.Checked = true;
                UpdateButtonColors(selectedRadioButton);
            }
        }
        
        RadioGroup BuildControl()
        {
            var height = ConvertToAndroid((float)Element.HeightRequest) > 0
                ? ConvertToAndroid((float)Element.HeightRequest)
                : ConvertToAndroid(_defaultControlHeight);
            _buttonHeight = height;
            _strokeWidth = ConvertToAndroid((float)Element.BorderWidth);
            _cornerRadius = ConvertToAndroid((float)Element.CornerRadius);

            _backgroundColor = Element.BackgroundColor.ToAndroid();
            _tintColor = Element.TintColor.ToAndroid();
            _strokeColor = Element.IsBorderColorSet() ? Element.BorderColor.ToAndroid() : Element.TintColor.ToAndroid();

            // Temporarily disabling these
            _unselectedTintColor = _backgroundColor;// Element.IsUnselectedTintColorSet() ? Element.UnselectedTintColor.ToAndroid() : _backgroundColor;
            _selectedTextColor = Color.White;// Element.SelectedTextColor.ToAndroid();
            _unSelectedTextColor = _tintColor;// Element.IsUnselectedTextColorSet() ? Element.UnselectedTextColor.ToAndroid() : _tintColor;

            _radioGroup = new RadioGroup(_context)
            {
                Orientation = Android.Widget.Orientation.Horizontal,
                LayoutParameters = new RadioGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent)
            };

            for (var i = 0; i < Element.Children.Count; i++)
            {
                var position = i == 0 ? Position.Left : i == Element.Children.Count - 1 ? Position.Right : Position.Middle;
                var rb = GetRadioButton(Element.Children[i].Title, position);
                ConfigureRadioButton(i, rb);
                _radioGroup.AddView(rb);
            }
            return _radioGroup;
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
                rb.SetTextColor(_unSelectedTextColor);
            }

            UpdateButtonColors(rb);

            rb.Enabled = Element.IsEnabled;
        }

        void OnRadioGroupCheckedChanged(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            if (!(sender is RadioGroup rg) || rg.CheckedRadioButtonId == -1)
                return;

            _currentRadioButton?.SetTextColor(_tintColor);
            var index = rg.IndexOfChild(rg.FindViewById(rg.CheckedRadioButtonId));

            var rb = (RadioButton)rg.GetChildAt(index);
            rb.SetTextColor(_selectedTextColor);
            UpdateButtonColors(rb);
            _currentRadioButton = rb;

            Element.SelectedIndex = index;
        }

        void UpdateButtonColors(RadioButton rb)
        {
            var gradientDrawable = (StateListDrawable)rb.Background;
            var drawableContainerState = (DrawableContainer.DrawableContainerState)gradientDrawable.GetConstantState();
            var children = drawableContainerState.GetChildren();

            // Make sure it works on API < 18
            var _selectedShape = (GradientDrawable)(children[0] as InsetDrawable)?.Drawable;
            _selectedShape.SetColor(Element.IsEnabled ? _tintColor : _disabledColor);

            var _unselectedShape = children[1] is GradientDrawable ? (GradientDrawable)children[1] : (GradientDrawable)((InsetDrawable)children[1]).Drawable;
            _unselectedShape.SetColor(_unselectedTintColor);
        }

        #region Drawable Resources

        RadioButton GetRadioButton(string title, Position position)
        {
            var rb = new RadioButton(_context)
            {
                Text = title,
                Gravity = GravityFlags.Center,
                TextAlignment = TextAlignment.Center
            };

            rb.SetButtonDrawable(null);
            rb.SetBackground(GetRadioButtonStateListDrawable(position));
            rb.LayoutParameters = new RadioGroup.LayoutParams(0, LayoutParams.MatchParent, 1.0f);
            rb.SetHeight(_buttonHeight);
            rb.SetTextSize(ComplexUnitType.Sp, _defaultTextSize);
            rb.SetAllCaps(true);
            rb.SetTypeface(null, TypefaceStyle.Bold);
            return rb;
        }

        StateListDrawable GetRadioButtonStateListDrawable(Position position)
        {
            var drawable = new StateListDrawable();
            drawable.AddState(new int[] { Android.Resource.Attribute.StateChecked }, GetCheckedDrawable(position));
            drawable.AddState(new int[] { -Android.Resource.Attribute.StateChecked }, GetUncheckedDrawable(position));
            return drawable;
        }

        InsetDrawable GetCheckedDrawable(Position position)
        {
            var rect = new GradientDrawable();
            rect.SetShape(ShapeType.Rectangle);
            rect.SetColor(_tintColor);
            rect.SetStroke(_strokeWidth, _strokeColor);            

            switch (position)
            {
                case Position.Left:
                    rect.SetCornerRadii(new float[] { _cornerRadius, _cornerRadius, 0, 0, 0, 0, _cornerRadius, _cornerRadius });
                    return new InsetDrawable(rect, 0);

                case Position.Right:
                    rect.SetCornerRadii(new float[] { 0, 0, _cornerRadius, _cornerRadius, _cornerRadius, _cornerRadius, 0, 0 });
                    break;
                default:
                    rect.SetCornerRadius(0);
                    break;
            }

            return new InsetDrawable(rect, -_strokeWidth, 0, 0, 0);
        }

        InsetDrawable GetUncheckedDrawable(Position position)
        {
            var rect = new GradientDrawable();
            rect.SetShape(ShapeType.Rectangle);
            rect.SetColor(_backgroundColor);
            rect.SetStroke(_strokeWidth, _strokeColor);

            switch (position)
            {
                case Position.Left:
                    rect.SetCornerRadii(new float[] { _cornerRadius, _cornerRadius, 0, 0, 0, 0, _cornerRadius, _cornerRadius });
                    return new InsetDrawable(rect, 0);
                case Position.Right:
                    rect.SetCornerRadii(new float[] { 0, 0, _cornerRadius, _cornerRadius, _cornerRadius, _cornerRadius, 0, 0 });
                    break;
                default:
                    rect.SetCornerRadius(0);
                    break;
            }

            return new InsetDrawable(rect, -_strokeWidth, 0, 0, 0);
        }

        #endregion

        public int ConvertToAndroid(float f)
        {
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, f, _context.Resources.DisplayMetrics);
        }

        /// <summary>
        /// Dispose the specified disposing.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (_radioGroup != null)
            {
                _radioGroup.CheckedChange -= OnRadioGroupCheckedChanged;
                _radioGroup?.RemoveAllViews();
                _radioGroup?.Dispose();
            }
            _currentRadioButton?.Dispose();
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Position of the segment. Left, Middle, Right.
    /// </summary>
    enum Position
    {
        Middle,
        Left,
        Right
    }
}
