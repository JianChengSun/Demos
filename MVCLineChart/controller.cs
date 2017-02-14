using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;

namespace ChartTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult CreateLineChart()
        {
            var sampleDatas = new List<SampleData>
            {
                new SampleData{CollectionTime="2/7/2017 18:00",UserCount=180, },
                new SampleData{CollectionTime="2/7/2017 19:00",UserCount=175, },
                new SampleData{CollectionTime="2/7/2017 20:00",UserCount=190, },
                new SampleData{CollectionTime="2/7/2017 21:00",UserCount=200, },
                new SampleData{CollectionTime="2/7/2017 22:00",UserCount=203, },
                new SampleData{CollectionTime="2/7/2017 23:00",UserCount=162, },
                new SampleData{CollectionTime="2/7/2017 24:00",UserCount=251, },
                new SampleData{CollectionTime="2/8/2017 0:00",UserCount=151, },
                new SampleData{CollectionTime="2/8/2017 1:00",UserCount=172, },
                new SampleData{CollectionTime="2/8/2017 2:00",UserCount=142, },
                new SampleData{CollectionTime="2/8/2017 3:00",UserCount=189, },
                new SampleData{CollectionTime="2/8/2017 4:00",UserCount=123, },
                new SampleData{CollectionTime="2/8/2017 5:00",UserCount=147, },
                new SampleData{CollectionTime="2/8/2017 6:00",UserCount=158, },
                new SampleData{CollectionTime="2/8/2017 7:00",UserCount=144, },
                new SampleData{CollectionTime="2/8/2017 8:00",UserCount=189, },
            };

            var chart = new System.Web.UI.DataVisualization.Charting.Chart();
            chart.Width = 800;
            chart.Height = 400;
            chart.BackColor = Color.FromArgb(211, 223, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
            chart.Titles.Add(CreateTitle());
            chart.Legends.Add(CreateLegend());
            chart.Series.Add(CreateSeries(SeriesChartType.Line, sampleDatas));
            chart.ChartAreas.Add(CreateChartArea());

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms);

            return File(ms.GetBuffer(), @"image/png");

        }



        public Title CreateTitle()
        {
            Title title = new Title
            {
                Text = "Global Remote Access Usage - Last Day",
                ShadowColor = Color.FromArgb(32, 0, 0, 0),
                Font = new Font("Trebuchet MS", 14F, FontStyle.Bold),
                ShadowOffset = 3,
                ForeColor = Color.FromArgb(26, 59, 105)
            };
            return title;

        }




        public Series CreateSeries(SeriesChartType chartType, ICollection<SampleData> list)
        {
            var series = new Series
            {
                Name = "Global Remote Access Usage - Last Day",
                IsValueShownAsLabel = true,
                Color = Color.FromArgb(198, 99, 99),
                ChartType = chartType,
                BorderWidth = 2,
                XAxisType = AxisType.Primary,
                YAxisType = AxisType.Secondary
            };




            foreach (var item in list)
            {
                var point = new DataPoint
                {
                    AxisLabel = item.CollectionTime,
                    YValues = new double[] { double.Parse(item.UserCount.ToString()) }
                };
                series.Points.Add(point);
            }
            return series;
        }

        public ChartArea CreateChartArea()
        {
            var chartArea = new ChartArea();
            chartArea.Name = "Global Remote Access Usage - Last Day";
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;

            return chartArea;

        }




        public Legend CreateLegend()
        {
            var legend = new Legend
            {
                Name = "Global Remote Access Usage - Last Day",
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center,
                BackColor = Color.Transparent,
                Font = new Font(new FontFamily("Trebuchet MS"), 9),
                LegendStyle = LegendStyle.Row
            };
            return legend;

        }
    }

    public class SampleData
    {
        public string CollectionTime { get; set; }
        public int UserCount { get; set; }
    }
}
