//-----------------------------------------------------------------------
// <copyright file="Parallax.cs" company="">
//     Copyright (c) mauri_000, . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Wygwam.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Media;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Data;


    /// <summary>
    /// TODO: Provide summary section in the documentation header.
    /// </summary>
    [TemplatePart(Name = Parallax.ContainerName, Type = typeof(Panel))]
    [TemplatePart(Name = Parallax.ScrollViewerName, Type = typeof(ScrollViewer))]
    public class Parallax : ContentControl
    {

        private const string ContainerName = "PART_Background";
        private const string ScrollViewerName = "PART_ScrollViewer";

        private FrameworkElement _background;
        private Panel _container;
        private ScrollViewer _scrollViewer;
        private double _ratio;
        private TranslateTransform _backgroundTransform;

        public static readonly DependencyProperty BackgroundTemplateProperty = DependencyProperty.Register(
            "BackgroundTemplate",
            typeof(DataTemplate),
            typeof(Parallax),
            new PropertyMetadata(null, Parallax.OnBackgroundTemplateChanged));

        public static readonly DependencyProperty ScrollViewerStyleProperty = DependencyProperty.Register(
            "ScrollViewerStyle",
            typeof(Style),
            typeof(Parallax),
            new PropertyMetadata(null, Parallax.OnScrollViewerStyleChanged));

        public DataTemplate BackgroundTemplate
        {
            get { return (DataTemplate)GetValue(BackgroundTemplateProperty); }
            set { SetValue(BackgroundTemplateProperty, value); }
        }

        public Style ScrollViewerStyle
        {
            get { return (Style)GetValue(ScrollViewerStyleProperty); }
            set { SetValue(ScrollViewerStyleProperty, value); }
        }



        public double HorizontalOffset
        {
            get { return (int)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(Parallax), new PropertyMetadata(0d));




        public double ExtentWidth
        {
            get { return (double)GetValue(ExtentWidthProperty); }
            set { SetValue(ExtentWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtentWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtentWidthProperty =
            DependencyProperty.Register("ExtentWidth", typeof(double), typeof(Parallax), new PropertyMetadata(0d, OnExtentChanged));

        private static void OnExtentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Parallax)d).ComputeExtendRatio();
        }


        public Parallax()
        {
            this.DefaultStyleKey = typeof(Parallax);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //this._background = this.GetTemplateChild(BackgroundTemplateContainerName) as ContentControl;

            //if (this._background == null)
            //{
            //    throw new InvalidOperationException("Missing Viewbox with name PART_Background in template.");
            //}

            _container = this.GetTemplateChild(ContainerName) as Panel;

            this._scrollViewer = this.GetTemplateChild(ScrollViewerName) as ScrollViewer;

            if (this._scrollViewer == null)
            {
                throw new InvalidOperationException("Missing ScrollViewer with name PART_ScrollViewer in template.");
            }

            this.ConfigureBackground();
            this.ConfigureScrollViewer();
        }

        private static void OnScrollViewerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Parallax)d).OnScrollViewerStyleChanged((Style)e.OldValue, (Style)e.NewValue);
        }

        private static void OnBackgroundTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Parallax)d).OnBackgroundTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        private void OnScrollViewerStyleChanged(Style oldValue, Style newValue)
        {
            this.SetScrollViewerStyle();
        }

        private void OnBackgroundTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
        {
            this.RefreshBackground();
        }

        private void ConfigureScrollViewer()
        {
            this.SetScrollViewerStyle();

            this._scrollViewer.SizeChanged += _scrollViewer_SizeChanged;

            Binding binding = new Binding();
            binding.Path = new PropertyPath("ExtentWidth");
            binding.Source = this._scrollViewer;

            this.SetBinding(Parallax.ExtentWidthProperty, binding);
        }

        void _scrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ComputeExtendRatio();
        }

        private void SetScrollViewerStyle()
        {
            if (this._scrollViewer != null)
            {
                this._scrollViewer.Style = this.ScrollViewerStyle;
            }
        }

        private void ConfigureBackground()
        {
            this.RefreshBackground();
            this.ComputeExtendRatio();
        }

        private void RefreshBackground()
        {
            if (_container != null)
            {
                if (_background != null)
                {
                    _container.Children.Remove(_background);
                }

                _background = this.BackgroundTemplate.LoadContent() as FrameworkElement;
                _container.Children.Insert(0, _background);

                if (!(this._background.RenderTransform is TranslateTransform))
                {
                    this._background.RenderTransform = new TranslateTransform();
                }

                this._backgroundTransform = this._background.RenderTransform as TranslateTransform;

                this._background.SizeChanged += _scrollViewer_SizeChanged;
            }
        }

        private void ComputeExtendRatio()
        {
            if (_scrollViewer == null || this._background == null)
            {
                return;
            }

            var extend = this._scrollViewer.ExtentWidth - this._scrollViewer.ViewportWidth;

            var backgroundExtend = this._background.ActualWidth - ((global::Windows.UI.Xaml.FrameworkElement)(this._background.Parent)).ActualWidth;

            _ratio = Math.Abs(backgroundExtend / extend);

            Binding binding = new Binding();
            binding.Converter = new MultiplyConverter();
            binding.ConverterParameter = -0.1d;
            binding.Path = new PropertyPath("HorizontalOffset");
            binding.Source = this._scrollViewer;
            binding.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(this._backgroundTransform, TranslateTransform.XProperty, binding);

            binding = new Binding();
            binding.Converter = new MultiplyConverter();
            binding.ConverterParameter = -1d;
            binding.Path = new PropertyPath("HorizontalOffset");
            binding.Source = this._scrollViewer;
            binding.Mode = BindingMode.OneWay;
            this.SetBinding(Parallax.HorizontalOffsetProperty, binding);
        }
    }

    public class MultiplyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double val = (double)value;
            double param;

            if (parameter is double)
            {
                param = (double)parameter;
            }
            else
            {
                param = System.Convert.ToDouble(parameter);
            }

            return val * param;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
