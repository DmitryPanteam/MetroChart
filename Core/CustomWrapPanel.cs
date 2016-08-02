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

    public class CustomWrapPanel : Panel
    {
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
          DependencyProperty.Register("Orientation",
          typeof(Orientation), typeof(CustomWrapPanel), null);

        public CustomWrapPanel()
        {
            // default orientation
            Orientation = Orientation.Horizontal;
        }
        
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
            {
                child.Measure(new Size(availableSize.Width, availableSize.Height));
            }

            Size u = SimpleArrange(availableSize.Width, availableSize.Height);
            return u;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size z = SimpleArrange(finalSize.Width, finalSize.Height);
            return z;
        }

        private Size SimpleArrange(double availableWidth, double availableHeight)
        {
            Point point = new Point(0, 0);
            int i = 0;
            int columnCount = 0;

            if (Orientation == Orientation.Vertical)
            {
                double largestItemWidth = 0.0;
                double finalWidth = 0.0;

                foreach (UIElement child in Children)
                {                    
                        child.Arrange(new Rect(point, new Point(point.X + child.DesiredSize.Width, point.Y + child.DesiredSize.Height)));
                        finalWidth = point.X + child.DesiredSize.Width;

                        if (child.DesiredSize.Width > largestItemWidth)
                        {
                            largestItemWidth = child.DesiredSize.Width;
                        }

                        point.Y = point.Y + child.DesiredSize.Height;

                        if ((i + 1) < Children.Count)
                        {
                            if ((point.Y + Children[i + 1].DesiredSize.Height) > availableHeight)
                            {
                                point.Y = 0;
                                point.X = point.X + largestItemWidth;
                                largestItemWidth = 0.0;
                                columnCount++;
                            }
                        }
                        i++;
                    
                }

                return new Size(finalWidth, availableHeight);
            }
            else
            {
                double largestItemHeight = 0.0;
                double finalHeight = 0.0;

                foreach (UIElement child in Children)
                {                    
                        child.Arrange(new Rect(point, new Point(point.X + child.DesiredSize.Width, point.Y + child.DesiredSize.Height)));
                        finalHeight = point.Y + child.DesiredSize.Height;

                        if (child.DesiredSize.Height > largestItemHeight)
                        {
                            largestItemHeight = child.DesiredSize.Height;
                        }

                        point.X = point.X + child.DesiredSize.Width;

                        if ((i + 1) < Children.Count)
                        {
                            if ((point.X + Children[i + 1].DesiredSize.Width) > availableWidth)
                            {
                                point.X = 0;
                                point.Y = point.Y + largestItemHeight;
                                largestItemHeight = 0.0;
                            }
                        }
                        i++;
                   
                }

                return new Size(availableWidth, finalHeight);
            }
        }

    }
}

