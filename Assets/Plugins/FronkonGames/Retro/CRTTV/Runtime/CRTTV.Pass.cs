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
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FronkonGames.Retro.CRTTV
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class CRTTV
  {
    private sealed class RenderPass : ScriptableRenderPass
    {
      private readonly Settings settings;

      private RenderTargetIdentifier colorBuffer;
      private RenderTextureDescriptor renderTextureDescriptor;

#if UNITY_2022_1_OR_NEWER
      RTHandle renderTextureHandle0;

      private readonly ProfilingSampler profilingSamples = new(Constants.Asset.AssemblyName);
      private ProfilingScope profilingScope;
#else
      private int renderTextureHandle0;
#endif
      private readonly Material material;

      private static readonly ProfilerMarker ProfilerMarker = new($"{Constants.Asset.AssemblyName}.Pass.Execute");

      private const string CommandBufferName = Constants.Asset.AssemblyName;

      private static class ShaderIDs
      {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");

        public static readonly int Frame = Shader.PropertyToID("_Frame");
        public static readonly int ShadowmaskStrength = Shader.PropertyToID("_ShadowmaskStrength");
        public static readonly int ShadowmaskLuminosity = Shader.PropertyToID("_ShadowmaskLuminosity");
        public static readonly int ShadowmaskScale = Shader.PropertyToID("_ShadowmaskScale");
        public static readonly int ShadowmaskColorOffset = Shader.PropertyToID("_ShadowmaskColorOffset");
        public static readonly int ShadowmaskVerticalGapHardness = Shader.PropertyToID("_ShadowmaskVerticalGapHardness");
        public static readonly int ShadowmaskHorizontalGapHardness = Shader.PropertyToID("_ShadowmaskHorizontalGapHardness");
        public static readonly int FishEyeStrength = Shader.PropertyToID("_FishEyeStrength");
        public static readonly int FishEyeZoom = Shader.PropertyToID("_FishEyeZoom");
        public static readonly int Distortion = Shader.PropertyToID("_Distortion");
        public static readonly int DistortionSpeed = Shader.PropertyToID("_DistortionSpeed");
        public static readonly int DistortionAmplitude = Shader.PropertyToID("_DistortionAmplitude");
        public static readonly int VignetteSmoothness = Shader.PropertyToID("_VignetteSmoothness");
        public static readonly int VignetteRounding = Shader.PropertyToID("_VignetteRounding");
        public static readonly int VignetteBorders = Shader.PropertyToID("_VignetteBorders");
        public static readonly int ShineStrength = Shader.PropertyToID("_ShineStrength");
        public static readonly int ShineColor = Shader.PropertyToID("_ShineColor");
        public static readonly int ShinePosition = Shader.PropertyToID("_ShinePosition");
        public static readonly int RGBOffsetStrength = Shader.PropertyToID("_RGBOffsetStrength");
        public static readonly int ColorBleedingStrength = Shader.PropertyToID("_ColorBleedingStrength");
        public static readonly int ColorBleedingDistance = Shader.PropertyToID("_ColorBleedingDistance");
        public static readonly int ColorCurves = Shader.PropertyToID("_ColorCurves");
        public static readonly int GammaColor = Shader.PropertyToID("_GammaColor");
        public static readonly int Scanlines = Shader.PropertyToID("_Scanlines");
        public static readonly int ScanlinesCount = Shader.PropertyToID("_ScanlinesCount");
        public static readonly int ScanlinesVelocity = Shader.PropertyToID("_ScanlinesVelocity");
        public static readonly int InterferenceStrength = Shader.PropertyToID("_InterferenceStrength");
        public static readonly int InterferencePeakStrength = Shader.PropertyToID("_InterferencePeakStrength");
        public static readonly int InterferencePeakPosition = Shader.PropertyToID("_InterferencePeakPosition");
        public static readonly int ShakeStrength = Shader.PropertyToID("_ShakeStrength");
        public static readonly int ShakeRate = Shader.PropertyToID("_ShakeRate");
        public static readonly int MovementStrength = Shader.PropertyToID("_MovementStrength");
        public static readonly int MovementRate = Shader.PropertyToID("_MovementRate");
        public static readonly int MovementSpeed = Shader.PropertyToID("_MovementSpeed");
        public static readonly int Grain = Shader.PropertyToID("_Grain");
        public static readonly int StaticNoise = Shader.PropertyToID("_StaticNoise");
        public static readonly int BarStrength = Shader.PropertyToID("_BarStrength");
        public static readonly int BarHeight = Shader.PropertyToID("_BarHeight");
        public static readonly int BarSpeed = Shader.PropertyToID("_BarSpeed");
        public static readonly int BarOverflow = Shader.PropertyToID("_BarOverflow");

        public static readonly int FlickerStrength = Shader.PropertyToID("_FlickerStrength");
        public static readonly int FlickerSpeed = Shader.PropertyToID("_FlickerSpeed");
        
        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");      
      }
      
      /// <summary> Render pass constructor. </summary>
      public RenderPass(Settings settings)
      {
        this.settings = settings;

        string shaderPath = $"Shaders/{Constants.Asset.ShaderName}_URP";
        Shader shader = Resources.Load<Shader>(shaderPath);
        if (shader == null)
          shader = Resources.Load<Shader>($"Shaders/{Constants.Asset.ShaderName}_URP");

        if (shader == null)
          Log.Error($"Shader 'Resources/Shaders/{Constants.Asset.ShaderName}_URP' not found");
        else if (shader.isSupported == false)
          Log.Error($"Shader 'Resources/Shaders/{Constants.Asset.ShaderName}_URP' not supported");
        else
          material = CoreUtils.CreateEngineMaterial(shader);
      }

      /// <inheritdoc/>
      public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
      {
        renderTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        renderTextureDescriptor.depthBufferBits = 0;

#if UNITY_2022_1_OR_NEWER
        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;

        RenderingUtils.ReAllocateIfNeeded(ref renderTextureHandle0, renderTextureDescriptor, settings.filterMode, TextureWrapMode.Clamp, false, 1, 0, $"_RTHandle0_{Constants.Asset.Name}");
#else
        colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;

        renderTextureHandle0 = Shader.PropertyToID($"_RTHandle0_{Constants.Asset.Name}");
        cmd.GetTemporaryRT(renderTextureHandle0, renderTextureDescriptor.width, renderTextureDescriptor.height, renderTextureDescriptor.depthBufferBits, settings.filterMode, RenderTextureFormat.ARGB32);
#endif
      }

      /// <inheritdoc/>
      public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
      {
        if (material == null ||
            renderingData.postProcessingEnabled == false ||
            settings.intensity == 0.0f ||
            settings.affectSceneView == false && renderingData.cameraData.isSceneViewCamera == true)
          return;

        CommandBuffer cmd = CommandBufferPool.Get(CommandBufferName);

        if (settings.enableProfiling == true)
#if UNITY_2022_1_OR_NEWER
          profilingScope = new ProfilingScope(cmd, profilingSamples);
#else
          ProfilerMarker.Begin();
#endif    

        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, settings.intensity);

        material.SetInt(ShaderIDs.Frame, Time.frameCount);
        material.SetFloat(ShaderIDs.ShadowmaskStrength, settings.shadowmaskStrength);
        material.SetFloat(ShaderIDs.ShadowmaskLuminosity, settings.shadowmaskLuminosity * 300.0f);
        material.SetFloat(ShaderIDs.ShadowmaskScale, settings.shadowmaskScale);
        material.SetVector(ShaderIDs.ShadowmaskColorOffset, settings.shadowmaskColorOffset);
        material.SetFloat(ShaderIDs.ShadowmaskVerticalGapHardness, settings.shadowmaskVerticalGapHardness);
        material.SetFloat(ShaderIDs.ShadowmaskHorizontalGapHardness, settings.shadowmaskHorizontalGapHardness);
        material.SetFloat(ShaderIDs.FishEyeStrength, settings.fishEyeStrength);
        material.SetVector(ShaderIDs.FishEyeZoom, settings.fishEyeZoom);
        material.SetFloat(ShaderIDs.Distortion, settings.distortion * 0.01f);
        material.SetFloat(ShaderIDs.DistortionSpeed, settings.distortionSpeed);
        material.SetFloat(ShaderIDs.DistortionAmplitude, settings.distortionAmplitude * 10.0f);
        material.SetFloat(ShaderIDs.VignetteSmoothness, settings.vignetteSmoothness);
        material.SetFloat(ShaderIDs.VignetteRounding, settings.vignetteRounding);
        material.SetVector(ShaderIDs.VignetteBorders, settings.vignetteBorders);
        material.SetFloat(ShaderIDs.ShineStrength, settings.shineStrength);
        material.SetColor(ShaderIDs.ShineColor, settings.shineColor);
        material.SetVector(ShaderIDs.ShinePosition, settings.shinePosition);
        material.SetFloat(ShaderIDs.RGBOffsetStrength, settings.rgbOffsetStrength);
        material.SetFloat(ShaderIDs.ColorBleedingStrength, settings.colorBleedingStrength);
        material.SetFloat(ShaderIDs.ColorBleedingDistance, settings.colorBleedingDistance);
        material.SetColor(ShaderIDs.ColorCurves, settings.colorCurves);
        material.SetVector(ShaderIDs.GammaColor, settings.gammaColor);
        material.SetFloat(ShaderIDs.Scanlines, settings.scanlines);
        material.SetFloat(ShaderIDs.ScanlinesCount, settings.scanlinesCount);
        material.SetFloat(ShaderIDs.ScanlinesVelocity, settings.scanlinesVelocity);
        material.SetFloat(ShaderIDs.InterferenceStrength, settings.interferenceStrength);
        material.SetFloat(ShaderIDs.InterferencePeakStrength, settings.interferencePeakStrength);
        material.SetFloat(ShaderIDs.InterferencePeakPosition, settings.interferencePeakPosition);
        material.SetFloat(ShaderIDs.ShakeStrength, settings.shakeStrength);
        material.SetFloat(ShaderIDs.ShakeRate, settings.shakeRate);
        material.SetFloat(ShaderIDs.MovementStrength, settings.movementStrength);
        material.SetFloat(ShaderIDs.MovementRate, settings.movementRate);
        material.SetFloat(ShaderIDs.MovementSpeed, settings.movementSpeed);
        material.SetFloat(ShaderIDs.Grain, settings.grain);
        material.SetFloat(ShaderIDs.StaticNoise, settings.staticNoise);
        material.SetFloat(ShaderIDs.BarStrength, settings.barStrength);
        material.SetFloat(ShaderIDs.BarHeight, settings.barHeight);
        material.SetFloat(ShaderIDs.BarSpeed, settings.barSpeed);
        material.SetFloat(ShaderIDs.BarOverflow, settings.barOverflow);
        
        material.SetFloat(ShaderIDs.FlickerStrength, settings.flickerStrength);
        material.SetFloat(ShaderIDs.FlickerSpeed, settings.flickerSpeed);

        material.SetFloat(ShaderIDs.Brightness, settings.brightness);
        material.SetFloat(ShaderIDs.Contrast, settings.contrast);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / settings.gamma);
        material.SetFloat(ShaderIDs.Hue, settings.hue);
        material.SetFloat(ShaderIDs.Saturation, settings.saturation);

#if UNITY_2022_1_OR_NEWER
        cmd.Blit(colorBuffer, renderTextureHandle0, material);
        cmd.Blit(renderTextureHandle0, colorBuffer);
#else
        Blit(cmd, colorBuffer, renderTextureHandle0, material);
        Blit(cmd, renderTextureHandle0, colorBuffer);
#endif

        if (settings.enableProfiling == true)
#if UNITY_2022_1_OR_NEWER
          profilingScope.Dispose();
#else
          ProfilerMarker.End();
#endif

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
      }
    }    
  }
}
