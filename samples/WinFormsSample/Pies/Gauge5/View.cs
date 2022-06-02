﻿using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Pies.Gauge5;

namespace WinFormsSample.Pies.Gauge5;

public partial class View : UserControl
{
    private readonly PieChart pieChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        pieChart = new PieChart
        {
            Series = viewModel.Series,
            InitialRotation = -90,
            MaxAngle = 270,
            Total = 100,
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(pieChart);

        var b1 = new Button { Text = "Update", Location = new System.Drawing.Point(0, 0) };
        b1.Click += (object sender, System.EventArgs e) => viewModel.DoRandomChange();
        Controls.Add(b1);
        b1.BringToFront();
    }
}
