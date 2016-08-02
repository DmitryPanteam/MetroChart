namespace Panteam.MetroChart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;  
    using System.Reflection;
    using System.Collections.Specialized;
    using System.Windows.Input;

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
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    
#endif

    [TemplateVisualState(Name = PiePiece.StateSelectionUnselected, GroupName = PiePiece.GroupSelectionStates)]
    [TemplateVisualState(Name = PiePiece.StateSelectionSelected, GroupName = PiePiece.GroupSelectionStates)]
    public class PiePiece : Control
    {
        #region Fields

        internal const string StateSelectionUnselected = "Unselected";
        internal const string StateSelectionSelected = "Selected";
        internal const string GroupSelectionStates = "SelectionStates";

        public static readonly DependencyProperty ClientHeightProperty =
            DependencyProperty.Register("ClientHeight", typeof(double), typeof(PiePiece),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnSizeChanged)));
        public static readonly DependencyProperty ClientWidthProperty =
            DependencyProperty.Register("ClientWidth", typeof(double), typeof(PiePiece),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnSizeChanged)));
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(PiePiece),
            new PropertyMetadata(0.0));
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(PiePiece),
            new PropertyMetadata(0.0, new PropertyChangedCallback(UpdatePie)));
        public static readonly DependencyProperty SumOfDataSeriesProperty =
            DependencyProperty.Register("SumOfDataSeries", typeof(double), typeof(PiePiece),
            new PropertyMetadata(0.0, new PropertyChangedCallback(UpdatePie)));
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(PiePiece),
            new PropertyMetadata(0.0, new PropertyChangedCallback(UpdatePie)));
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(PiePiece),
            new PropertyMetadata(0.0, new PropertyChangedCallback(UpdatePie)));
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(PiePiece),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));
        public static readonly DependencyProperty IsClickedByUserProperty =
            DependencyProperty.Register("IsClickedByUser", typeof(bool), typeof(PiePiece),
            new PropertyMetadata(false));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PiePiece source = (PiePiece)d;
            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;
            source.OnIsSelectedPropertyChanged(oldValue, newValue);
        }

        protected virtual void OnIsSelectedPropertyChanged(bool oldValue, bool newValue)
        {
            this.IsClickedByUser = false;
            VisualStateManager.GoToState(this, newValue ? StateSelectionSelected : StateSelectionUnselected, true);
        }

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(PiePiece),
            new PropertyMetadata(null));
        public static readonly DependencyProperty SelectionGeometryProperty =
            DependencyProperty.Register("SelectionGeometry", typeof(Geometry), typeof(PiePiece),
            new PropertyMetadata(null));
        public static readonly DependencyProperty MouseOverGeometryProperty =
            DependencyProperty.Register("MouseOverGeometry", typeof(Geometry), typeof(PiePiece),
            new PropertyMetadata(null));
        public static readonly DependencyProperty LineGeometryProperty =
            DependencyProperty.Register("LineGeometry", typeof(Geometry), typeof(PiePiece),
            new PropertyMetadata(null));

        public static readonly DependencyProperty LabelXPosProperty =
            DependencyProperty.Register("LabelXPos", typeof(double), typeof(PiePiece),
            new PropertyMetadata(10.0));
        public static readonly DependencyProperty LabelYPosProperty =
            DependencyProperty.Register("LabelYPos", typeof(double), typeof(PiePiece),
            new PropertyMetadata(10.0));

        #endregion Fields

        private Path slice;

        #region Constructors

        static PiePiece()        
        {
#if NETFX_CORE
                        
#elif SILVERLIGHT
    
#else
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PiePiece), new FrameworkPropertyMetadata(typeof(PiePiece)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PiePiece"/> class.
        /// </summary>
        public PiePiece()
        {
#if NETFX_CORE
            this.DefaultStyleKey = typeof(PiePiece);
#endif
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(PiePiece);
#endif
            Loaded += PiePiece_Loaded;
        }

        private static void UpdatePie(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PiePiece).DrawGeometry();
        }

        void PiePiece_Loaded(object sender, RoutedEventArgs e)
        {
              DrawGeometry();
        }

        #endregion Constructors

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
        TextBlock label = null;
        private void InternalOnApplyTemplate()
        {
            label = this.GetTemplateChild("PART_Label") as TextBlock; 
            slice = this.GetTemplateChild("Slice") as Path;
            if (slice != null)
            {
#if NETFX_CORE
                slice.PointerPressed += delegate 
#else
                slice.MouseLeftButtonUp += delegate
#endif
                {
                    InternalMousePressed();
                };

#if NETFX_CORE
                slice.PointerMoved += delegate
#else
                slice.MouseMove += delegate
#endif
                {
                    InternalMouseMoved();
                };
            }
        }

        private void InternalMousePressed()
        {
            SetValue(PiePiece.IsClickedByUserProperty, true);
        }

        private void InternalMouseMoved()
        {
            //SetValue(PiePiece.Is, true);
        }

        #region Properties

        /// <summary>
        /// Gets or sets the height of the client.
        /// </summary>
        /// <value>The height of the client.</value>
        public string Caption
        {
            get
            {
                return (this.DataContext as DataPoint).DisplayName;
            }
        }
        
        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //(d as PiePiece).DrawGeometry();
        }
        
        /// <summary>
        /// Gets or sets the height of the client.
        /// </summary>
        /// <value>The height of the client.</value>
        public double ClientHeight
        {
            get { return (double)GetValue(ClientHeightProperty); }
            set { SetValue(ClientHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the client.
        /// </summary>
        /// <value>The width of the client.</value>
        public double ClientWidth
        {
            get { return (double)GetValue(ClientWidthProperty); }
            set { SetValue(ClientWidthProperty, value); }
        }

        public double LabelXPos
        {
            get { return (double)GetValue(LabelXPosProperty); }
            set { SetValue(LabelXPosProperty, value); }
        }
        public double LabelYPos
        {
            get { return (double)GetValue(LabelYPosProperty); }
            set { SetValue(LabelYPosProperty, value); }
        }

        public Geometry LineGeometry
        {
            get { return (Geometry)GetValue(LineGeometryProperty); }
            set { SetValue(LineGeometryProperty, value); }
        }
        
        
        public Geometry Geometry
        {
            get { return (Geometry)GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
        }

        public Geometry SelectionGeometry
        {
            get { return (Geometry)GetValue(SelectionGeometryProperty); }
            set { SetValue(SelectionGeometryProperty, value); }
        }

        public Geometry MouseOverGeometry
        {
            get { return (Geometry)GetValue(MouseOverGeometryProperty); }
            set { SetValue(MouseOverGeometryProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public bool IsClickedByUser
        {
            get { return (bool)GetValue(IsClickedByUserProperty); }
            set { SetValue(IsClickedByUserProperty, value); }
        }

        /// <summary>
        /// Gets or sets the percent.
        /// </summary>
        /// <value>The percent.</value>
        public double Percent
        {
            get
            {
                if (SumOfDataSeries > 0.0)
                {
                    return (Value / SumOfDataSeries) * 100;
                }
                return 0.0;
            }
        }

        /// <summary>
        /// The value that this pie piece represents.
        /// </summary>
        public double SumOfDataSeries
        {
            get { return (double)GetValue(SumOfDataSeriesProperty); }
            set { SetValue(SumOfDataSeriesProperty, value); }
        }

        /// <summary>
        /// The value that this pie piece represents.
        /// </summary>
        public double StartValue
        {
            get { return (double)GetValue(StartValueProperty); }
            set { SetValue(StartValueProperty, value); }
        }

        /// <summary>
        /// The value that this pie piece represents.
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        /// <summary>
        /// The value that this pie piece represents.
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        
        #endregion Properties

        #region Methods

        /// <summary>
        /// Draws the geometry.
        /// </summary>
        /// <param name="context">The context.</param>
        private void DrawGeometry()
        {    
            try
            {
                //Children.Clear();

                if (this.ClientWidth <= 0.0)
                {
                    return;
                }
                if (this.ClientHeight <= 0.0)
                {
                    return;
                }
                if (SumOfDataSeries > 0)
                {
                    double sss = this.ActualWidth;

                    double x = this.ClientWidth;
                    double m_startpercent = StartValue / SumOfDataSeries * 100;
                    double m_endpercent = (StartValue + Value) / SumOfDataSeries * 100;

                    Point center = GetCenter();

                    double startAngle = (360 / 100.0) * m_startpercent;
                    double endAngle = (360 / 100.0) * m_endpercent;
                    double radius = GetRadius();
                    bool isLarge = (endAngle - startAngle) > 180.0;

                    LayoutSegment(startAngle, endAngle, radius, 0.25, center, true);
                }
            }
            catch (Exception ex)
            {
            }
        }

        
#if NETFX_CORE
         protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            HandleMouseDown();
            e.Handled = true;
        }
#else
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            HandleMouseDown();
            e.Handled = true;
        }
#endif

         private void HandleMouseDown()
         {
             IsClickedByUser = true;
            // IsSelected = true; // (ModifierKeys.None == (ModifierKeys.Control & Keyboard.Modifiers));
             /*if (IsSelectionEnabled)
            {
                IsSelected = (ModifierKeys.None == (ModifierKeys.Control & Keyboard.Modifiers));
                e.Handled = true;
            }
             * */
         }
        private Point GetCircumferencePoint(double angle, double radius, double centerx, double centery)
        {
            angle = angle - 90;

            double angleRad = (Math.PI / 180.0) * angle;

            double x = centerx + radius * Math.Cos(angleRad);
            double y = centery + radius * Math.Sin(angleRad);

            return new Point(Math.Ceiling(x), Math.Ceiling(y));
        }

        private void LayoutSegment(double startAngle, double endAngle, double radius, double gapScale, Point center, bool isDoughnut)
        {
            try
            {
                // Segment Geometry
                double pieRadius = radius;
                double gapRadius = pieRadius * ((gapScale == 0.0) ? 0.25 : gapScale);

                Point A = GetCircumferencePoint(startAngle, pieRadius, center.X, center.Y);
                Point B = isDoughnut ? GetCircumferencePoint(startAngle, gapRadius, center.X, center.Y) : center;
                Point C = GetCircumferencePoint(endAngle, gapRadius, center.X, center.Y);
                Point D = GetCircumferencePoint(endAngle, pieRadius, center.X, center.Y);

                bool isReflexAngle = Math.Abs(endAngle - startAngle) > 180.0;
                
                PathSegmentCollection segments = new PathSegmentCollection();
                segments.Add(new LineSegment() { Point = B });

                if (isDoughnut)
                {
                    segments.Add(new ArcSegment()
                    {
                        Size = new Size(gapRadius, gapRadius),
                        Point = C,
                        SweepDirection = SweepDirection.Clockwise,
                        IsLargeArc = isReflexAngle
                    });
                }

                segments.Add(new LineSegment() { Point = D });
                segments.Add(new ArcSegment()
                {
                    Size = new Size(pieRadius, pieRadius),
                    Point = A,
                    SweepDirection = SweepDirection.Counterclockwise,
                    IsLargeArc = isReflexAngle
                });
                
                Path segmentPath = new Path()
                {
                    StrokeLineJoin = PenLineJoin.Round,
                    Stroke = new SolidColorBrush() { Color = Colors.Black },
                    StrokeThickness = 0.0d,
                    Data = new PathGeometry()
                    {
                        Figures = new PathFigureCollection()
                        {
                            new PathFigure()
                            {
                                IsClosed = true,
                                StartPoint = A,
                                Segments = segments
                            }
                        }
                    }
                };
                SetValue(PiePiece.GeometryProperty, CloneDeep(segmentPath.Data as PathGeometry));
                SetValue(PiePiece.SelectionGeometryProperty, CloneDeep(segmentPath.Data as PathGeometry));


                double inRadius = radius * 0.65;
                double outRadius = radius * 1.25;

                double midAngle = startAngle + ((endAngle - startAngle) / 2.0);
                Point pointOnCircle = GetCircumferencePoint(midAngle, pieRadius, center.X, center.Y);

                //recalculate midangle if point is to close to top or lower border
                double distanceToCenter = Math.Abs(pointOnCircle.Y - center.Y);
                double factor = distanceToCenter / center.Y;

                double midAngleBefore = midAngle;
                if ((GetQuadrant(pointOnCircle, center) == 1) || (GetQuadrant(pointOnCircle, center) == 3))
                {   //point is in quadrant 1 center, we go further the end angle
                    midAngle = startAngle + ((endAngle - startAngle) / 2.0) + (((endAngle - startAngle) / 2.0) * factor);
                }
                else
                {
                    //point 
                    midAngle = startAngle + ((endAngle - startAngle) / 2.0) - (((endAngle - startAngle) / 2.0) * factor);
                }
                
                pointOnCircle = GetCircumferencePoint(midAngle, pieRadius, center.X, center.Y);

                Point pointOuterCircle = GetCircumferencePoint(midAngle, pieRadius + 10, center.X, center.Y);
                Point pointerMoreOuter = new Point(pointOuterCircle.X - 20, pointOuterCircle.Y);
                if (pointOnCircle.X > center.X)
                {
                    pointerMoreOuter = new Point(pointOuterCircle.X + 20, pointOuterCircle.Y);
                }

                PathSegmentCollection linesegments = new PathSegmentCollection();
                //linesegments.Add(new LineSegment() { Point = pointOnCircle });
                linesegments.Add(new LineSegment() { Point = pointOuterCircle });
                linesegments.Add(new LineSegment() { Point = pointerMoreOuter });

                Path linesegmentPath = new Path()
                {
                    StrokeLineJoin = PenLineJoin.Round,
                    Stroke = new SolidColorBrush() { Color = Colors.Black },
                    StrokeThickness = 2.0d,
                    Data = new PathGeometry()
                    {
                        Figures = new PathFigureCollection()
                        {
                            new PathFigure()
                            {
                                IsClosed = false,
                                StartPoint = pointOnCircle,
                                Segments = linesegments
                            }
                        }
                    }
                };
                SetValue(PiePiece.LineGeometryProperty, CloneDeep(linesegmentPath.Data as PathGeometry));

                //SetValue(PiePiece.LabelXPosProperty, pointerMoreOuter.X);
                //SetValue(PiePiece.LabelYPosProperty, pointerMoreOuter.Y);

                label.Measure(new Size(400, 400));
                Size s = label.DesiredSize;
                double x = this.Value;

                label.SetValue(Canvas.TopProperty, pointerMoreOuter.Y - (label.DesiredSize.Height / 2.0));
                if (pointerMoreOuter.X > center.X)
                {
                    label.SetValue(Canvas.LeftProperty, pointerMoreOuter.X);
                }
                else
                {
                    label.SetValue(Canvas.LeftProperty, pointerMoreOuter.X - (label.DesiredSize.Width));
                }
                //Geometry = segmentPath.Data;

                // Segment MouseEnter Animation
                /*
#if NETFX_CORE
            segmentPath.PointerPressed += delegate
#else
                segmentPath.MouseLeftButtonUp += delegate
#endif
                {
                    if (true) //!segment.IsAnimating)
                    {
                        //IsSelected = true;

                        //segment.IsAnimating = true;

                        Point transBy = GetCircumferencePoint(midAngle, 20.0, center.X, center.Y);
                        Point translate = new Point(transBy.X - center.X, transBy.Y - center.Y);

                        TranslateTransform tt = new TranslateTransform();
                        segmentPath.RenderTransform = tt;
                        segmentLabel.RenderTransform = tt;

                        Duration dur = new Duration(TimeSpan.FromSeconds(1.0));

                        DoubleAnimation xda = new DoubleAnimation();
                        xda.To = translate.X;
                        xda.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };
                        xda.Duration = dur;

                        DoubleAnimation yda = new DoubleAnimation();
                        yda.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };
                        yda.To = translate.Y;
                        yda.Duration = dur;


                        Storyboard sb = new Storyboard();
                        sb.FillBehavior = FillBehavior.HoldEnd;

                        sb.Children.Add(xda);
                        sb.Children.Add(yda);

                        Storyboard.SetTarget(xda, tt);
                        Storyboard.SetTarget(yda, tt);

#if NETFX_CORE
                    Storyboard.SetTargetProperty(xda, "(TranslateTransform.X)");
                    Storyboard.SetTargetProperty(yda, "(TranslateTransform.Y)");
#else
                        Storyboard.SetTargetProperty(xda, new PropertyPath(TranslateTransform.XProperty));
                        Storyboard.SetTargetProperty(yda, new PropertyPath(TranslateTransform.YProperty));
#endif
                        sb.Completed += delegate
                        {
                            //segment.IsAnimating = false; 
                        };
                        sb.Begin();
                    }
                };

                //this.Children.Add(segmentPath);
                //this.Children.Add(segmentLabel);
                */
            }
            catch (Exception ex)
            {
            }
        }

        private int GetQuadrant(Point pointOnCircle, Point center)
        {
            if (pointOnCircle.X > center.X)
            {
                if (pointOnCircle.Y > center.Y)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (pointOnCircle.Y > center.Y)
                {
                    return 3;
                }
                else
                {
                    return 4;
                }
            }
        }
        
        /// <summary>
        /// Gets the center.
        /// </summary>
        /// <returns></returns>
        private Point GetCenter()
        {
            return new Point(ClientWidth / 2, ClientHeight/2);
        }

        public PathGeometry CloneDeep(PathGeometry pathGeometry)
        {
            var newPathGeometry = new PathGeometry();
            foreach (var figure in pathGeometry.Figures)
            {
                var newFigure = new PathFigure();
                newFigure.StartPoint = figure.StartPoint;
                // Even figures have to be deep cloned. Assigning them directly will result in
                //  an InvalidOperationException being thrown with the message "Element is already the child of another element."
                foreach (var segment in figure.Segments)
                {
                    // I only impemented cloning the abstract PathSegments to one implementation, 
                    //  the PolyLineSegment class. If your paths use other kinds of segments, you'll need
                    //  to implement that kind of coding yourself.
                    var segmentAsPolyLineSegment = segment as PolyLineSegment;
                    if (segmentAsPolyLineSegment != null)
                    {
                        var newSegment = new PolyLineSegment();
                        foreach (var point in segmentAsPolyLineSegment.Points)
                        {
                            newSegment.Points.Add(point);
                        }
                        newFigure.Segments.Add(newSegment);
                    }

                    var segmentAsLineSegment = segment as LineSegment;
                    if (segmentAsLineSegment != null)
                    {
                        var newSegment = new LineSegment();
                        newSegment.Point = segmentAsLineSegment.Point;
                        newFigure.Segments.Add(newSegment);
                    }

                    var segmentAsArcSegment = segment as ArcSegment;
                    if (segmentAsArcSegment != null)
                    {
                        var newSegment = new ArcSegment();
                        newSegment.Point = segmentAsArcSegment.Point;
                        newSegment.SweepDirection = segmentAsArcSegment.SweepDirection;
                        newSegment.RotationAngle = segmentAsArcSegment.RotationAngle;
                        newSegment.IsLargeArc = segmentAsArcSegment.IsLargeArc;
                        newSegment.Size = segmentAsArcSegment.Size;
                        newFigure.Segments.Add(newSegment);
                    }
                }
                newPathGeometry.Figures.Add(newFigure);
            }
            return newPathGeometry;
        }

        /// <summary>
        /// Gets the radius.
        /// </summary>
        /// <returns></returns>
        private double GetRadius()
        {
            double result;
            if (ClientHeight < ClientWidth)
            {
                result = ClientHeight / 2;
            }
            else
            {
                result = ClientWidth / 2;
            }

            return result - 20;
        }

        #endregion Methods

        public event PropertyChangedEventHandler PropertyChanged;
    }
}