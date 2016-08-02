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
    public class EvenlyDistributedColumnsGrid : Panel
#else
    public class EvenlyDistributedColumnsGrid : Grid
#endif
    {

        protected override Size MeasureOverride(Size availableSize)
        {
            try
            {
                Size baseSize = base.MeasureOverride(availableSize);
                Size cellSize = GetCellSize(availableSize);

                foreach (UIElement child in Children)
                {
                    child.Measure(availableSize);
                }

                //is there any element which would exceed the cell width
                if (OneElementExceedsCellWidth(cellSize.Width))
                {
                    //we switch to 2 rows, we need the order space for 2 rows
                    double heightOfOneRow = GetHighestElement();
                    return new Size(baseSize.Width, heightOfOneRow * 2);
                }
                return baseSize;
            }
            catch (Exception ex)
            {
                return new Size(0,0);
            }

            
        }

        private double GetHighestElement()
        {
            double highestElementHeight = 0.0;
            foreach (UIElement child in Children)
            {
                if (child.DesiredSize.Height > highestElementHeight)
                {
                    highestElementHeight = child.DesiredSize.Height;
                }
            }
            return highestElementHeight;
        }

        private bool OneElementExceedsCellWidth(double cellWidth)
        {
            foreach (UIElement child in Children)
            {
                if (child.DesiredSize.Width > cellWidth)
                {
                    return true;
                }
            }
            return false;
        }

        private Size GetCellSize(Size availableSize)
        {
            return new Size(Math.Ceiling(availableSize.Width / Children.Count), availableSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //calculate the space for each column
            Size cellSize = GetCellSize(finalSize);
            double cellWidth = cellSize.Width;
            double cellHeight = cellSize.Height;

            //check if any element needs more space
            bool showElementsInTwoRows = false;
            if (OneElementExceedsCellWidth(cellSize.Width))
            {
                //ok, than we display the elements in 2 columns, alternating
                showElementsInTwoRows = true;
                cellHeight = cellHeight / 2;

                //increase cellWidth because each Element has now more space
                cellWidth = cellWidth * 2;
            }

            int row = 0, col = 0;
            foreach (UIElement child in Children)
            {
                double middlePointX = cellSize.Width * col + cellSize.Width / 2.0;

                child.Arrange(new Rect(new Point(middlePointX - cellWidth / 2.0, cellHeight * row), new Size(cellWidth, cellHeight)));
                if (showElementsInTwoRows)
                {
                    if (row == 0)
                    {
                        row = 1;
                    }
                    else
                    {
                        row = 0;
                    }
                }
                col++;
            }
            return finalSize;
        }
    }
}
