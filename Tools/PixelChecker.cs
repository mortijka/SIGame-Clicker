using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SIGame_Clicker.Tools
{
    public class PixelChecker
    {
        [DllImport("user32.dll")]
        public static extern nint GetDC(nint hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(nint hWnd, nint hDC);

        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(nint hDC, int x, int y);

        public static bool IsPixelWhite(int x, int y)
        {
            Color pixelColor = GetPixelColor(x, y);
            return pixelColor.R == 255 && pixelColor.G == 255 && pixelColor.B == 255;
        }

        private static Color GetPixelColor(int x, int y)
        {
            nint hdc = GetDC(nint.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(nint.Zero, hdc);

            int red = (int)(pixel & 0x000000FF);
            int green = (int)((pixel & 0x0000FF00) >> 8);
            int blue = (int)((pixel & 0x00FF0000) >> 16);

            return Color.FromArgb(red, green, blue);
        }
    }
}
