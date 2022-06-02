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

using System;
using System.Collections.Generic;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore;

/// <summary>
/// Defines the row series 
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="BarSeries{TModel, TVisual, TLabel, TDrawingContext}" />
public class RowSeries<TModel, TVisual, TLabel, TDrawingContext> : BarSeries<TModel, TVisual, TLabel, TDrawingContext>
    where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
    where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    where TDrawingContext : DrawingContext
{
    private readonly bool _isRounded = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="RowSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
    /// </summary>
    public RowSeries()
        : base(
              SeriesProperties.Bar | SeriesProperties.PrimaryAxisHorizontalOrientation
              | SeriesProperties.Solid | SeriesProperties.PrefersYStrategyTooltips)
    {
        _isRounded = typeof(IRoundedRectangleChartPoint<TDrawingContext>).IsAssignableFrom(typeof(TVisual));
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
    public override void Measure(Chart<TDrawingContext> chart)
    {
        var cartesianChart = (CartesianChart<TDrawingContext>)chart;
        var primaryAxis = cartesianChart.YAxes[ScalesYAt];
        var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

        var drawLocation = cartesianChart.DrawMarginLocation;
        var drawMarginSize = cartesianChart.DrawMarginSize;
        var secondaryScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);
        var previousSecondaryScale =
            !primaryAxis.ActualBounds.HasPreviousState ? null : new Scaler(drawLocation, drawMarginSize, primaryAxis);
        var primaryScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);

        var uw = secondaryScale.MeasureInPixels(primaryAxis.UnitWidth); //secondaryScale.ToPixels(0f) - secondaryScale.ToPixels(primaryAxis.UnitWidth);
        var uwm = 0.5f * uw;

        var gp = (float)GroupPadding;
        if (uw - gp < 1) gp -= uw - gp;
        uw -= gp;
        //puw -= (float)GroupPadding;

        var sw = Stroke?.StrokeThickness ?? 0;
        var p = primaryScale.ToPixels((float)Pivot);

        var pos = cartesianChart.SeriesContext.GetColumnPostion(this);
        var count = cartesianChart.SeriesContext.GetColumnSeriesCount();
        var cp = 0f;

        if (!IgnoresBarPosition && count > 1)
        {
            uw /= count;
            uwm = 0.5f * uw;
            cp = (pos - count / 2f) * uw + uwm;
        }

        if (uw > MaxBarWidth)
        {
            uw = (float)MaxBarWidth;
            uwm = uw / 2f;
        }

        var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
        if (Fill is not null)
        {
            Fill.ZIndex = actualZIndex + 0.1;
            Fill.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(Fill);
        }
        if (Stroke is not null)
        {
            Stroke.ZIndex = actualZIndex + 0.1;
            Stroke.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(Stroke);
        }
        if (DataLabelsPaint is not null)
        {
            DataLabelsPaint.ZIndex = actualZIndex + 0.1;
            //DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
        }

        var dls = (float)DataLabelsSize;
        var toDeletePoints = new HashSet<ChartPoint>(everFetched);

        var rx = (float)Rx;
        var ry = (float)Ry;

        foreach (var point in Fetch(cartesianChart))
        {
            var visual = point.Context.Visual as TVisual;
            var primary = primaryScale.ToPixels(point.PrimaryValue);
            var secondary = secondaryScale.ToPixels(point.SecondaryValue);
            var b = Math.Abs(primary - p);

            if (point.IsNull)
            {
                if (visual is not null)
                {
                    visual.X = p;
                    visual.Y = secondary - uwm + cp;
                    visual.Width = 0;
                    visual.Height = uw;
                    visual.RemoveOnCompleted = true;
                    point.Context.Visual = null;
                }
                continue;
            }

            if (visual is null)
            {
                var yi = secondary - uwm + cp;
                if (previousSecondaryScale is not null) yi = previousSecondaryScale.ToPixels(point.SecondaryValue) - uwm + cp;

                var r = new TVisual
                {
                    X = p,
                    Y = yi,
                    Width = 0,
                    Height = uw
                };

                if (_isRounded)
                {
                    var rounded = (IRoundedRectangleChartPoint<TDrawingContext>)r;
                    rounded.Rx = rx;
                    rounded.Ry = ry;
                }

                visual = r;
                point.Context.Visual = visual;
                OnPointCreated(point);

                _ = everFetched.Add(point);
            }

            if (Fill is not null) Fill.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
            if (Stroke is not null) Stroke.AddGeometryToPaintTask(cartesianChart.Canvas, visual);

            var sizedGeometry = visual;

            var cx = point.PrimaryValue > Pivot ? primary - b : primary;
            var y = secondary - uwm + cp;

            sizedGeometry.X = cx;
            sizedGeometry.Y = y;
            sizedGeometry.Width = b;
            sizedGeometry.Height = uw;
            if (_isRounded)
            {
                var rounded = (IRoundedRectangleChartPoint<TDrawingContext>)visual;
                rounded.Rx = rx;
                rounded.Ry = ry;
            }
            sizedGeometry.RemoveOnCompleted = false;

            point.Context.HoverArea = new RectangleHoverArea().SetDimensions(cx, y, b, uw);

            _ = toDeletePoints.Remove(point);

            if (DataLabelsPaint is not null)
            {
                var label = (TLabel?)point.Context.Label;

                if (label is null)
                {
                    var l = new TLabel { X = p, Y = secondary - uwm + cp, RotateTransform = (float)DataLabelsRotation };

                    _ = l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                        .WithAnimation(animation =>
                            animation
                                .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                                .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction));

                    l.CompleteTransition(null);
                    label = l;
                    point.Context.Label = l;
                }

                DataLabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, label);
                label.Text = DataLabelsFormatter(new ChartPoint<TModel, TVisual, TLabel>(point));
                label.TextSize = dls;
                label.Padding = DataLabelsPadding;
                var labelPosition = GetLabelPosition(
                    cx, y, b, uw, label.Measure(DataLabelsPaint), DataLabelsPosition, SeriesProperties,
                    point.PrimaryValue > Pivot, drawLocation, drawMarginSize);
                label.X = labelPosition.X;
                label.Y = labelPosition.Y;
            }

            OnPointMeasured(point);
        }

        foreach (var point in toDeletePoints)
        {
            if (point.Context.Chart != cartesianChart.View) continue;
            SoftDeleteOrDisposePoint(point, primaryScale, secondaryScale);
            _ = everFetched.Remove(point);
        }
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, ICartesianAxis, ICartesianAxis)"/>
    public override SeriesBounds GetBounds(
        CartesianChart<TDrawingContext> chart, ICartesianAxis secondaryAxis, ICartesianAxis primaryAxis)
    {
        var baseSeriesBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);
        if (baseSeriesBounds.HasData) return baseSeriesBounds;
        var baseBounds = baseSeriesBounds.Bounds;

        var tickPrimary = primaryAxis.GetTick(chart.ControlSize, baseBounds.VisiblePrimaryBounds);
        var tickSecondary = secondaryAxis.GetTick(chart.ControlSize, baseBounds.VisibleSecondaryBounds);

        var ts = tickSecondary.Value * DataPadding.X;
        var tp = tickPrimary.Value * DataPadding.Y;

        if (baseBounds.VisibleSecondaryBounds.Delta == 0)
        {
            var ms = baseBounds.VisibleSecondaryBounds.Min == 0 ? 1 : baseBounds.VisibleSecondaryBounds.Min;
            ts = 0.1 * ms * DataPadding.X;
        }

        if (baseBounds.VisiblePrimaryBounds.Delta == 0)
        {
            var mp = baseBounds.VisiblePrimaryBounds.Min == 0 ? 1 : baseBounds.VisiblePrimaryBounds.Min;
            tp = 0.1 * mp * DataPadding.Y;
        }

        return
            new SeriesBounds(
                new DimensionalBounds
                {
                    PrimaryBounds = new Bounds
                    {
                        Max = baseBounds.SecondaryBounds.Max + 0.5 * secondaryAxis.UnitWidth,
                        Min = baseBounds.SecondaryBounds.Min - 0.5 * secondaryAxis.UnitWidth,
                        MinDelta = baseBounds.SecondaryBounds.MinDelta,
                        PaddingMax = ts,
                        PaddingMin = ts
                    },
                    SecondaryBounds = new Bounds
                    {
                        Max = baseBounds.PrimaryBounds.Max,
                        Min = baseBounds.PrimaryBounds.Min,
                        MinDelta = baseBounds.PrimaryBounds.MinDelta,
                        PaddingMax = tp,
                        PaddingMin = tp
                    },
                    VisiblePrimaryBounds = new Bounds
                    {
                        Max = baseBounds.VisibleSecondaryBounds.Max + 0.5 * secondaryAxis.UnitWidth,
                        Min = baseBounds.VisibleSecondaryBounds.Min - 0.5 * secondaryAxis.UnitWidth
                    },
                    VisibleSecondaryBounds = new Bounds
                    {
                        Max = baseBounds.VisiblePrimaryBounds.Max,
                        Min = baseBounds.VisiblePrimaryBounds.Min
                    }
                },
                false);
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SetDefaultPointTransitions(ChartPoint)"/>
    protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
    {
        var chart = chartPoint.Context.Chart;

        if (chartPoint.Context.Visual is not TVisual visual) throw new Exception("Unable to initialize the point instance.");

        _ = visual
            .TransitionateProperties(
                nameof(visual.X),
                nameof(visual.Width),
                nameof(visual.Y),
                nameof(visual.Height))
            .WithAnimation(animation =>
                animation
                    .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                    .WithEasingFunction(EasingFunction ?? chart.EasingFunction))
            .CompleteCurrentTransitions();
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.SoftDeleteOrDisposePoint(ChartPoint, Scaler, Scaler)"/>
    protected internal override void SoftDeleteOrDisposePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        if (DataFactory is null) throw new Exception("Data provider not found");

        var chartView = (ICartesianChartView<TDrawingContext>)point.Context.Chart;
        if (chartView.Core.IsZoomingOrPanning)
        {
            visual.CompleteTransition(null);
            visual.RemoveOnCompleted = true;
            DataFactory.DisposePoint(point);
            return;
        }

        var p = primaryScale.ToPixels(pivot);
        var secondary = secondaryScale.ToPixels(point.SecondaryValue);

        visual.X = p;
        visual.Y = secondary;
        visual.Width = 0;
        visual.RemoveOnCompleted = true;

        DataFactory.DisposePoint(point);

        var label = (TLabel?)point.Context.Label;
        if (label is null) return;

        label.TextSize = 1;
        label.RemoveOnCompleted = true;
    }
}
