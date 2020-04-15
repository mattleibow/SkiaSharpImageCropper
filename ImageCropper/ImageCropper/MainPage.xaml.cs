using Plugin.FilePicker;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace ImageCropper
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private SKImage pickedImage;
        private List<SKPoint> cropPoints = new List<SKPoint>();
        private SKPath cropPath;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnPickImage(object sender, EventArgs e)
        {
            pickedImage?.Dispose();
            pickedImage = null;

            var picked = await CrossFilePicker.Current.PickFile();
            pickedImage = SKImage.FromEncodedData(picked.GetStream());

            skiaView.InvalidateSurface();
        }

        private void OnClear(object sender, EventArgs e)
        {
            cropPoints.Clear();
            cropPath?.Dispose();
            cropPath = null;

            skiaView.InvalidateSurface();
        }

        private void OnPaintSurafce(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.Black);

            if (pickedImage != null)
            {
                canvas.DrawImage(pickedImage, 0, 0);
            }

            if (cropPoints.Count > 0)
            {
                using var pathPaint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 6,
                    Color = SKColors.Gray,
                };
                canvas.DrawPath(cropPath, pathPaint);

                pathPaint.StrokeWidth = 4;
                pathPaint.Color = SKColors.White;
                canvas.DrawPath(cropPath, pathPaint);
            }
        }

        private void OnTouchSurface(object sender, SKTouchEventArgs e)
        {
            if (e.InContact)
            {
                cropPoints.Add(e.Location);

                cropPath?.Dispose();
                cropPath = new SKPath();
                cropPath.AddPoly(cropPoints.ToArray());

                skiaView.InvalidateSurface();
            }

            e.Handled = true;
        }

        private void OnCropImage(object sender, EventArgs e)
        {
            using var cropPath = new SKPath();
            cropPath.AddPoly(cropPoints.ToArray());

            // add a little padding all round
            var bounds = SKRectI.Ceiling(cropPath.TightBounds);
            bounds.Inflate(4, 4);

            var info = new SKImageInfo(bounds.Width, bounds.Height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.Transparent);

            canvas.Translate(-bounds.Left, -bounds.Top);
            canvas.ClipPath(cropPath);

            canvas.DrawImage(pickedImage, 0, 0);

            var image = surface.Snapshot();

            Navigation.PushAsync(new PreviewPage(image));
        }
    }
}
