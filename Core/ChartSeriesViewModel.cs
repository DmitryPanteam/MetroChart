namespace Panteam.MetroChart
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

#if NETFX_CORE

    using Windows.UI.Xaml.Media;

#else

    using System.Windows.Media;

#endif

    /// <summary>
    /// we cannot use the ChartSeries directly because we will join the data to internal Chartseries
    /// </summary>
    public class ChartLegendItemViewModel
    {
        public string Caption { get; set; }

        public Brush ItemBrush { get; set; }
    }
}
