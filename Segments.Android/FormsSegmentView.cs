using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

namespace Segments.Droid
{
    public class FormsSegmentView : RadioGroup
    {
        readonly Context _context;
        readonly SegmentView _element;
        RadioButton _currentRadioButton;

        public FormsSegmentView(Context context, SegmentView element) : base(context)
        {
            _context = context;
            _element = element;
            Orientation = Android.Widget.Orientation.Horizontal;
            LayoutParameters = new RadioGroup.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
            CheckedChange += FormsSegmentView_CheckedChange;
            InitializeColors();
            PopulateSegments();
        }

        private void FormsSegmentView_CheckedChange(object sender, CheckedChangeEventArgs e)
        {
            var rg = (RadioGroup)sender;
            if (rg.CheckedRadioButtonId == -1)
                return;

            var id = rg.CheckedRadioButtonId;
            var radioButton = rg.FindViewById(id);
            var radioId = rg.IndexOfChild(radioButton);

            var rb = (RadioButton)rg.GetChildAt(radioId);

            _currentRadioButton?.SetTextColor(TintColor);
            rb.SetTextColor(SelectedTextColor);
            UpdateButtonColors(rb);

            _currentRadioButton = rb;
            _element.SelectedIndex = radioId;
        }

        public Color TintColor { get; set; }
        public Color UnselectedTintColor { get; set; }
        public Color BackgroundColor { get; set; }
        public Color SelectedTextColor { get; set; }
        public Color UnselectedTextColor { get; set; }
        public IEnumerable<Segment> Segments => _element.Children;
        
        void PopulateSegments()
        {
            RemoveAllViews();
            for (var i = 0; i < _element.Children.Count; i++)
            {
                var rb = GetRadioButton(_element.Children[i].Title);
                ConfigureRadioButton(i, rb);
                AddView(rb);
            }
        }

        void ConfigureRadioButton(int i, RadioButton rb)
        {
            if (i == _element.SelectedIndex)
            {
                rb.SetTextColor(TintColor);
                _currentRadioButton = rb;
            }
            else
            {
                rb.SetTextColor(UnselectedTextColor);
            }

            UpdateButtonColors(rb);
        }

        void UpdateButtonColors(RadioButton rb)
        {
            var gradientDrawable = (StateListDrawable)rb.Background;
            var drawableContainerState = (DrawableContainer.DrawableContainerState)gradientDrawable.GetConstantState();
            var children = drawableContainerState.GetChildren();

            var color = _element.IsEnabled ? TintColor : Color.Gray;

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
            _selectedShape.SetStroke((int)_element.BorderWidth, color);
            _selectedShape.SetColor(color);

            _unselectedShape = children[1] is GradientDrawable ? (GradientDrawable)children[1] : (GradientDrawable)((InsetDrawable)children[1]).Drawable;
            _unselectedShape.SetColor(UnselectedTintColor);
            _unselectedShape.SetStroke(0, color);
        }

        void InitializeColors()
        {
            BackgroundColor = _element.BackgroundColor.ToAndroid();
            TintColor = _element.TintColor.ToAndroid();
            //UnselectedTintColor = _element.UnselectedTintColor.ToAndroid();
            //UnselectedTextColor = _element.SelectedTextColor.ToAndroid();
            //SelectedTextColor = _element.UnselectedTextColor.ToAndroid();
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
            rb.SetMinimumHeight(Height);
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
            rect.SetStroke(1, TintColor);
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
            rect.SetStroke(0, UnselectedTintColor);

            return new InsetDrawable(rect, 0);
        }

        ColorStateList GetColorList()
        {
            return new ColorStateList(new int[][] {
                new int[] { Android.Resource.Attribute.StateChecked,
                            -Android.Resource.Attribute.StateChecked }},
                new int[] { UnselectedTintColor,
                            TintColor });
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            CheckedChange -= FormsSegmentView_CheckedChange;
        }
    }
}
