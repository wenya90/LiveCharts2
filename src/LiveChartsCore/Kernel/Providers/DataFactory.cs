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
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;

namespace LiveChartsCore.Kernel.Providers;

/// <summary>
/// Defines a data provider.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TDrawingContext"></typeparam>
public class DataFactory<TModel, TDrawingContext>
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets the by value map.
    /// </summary>
    protected Dictionary<object, Dictionary<int, ChartPoint>> ByChartbyValueVisualMap { get; } = new();

#nullable disable

    // note #270422
    // We use the ByChartbyValueVisualMap for nullable and value types
    // for reference types we use the ByChartByReferenceVisualMap it does not allows nulls, it throws in the mapper
    // when it founds a null type and then it warns you on how to use it.
    // this is a current limitation and could be supported in future verions.

    /// <summary>
    /// Gets the by reference map.
    /// </summary>
    protected Dictionary<object, Dictionary<TModel, ChartPoint>> ByChartByReferenceVisualMap { get; } = new();

#nullable restore

    /// <summary>
    /// Indicates whether the factory uses value or reference types.
    /// </summary>
    protected bool IsValueType { get; private set; } = false;

    /// <summary>
    /// Gets or sets the previous known bounds.
    /// </summary>
    public DimensionalBounds PreviousKnownBounds { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataFactory{TModel, TDrawingContext}"/> class.
    /// </summary>
    public DataFactory()
    {
        var t = typeof(TModel);
        IsValueType = t.IsValueType;

        var bounds = new DimensionalBounds(true);
        PreviousKnownBounds = bounds;
    }

    /// <summary>
    /// Fetches the points for the specified series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <param name="chart">The chart.</param>
    /// <returns></returns>
    public virtual IEnumerable<ChartPoint> Fetch(ISeries<TModel> series, IChart chart)
    {
        if (series.Values is null) yield break;

        var canvas = (MotionCanvas<TDrawingContext>)chart.Canvas;

        var mapper = series.Mapping ?? LiveCharts.CurrentSettings.GetMap<TModel>();
        var index = 0;

        if (IsValueType)
        {
            _ = ByChartbyValueVisualMap.TryGetValue(canvas.Sync, out var d);
            if (d is null)
            {
                d = new Dictionary<int, ChartPoint>();
                ByChartbyValueVisualMap[canvas.Sync] = d;
            }
            var byValueVisualMap = d;

            foreach (var item in series.Values)
            {
                if (!byValueVisualMap.TryGetValue(index, out var cp))
                    byValueVisualMap[index] = cp = new ChartPoint(chart.View, series);

                cp.Context.Index = index++;
                cp.Context.DataSource = item;

                mapper(item, cp);

                yield return cp;
            }
        }
        else
        {
            _ = ByChartByReferenceVisualMap.TryGetValue(canvas.Sync, out var d);
            if (d is null)
            {
#nullable disable
                // see note #270422
                d = new Dictionary<TModel, ChartPoint>();
#nullable restore
                ByChartByReferenceVisualMap[canvas.Sync] = d;
            }
            var byReferenceVisualMap = d;

            foreach (var item in series.Values)
            {
                if (!byReferenceVisualMap.TryGetValue(item, out var cp))
                    byReferenceVisualMap[item] = cp = new ChartPoint(chart.View, series);

                cp.Context.Index = index++;
                cp.Context.DataSource = item;
                mapper(item, cp);

                yield return cp;
            }
        }
    }

    /// <summary>
    /// Disposes a given point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns></returns>
    public virtual void DisposePoint(ChartPoint point)
    {
        if (IsValueType)
        {
            var canvas = (MotionCanvas<TDrawingContext>)point.Context.Chart.CoreChart.Canvas;
            _ = ByChartbyValueVisualMap.TryGetValue(canvas.Sync, out var d);
            var byValueVisualMap = d;
            if (byValueVisualMap is null) return;
            _ = byValueVisualMap.Remove(point.Context.Index);
        }
        else
        {
            if (point.Context.DataSource is null) return;
            var canvas = (MotionCanvas<TDrawingContext>)point.Context.Chart.CoreChart.Canvas;
            _ = ByChartByReferenceVisualMap.TryGetValue(canvas.Sync, out var d);
            var byReferenceVisualMap = d;
            if (byReferenceVisualMap is null) return;
            _ = byReferenceVisualMap.Remove((TModel)point.Context.DataSource);
        }
    }

    /// <summary>
    /// Disposes the data provider from the given chart.
    /// </summary>
    /// <param name="chart"></param>
    public virtual void Dispose(IChart chart)
    {
        if (IsValueType)
        {
            var canvas = (MotionCanvas<TDrawingContext>)chart.Canvas;
            _ = ByChartbyValueVisualMap.Remove(canvas.Sync);
        }
        else
        {
            var canvas = (MotionCanvas<TDrawingContext>)chart.Canvas;
            _ = ByChartByReferenceVisualMap.Remove(canvas.Sync);
        }
    }

    /// <summary>
    /// Gets the Cartesian bounds.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="series">The series.</param>
    /// <param name="plane1">The x.</param>
    /// <param name="plane2">The y.</param>
    /// <returns></returns>
    public virtual SeriesBounds GetCartesianBounds(
        Chart<TDrawingContext> chart,
        IChartSeries<TDrawingContext> series,
        IPlane plane1,
        IPlane plane2)
    {
        var stack = chart.SeriesContext.GetStackPosition(series, series.GetStackGroup());

        var xMin = plane1.MinLimit ?? double.MinValue;
        var xMax = plane1.MaxLimit ?? double.MaxValue;
        var yMin = plane2.MinLimit ?? double.MinValue;
        var yMax = plane2.MaxLimit ?? double.MaxValue;

        var hasData = false;

        var bounds = new DimensionalBounds();

        ChartPoint? previous = null;

        foreach (var point in series.Fetch(chart))
        {
            var primary = point.PrimaryValue;
            var secondary = point.SecondaryValue;
            var tertiary = point.TertiaryValue;

            if (stack is not null) primary = stack.StackPoint(point);

            bounds.PrimaryBounds.AppendValue(primary);
            bounds.SecondaryBounds.AppendValue(secondary);
            bounds.TertiaryBounds.AppendValue(tertiary);

            if (primary >= yMin && primary <= yMax && secondary >= xMin && secondary <= xMax)
            {
                bounds.VisiblePrimaryBounds.AppendValue(primary);
                bounds.VisibleSecondaryBounds.AppendValue(secondary);
                bounds.VisibleTertiaryBounds.AppendValue(tertiary);
            }

            if (previous is not null)
            {
                var dx = Math.Abs(previous.SecondaryValue - point.SecondaryValue);
                var dy = Math.Abs(previous.PrimaryValue - point.PrimaryValue);
                if (dx < bounds.SecondaryBounds.MinDelta) bounds.SecondaryBounds.MinDelta = dx;
                if (dy < bounds.PrimaryBounds.MinDelta) bounds.PrimaryBounds.MinDelta = dy;
            }

            previous = point;
            hasData = true;
        }

        return !hasData
            ? new SeriesBounds(PreviousKnownBounds, true)
            : new SeriesBounds(PreviousKnownBounds = bounds, false);
    }

    /// <summary>
    /// Gets the financial bounds.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="series">The series.</param>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns></returns>
    public virtual SeriesBounds GetFinancialBounds(
        CartesianChart<TDrawingContext> chart,
        IChartSeries<TDrawingContext> series,
        ICartesianAxis x,
        ICartesianAxis y)
    {
        var xMin = x.MinLimit ?? double.MinValue;
        var xMax = x.MaxLimit ?? double.MaxValue;
        var yMin = y.MinLimit ?? double.MinValue;
        var yMax = y.MaxLimit ?? double.MaxValue;

        var hasData = false;

        var bounds = new DimensionalBounds();
        ChartPoint? previous = null;
        foreach (var point in series.Fetch(chart))
        {
            var primaryMax = point.PrimaryValue;
            var primaryMin = point.QuinaryValue;
            var secondary = point.SecondaryValue;
            var tertiary = point.TertiaryValue;

            bounds.PrimaryBounds.AppendValue(primaryMax);
            bounds.PrimaryBounds.AppendValue(primaryMin);
            bounds.SecondaryBounds.AppendValue(secondary);
            bounds.TertiaryBounds.AppendValue(tertiary);

            if (primaryMax >= yMin && primaryMin <= yMax && secondary >= xMin && secondary <= xMax)
            {
                bounds.VisiblePrimaryBounds.AppendValue(primaryMax);
                bounds.VisiblePrimaryBounds.AppendValue(primaryMin);
                bounds.VisibleSecondaryBounds.AppendValue(secondary);
                bounds.VisibleTertiaryBounds.AppendValue(tertiary);
            }

            if (previous is not null)
            {
                var dx = Math.Abs(previous.SecondaryValue - point.SecondaryValue);
                var dy = Math.Abs(previous.PrimaryValue - point.PrimaryValue);
                if (dx < bounds.SecondaryBounds.MinDelta) bounds.SecondaryBounds.MinDelta = dx;
                if (dy < bounds.PrimaryBounds.MinDelta) bounds.PrimaryBounds.MinDelta = dy;
            }

            previous = point;
            hasData = true;
        }

        return !hasData
            ? new SeriesBounds(PreviousKnownBounds, true)
            : new SeriesBounds(PreviousKnownBounds = bounds, false);
    }

    /// <summary>
    /// Gets the pie bounds.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="series">The series.</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">Unexpected null stacker</exception>
    public virtual SeriesBounds GetPieBounds(
        PieChart<TDrawingContext> chart, IPieSeries<TDrawingContext> series)
    {
        var stack = chart.SeriesContext.GetStackPosition(series, series.GetStackGroup());
        if (stack is null) throw new NullReferenceException("Unexpected null stacker");

        var bounds = new DimensionalBounds();
        var hasData = false;

        foreach (var point in series.Fetch(chart))
        {
            _ = stack.StackPoint(point);
            bounds.PrimaryBounds.AppendValue(point.PrimaryValue);
            bounds.SecondaryBounds.AppendValue(point.SecondaryValue);
            bounds.TertiaryBounds.AppendValue(series.Pushout > series.HoverPushout ? series.Pushout : series.HoverPushout);
            hasData = true;
        }

        if (!hasData)
        {
            bounds.PrimaryBounds.AppendValue(0);
            bounds.SecondaryBounds.AppendValue(0);
            bounds.TertiaryBounds.AppendValue(0);
        }

        return new SeriesBounds(bounds, false);
    }

    /// <summary>
    /// Clears the visuals in the cache.
    /// </summary>
    /// <returns></returns>
    public virtual void RestartVisuals()
    {
        foreach (var byReferenceVisualMap in ByChartByReferenceVisualMap)
        {
            foreach (var item in byReferenceVisualMap.Value)
            {
                if (item.Value.Context.Visual is not IAnimatable visual) continue;
                visual.RemoveTransition(null);
            }
            byReferenceVisualMap.Value.Clear();
        }

        foreach (var byValueVisualMap in ByChartbyValueVisualMap)
        {
            foreach (var item in byValueVisualMap.Value)
            {
                if (item.Value.Context.Visual is not IAnimatable visual) continue;
                visual.RemoveTransition(null);
            }
            byValueVisualMap.Value.Clear();
        }
    }
}
