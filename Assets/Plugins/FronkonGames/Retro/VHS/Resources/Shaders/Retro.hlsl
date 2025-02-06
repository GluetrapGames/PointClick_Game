////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#pragma once

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

uniform TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
float4 _MainTex_TexelSize;

#define SAMPLE_MAIN(uv) SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uv)

float _Intensity;

int _Samples;
float _Vignette;
float _ColorNoise;
float _ChromaBand;
float _LumaBand;
float _TapeNoiseHigh;
float _TapeNoiseLow;
float _ACBeatVelocity;
float _ACBeatCount;
float _ACBeatStrength;
float _TapeCreaseStrength;
float _TapeCreaseVelocity;
float _TapeCreaseCount;
float _TapeCreaseNoise;
float _TapeCreaseDistortion;
float _BottomWarpHeight;
float _BottomWarpDistortion;
float _BottomWarpJitterExtent;
float3 _ShadowTint;
float3 _YIQ;
float _WhiteLevel;
float _BlackLevel;

#ifdef SURFACE_MODE
#define WIDTH _MainTex_TexelSize.z
#define HEIGHT _MainTex_TexelSize.w
#define WIDTH_HEIGHT _MainTex_TexelSize.zw
#else
#define WIDTH _ScreenParams.x
#define HEIGHT _ScreenParams.y
#define WIDTH_HEIGHT _ScreenParams.xy
#endif

#if SHADER_API_PS4
inline float2 lerp(float2 a, float2 b, float t) { return lerp(a, b, (float2)t); }
inline float3 lerp(float3 a, float3 b, float t) { return lerp(a, b, (float3)t); }
inline float4 lerp(float4 a, float4 b, float t) { return lerp(a, b, (float4)t); }
#endif

inline float mod(float x, float y)    { return x - y * floor(x / y); }
inline float2 mod(float2 a, float2 b) { return a - floor(a / b) * b; }
inline float3 mod(float3 a, float3 b) { return a - floor(a / b) * b; }
inline float4 mod(float4 a, float4 b) { return a - floor(a / b) * b; }

inline float Rand(const float c)
{
  return frac(sin(dot(float2(c, 1.0 - c), float2(12.9898, 78.233))) * 43758.5453);
}

inline float Rand(const float2 c)
{
  return frac(sin(dot(c, float2(12.9898, 78.233))) * 43758.5453);
}

inline float Trunc(float x, float num_levels)
{
  return floor(x * num_levels) / num_levels;
}

inline float2 Trunc(float2 x, float2 num_levels)
{
  return floor(x * num_levels) / num_levels;
}

/// Noise by Ian McEwan, Ashima Arts.
inline float3 mod289(const float3 x)  { return x - floor(x * (1.0 / 289.0)) * 289.0; }
inline float2 mod289(const float2 x)  { return x - floor(x * (1.0 / 289.0)) * 289.0; }
inline float3 permute(const float3 x) { return mod289(((x * 34.0) + 1.0) * x); }
inline float snoise(const float2 v)
{
  const float4 C = float4(0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439);
  
  float2 i  = floor(v + dot(v, C.yy) );
  float2 x0 = v -   i + dot(i, C.xx);

  float2 i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
  
  float4 x12 = x0.xyxy + C.xxzz;
  x12.xy -= i1;
  
  i = mod289(i);
  const float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0)) + i.x + float3(0.0, i1.x, 1.0));
  
  float3 m = max(0.5 - float3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
  m = m*m;
  m = m*m;

  const float3 x = 2.0 * frac(p * C.www) - 1.0;
  float3 h = abs(x) - 0.5;
  const float3 ox = floor(x + 0.5);
  float3 a0 = x - ox;
  
  m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);
  
  float3 g;
  g.x  = a0.x  * x0.x  + h.x  * x0.y;
  g.yz = a0.yz * x12.xz + h.yz * x12.yw;

  return 130.0 * dot(m, g);
}

float GoldNoise(const in float2 xy, const in float seed)
{
  return frac(sin(dot(xy * seed, float2(12.9898, 78.233))) * 43758.5453);
}

float4 Noise(const in float grainSize, const in bool monochromatic, in float2 fragCoord, float fps)
{
  float seed = fps > 0.0 ? floor(frac(_Time.y) * fps) / fps : _Time.y;
  seed += 1.0;

  UNITY_BRANCH
  if (grainSize > 1.0)
  {
    fragCoord.x = floor(fragCoord.x / grainSize);
    fragCoord.y = floor(fragCoord.y / grainSize);
  }

  fragCoord.x += 1.0;

  float r = GoldNoise(fragCoord, seed);    
  float g = monochromatic ? r : GoldNoise(fragCoord, seed + 1.0);
  float b = monochromatic ? r : GoldNoise(fragCoord, seed + 2.0);

  return float4(r, g, b, 1.0);
}

// BlendSoftLight credit to Jamie Owen: https://github.com/jamieowen/glsl-blend
float BlendSoftLight(float base, float blend) 
{
  return (blend<0.5)?(2.0*base*blend+base*base*(1.0-2.0*blend)):(sqrt(base)*(2.0*blend-1.0)+2.0*base*(1.0-blend));
}

float4 BlendSoftLight(float4 base, float4 blend) 
{
  return float4(BlendSoftLight(base.r,blend.r),BlendSoftLight(base.g,blend.g),BlendSoftLight(base.b,blend.b), 1.0);
}

float4 BlendSoftLight(const float4 base, const float4 blend, const float opacity)
{
  return (BlendSoftLight(base, blend) * opacity + base * (1.0 - opacity));
}

float3 ClipColor(in float3 c)
{
  const float l = Luminance(c);
  const float n = min(min(c.r, c.g), c.b);
  const float x = max(max(c.r, c.g), c.b);

  UNITY_BRANCH
  if (n < 0.0)
  {
    c.r = l + (((c.r - l) * l) / (l - n));
    c.g = l + (((c.g - l) * l) / (l - n));
    c.b = l + (((c.b - l) * l) / (l - n));
  }

  UNITY_BRANCH
  if (x > 1.0)
  {
    c.r = l + (((c.r - l) * (1.0 - l)) / (x - l));
    c.g = l + (((c.g - l) * (1.0 - l)) / (x - l));
    c.b = l + (((c.b - l) * (1.0 - l)) / (x - l));
  }

  return c;
}

inline float3 SetLum(const in float3 c, const in float l)
{
  return ClipColor(c + l - Luminance(c));
}      

inline float3 BlendLuminosity(const in float3 base, const in float3 blend)
{
  return SetLum(base, Luminance(blend));
}

inline float3 BlendColor(const in float3 base, const in float3 blend)
{
  return SetLum(blend, Luminance(base));
}

float3 Shrink(in float2 coord, const in float shrinkRatio)
{
  const float numBands = WIDTH * shrinkRatio;
  const float bandWidth = WIDTH / numBands;

  // How far are we along the band
  const float t = mod(coord.x, bandWidth) / bandWidth;

  // Sample current band in lower res
  coord.x = floor(coord.x * shrinkRatio) / shrinkRatio;
  float2 uv = coord / WIDTH_HEIGHT;

  const float3 colorA = SAMPLE_MAIN(uv).rgb;

  // Sample next band for interpolation
  uv.x += bandWidth * (1.0 / WIDTH);

  const float3 colorB = SAMPLE_MAIN(uv).rgb;

  return lerp(colorA, colorB, t);
}

inline float3 rgb2yiq(const in float3 rgb)
{
  return mul(float3x3(0.299, 0.596, 0.211, 0.587, -0.274, -0.523, 0.114, -0.322, 0.312), rgb);
}

inline float3 yiq2rgb(const in float3 yiq)
{
  return mul(float3x3(1.000, 1.000, 1.000, 0.956, -0.272, -1.106, 0.621, -0.647, 1.703), yiq);
}

#define BLACK float3(0.0, 0.0, 0.0)
#define WHITE float3(1.0, 1.0, 1.0)
float3 ClampLevels(in float3 pixel, const in float blackLevel, const in float whiteLevel)
{
  pixel = lerp(pixel, BLACK, 1.0 - whiteLevel);
  pixel = lerp(pixel, WHITE, blackLevel);

  return pixel;
}

float4 Saturation(float4 pixel, float adjustment)
{
  const float3 intensity = dot(pixel.rgb, float3(0.2126, 0.7152, 0.0722));

  return float4(lerp(intensity, pixel.rgb, adjustment), 1.0);
}

// An approximation of Photoshop's color balance > shadows
float4 TintShadows(float4 pixel, float3 color)
{
  const float POWER = 1.5;

  UNITY_BRANCH
  if (color.r > 0.0)
    pixel.r = lerp(pixel.r, 1.0 - pow(abs(pixel.r - 1.0), POWER), color.r);

  UNITY_BRANCH
  if (color.g > 0.0)
    pixel.g = lerp(pixel.g, 1.0 - pow(abs(pixel.g - 1.0), POWER), color.g);

  UNITY_BRANCH
  if (color.b > 0.0)
    pixel.b = lerp(pixel.b, 1.0 - pow(abs(pixel.b - 1.0), POWER), color.b);

  return pixel;
}

inline float2x2 Rotate2D(const in float t)
{
  return float2x2(cos(t), sin(t),
                 -sin(t), cos(t));
}

float3 SampleVHS(const in float2 uv, const in float rot)
{
  float3 color = float3(0.1, 0.1, 0.1);

  UNITY_BRANCH
  if (abs(uv.x - 0.5) < 0.5 && abs(uv.y - 0.5) < 0.5)
  {
    float3 yiq = (float3)0.0;

    const int iterationCount = min(_Samples, 10);
    [unroll]
    for (int i = 0; i < iterationCount; i++)
    {
      yiq += (rgb2yiq(SAMPLE_MAIN(uv - float2((float)i, 0.0) / _ScreenParams.xy).xyz) *
                float2((float)i, (float)(_Samples - 1 - i)).yxx / (float)(_Samples - 1)) / (float)_Samples * 2.0;
    }

    if (rot != 0.0)
      yiq.yz = mul(Rotate2D(rot), yiq.yz);

    color = yiq2rgb(yiq);
  }

  return color;
}

inline float3 Vignette(const in float2 uv)
{
  return pow(abs(uv.x * (1.0 - uv.x) * uv.y * (1.0 - uv.y)), _Vignette) * (1.0 + (_Vignette * 2.0));
}

float _Brightness;
float _Contrast;
float _Gamma;
float _Hue;
float _Saturation;

inline float3 ColorAdjust(float3 pixel)
{
  pixel = max(0.0, (pixel - (float3)0.5) * _Contrast + (float3)0.5 + _Brightness);

  float3 hsv = RgbToHsv(pixel);
  hsv.x += _Hue;
  pixel = HsvToRgb(hsv);

  pixel = pow(abs(pixel), _Gamma);

  float luma = Luminance(pixel);
  pixel = luma.xxx + _Saturation * (pixel - luma.xxx);

  return max(0.0, pixel);
}

struct VertexInput
{
  float4 positionOS : POSITION;
  float2 texcoord   : TEXCOORD0;
  UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput
{
  half4  positionCS   : SV_POSITION;
  half4  uv           : TEXCOORD0;
  UNITY_VERTEX_OUTPUT_STEREO
};

VertexOutput RetroVert(VertexInput input)
{
  VertexOutput output;
  UNITY_SETUP_INSTANCE_ID(input);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

  output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

  float4 projPos = output.positionCS * 0.5;
  projPos.xy = projPos.xy + projPos.w;

  output.uv.xy = UnityStereoTransformScreenSpaceTex(input.texcoord);
  output.uv.zw = projPos.xy;

  return output;
}
