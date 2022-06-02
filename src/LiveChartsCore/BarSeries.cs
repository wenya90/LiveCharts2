﻿// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore;

/// <summary>
/// Defines a bar series point.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}" />
/// <seealso cref="IBarSeries{TDrawingContext}" />
public abstract class BarSeries<TModel, TVisual, TLabel, TDrawingContext>
    : StrokeAndFillCartesianSeries<TModel, TVisual, TLabel, TDrawingContext>, IBarSeries<TDrawingContext>
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
{
    private double _groupPadding = 10;
    private double _maxBarWidth = 50;
    private bool _ignoresBarPosition = false;
    private double _rx;
    private double _ry;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
    /// </summary>
    /// <param name="properties">The properties.</param>
    protected BarSeries(SeriesProperties properties)
        : base(properties)
    { }

    /// <inheritdoc cref="IBarSeries{TDrawingContext}.GroupPadding"/>
    public double GroupPadding { get => _groupPadding; set { _groupPadding = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IBarSeries{TDrawingContext}.MaxBarWidth"/>
    public double MaxBarWidth { get => _maxBarWidth; set { _maxBarWidth = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IBarSeries{TDrawingContext}.IgnoresBarPosition"/>
    public bool IgnoresBarPosition { get => _ignoresBarPosition; set { _ignoresBarPosition = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IBarSeries{TDrawingContext}.Rx"/>
    public double Rx { get => _rx; set { _rx = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IBarSeries{TDrawingContext}.Ry"/>
    public double Ry { get => _ry; set { _ry = value; OnPropertyChanged(); } }

    /// <summary>
    /// Called when the paint context changes.
    /// </summary>
    protected override void OnSeriesMiniatureChanged()
    {
        var context = new CanvasSchedule<TDrawingContext>();
        var w = LegendShapeSize;
        var sh = 0f;

        if (Stroke is not null)
        {
            var strokeClone = Stroke.CloneTask();
            var st = Stroke.StrokeThickness;
            if (st > MaxSeriesStroke)
            {
                st = MaxSeriesStroke;
                strokeClone.StrokeThickness = MaxSeriesStroke;
            }

            var visual = new TVisual
            {
                X = st + MaxSeriesStroke - st,
                Y = st + MaxSeriesStroke - st,
                Height = (float)LegendShapeSize,
                Width = (float)LegendShapeSize
            };
            sh = st;
            strokeClone.ZIndex = 1;
            context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(strokeClone, visual));
        }

        if (Fill is not null)
        {
            var fillClone = Fill.CloneTask();
            var visual = new TVisual
            {
                X = sh + MaxSeriesStroke - sh,
                Y = sh + MaxSeriesStroke - sh,
                Height = (float)LegendShapeSize,
                Width = (float)LegendShapeSize
            };
            context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(fillClone, visual));
        }

        context.Width = w + MaxSeriesStroke * 2;
        context.Height = w + MaxSeriesStroke * 2;

        CanvasSchedule = context;
    }
}
