using System;
using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
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
        //Color _unselectedTintColor;
        Color _selectedTextColor;
        Color _unselectedTextColor;
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
        protected override void OnElementChanged(ElementChangedEventArgs<SegmentView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                InitializeFields();
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

            if (e.PropertyName.Equals(nameof(Element.SelectedIndex)))
            {
                var selectedRadioButton = (RadioButton)_radioGroup.GetChildAt(Element.SelectedIndex);
                
                if (selectedRadioButton != null)
                    selectedRadioButton.Checked = true;
                UpdateButtonColors(selectedRadioButton);
            }
        }

        void InitializeFields()
        {
            var height = ConvertToAndroid((float)Element.HeightRequest) > 0
                ? ConvertToAndroid((float)Element.HeightRequest)
                : ConvertToAndroid(30.0f);
            _buttonHeight = height;
            _strokeWidth = ConvertToAndroid((float)Element.BorderWidth);
            _cornerRadius = ConvertToAndroid((float)Element.CornerRadius);
        }

        void PopulateSegments()
        {
            if (_radioGroup != null)
            {
                _radioGroup.RemoveAllViews();
            }

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
            _radioGroup.SetMinimumHeight(_buttonHeight);
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
            //_unselectedTintColor = Element.UnselectedTintColor.ToAndroid();
            _unselectedTextColor = Element.SelectedTextColor.ToAndroid();
            _selectedTextColor = Element.UnselectedTextColor.ToAndroid();
            _strokeColor = Element.IsBorderColorSet() ? Element.BorderColor.ToAndroid() : Element.TintColor.ToAndroid();
        }

        void UpdateButtonColors(RadioButton rb)
        {
            var gradientDrawable = (StateListDrawable)rb.Background;
            var drawableContainerState = (DrawableContainer.DrawableContainerState)gradientDrawable.GetConstantState();
            var children = drawableContainerState.GetChildren();

            var color = Element.IsEnabled ? _tintColor : _disabledColor;

            // Make sure it works on API < 18
            var _selectedShape = (GradientDrawable)(children[0] as InsetDrawable)?.Drawable;

            _selectedShape.SetStroke(_strokeWidth, _strokeColor);
            _selectedShape.SetColor(color);

            var _unselectedShape = children[1] is GradientDrawable ? (GradientDrawable)children[1] : (GradientDrawable)((InsetDrawable)children[1]).Drawable;
            _unselectedShape.SetColor(_backgroundColor);
            _unselectedShape.SetStroke(_strokeWidth, _strokeColor);
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

        RadioButton GetRadioButton(string title, Position position)
        {
            var rb = new RadioButton(_context)
            {
                Text = title,
                Gravity = GravityFlags.Center,
                TextAlignment = TextAlignment.Center,
            };
            rb.SetButtonDrawable(null);
            rb.SetBackground(GetRadioButtonDrawable(position));
            rb.SetTextColor(RadioButtonColorStateList);
            rb.LayoutParameters = new RadioGroup.LayoutParams(0, LayoutParams.MatchParent, 1.0f);
            rb.SetHeight(_buttonHeight);
            rb.SetTextSize(ComplexUnitType.Sp, 15.0f);
            rb.SetAllCaps(true);
            rb.SetTypeface(null, TypefaceStyle.Bold);
            return rb;
        }

        StateListDrawable GetRadioButtonDrawable(Position position)
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
            rect.SetColor(RadioButtonColorStateList);
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
            rect.SetColor(RadioButtonColorStateList);

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

        ColorStateList _radioButtonColorStateList;
        internal ColorStateList RadioButtonColorStateList
        {
            get
            {
                if (_radioButtonColorStateList == null)
                {
                    _radioButtonColorStateList = new ColorStateList(new int[][] {
                        new int[] {
                            Android.Resource.Attribute.StateChecked,
                            -Android.Resource.Attribute.StateChecked }},
                        new int[] {
                            _backgroundColor,
                            _strokeColor });
                }
                return _radioButtonColorStateList;
            }
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
                _radioGroup.CheckedChange -= OnRadioGroupCheckedChanged;
            _radioButtonColorStateList?.Dispose();
            base.Dispose(disposing);
        }
    }

    enum Position
    {
        Middle,
        Left,
        Right
    }
}
