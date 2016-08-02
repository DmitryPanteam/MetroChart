namespace Panteam.MetroChart
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

#if NETFX_CORE
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Core;
#else
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;
#endif

#if SILVERLIGHT
    public class AutoSizeTextBlock : Control
#else
    public class AutoSizeTextBlock : Control
#endif
    {
        public static readonly DependencyProperty TextBlockStyleProperty =
            DependencyProperty.Register("TextBlockStyle", typeof(Style), typeof(AutoSizeTextBlock),
            new PropertyMetadata(null));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AutoSizeTextBlock),
            new PropertyMetadata(null));

        static AutoSizeTextBlock()
        {
#if NETFX_CORE
            //do nothing
#elif SILVERLIGHT
            //do nothing
#else
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoSizeTextBlock), new FrameworkPropertyMetadata(typeof(AutoSizeTextBlock))); 
#endif
        }

        public AutoSizeTextBlock()
        {
#if NETFX_CORE
            this.DefaultStyleKey = typeof(AutoSizeTextBlock);
#elif SILVERLIGHT
            this.DefaultStyleKey = typeof(AutoSizeTextBlock);
#else
            //do nothing
#endif
        }

#if NETFX_CORE
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InternalOnApplyTemplate();
        }
#else
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InternalOnApplyTemplate();
        }
#endif
        Border mainBorder = null;
        TextBlock mainTextBlock = null;
        double initialheight = 0.0;
        private void InternalOnApplyTemplate()
        {
            mainBorder = this.GetTemplateChild("PART_Border") as Border; 
            mainTextBlock = this.GetTemplateChild("PART_TextBlock") as TextBlock;
            initialheight = mainTextBlock.LineHeight + mainTextBlock.Margin.Top + mainTextBlock.Margin.Bottom;
        }

        public Style TextBlockStyle
        {
            get { return (Style)GetValue(TextBlockStyleProperty); }
            set { SetValue(TextBlockStyleProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            mainTextBlock.Visibility = Visibility.Collapsed;
            return new Size(0, initialheight);

           // Size baseSize = base.MeasureOverride(availableSize);
           // return baseSize;
        }

        private TextBlock GetCopyOfMainTextBlock()
        {
            TextBlock b = new TextBlock();
            b.FontFamily = mainTextBlock.FontFamily;
            b.FontSize = mainTextBlock.FontSize;
            b.FontStyle = mainTextBlock.FontStyle;
            b.LineHeight = mainTextBlock.LineHeight;
            b.LineStackingStrategy = mainTextBlock.LineStackingStrategy;
            b.FontWeight = mainTextBlock.FontWeight;
            return b;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //mainTextBlock.MaxWidth = finalSize.Width;
            //mainTextBlock.Visibility = Visibility.Visible;
            TextBlock tempBlock = GetCopyOfMainTextBlock();
            tempBlock.Text = mainTextBlock.Text;
            tempBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double currentWidth = tempBlock.DesiredSize.Width;

            //is textblock larger than the available width, then we scale it down
            if (currentWidth > finalSize.Width)
            {
                /*
                double factor = finalSize.Width / currentWidth;
                ScaleTransform tt = new ScaleTransform();
                tt.ScaleX = factor;
                tt.ScaleY = factor;
                mainTextBlock.RenderTransform = tt;
                * */
            }
            else
            {
                mainTextBlock.Visibility = Visibility.Visible;
            }

            mainBorder.Width = finalSize.Width;
            mainBorder.Height = finalSize.Height;
            return base.ArrangeOverride(finalSize);
        }
    }
}
