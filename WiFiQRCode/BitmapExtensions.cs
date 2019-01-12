using System.Collections.Generic;
using System.Drawing;

namespace WiFiQRCode
{
    static class BitmapExtensions
    {
        public static byte[] ToGrayscaleBytes(this Bitmap bitmap)
        {
            LockBitmap lockBitmap = new LockBitmap(bitmap);
            lockBitmap.LockBits();
            List<byte> bytes = new List<byte>();
            for (int y = 0; y < lockBitmap.Height; y++)
                for (int x = 0; x < lockBitmap.Width; x++)
                {
                    bytes.Add((byte)((lockBitmap.GetPixel(x, y).R + lockBitmap.GetPixel(x, y).G + lockBitmap.GetPixel(x, y).B) / 3));
                }
            lockBitmap.UnlockBits();
            return bytes.ToArray();
        }
    }
}
