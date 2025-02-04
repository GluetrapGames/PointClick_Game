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
Shader "Fronkon Games/Retro/VHS"
{
  Properties
  {
    _MainTex("Albedo", 2D) = "white" {}
    _BaseColor("Color", Color) = (1, 1, 1, 1)

    [Space]
    _Intensity("Intensity", Range(0.0, 1.0)) = 1.0
    _Samples("Samples", Range(2, 10)) = 6
    _EmissionStrength("Emission", Range(0.0, 2.0)) = 0.5

    [Header(Tape distortion)]
    _TapeNoiseHigh("Tape noise high", Range(0.0, 1.0)) = 0.1
    _TapeNoiseLow("Low", Range(0.0, 1.0)) = 0.1
    _ColorNoise("Tape noise", Range(0.0, 1.0)) = 0.1

    [Header(Tape crease)]
    _TapeCreaseStrength("Tape crease", Range(0.0, 1.0)) = 1.0
    _TapeCreaseCount("Count", Range(0, 50)) = 8
    _TapeCreaseVelocity("Velocity", Range(-5.0, 5.0)) = 1.2
    _TapeCreaseNoise("Noise", Range(0.0, 1.0)) = 0.7
    _TapeCreaseDistortion("Distortion", Range(0.0, 1.0)) = 0.2

    [Header(Bottom warp)]
    _BottomWarpHeight("Bottom warp height", Range(0, 100)) = 15
    _BottomWarpDistortion("Distortion", Range(-1.0, 1.0)) = 0.1
    _BottomWarpJitterExtent("Jitter extent", Range(0, 100)) = 50

    [Header(AC beat)]
    _ACBeatStrength("AC beat", Range(0.0, 1.0)) = 0.1
    _ACBeatCount("Count", Range(0.0, 1.0)) = 0.1
    _ACBeatVelocity("Velocity", Range(-1.0, 1.0)) = 0.2

    [Space]
    _YIQ("YIQ color space", Vector) = (0.9, 1.1, 1.5)

    [Header(Color levels)]
    _WhiteLevel("White", Range(0.0, 1.0)) = 1.0
    _BlackLevel("Black", Range(0.0, 1.0)) = 0.0

    [Space]
    _ShadowTint("Shadow tint", Color) = (0.7, 0.0, 0.9, 1.0)

    [Space]
    _Vignette("Vignette", Range(0.0, 1.0)) = 0.25

    [Header(Color)]
    _Brightness("Brightness", Range(-1.0, 1.0)) = 0.0
    _Contrast("Contrast", Range(0.0, 10.0)) = 1.0
    _Gamma("Gamma", Range(0.1, 10.0)) = 1.0
    _Hue("HUE", Range(0.0, 1.0)) = 0.0
    _Saturation("Saturation", Range(0.0, 2.0)) = 1.0
  }

  SubShader
  {
    Tags
    {
      "RenderPipeline" = "UniversalPipeline"
      "RenderType" = "Opaque"
      "Queue" = "Geometry"
    }
    LOD 100

    Pass
    {
      Name "Universal Forward"
      Tags
      {
        "LightMode" = "UniversalForward"
      }

      Cull Back
      Blend One Zero
      ZWrite On
      ZTest LEqual
      
      HLSLPROGRAM
      #pragma prefer_hlslcc gles
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_instancing      
      #pragma vertex vert
      #pragma fragment frag

      #define SURFACE_MODE
      #include "Retro.hlsl"

      CBUFFER_START(UnityPerMaterial)
      float4 _MainTex_ST;
      half4 _BaseColor;      
      CBUFFER_END

      float _EmissionStrength;

      struct Attributes
      {
        float3 positionOS : POSITION;
        float3 normalOS   : NORMAL;
        float2 uv0        : TEXCOORD;
      };

      struct Interpolators
      {
        float4 positionCS : SV_POSITION;
        float2 uv0        : TEXCOORD0;
        float3 normalWS   : TEXCOORD1;
        float fogCoord    : TEXCOORD2;
      };

      Interpolators vert(Attributes input)
      {
        Interpolators output = (Interpolators)0;
        const VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS);
        const VertexNormalInputs normalInputs = GetVertexNormalInputs(input.positionOS);

        output.positionCS = positionInputs.positionCS;
        output.uv0 = TRANSFORM_TEX(input.uv0, _MainTex);
        output.normalWS = normalInputs.normalWS;
        output.fogCoord = ComputeFogFactor(positionInputs.positionCS.z);

        return output;
      }

      inline float3 SampleNoise(const in float2 uv)
      {
        return Rand(uv);
      }

      half4 frag(const Interpolators input) : SV_Target
      {
        const half4 color = SAMPLE_MAIN(input.uv0);
        half4 pixel = color;
        float2 uvn = input.uv0;

        InputData lightingInput = (InputData)0;
        lightingInput.normalWS = input.normalWS;

        // Tape wave.
        uvn.x += (SampleNoise(float2(uvn.y / 10.0, _Time.y / 10.0) / 1.0).x - 0.5) / WIDTH * _TapeNoiseLow * 10.0;
        uvn.x += (SampleNoise(float2(uvn.y, _Time.y * 1.0)).x - 0.5) / WIDTH * _TapeNoiseHigh * 10.0;

        // Tape crease.
        const float tcPhase = smoothstep(0.9, 0.96, sin(uvn.y * _TapeCreaseCount - (_Time.y + 0.14 * SampleNoise(_Time.y * float2(0.67, 0.59)).x) * PI * _TapeCreaseVelocity)) * _TapeCreaseStrength;
        const float tcNoise = smoothstep(0.3, 1.0, SampleNoise(float2(uvn.y * 4.77, _Time.y)).x);
        float tc = tcPhase * tcNoise;
        uvn.x = uvn.x - tc / WIDTH * 8.0;

        // Switching noise.
        const float snPhase = smoothstep(6.0 / HEIGHT, 0.0, uvn.y);
        uvn.y += snPhase * 0.3;
        uvn.x += snPhase * ((SampleNoise(float2(input.uv0.y * 100.0, _Time.y * 10.0)).x - 0.5) / WIDTH * 24.0);

        // Wrap bottom.
        const float uvHeight = _BottomWarpHeight / HEIGHT;
        UNITY_BRANCH
        if (input.uv0.y <= uvHeight)
        {
          const float offsetUV = (input.uv0.y / uvHeight) * (_BottomWarpDistortion / WIDTH);
          const float jitterUV = (GoldNoise(float2(500.0, 500.0), frac(_Time.y)) * _BottomWarpJitterExtent) / WIDTH; 
        
          uvn = float2(input.uv0.x - offsetUV - jitterUV, input.uv0.y);
        }

        pixel.rgb = SampleVHS(uvn, tcPhase * _TapeCreaseDistortion + snPhase * 2.0);

        // Crease noise.
        const float cn = tcNoise * (0.3 + _TapeCreaseNoise * tcPhase);
        UNITY_BRANCH
        if (0.3 < cn)
        {
          const float2 V = float2(0.0, 1.0);
          const float2 uvt = (uvn + V.yx * SampleNoise(float2(uvn.y, _Time.y)).x) * float2(0.1, 1.0);
          const float n0 = SampleNoise(uvt).x;
          const float n1 = SampleNoise(uvt + V.yx / WIDTH).x;
          if (n1 < n0)
            pixel.rgb = lerp(pixel.rgb, 2.0 * V.yyy, pow(n0, 10.0));
        }

        // AC beat.
        pixel.rgb *= 1.0 + _ACBeatStrength * smoothstep(0.4, 0.6, SampleNoise(float2(0.0, _ACBeatCount * (input.uv0.y + _Time.y * _ACBeatVelocity)) / 10.0).x);

        // Color noise.
        pixel.rgb *= (1.0 - _ColorNoise) + _ColorNoise * SampleNoise(mod(uvn * float2(1.0, 1.0) + _Time.y * float2(5.97, 4.45), (float2)1.0));
        pixel.rgb = saturate(pixel.rgb);

        // YIQ space color.
        pixel.rgb = rgb2yiq(pixel.rgb);
        pixel.rgb = float3(0.1, -0.1, 0.0) + _YIQ * pixel.rgb;
        pixel.rgb = yiq2rgb(pixel.rgb);

        pixel.rgb = ClampLevels(pixel.rgb, _BlackLevel, _WhiteLevel);

        pixel = TintShadows(pixel, _ShadowTint);

        // Vignette.
        pixel.rgb *= Vignette(input.uv0);

        pixel.rgb = ColorAdjust(pixel.rgb);

        pixel = lerp(color, pixel, _Intensity);

        pixel.rgb = MixFogColor(pixel.rgb, half3(1.0, 1.0, 1.0), input.fogCoord);

        SurfaceData surfaceInput = (SurfaceData)0;
        surfaceInput.albedo = pixel.rgb;
        surfaceInput.emission = pixel.rgb * _EmissionStrength;

        surfaceInput.alpha = pixel.a;

        return UniversalFragmentBlinnPhong(lightingInput, surfaceInput);
      }
      ENDHLSL
    }

    UsePass "Universal Render Pipeline/Lit/DepthOnly"
    UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    UsePass "Universal Render Pipeline/Simple Lit/Meta"
  }
  
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
  CustomEditor "FronkonGames.Retro.VHS.Editor.VHSSurfaceGUI"
}

