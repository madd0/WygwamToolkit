//-----------------------------------------------------------------------
// <copyright file="TileTexturePanel.cs" company="askalll">
//     Copyright (c) 2013 Askalll.
//     Licensed under the Microsoft Public License (Ms-PL) (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//
//         http://opensource.org/licenses/Ms-PL.html
//
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

namespace Wygwam.Windows.WinRT.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Data;
    using global::Windows.UI.Xaml.Media;
    using global::Windows.UI.Xaml.Shapes;
    using global::Windows.Foundation;

    public class TileTexturePanel : Panel
    {
        public double TileSize
        {
            get { return (double)GetValue(TileSizeProperty); }
            set { SetValue(TileSizeProperty, value); }
        }

        public static readonly DependencyProperty TileSizeProperty =
            DependencyProperty.Register("TileSize", typeof(double), typeof(TileTexturePanel), new PropertyMetadata(150.0));

        public ImageBrush Fill
        {
            get { return (ImageBrush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(ImageBrush), typeof(TileTexturePanel), new PropertyMetadata(null));

        protected override Size ArrangeOverride(Size finalSize)
        {
            var rows = (int)Math.Ceiling(finalSize.Height / TileSize);
            var cols = (int)Math.Ceiling(finalSize.Width / TileSize);

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    var itemIndex = j * cols + i;
                    var rect = this.Children[itemIndex];
                    rect.Arrange(new Rect(i * TileSize, j * TileSize, TileSize, TileSize));
                }
            }

            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            var rows = (int)Math.Ceiling(availableSize.Height / TileSize);
            var cols = (int)Math.Ceiling(availableSize.Width / TileSize);

            var cellCount = rows * cols;

            if (this.Children.Count < cellCount || this.Children.Count > cellCount)
            {
                var missingItems = rows * cols - this.Children.Count;
                for (int i = 0; i < missingItems; i++)
                {
                    var rectangle = new Rectangle();
                    rectangle.SetBinding(Rectangle.FillProperty, new Binding() { Source = this, Path = new PropertyPath("Fill") });
                    this.Children.Add(rectangle);
                }
                if (missingItems < 0)
                {
                    for (int i = 0; i > missingItems; i--)
                    {
                        this.Children.Remove(this.Children.Last());
                    }
                }
            }

            foreach (var item in Children)
            {
                item.Measure(new Size(TileSize, TileSize));
            }
            return availableSize;
        }

    }
}
