#ifndef RGB_TO_HSV_INCLUDED
#define RGB_TO_HSV_INCLUDED

void RGBtoHSV(float3 RGB, out float3 HSV)
{
	float maxC = max(RGB.r, max(RGB.g, RGB.b));
	float minC = min(RGB.r, min(RGB.g, RGB.b));
	float delta = maxC - minC;

	float H = 0;
	if (delta > 0)
	{
		if (maxC == RGB.r)
		{
			H = 60 * fmod(((RGB.g - RGB.b) / delta), 6);
		} else if (maxC == RGB.g)
		{
			H = 60 * (((RGB.b - RGB.r) / delta) + 2);
		} else
		{
			H = 60 * (((RGB.r - RGB.g) / delta) + 4);
		}
	}
	if (H < 0)
		H += 360;

	float S = (maxC == 0) ? 0 : (delta / maxC);
	float V = maxC;

	HSV = float3(H / 360, S, V); // Normalize Hue to 0-1
}

#endif
