using System.Drawing;
using System.Windows.Forms;

namespace GlassCalculator
{
    public static class ColorPalette
    {
        public static readonly Color GradientTop    = ColorTranslator.FromHtml("#1a1a2e");
        public static readonly Color GradientBottom = ColorTranslator.FromHtml("#16213e");

        public static Color GlassFill(int alpha = 40)
            => Color.FromArgb(alpha, 255, 255, 255);
        public static Color GlassBorder(int alpha = 80)
            => Color.FromArgb(alpha, 255, 255, 255);

        public static readonly Color AccentBlue   = ColorTranslator.FromHtml("#4fc3f7");
        public static readonly Color AccentPurple = ColorTranslator.FromHtml("#ce93d8");
        public static readonly Color AccentGreen  = ColorTranslator.FromHtml("#a5d6a7");
        public static readonly Color AccentOrange = ColorTranslator.FromHtml("#ffcc80");
        public static readonly Color AccentRed    = ColorTranslator.FromHtml("#ef9a9a");
        public static readonly Color AccentCyan   = ColorTranslator.FromHtml("#80deea");

        public static readonly Color TextPrimary   = Color.White;
        public static readonly Color TextSecondary = Color.FromArgb(180, 255, 255, 255);
        public static readonly Color TextDim       = Color.FromArgb(100, 255, 255, 255);
    }
}
