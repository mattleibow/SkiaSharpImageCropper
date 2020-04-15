using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace ImageCropper
{
    public partial class PreviewPage : ContentPage
    {
        private const int BaseBlockSize = 12;

        public PreviewPage(SKImage image)
        {
            InitializeComponent();

            previewImage.Source = (SKImageImageSource)image;
        }

        private void OnPaintBackground(object sender, SKPaintSurfaceEventArgs e)
        {
            var scale = e.Info.Width / (float)((View)sender).Width;
            var blockSize = BaseBlockSize * scale;

            var offsetMatrix = SKMatrix.CreateScale(2 * blockSize, blockSize);
            var skewMatrix = SKMatrix.CreateSkew(0.5f, 0);
            var matrix = offsetMatrix.PreConcat(skewMatrix);

            using var path = new SKPath();
            path.AddRect(SKRect.Create(blockSize / -2, blockSize / -2, blockSize, blockSize));

            using var paint = new SKPaint
            {
                PathEffect = SKPathEffect.Create2DPath(matrix, path),
                Color = 0xFFF0F0F0
            };

            var canvas = e.Surface.Canvas;
            var area = SKRect.Create(e.Info.Width + blockSize, e.Info.Height + blockSize);
            canvas.DrawRect(area, paint);
        }
    }
}
