## ZIndex property

Indicates an order in the Z axis, this order controls which series is above or behind.

## IsVisible property

Indicates if the series is visible in the user interface.

## DataPadding

The data padding is the minimum distance from the edges of the series to the axis limits, it is of type `System.Drawing.PointF` 
both coordinates (X and Y) goes from 0 to 1, where 0 is nothing and 1 is the axis tick an axis tick is the separation between
every label or separator (even if they are not visible).

If this property is not set, the library will set it according to the series type, take a look at the following samples:

<pre><code>new LineSeries&lt;double>
{
    DataPadding = new System.Drawing.PointF(0, 0),
    Values = new ObservableCollection<double> { 2, 1, 3, 5, 3, 4, 6 },
    GeometryStroke = null,
    GeometryFill = null,
    Fill = null
}</code></pre>

Produces the following result:

![image]({{ assets_url }}/docs/_assets/1.8.padding00.png)

But you can remove the padding only from an axis, for example:

<pre><code>new LineSeries&lt;double>
{
    DataPadding = new System.Drawing.PointF(0.5f, 0),
    Values = new ObservableCollection&lt;double> { 2, 1, 3, 5, 3, 4, 6 },
    GeometryStroke = null,
    GeometryFill = null,
    Fill = null
}</code></pre>

![image]({{ assets_url }}/docs/_assets/1.8.padding50.png)

Or you can increase the distance:

<pre><code>new LineSeries&lt;double>
{
    DataPadding = new System.Drawing.PointF(2, 2),
    Values = new ObservableCollection&lt;double> { 2, 1, 3, 5, 3, 4, 6 },
    GeometryStroke = null,
    GeometryFill = null,
    Fill = null
}</code></pre>

![image]({{ assets_url }}/docs/_assets/1.8.padding22.png)