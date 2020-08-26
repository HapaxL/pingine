using OpenTK.Graphics;

namespace pingine.Game.Graphics
{
    /* encodes a color in Hue, Saturation, Lightness, Alpha format
     * equivalent of OpenTK's Color4 but in HSLA instead of RGBA
     * algorithm taken from https://www.programmingalgorithms.com/algorithm/hsl-to-rgb 
     * TODO better version, clean code, Math.Round, etc */
    public class ColorHSLA
    {
        public float H { get; set; }
        public float S { get; set; }
        public float L { get; set; }
        public float A { get; set; }

        public Color4 ToRGBA()
        {
            float r = 0;
            float g = 0;
            float b = 0;

            if (S == 0)
            {
                r = g = b = L;
            }
            else
            {
                float v1, v2;
                float hue = H;

                v2 = (L < 0.5) ? (L * (1 + S)) : ((L + S) - (L * S));
                v1 = 2 * L - v2;

                r = HueToRGB(v1, v2, hue + (1.0f / 3));
                g = HueToRGB(v1, v2, hue);
                b = HueToRGB(v1, v2, hue - (1.0f / 3));
            }

            return new Color4(r, g, b, A);
        }

        private float HueToRGB(float v1, float v2, float vH)
        {
            if (vH < 0)
                vH += 1;

            if (vH > 1)
                vH -= 1;

            if ((6 * vH) < 1)
                return (v1 + (v2 - v1) * 6 * vH);

            if ((2 * vH) < 1)
                return v2;

            if ((3 * vH) < 2)
                return (v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6);

            return v1;
        }
    }
}
