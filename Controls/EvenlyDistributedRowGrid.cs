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

    public class EvenlyDistributedRowGrid : Panel
    {
        public static readonly DependencyProperty IsReverseOrderProperty =
            DependencyProperty.Register("IsReverseOrder",
            typeof(bool),
            typeof(EvenlyDistributedRowGrid),
            new PropertyMetadata(false));      

        public bool IsReverseOrder
        {
            get { return (bool)GetValue(IsReverseOrderProperty); }
            set { SetValue(IsReverseOrderProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //Size minimumSize = new Size(40.0, 1000.0);

            double largestElementWidth = 0.0;
            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
                if (child.DesiredSize.Width > largestElementWidth)
                {
                    largestElementWidth = child.DesiredSize.Width;
                }
            }
            Size result = base.MeasureOverride(availableSize);
            if (largestElementWidth > 0.0)
            {
                return new Size(largestElementWidth, result.Height);
            }
            return result;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size cellSize = new Size(Math.Ceiling(finalSize.Width), finalSize.Height / Children.Count);
            int row = 0, col = 0;
            double reverseStartPoint = finalSize.Height - cellSize.Height;
            foreach (UIElement child in Children)
            {
                if (IsReverseOrder)
                {
                    child.Arrange(new Rect(new Point(cellSize.Width * col, reverseStartPoint - cellSize.Height * row), cellSize));
                }
                else
                {
                    child.Arrange(new Rect(new Point(cellSize.Width * col, cellSize.Height * row), cellSize));
                }
                row++;
            }
            return finalSize;
        }
    }
}
