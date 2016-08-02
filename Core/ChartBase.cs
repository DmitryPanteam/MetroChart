namespace Panteam.MetroChart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows;

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
    using Windows.UI.Xaml.Data;
#else
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows.Data;
#endif

    public abstract class ChartBase : Control, INotifyPropertyChanged
    {
        #region Fields

        public static readonly DependencyProperty PlotterAreaStyleProperty =
            DependencyProperty.Register("PlotterAreaStyle",
            typeof(Style),
            typeof(ChartBase),
            new PropertyMetadata(null));
        public static readonly DependencyProperty ChartAreaStyleProperty =
            DependencyProperty.Register("ChartAreaStyle",
            typeof(Style),
            typeof(ChartBase),
            new PropertyMetadata(null));
        public static readonly DependencyProperty ChartLegendItemStyleProperty =
            DependencyProperty.Register("ChartLegendItemStyle",
            typeof(Style),
            typeof(ChartBase),
            new PropertyMetadata(null));
        

        public static readonly DependencyProperty ChartTitleProperty =
            DependencyProperty.Register("ChartTitle",
            typeof(string),
            typeof(ChartBase),
            new PropertyMetadata("WpfSimpleChart"));
        public static readonly DependencyProperty ChartSubTitleProperty =
            DependencyProperty.Register("ChartSubTitle",
            typeof(string),
            typeof(ChartBase),
            new PropertyMetadata("WpfSimpleChart"));
        public static readonly DependencyProperty ChartBackgroundBorderColorProperty =
            DependencyProperty.Register("ChartBackgroundBorderColor",
            typeof(Brush),
            typeof(ChartBase),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xB1, 0xB1, 0x4A))));
        public static readonly DependencyProperty ChartBackgroundBorderThicknessProperty =
            DependencyProperty.Register("ChartBackgroundBorderThickness",
            typeof(Thickness),
            typeof(ChartBase),
            new PropertyMetadata(new Thickness(0, 0, 0, 0)));
        public static readonly DependencyProperty ChartBackgroundCornerRadiusProperty =
            DependencyProperty.Register("ChartBackgroundCornerRadius",
            typeof(CornerRadius),
            typeof(ChartBase),
            new PropertyMetadata(new CornerRadius(0, 0, 0, 0)));
        public static readonly DependencyProperty ChartBackgroundProperty =
            DependencyProperty.Register("ChartBackground",
            typeof(Brush),
            typeof(ChartBase),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xD9, 0xD9, 0x98))));
        public static readonly DependencyProperty ChartBorderColorProperty =
            DependencyProperty.Register("ChartBorderColor",
            typeof(Brush),
            typeof(ChartBase),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 0, 0))));
        public static readonly DependencyProperty ChartBorderThicknessProperty =
            DependencyProperty.Register("ChartBorderThickness",
            typeof(Thickness),
            typeof(ChartBase),
            new PropertyMetadata(new Thickness(0, 0, 0, 0)));
        public static readonly DependencyProperty ChartColorMouseOverProperty =
            DependencyProperty.Register("ChartColorMouseOver",
            typeof(Brush),
            typeof(ChartBase),
            new PropertyMetadata(new SolidColorBrush(Colors.Red)));
        public static readonly DependencyProperty ChartColorProperty =
            DependencyProperty.Register("ChartColor",
            typeof(Brush),
            typeof(ChartBase),
            new PropertyMetadata(new SolidColorBrush(Colors.Red)));      
        public static readonly DependencyProperty ChartLegendVisibilityProperty =
            DependencyProperty.Register("ChartLegendVisibility",
            typeof(Visibility),
            typeof(ChartBase),
            new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty ChartMarginProperty =
            DependencyProperty.Register("ChartMargin",
            typeof(Thickness),
            typeof(ChartBase),
            new PropertyMetadata(new Thickness(20)));
        public static readonly DependencyProperty PaletteProperty =
            DependencyProperty.Register("Palette",
            typeof(ResourceDictionaryCollection),
            typeof(ChartBase),
            new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem",
            typeof(object),
            typeof(ChartBase),
            new PropertyMetadata(null));

        public static readonly DependencyProperty TitleStyleProperty =
            DependencyProperty.Register("TitleStyle",
            typeof(Style),
            typeof(ChartBase),
            new PropertyMetadata(null));
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series",
            typeof(ObservableCollection<ChartSeries>),
            typeof(ChartBase),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSeriesChanged)));
        public static readonly DependencyProperty InternalDataContextProperty = 
            DependencyProperty.Register("InternalDataContext",
            typeof(object),
            typeof(ChartBase),
            new PropertyMetadata(null, new PropertyChangedCallback(InternalDataContextChanged)));

        public static readonly DependencyProperty SeriesSourceProperty =
           DependencyProperty.Register("SeriesSource",
           typeof(IEnumerable),
           typeof(ChartBase),
           new PropertyMetadata(null, OnSeriesSourceChanged));

        private static void OnSeriesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IEnumerable oldValue = (IEnumerable)e.OldValue;
            IEnumerable newValue = (IEnumerable)e.NewValue;
            ChartBase source = (ChartBase)d;
            source.OnSeriesSourceChanged(oldValue, newValue);
        }

        private void OnSeriesSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            this.Series.Clear();
            if (newValue != null)
            {
                foreach (object item in newValue)
                {
                    if (SeriesTemplate != null)
                    {
                        ChartSeries series = SeriesTemplate.LoadContent() as ChartSeries;

                        if (series != null)
                        {
                            // set data context
                            series.DataContext = item;
                            this.Series.Add(series);
                        }
                    }
                }
            }
            UpdateGroupedSeries();
        }

        

        public static readonly DependencyProperty SeriesTemplateProperty =
           DependencyProperty.Register("SeriesTemplate",
           typeof(DataTemplate),
           typeof(ChartBase),
           new PropertyMetadata(null));

        public IEnumerable SeriesSource
        {
            get { return (IEnumerable)GetValue(SeriesSourceProperty); }
            set { SetValue(SeriesSourceProperty, value); }
        }
        public DataTemplate SeriesTemplate
        {
            get { return (DataTemplate)GetValue(SeriesTemplateProperty); }
            set { SetValue(SeriesTemplateProperty, value); }
        }

        public static readonly DependencyProperty MaxDataPointValueProperty =
            DependencyProperty.Register("MaxDataPointValue",
            typeof(double),
            typeof(ChartBase),
            new PropertyMetadata(0.0, OnMaxDataPointValueChanged));

        private static void OnMaxDataPointValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartBase).OnMaxDataPointValueChanged((double)e.NewValue);
        }

        protected virtual void OnMaxDataPointValueChanged(double p)
        {
            
        }

       public static readonly DependencyProperty MaxDataPointGroupSumProperty =
            DependencyProperty.Register("MaxDataPointGroupSum",
            typeof(double),
            typeof(ChartBase),
            new PropertyMetadata(0.0, OnMaxDataPointGroupSumChanged));

       private static void OnMaxDataPointGroupSumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
       {
           (d as ChartBase).OnMaxDataPointGroupSumChanged((double)e.NewValue);
       }

       protected virtual void OnMaxDataPointGroupSumChanged(double p)
       {
           
       }

       public static readonly DependencyProperty SumOfDataPointGroupProperty =
            DependencyProperty.Register("SumOfDataPointGroup",
            typeof(double),
            typeof(ChartBase),
            new PropertyMetadata(0.0, OnSumOfDataPointGroupChanged));

       private static void OnSumOfDataPointGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
       {
           (d as ChartBase).SumOfDataPointGroupChanged((double)e.NewValue);
       }

       private void SumOfDataPointGroupChanged(double p)
       {
           
       }

       public double SumOfDataPointGroup
       {
           get { return (double)GetValue(SumOfDataPointGroupProperty); }
           set { SetValue(SumOfDataPointGroupProperty, value); }
       }
#endregion Fields

        #region DataContext stuff

        public static DependencyProperty DataContextWatcherProperty = DependencyProperty.Register(
            "DataContextWatcher",
            typeof(object),
            typeof(ChartBase),
            new PropertyMetadata(null, DataContextWatcher_Changed));

        public static void DataContextWatcher_Changed(
               DependencyObject sender,
               DependencyPropertyChangedEventArgs args)
        {
            ChartBase senderControl = sender as ChartBase;
            if (senderControl != null)
            {
                (senderControl as ChartBase).InternalDataContextChanged();
            }
        } 

        private static void InternalDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartBase).InternalDataContextChanged();
        }

        private void InternalDataContextChanged()
        {
            UpdateDataContextOfSeries();
        }

        public object InternalDataContext
        {
            get { return GetValue(InternalDataContextProperty); }
            set { SetValue(InternalDataContextProperty, value); }
        }

        private void UpdateDataContextOfSeries()
        {
            foreach (var newItem in this.Series)
            {
                if (newItem is FrameworkElement)
                {
                    (newItem as FrameworkElement).DataContext = this.DataContext;
                }
            }
        }

        #endregion

        #region INotifiy stuff

        private static void OnSeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartBase).AttachEventHandler(e.NewValue);
        }

        private void AttachEventHandler(object collection)
        {
            if (collection is INotifyCollectionChanged)
            {
                (collection as INotifyCollectionChanged).CollectionChanged += ChartBase_CollectionChanged;
            }
        }

        void ChartBase_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newSeries in e.NewItems)
                {
                    if (newSeries is ItemsControl)
                    {
#if NETFX_CORE
                        (newSeries as ItemsControl).Items.VectorChanged += Items_VectorChanged;

#else
                        if ((newSeries as ItemsControl).Items is INotifyCollectionChanged)
                        {
                            ((INotifyCollectionChanged)(newSeries as ItemsControl).Items).CollectionChanged += new NotifyCollectionChangedEventHandler(Window1_CollectionChanged);
                        }
#endif
                    }
                }
            }
        }

#if NETFX_CORE
        void Items_VectorChanged(Windows.Foundation.Collections.IObservableVector<object> sender, Windows.Foundation.Collections.IVectorChangedEventArgs @event)
        {
            UpdateGroupedSeries();
            UpdateGroupedPieSeries();
        }
#else
        private void Window1_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //new items added to a series, we may update them
            UpdateGroupedSeries();
            UpdateGroupedPieSeries();
        }
#endif
        #endregion

        #region Constructors               

        public ChartBase()
        {
            Series = new ObservableCollection<ChartSeries>();
            InitializeChartComponent();

            SetBinding(DataContextWatcherProperty, new Binding());
            

            UpdateGridLines();
        }

        #endregion Constructors

        #region Properties

        public Style PlotterAreaStyle
        {
            get { return (Style)GetValue(PlotterAreaStyleProperty); }
            set { SetValue(PlotterAreaStyleProperty, value); }
        }

        public Style ChartAreaStyle
        {
            get { return (Style)GetValue(ChartAreaStyleProperty); }
            set { SetValue(ChartAreaStyleProperty, value); }
        }

        public Style ChartLegendItemStyle
        {
            get { return (Style)GetValue(ChartLegendItemStyleProperty); }
            set { SetValue(ChartLegendItemStyleProperty, value); }
        }
        
        
        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string ChartTitle
        {
            get { return (string)GetValue(ChartTitleProperty); }
            set { SetValue(ChartTitleProperty, value); }
        }

        public string ChartSubTitle
        {
            get { return (string)GetValue(ChartSubTitleProperty); }
            set { SetValue(ChartSubTitleProperty, value); }
        }

        public Style TitleStyle
        {
            get { return (Style)GetValue(TitleStyleProperty); }
            set { SetValue(TitleStyleProperty, value); }
        }

        public double MaxDataPointValue
        {
            get { return (double)GetValue(MaxDataPointValueProperty); }
            set { SetValue(MaxDataPointValueProperty, value); }
        }

        public double MaxDataPointGroupSum
        {
            get { return (double)GetValue(MaxDataPointGroupSumProperty); }
            set { SetValue(MaxDataPointGroupSumProperty, value); }
        }

        public ObservableCollection<ChartSeries> Series
        {
            get { return (ObservableCollection<ChartSeries>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }


        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public ResourceDictionaryCollection Palette
        {
            get { return (ResourceDictionaryCollection)GetValue(PaletteProperty); }
            set { SetValue(PaletteProperty, value); }
        }

        /// <summary>
        /// Gets or sets the chart background.
        /// </summary>
        /// <value>The chart background.</value>
        public Brush ChartBackground
        {
            get { return (Brush)GetValue(ChartBackgroundProperty); }
            set { SetValue(ChartBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the chart background border.
        /// </summary>
        /// <value>The color of the chart background border.</value>
        public Brush ChartBackgroundBorderColor
        {
            get { return (Brush)GetValue(ChartBackgroundBorderColorProperty); }
            set { SetValue(ChartBackgroundBorderColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the chart background border thickness.
        /// </summary>
        /// <value>The chart background border thickness.</value>
        public Thickness ChartBackgroundBorderThickness
        {
            get { return (Thickness)GetValue(ChartBackgroundBorderThicknessProperty); }
            set { SetValue(ChartBackgroundBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the chart background corner radius.
        /// </summary>
        /// <value>The chart background corner radius.</value>
        public CornerRadius ChartBackgroundCornerRadius
        {
            get { return (CornerRadius)GetValue(ChartBackgroundCornerRadiusProperty); }
            set { SetValue(ChartBackgroundCornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the chart margin.
        /// </summary>
        /// <value>The chart margin.</value>
        public Thickness ChartMargin
        {
            get { return (Thickness)GetValue(ChartMarginProperty); }
            set { SetValue(ChartMarginProperty, value);}
        }

        /// <summary>
        /// Gets or sets the color of the chart border.
        /// </summary>
        /// <value>The color of the chart border.</value>
        public Brush ChartBorderColor
        {
            get
            {
                return (Brush)GetValue(ChartBorderColorProperty);
            }
            set
            {
                SetValue(ChartBorderColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the chart border thickness.
        /// </summary>
        /// <value>The chart border thickness.</value>
        public Thickness ChartBorderThickness
        {
            get
            {
                return (Thickness)GetValue(ChartBorderThicknessProperty);
            }
            set
            {
                SetValue(ChartBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the chart.
        /// </summary>
        /// <value>The color of the chart.</value>
        public Brush ChartColor
        {
            get
            {
                return (Brush)GetValue(ChartColorProperty);
            }
            set
            {
                SetValue(ChartColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the chart color mouse over.
        /// </summary>
        /// <value>The chart color mouse over.</value>
        public Brush ChartColorMouseOver
        {
            get
            {
                return (Brush)GetValue(ChartColorMouseOverProperty);
            }
            set
            {
                SetValue(ChartColorMouseOverProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the chart legend visibility.
        /// </summary>
        /// <value>The chart legend visibility.</value>
        public Visibility ChartLegendVisibility
        {
            get
            {
                return (Visibility)GetValue(ChartLegendVisibilityProperty);
            }
            set
            {
                SetValue(ChartLegendVisibilityProperty, value);
            }
        }

        #endregion Properties

        public ObservableCollection<ChartLegendItemViewModel> ChartLegendItems
        {
            get
            {
                int index = 0;
                ObservableCollection<ChartLegendItemViewModel> result = new ObservableCollection<ChartLegendItemViewModel>();
                foreach (ChartSeries series in Series)
                {
                    result.Add(new ChartLegendItemViewModel() { Caption = series.Caption, ItemBrush = GetItemBrush(index) });
                    index++;
                }
                return result;
            }
        }

        private string GetPropertyValue(object item, string propertyName)
        {
            foreach (PropertyInfo info in item.GetType().GetAllProperties())
            {
                if (info.Name == propertyName)
                {
                    object v = info.GetValue(item, null);
                    return v.ToString();
                }
            }
            throw new Exception("Value not found");
        }

        private Brush GetItemBrush(int index)
        {
            if (this.Palette != null)
            {
                int paletteCounter = 0;
                foreach (var resDictionary in Palette)
                {
                    if (paletteCounter == index)
                    {
                        try
                        {
                            foreach (var entry in resDictionary.Values)
                            {
                                if (entry is Brush)
                                {
                                    return entry as Brush;
                                }
                            }                           
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    paletteCounter++;
                }
            }
            return new SolidColorBrush(Colors.Red);
        }

        private ObservableCollection<string> gridLines = new ObservableCollection<string>();
        public ObservableCollection<string> GridLines
        {
            get
            {
                return gridLines;                
            }
        }

        /// <summary>
        /// take a number, e.g.
        /// 43456 -> 50000
        /// 1324 -> 1400
        /// 123 -> 130
        /// 8 -> 10
        /// 23 -> 30
        /// 82 -> 90
        /// 92 -> 100
        /// 1.5 -> 2
        /// 33 -> 40
        /// </summary>
        /// <param name="newMaxValue"></param>
        /// <returns></returns>
        private double CalculateMaxValue(double newMaxValue)
        {
            double bestMaxValue = 0.0;
            int bestDivisor = 1;

            GetBestValues(newMaxValue, ref bestMaxValue, ref bestDivisor);

            return bestMaxValue;
        }

        private double CalculateDistance(double givenBestMaxValue)
        {
            double bestMaxValue = 0.0;
            int bestDivisor = 1;
            double distance = 0.0;

            GetBestValues(givenBestMaxValue, ref bestMaxValue, ref bestDivisor);
            distance = bestMaxValue / bestDivisor;

            return distance;
        }


        private void GetBestValues(double wert, ref double bestMaxValue, ref int bestDivisor)
        {
            string wertString = wert.ToString(System.Globalization.CultureInfo.InvariantCulture);
            double tensBelowNull = 1;

            if (wert <= 1)
            {
                //0.72  -> 0.8
                //0.00145
                //0.0007453 0> 7453

                //count digits after comma
                int digitsAfterComma = wertString.Replace("0.", "").Length;
                tensBelowNull = Math.Pow(10, digitsAfterComma);
                wert = wert * tensBelowNull;
                wertString = wert.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            if (wertString.Contains("."))
            {
                wertString = wertString.Substring(0, wertString.IndexOf("."));
            }
            int digitsBeforeComma = wertString.Length;
            int roundedValue = (int)Math.Ceiling(wert);
            double tens = 0;
            if (digitsBeforeComma > 2)
            {
                tens = Math.Pow(10, digitsBeforeComma - 2);
                double wertWith2Digits = wert / tens;
                roundedValue = (int)Math.Ceiling(wertWith2Digits);
            }
            else if (digitsBeforeComma == 1)
            {
                tens = 0.1;
                double wertWith2Digits = wert / tens;
                roundedValue = (int)Math.Ceiling(wertWith2Digits);
            }

            int finaldivisor = FindBestDivisor(ref roundedValue);

            double roundedValueDouble = roundedValue / tensBelowNull;

            if (tens > 0)
            {
                roundedValueDouble = roundedValueDouble * tens;
            }

            bestMaxValue = roundedValueDouble;
            bestDivisor = finaldivisor;

        }

        private int FindBestDivisor(ref int roundedValue)
        {
            if (IsUseNextBiggestMaxValue)
            {
                roundedValue += 1;
            }
            while (true)
            {
                int[] divisors = new int[] { 2, 5, 10, 25 };
                foreach (int divisor in divisors)
                {
                    int div = roundedValue % divisor;
                    int mod = roundedValue / divisor;

                    if ((roundedValue < 10) && (mod == 1))
                    {
                        return roundedValue;
                    }

                    if ((div == 0) && (mod <= 10))
                    {
                        return mod;
                    }
                }
                roundedValue = roundedValue + 1;
            }
        }

        protected abstract double GridLinesMaxValue
        {
            get;
        }

        protected void UpdateGridLines()
        {
            double distance = CalculateDistance(GridLinesMaxValue);
            gridLines.Clear();
            for (var i = distance; i <= GridLinesMaxValue; i += distance)
            {
                gridLines.Add(i.ToString());
            }
        }

        ObservableCollection<DataPointGroup> groupedPieSeries = new ObservableCollection<DataPointGroup>();
        private void UpdateGroupedPieSeries()
        {
            //gruppiere die Series neu
            List<DataPointGroup> result = new List<DataPointGroup>();
            try
            {
                ///sammle erst alle Gruppen zusammen
                foreach (ChartSeries initialSeries in this.Series)
                {
                    if (initialSeries.Items != null)
                    {
                        if (initialSeries.Items.Count > 0)
                        {
                            DataPointGroup addToGroup = new DataPointGroup();
                            addToGroup.Caption = initialSeries.Caption;
                            //addToGroup.PropertyChanged += addToGroup_PropertyChanged;
                            result.Add(addToGroup);

                            var groupBinding = new Binding();
                            groupBinding.Source = this;
                            groupBinding.Mode = BindingMode.TwoWay;
                            groupBinding.Path = new PropertyPath("SelectedItem");
                            BindingOperations.SetBinding(addToGroup, DataPointGroup.SelectedItemProperty, groupBinding);

                            int seriesIndex = 0;
                            foreach (var seriesItem in initialSeries.Items)
                            {
                                DataPoint groupdItem = new DataPoint();
                                addToGroup.DataPoints.Add(groupdItem);

                                groupdItem.SeriesCaption = initialSeries.Caption;
                                groupdItem.ValueMember = initialSeries.ValueMember;
                                groupdItem.DisplayMember = initialSeries.DisplayMember;
                                groupdItem.ItemBrush = GetItemBrush(seriesIndex);
                                groupdItem.ReferencedObject = seriesItem;

                                var selectionBinding = new Binding();
                                selectionBinding.Source = addToGroup;
                                selectionBinding.Mode = BindingMode.OneWay;
                                selectionBinding.Path = new PropertyPath("SelectedItem");
                                BindingOperations.SetBinding(groupdItem, DataPoint.SelectedItemProperty, selectionBinding);

                                var sumBinding = new Binding();
                                sumBinding.Source = addToGroup;
                                sumBinding.Mode = BindingMode.OneWay;
                                sumBinding.Path = new PropertyPath("SumOfDataPointGroup");
                                BindingOperations.SetBinding(groupdItem, DataPoint.SumOfDataPointGroupProperty, sumBinding);

                                
                                seriesIndex++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            //is there a whole change or only a change at the end??
            groupedPieSeries.Clear();            
            foreach (var item in result)
            {
                groupedPieSeries.Add(item);
            }
        }

        private void addToGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {

            }
        }

        ObservableCollection<DataPointGroup> groupedSeries = new ObservableCollection<DataPointGroup>();
        private void UpdateGroupedSeries()
        {            
            List<DataPointGroup> result = new List<DataPointGroup>();
            try
            {
                ///sammle erst alle Gruppen zusammen
                foreach (ChartSeries initialSeries in this.Series)
                {
                    foreach (var seriesItem in initialSeries.Items)
                    {
                        string seriesItemCaption = GetPropertyValue(seriesItem, initialSeries.DisplayMember); //Security
                        DataPointGroup dataPointGroup = result.Where(group => group.Caption == seriesItemCaption).FirstOrDefault();
                        if (dataPointGroup == null)
                        {
                            dataPointGroup = new DataPointGroup();
                            dataPointGroup.Caption = seriesItemCaption;
                            dataPointGroup.PropertyChanged += dataPointGroup_PropertyChanged;
                            result.Add(dataPointGroup);

                            var groupBinding = new Binding();
                            groupBinding.Source = this;
                            groupBinding.Mode = BindingMode.TwoWay;
                            groupBinding.Path = new PropertyPath("SelectedItem");
                            BindingOperations.SetBinding(dataPointGroup, DataPointGroup.SelectedItemProperty, groupBinding);

                            int seriesIndex = 0;
                            foreach (ChartSeries allSeries in this.Series)
                            {
                                DataPoint datapoint = new DataPoint();
                                datapoint.SeriesCaption = allSeries.Caption;
                                datapoint.ValueMember = allSeries.ValueMember;
                                datapoint.DisplayMember = allSeries.DisplayMember;
                                datapoint.ItemBrush = GetItemBrush(seriesIndex);
                                datapoint.PropertyChanged += groupdItem_PropertyChanged;

                                //Sende an Datapoints the maximalvalue des Charts mit (wichtig in clustered Column chart)
                                var maxDataPointValueBinding = new Binding();
                                maxDataPointValueBinding.Source = this;
                                maxDataPointValueBinding.Mode = BindingMode.OneWay;
                                maxDataPointValueBinding.Path = new PropertyPath("MaxDataPointValue");
                                BindingOperations.SetBinding(datapoint, DataPoint.MaxDataPointValueProperty, maxDataPointValueBinding);

                                //Sende den Datapoints the höchste Summe einer DataPointGroup mit (wichtig für stacked chart)
                                var maxDataPointGroupSumBinding = new Binding();
                                maxDataPointGroupSumBinding.Source = this;
                                maxDataPointGroupSumBinding.Mode = BindingMode.OneWay;
                                maxDataPointGroupSumBinding.Path = new PropertyPath("MaxDataPointGroupSum");
                                BindingOperations.SetBinding(datapoint, DataPoint.MaxDataPointGroupSumProperty, maxDataPointGroupSumBinding);

                                //Sende den Datapoint die Summe seiner Datagroup
                                var sumBinding = new Binding();
                                sumBinding.Source = dataPointGroup;
                                sumBinding.Mode = BindingMode.OneWay;
                                sumBinding.Path = new PropertyPath("SumOfDataPointGroup");
                                BindingOperations.SetBinding(datapoint, DataPoint.SumOfDataPointGroupProperty, sumBinding);

                                var selectionBinding = new Binding();
                                selectionBinding.Source = dataPointGroup;
                                selectionBinding.Mode = BindingMode.TwoWay;
                                selectionBinding.Path = new PropertyPath("SelectedItem");
                                BindingOperations.SetBinding(datapoint, DataPoint.SelectedItemProperty, selectionBinding);

                                dataPointGroup.DataPoints.Add(datapoint);
                                seriesIndex++;
                            }
                        }
                    }
                }

                ///gehe alle Series durch (Security, Naming etc.)
                foreach (ChartSeries series in this.Series)
                {
                    foreach (var seriesItem in series.Items)
                    {
                        string seriesItemCaption = GetPropertyValue(seriesItem, series.DisplayMember); //Security

                        //finde die gruppe mit dem Namen
                        DataPointGroup addToGroup = result.Where(group => group.Caption == seriesItemCaption).FirstOrDefault();

                        //finde in der Gruppe 
                        DataPoint groupdItem = addToGroup.DataPoints.Where(item => item.SeriesCaption == series.Caption).FirstOrDefault();
                        groupdItem.ReferencedObject = seriesItem;                        
                    }
                }
                //UpdateMaxValue(maxValue);                        
            }
            catch (Exception ex)
            {
            }

            //finished, copy all to the array
            groupedSeries.Clear();
            foreach (var item in result)
            {
                groupedSeries.Add(item);
            }
            RecalcSumOfDataPointGroup();
        }

        void dataPointGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SumOfDataPointGroup")
            {
                RecalcSumOfDataPointGroup();
            }
        }

        private void RecalcSumOfDataPointGroup()
        {
            double maxValue = 0.0;
            foreach(var dataPointGroup in DataPointGroups)
            {
                if (dataPointGroup.SumOfDataPointGroup > maxValue)
                {
                    maxValue = dataPointGroup.SumOfDataPointGroup;
                }
            }
            MaxDataPointGroupSum = CalculateMaxValue(maxValue);
        }

        void groupdItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RecalcMaxDataPointValue();
        }

        private void RecalcMaxDataPointValue()
        {
            double maxValue = 0.0;
            foreach (var dataPointGroup in DataPointGroups)
            {
                foreach (var dataPoint in dataPointGroup.DataPoints)
                {
                    if (dataPoint.Value > maxValue)
                    {
                        maxValue = dataPoint.Value;
                    }
                }
            }
            MaxDataPointValue = CalculateMaxValue(maxValue);
        }

        public ObservableCollection<DataPointGroup> DataPointGroups
        {
            get
            {
                return groupedSeries;                
            }
        }

        public ObservableCollection<DataPointGroup> GroupedPieSeries
        {
            get
            {
                return groupedPieSeries;                
            }
        }        


        #region Methods


        private void UpdateControls()
        {
            if (this.DataContext != null)
            {
                if (this.DataContext is IEnumerable)
                {
                    foreach (object item in (this.DataContext as IEnumerable))
                    {
                        if (item is INotifyPropertyChanged)
                        {
                            INotifyPropertyChanged observable = (INotifyPropertyChanged)item;
                            observable.PropertyChanged += new PropertyChangedEventHandler(observable_PropertyChanged);
                        }
                    }
                }
            }
        }

        private void RaisePropertyChangeEvent(String propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Initializes the chart component.
        /// </summary>
        private void InitializeChartComponent()
        {
            LinearGradientBrush chartBrush = new LinearGradientBrush();
            chartBrush.StartPoint = new Point(0, 0.032);
            chartBrush.EndPoint = new Point(0, 1);
            chartBrush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 186, 218, 243), Offset = 1 });
            chartBrush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 208, 240, 252), Offset = 0 });

            LinearGradientBrush chartMouseOverBrush = new LinearGradientBrush();
            chartMouseOverBrush.StartPoint = new Point(0, 0.03);
            chartMouseOverBrush.EndPoint = new Point(0, 1);
            chartMouseOverBrush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 202, 233, 255), Offset = 1 });
            chartMouseOverBrush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(255, 221, 245, 255), Offset = 0 });

            ChartColor = chartBrush;
            ChartColorMouseOver = chartMouseOverBrush;
        }

         /// <summary>
        /// Handles the PropertyChanged event of the observable control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void observable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            /*
            CollectionView myCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(this.DataContext);
            if (myCollectionView != null)
            {
                myCollectionView.Refresh();
            }
            */
        }

        #endregion Methods



        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// In ColumnGrid we need some space above the column to show the number above the column,
        /// this is not needed in StackedChart
        /// </summary>
        public virtual bool IsUseNextBiggestMaxValue
        {
            get
            {
                return false;
            }
        }
    }
}