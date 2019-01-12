using System.Drawing;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using ZXing.Rendering;

namespace WiFiQRCode
{
    static class StringExtensions
    {
        public static byte[] ToBarcodeBytes(this string context, Size size)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Width = size.Width,
                    Height = size.Height,
                    CharacterSet = "UTF-8",

                    /* 
                     * Error Correction Level
                     * L:  7%
                     * M: 15%
                     * Q: 25%
                     * H: 30%
                    */
                    ErrorCorrection = ErrorCorrectionLevel.H
                },
                Renderer = new PixelDataRenderer()
                {
                    Foreground = PixelDataRenderer.Color.Black,
                    Background = PixelDataRenderer.Color.White
                }
            };
            return writer.Write(context).Pixels;
        }
    }
}
