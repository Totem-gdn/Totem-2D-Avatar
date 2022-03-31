#ifndef TOTEM_2D_COLORS
#define TOTEM_2D_COLORS

#include "UnityCG.cginc"

// https://www.chilliant.com/rgb2hsv.html

float Epsilon = 1e-10;

fixed3 RGBtoHCV(in fixed3 RGB)
{
    // Based on work by Sam Hocevar and Emil Persson
    fixed4 P = (RGB.g < RGB.b) ? fixed4(RGB.bg, -1.0, 2.0 / 3.0) : fixed4(RGB.gb, 0.0, -1.0 / 3.0);
    fixed4 Q = (RGB.r < P.x) ? fixed4(P.xyw, RGB.r) : fixed4(RGB.r, P.yzx);
    fixed C = Q.x - min(Q.w, Q.y);
    fixed H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
    return fixed3(H, C, Q.x);
}


fixed3 RGBtoHSV(in fixed3 RGB)
{
    fixed3 HCV = RGBtoHCV(RGB);
    fixed S = HCV.y / (HCV.z + Epsilon);
    return fixed3(HCV.x, S, HCV.z);
}

fixed3 HUEtoRGB(in float H)
{
    fixed R = abs(H * 6 - 3) - 1;
    fixed G = 2 - abs(H * 6 - 2);
    fixed B = 2 - abs(H * 6 - 4);
    return saturate(fixed3(R, G, B));
}

fixed3 HSVtoRGB(in fixed3 HSV)
{
    fixed3 RGB = HUEtoRGB(HSV.x);
    return ((RGB - 1) * HSV.y + 1) * HSV.z;
}

#endif
