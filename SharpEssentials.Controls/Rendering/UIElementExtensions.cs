// Sharp Essentials
// Copyright 2017 Matthew Hamilton - matthamilton@live.com
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SharpEssentials.Controls.Rendering
{
    /// <summary>
    /// Provides extension methods for <see cref="UIElement"/>s.
    /// </summary>
    public static class UIElementExtensions
    {
        /// <summary>
        /// Renders a <see cref="UIElement"/> to an <see cref="BitmapSource"/>.
        /// </summary>
        public static BitmapSource ToBitmap(this UIElement element, double opacity = 1)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var size = new Size(element.DesiredSize.Width, 
                                element.DesiredSize.Height);

            var dpi = GetDPI(element);
            var outputImage = new RenderTargetBitmap((int)size.Width, (int)size.Height,
                                                     dpi.X, dpi.Y, PixelFormats.Default);
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                var visualBrush = new VisualBrush(element)
                {
                    Opacity = opacity
                };
                drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(), size));
            }
            outputImage.Render(drawingVisual);
            return outputImage;
        }

        /// <summary>
        /// Renders a <see cref="UIElement"/> to a <see cref="Cursor"/>.
        /// </summary>
        public static Cursor ToCursor(this UIElement element, double opacity = 1)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var image = element.ToBitmap(opacity);

            var encoder = new PngBitmapEncoder
            {
                Frames = { BitmapFrame.Create(image) }
            };

            using (var imageStream = new MemoryStream())
            {
                encoder.Save(imageStream);
                using (var bmp = new System.Drawing.Bitmap(imageStream))
                {
                    return bmp.ToCursor();
                }
            }
        }

        private static DPI GetDPI(Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);
            var transformation = source.CompositionTarget.TransformToDevice;

            return new DPI(transformation.M11 * 96, transformation.M22 * 96);
        }

        private struct DPI
        {
            public DPI(double x, double y)
            {
                X = x;
                Y = y;
            }

            public double X { get; }
            public double Y { get; }
        }
    }
}