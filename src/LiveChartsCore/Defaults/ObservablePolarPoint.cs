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

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LiveChartsCore.Defaults;

/// <summary>
/// Defines a point for the polar coordinate system that implements <see cref="INotifyPropertyChanged"/>.
/// </summary>
public class ObservablePolarPoint : INotifyPropertyChanged
{
    private double? _angle;
    private double? _radius;

    /// <summary>
    /// Inializes a new instance of the <see cref="ObservablePoint"/> class.
    /// </summary>
    public ObservablePolarPoint()
    { }

    /// <summary>
    /// Inializes a new instance of the <see cref="ObservablePoint"/> class.
    /// </summary>
    /// <param name="angle">The angle.</param>
    /// <param name="radius">The radius.</param>
    public ObservablePolarPoint(double? angle, double? radius)
    {
        _angle = angle;
        _radius = radius;
    }

    /// <summary>
    /// Gets or sets the angle.
    /// </summary>
    public double? Angle { get => _angle; set { _angle = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the Radius.
    /// </summary>
    public double? Radius { get => _radius; set { _radius = value; OnPropertyChanged(); } }

    /// <summary>
    /// Called when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the property changed event.
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
