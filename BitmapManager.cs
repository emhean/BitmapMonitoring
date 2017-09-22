using System;
using System.Drawing;
using System.Collections.Generic;

namespace Serathys.Drawing
{
    /// <summary>
    /// Provides tools to manipulate, store and detect duplicate Bitmaps.
    /// </summary>
    static class BitmapManager
    {
        /// <summary>
        /// Get a list of pixels from bitmap.
        /// </summary>
        private static List<Color> _GetPixels(Bitmap bitmap)
        {
            var pixels = new List<Color>();
            // Add from bitmap
            for (int x = 0; x < bitmap.Width; ++x)
            {
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    pixels.Add(bitmap.GetPixel(x, y));
                }
            }
            return pixels;
        }

        /// <summary>
        /// Turn bitmap into grayscale.
        /// </summary>
        public static void GrayScale(Bitmap bitmap)
        {
            int rgb;
            Color c;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    c = bitmap.GetPixel(x, y);
                    rgb = (int)(System.Math.Round(((double)(c.R + c.G + c.B) / 3.0) / 255) * 255);
                    bitmap.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            }
        }

        /// <summary>
        /// Compare two bitmaps and return different ratio.
        /// </summary>
        public static uint[] Compare(Bitmap bitmap1, Bitmap bitmap2, out int matches, out float difference)
        {
            var bitmap1_pixels = _GetPixels(bitmap1);
            var bitmap2_pixels = _GetPixels(bitmap2);

            uint[] sum = new uint[3]; // RBG
            difference = 0;
            matches = 0;
            for (int i = 0; i < bitmap1_pixels.Count; ++i)
            {
                if (bitmap1_pixels[i].ToArgb().Equals(bitmap2_pixels[i].ToArgb()) == false)
                {
                    matches++; // Add one to match.
                    sum[0] += Convert.ToUInt32(
                        (bitmap1_pixels[i].R + bitmap2_pixels[i].R));
                    sum[1] += Convert.ToUInt32(
                        (bitmap1_pixels[i].G + bitmap2_pixels[i].G));
                    sum[2] += Convert.ToUInt32(
                        (bitmap1_pixels[i].B + bitmap2_pixels[i].B));
                }
            }
            difference = matches * 100.0f / 256;
            return sum;
        }

        /// <summary>
        /// Remove duplicate bitmaps from the collection.
        /// </summary>
        public static IEnumerable<Bitmap> RemoveDuplicates(int matches, float difference, params Bitmap[] bitmaps)
        {
            List<Bitmap> newBitmaps = new List<Bitmap>(bitmaps);

            for (int i = 0; i < bitmaps.Length; ++i)
            {
                GrayScale(bitmaps[i]);
            }

            uint[] comp;
            float diff;
            int matc;

            for(int i = 0; i < bitmaps.Length; ++i)
            {
                for(int j = 0; j < bitmaps.Length; ++j)
                {
                    if (j == i)
                        continue;

                    comp = Compare(bitmaps[i], bitmaps[j], out matc, out diff);

                    if(matc > matches)
                    {
                        if (diff > difference)
                        {
                            newBitmaps.RemoveAt(i);
                            newBitmaps.RemoveAt(j);
                        }
                    }
                }
            }

            return newBitmaps;
        }
    }
}