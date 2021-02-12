﻿// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharp.Drawing;
using LiveChartsCore.SkiaSharp.Motion;
using SkiaSharp;
using System;
using System.Drawing;

namespace LiveChartsCore.SkiaSharp.Painting
{
    public class TextPaintTask : PaintTask, IWritableTask<SkiaDrawingContext>
    {
        private readonly ColorMotionProperty colorTransition;
        private readonly FloatMotionProperty textSizeTransition;

        public TextPaintTask()
        {
            colorTransition = RegisterTransitionProperty(new ColorMotionProperty(nameof(Color), new SKColor()));
            textSizeTransition = RegisterTransitionProperty(new FloatMotionProperty(nameof(TextSize), 13f));
        }

        public TextPaintTask(SKColor color, float fontSize)
        {
            colorTransition = RegisterTransitionProperty(new ColorMotionProperty(nameof(Color), color));
            textSizeTransition = RegisterTransitionProperty(new FloatMotionProperty(nameof(TextSize), fontSize));
        }

        public SKColor Color { get => colorTransition.GetMovement(this); set { colorTransition.SetMovement(value, this); } }
        public bool IsAntialias { get; set; } = true;
        public float TextSize { get => textSizeTransition.GetMovement(this); set { textSizeTransition.SetMovement(value, this); } }

        public override IDrawableTask<SkiaDrawingContext> CloneTask()
        {
            var clone = new TextPaintTask
            {
                Style = Style,
                IsStroke = IsStroke,
                Color = Color,
                IsAntialias = IsAntialias,
                TextSize = TextSize,
                StrokeWidth = StrokeWidth
            };

            return clone;
        }

        public override void InitializeTask(SkiaDrawingContext drawingContext)
        {
            if (skiaPaint == null) skiaPaint = new SKPaint();

            skiaPaint.Color = Color;
            skiaPaint.IsAntialias = IsAntialias;
            skiaPaint.IsStroke = IsStroke;
            skiaPaint.StrokeWidth = StrokeWidth;
            skiaPaint.TextSize = TextSize;

            drawingContext.Paint = skiaPaint;
        }

        public SizeF MeasureText(string content)
        {
            var p = new SKPaint
            {
                Color = Color,
                IsAntialias = IsAntialias,
                IsStroke = IsStroke,
                StrokeWidth = StrokeWidth,
                TextSize = TextSize
            };

            var bounds = new SKRect();
            p.MeasureText(content, ref bounds);
            Dispose();
            return new SizeF(bounds.Size.Width, bounds.Size.Height);
        }
    }
}
