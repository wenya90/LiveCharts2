<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

{{ render this "~/shared/polarlabels.md" }}

## Stroke property

If the stroke property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

![image]({{ assets_url }}/docs/_assets/polarlinestroke.png)

<pre><code>Series = new ISeries[]
{
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 }, // mark
        Fill = null,
        GeometryFill = null,
        GeometryStroke = null
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 8 }, // mark
        Fill = null,
        GeometryFill = null,
        GeometryStroke = null
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 4, 2, 5, 3, 9 },
        Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 1 }, // mark
        Fill = null,
        GeometryFill = null,
        GeometryStroke = null
    }
};</code></pre>

## Fill property

If the fill property is not set, then LiveCharts will create it based on the series position in your series collection
and the current theme.

The alpha channel enables transparency, it goes from 0 to 255, 0 is transparent and 255 disables transparency completely.

![image]({{ assets_url }}/docs/_assets/polarlinefill.png)

<pre><code>Series = new ISeries[]
{
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(90)), // mark
        Stroke = null,
        GeometryFill = null,
        GeometryStroke = null
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        Fill = new SolidColorPaint(SKColors.Red.WithAlpha(90)), // mark
        Stroke = null,
        GeometryFill = null,
        GeometryStroke = null
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 4, 2, 5, 3, 9 },
        Fill = new SolidColorPaint(SKColors.Green.WithAlpha(90)), // mark
        Stroke = null,
        GeometryFill = null,
        GeometryStroke = null
    }
};</code></pre>

## GeometryFill and GeometryStroke properties

The geometry is the circle shape (by default) that the line series draws for every point, you can customize
the fill and stroke of this shape, if none of these properties are set then LiveCharts will create them based on 
the series position in your series collection and the current theme.

![image]({{ assets_url }}/docs/_assets/polarlinegsf.png)

<pre><code>Series = new ISeries[]
{
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 4 },
        Fill = null,
        GeometryFill = new SolidColorPaint(SKColors.AliceBlue), // mark
        GeometryStroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 4 } // mark
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 8 },
        Fill = null,
        GeometryFill = new SolidColorPaint(SKColors.IndianRed), // mark
        GeometryStroke = new SolidColorPaint(SKColors.DarkSalmon) { StrokeThickness = 8 } // mark
    }
};</code></pre>

## GeometrySize property

Determines the size of the geometry, if this property is not set, then the library will decide it based on the theme.

![image]({{ assets_url }}/docs/_assets/polarlinegs.png)

<pre><code>Series = new ISeries[]
{
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 4, 4, 7, 2, 8 },
        GeometrySize = 10 // mark
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 7, 5, 3, 2, 6 },
        GeometrySize = 30 // mark
    }
};</code></pre>

## LineSmoothness property

Determines if the series line is straight or curved, this property is of type `double` and goes from `0` to `1` any other
value will be ignored, where 0 is straight and 1 is the most curved line.

![image]({{ assets_url }}/docs/_assets/polarlinesmothness.png)

<pre><code>Series = new ISeries[]
{
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 5, 4, 7, 3, 8 },
        LineSmoothness = 0 // mark
    },
    new PolarLineSeries&lt;int>
    {
        Values = new [] { 7, 2, 6, 2, 6 },
        LineSmoothness = 1 // mark
    }
};</code></pre>

## EnableNullSplitting property

This property is enabled by default (`true`), it has a performance cost and allows the series to create gaps, when the
series finds a `null` instance then the series will create a gap.

![image]({{ assets_url }}/docs/_assets/polarlinesnullsplit.png)

<pre><code>Series = new ISeries[]
{
    new PolarLineSeries&lt;int?>
    {
        Values = new int?[] 
        { 
            5, 
            4, 
            2, 
            null, // mark
            3, 
            8, 
            6 
        },
        LineSmoothness = 0,
        IsClosed = false
    }
};</code></pre>

## Plotting custom types

You can teach LiveCharts to plot anything, imagine the case where we have an array of the `City` class defined bellow:

<pre><code>public class City
{
    public string Name { get; set; }
    public double Population { get; set; }
    public double LandArea { get; set; }
}</code></pre>

You can register this type **globally**, this means that every time LiveCharts finds a `City` instance in a chart
it will use the mapper we registered, global mappers are unique for a type, if you need to plot multiple
properties then you should use local mappers.

<pre><code>// Ideally you should call this when your application starts
// If you need help to decide where to add this code
// please see the installation guide in this docs.

// in this case we have an array of the City class
// we need to compare the Population property of every city in our array

LiveCharts.Configure(config =>
    config
        .HasMap&lt;City>((city, point) =>
        {
            // in this lambda function we take an instance of the City class (see city parameter)
            // and the point in the chart for that instance (see point parameter)
            // LiveCharts will call this method for every instance of our City class array,
            // now we need to populate the point coordinates from our City instance to our point

            // in this case we will use the Population property as our primary value (normally the Y coordinate)
            point.PrimaryValue = (float)city.Population;

            // then the secondary value (normally the X coordinate)
            // will be the index of city in our cities array
            point.SecondaryValue = point.Context.Index;

            // but you can use another property of the city class as the X coordinate
            // for example lets use the LandArea property to create a plot that compares
            // Population and LandArea in chart:

            // point.SecondaryValue = (float)city.LandArea;
        })
        .HasMap&lt;Foo>(...) // you can register more types here using our fluent syntax
        .HasMap&lt;Bar>(...)
    );</code></pre>

Now we are ready to plot cities all over our application:

<pre><code>Series = new[]
{
    new PolarLineSeries&lt;City>
    {
        Name = "Population",
        TooltipLabelFormatter = point => $"{point.Model.Name} {point.PrimaryValue:N2}M",
        Values = new[]
        {
            new City { Name = "Tokyo", Population = 4, LandArea = 3 },
            new City { Name = "New York", Population = 6, LandArea = 4 },
            new City { Name = "Seoul", Population = 2, LandArea = 1 },
            new City { Name = "Moscow", Population = 8, LandArea = 7 },
            new City { Name = "Shanghai", Population = 3, LandArea = 2 },
            new City { Name = "Guadalajara", Population = 4, LandArea = 5 }
        }
    }
};</code></pre>

![image]({{ assets_url }}/docs/_assets/polarlinect.png)

Alternatively you could create a **local** mapper that will only work for a specific series, global mappers will be 
ignored when the series `Mapping` property is not null.

<pre><code>var cities = new[]
{
    new City { Name = "Tokyo", Population = 4, LandArea = 3 },
    new City { Name = "New York", Population = 6, LandArea = 4 },
    new City { Name = "Seoul", Population = 2, LandArea = 1 },
    new City { Name = "Moscow", Population = 8, LandArea = 7 },
    new City { Name = "Shanghai", Population = 3, LandArea = 2 },
    new City { Name = "Guadalajara", Population = 4, LandArea = 5 }
};

Series = new[]
{
    // this series draws the Population property in the Y axis
    new PolarLineSeries&lt;City>
    {
        Name = "Population",
        TooltipLabelFormatter = point => $"{point.Model.Name} population: {point.PrimaryValue:N2}M",
        Values = cities,
        Mapping = (city, point) =>
        {
            point.PrimaryValue = (float)city.Population;
            point.SecondaryValue = point.Context.Index;
        }
    },

    // draws the LandArea property in the Y axis
    new LineSeries&lt;City>
    {
        Name = "Population",
        TooltipLabelFormatter = (point) => $"{point.Model.Name} area: {point.PrimaryValue:N2}KM2",
        Values = cities,
        Mapping = (city, point) =>
        {
            point.PrimaryValue = (float)city.LandArea;
            point.SecondaryValue = point.Context.Index;
        }
    },

    // compares Population (Y) and LandArea (Y)
    //new LineSeries&lt;City>
    //{
    //    Name = "Population",
    //    TooltipLabelFormatter = (point) => $"{point.Model.Name} population: {point.PrimaryValue:N2}M, area: {point.SecondaryValue}KM2",
    //    Values = cities,
    //    Mapping = (city, point) =>
    //    {
    //        point.PrimaryValue = (float)city.Population;
    //        point.SecondaryValue = (float)city.LandArea;
    //    }
    //}
};</code></pre>

## Custom geometries

You can use any geometry to represent a point in a line series.

![image]({{ assets_url }}/docs/_assets/polarlinecg.png)

<pre><code>Series = new List&lt;ISeries>
{
    // use the second argument type to specify the geometry to draw for every point
    // there are already many predefined geometries in the
    // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
    new PolarLineSeries&lt;double, LiveChartsCore.SkiaSharpView.Drawing.Geometries.RectangleGeometry>
    {
        Values = new List&lt;double> { 3, 3, -3, -2, -4, -3, -1 },
        Fill = null,
        LineSmoothness = 1
    },

    // you can also define your own SVG geometry
    new PolarLineSeries&lt;double, MyGeometry>
    {
        Values = new List&lt;double> { -2, 2, 1, 3, -1, 4, 3 },

        Stroke = new SolidColorPaint(SKColors.DarkOliveGreen, 3),
        Fill = null,
        GeometryStroke = null,
        GeometryFill = new SolidColorPaint(SKColors.DarkOliveGreen),
        GeometrySize = 40
    }
};</code></pre>

Where `MyGeometry` class is our custom shape, you can draw anything `SkiaSharp` supports at this point,
but in this case we will draw an SVG path, we inherit from `SVGPathGeometry`, and for performance reasons
we use a static variable to parse the SVG path, this ways the parse operation only runs once.

<pre><code>public class MyGeometry : LiveChartsCore.SkiaSharpView.Drawing.Geometries.SVGPathGeometry
{
    // the static field is important to prevent the svg path is parsed multiple times // mark
    // Icon from Google Material Icons font.
    // https://fonts.google.com/icons?selected=Material%20Icons%20Outlined%3Atask_alt%3A
    public static SKPath svgPath = SKPath.ParseSvgPathData(
        "M22,5.18L10.59,16.6l-4.24-4.24l1.41-1.41l2.83,2.83l10-10L22,5.18z M19.79,10.22C19.92,10.79,20,11.39,20,12 " +
        "c0,4.42-3.58,8-8,8s-8-3.58-8-8c0-4.42,3.58-8,8-8c1.58,0,3.04,0.46,4.28,1.25l1.44-1.44C16.1,2.67,14.13,2,12,2C6.48,2,2,6.48,2,12 " +
        "c0,5.52,4.48,10,10,10s10-4.48,10-10c0-1.19-0.22-2.33-0.6-3.39L19.79,10.22z");

    public MyGeometry()
        : base(svgPath)
    {
    }

    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        // lets also draw a white circle as background before the svg path is drawn
        // this will just make things look better

        using (var backgroundPaint = new SKPaint())
        {
            backgroundPaint.Style = SKPaintStyle.Fill;
            backgroundPaint.Color = SKColors.White;

            var r = Width / 2;
            context.Canvas.DrawCircle(X + r, Y + r, r, backgroundPaint);
        }

        base.OnDraw(context, paint);
    }
}</code></pre>

{{ render this "~/shared/series2.md" }}