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

namespace FronkonGames.Retro.VHS
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class VHS
  {
    private sealed class RenderPass : ScriptableRenderPass
    {
      private readonly Settings settings;

      private RenderTargetIdentifier colorBuffer;
      private RenderTextureDescriptor renderTextureDescriptor;

#if UNITY_2022_1_OR_NEWER
      RTHandle renderTextureHandle0, renderTextureHandle1;

      private readonly ProfilingSampler profilingSamples = new(Constants.Asset.AssemblyName);
      private ProfilingScope profilingScope;
#else
      private int renderTextureHandle0, renderTextureHandle1;
#endif
      private readonly Material material;

      private static readonly ProfilerMarker ProfilerMarker = new($"{Constants.Asset.AssemblyName}.Pass.Execute");

      private const string CommandBufferName = Constants.Asset.AssemblyName;

      private readonly Texture2D noiseTexture;
      
      private static class ShaderIDs
      {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");

        public static readonly int Samples = Shader.PropertyToID("_Samples");
        public static readonly int Vignette = Shader.PropertyToID("_Vignette");
        public static readonly int ShadowTint = Shader.PropertyToID("_ShadowTint");
        public static readonly int WhiteLevel = Shader.PropertyToID("_WhiteLevel");
        public static readonly int BlackLevel = Shader.PropertyToID("_BlackLevel");
        public static readonly int ColorNoise = Shader.PropertyToID("_ColorNoise");
        public static readonly int ChromaBand = Shader.PropertyToID("_ChromaBand");
        public static readonly int LumaBand = Shader.PropertyToID("_LumaBand");
        public static readonly int TapeNoiseHigh = Shader.PropertyToID("_TapeNoiseHigh");
        public static readonly int TapeNoiseLow = Shader.PropertyToID("_TapeNoiseLow");
        public static readonly int ACBeatVelocity = Shader.PropertyToID("_ACBeatVelocity");
        public static readonly int ACBeatCount = Shader.PropertyToID("_ACBeatCount");
        public static readonly int ACBeatStrength = Shader.PropertyToID("_ACBeatStrength");
        public static readonly int TapeCreaseStrength = Shader.PropertyToID("_TapeCreaseStrength");
        public static readonly int TapeCreaseVelocity = Shader.PropertyToID("_TapeCreaseVelocity");
        public static readonly int TapeCreaseCount = Shader.PropertyToID("_TapeCreaseCount");
        public static readonly int TapeCreaseNoise = Shader.PropertyToID("_TapeCreaseNoise");
        public static readonly int TapeCreaseDistortion = Shader.PropertyToID("_TapeCreaseDistortion");
        public static readonly int BottomWarpHeight = Shader.PropertyToID("_BottomWarpHeight");
        public static readonly int BottomWarpDistortion = Shader.PropertyToID("_BottomWarpDistortion");
        public static readonly int BottomWarpJitterExtent = Shader.PropertyToID("_BottomWarpJitterExtent");
        public static readonly int YIQ = Shader.PropertyToID("_YIQ");

        public static readonly int NoiseTexture = Shader.PropertyToID("_NoiseTex");
        
        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");      
      }

      private static class Keywords
      {
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass(Settings settings)
      {
        this.settings = settings;

        string shaderPath = $"Shaders/{Constants.Asset.ShaderName}_URP";
        Shader shader = Resources.Load<Shader>(shaderPath);
        if (shader != null && shader.isSupported == true)
        {
          material = CoreUtils.CreateEngineMaterial(shader);
          
          if (noiseTexture == null)
            noiseTexture = Resources.Load<Texture2D>("Textures/Noise");          
        }
      }

      /// <inheritdoc/>
      public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
      {
        renderTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        renderTextureDescriptor.depthBufferBits = 0;

#if UNITY_2022_1_OR_NEWER
        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;

        RenderingUtils.ReAllocateIfNeeded(ref renderTextureHandle0, renderTextureDescriptor, settings.filterMode, TextureWrapMode.Clamp, false, 1, 0, $"_RTHandle0_{Constants.Asset.Name}");
        RenderingUtils.ReAllocateIfNeeded(ref renderTextureHandle1, renderTextureDescriptor, settings.filterMode, TextureWrapMode.Clamp, false, 1, 0, $"_RTHandle1_{Constants.Asset.Name}");
#else
        colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;

        renderTextureHandle0 = Shader.PropertyToID($"_RTHandle0_{Constants.Asset.Name}");
        cmd.GetTemporaryRT(renderTextureHandle0, renderTextureDescriptor.width, renderTextureDescriptor.height, renderTextureDescriptor.depthBufferBits, settings.filterMode, RenderTextureFormat.ARGB32);
        
        renderTextureHandle1 = Shader.PropertyToID($"_RTHandle1_{Constants.Asset.Name}");
        cmd.GetTemporaryRT(renderTextureHandle1, renderTextureDescriptor.width, renderTextureDescriptor.height, renderTextureDescriptor.depthBufferBits, settings.filterMode, RenderTextureFormat.ARGB32);
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

        material.SetFloat(ShaderIDs.Brightness, settings.brightness);
        material.SetFloat(ShaderIDs.Contrast, settings.contrast);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / settings.gamma);
        material.SetFloat(ShaderIDs.Hue, settings.hue);
        material.SetFloat(ShaderIDs.Saturation, settings.saturation);

        material.SetFloat(ShaderIDs.Vignette, settings.vignette);
        material.SetColor(ShaderIDs.ShadowTint, settings.shadowTint);
        material.SetFloat(ShaderIDs.WhiteLevel, settings.whiteLevel);
        material.SetFloat(ShaderIDs.BlackLevel, settings.blackLevel);
        material.SetFloat(ShaderIDs.ColorNoise, settings.colorNoise);
        material.SetFloat(ShaderIDs.ChromaBand, settings.chromaBand > 0 ? 1.0f / settings.chromaBand : 1.0f);
        material.SetFloat(ShaderIDs.LumaBand, settings.lumaBand > 0 ? 1.0f / settings.lumaBand : 1.0f);
        material.SetFloat(ShaderIDs.TapeNoiseHigh, settings.tapeNoiseHigh);
        material.SetFloat(ShaderIDs.TapeNoiseLow, settings.tapeNoiseLow);
        material.SetFloat(ShaderIDs.ACBeatStrength, settings.acBeatStrength);
        material.SetFloat(ShaderIDs.ACBeatVelocity, settings.acBeatVelocity);
        material.SetFloat(ShaderIDs.ACBeatCount, settings.acBeatCount);
        material.SetFloat(ShaderIDs.TapeCreaseStrength, settings.tapeCreaseStrength);
        material.SetFloat(ShaderIDs.TapeCreaseVelocity, settings.tapeCreaseVelocity);
        material.SetFloat(ShaderIDs.TapeCreaseCount, settings.tapeCreaseCount);
        material.SetFloat(ShaderIDs.TapeCreaseNoise, settings.tapeCreaseNoise);
        material.SetFloat(ShaderIDs.BottomWarpHeight, settings.bottomWarpHeight);
        material.SetFloat(ShaderIDs.BottomWarpDistortion, settings.bottomWarpDistortion * 1000.0f);
        material.SetFloat(ShaderIDs.BottomWarpJitterExtent, settings.bottomWarpJitterExtent);
        material.SetVector(ShaderIDs.YIQ, settings.yiq);
        
        RenderTextureDescriptor textureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        switch (settings.resolution)
        {
          case Resolution.Half:
            textureDescriptor.width = Screen.width / 2;
            textureDescriptor.height = Screen.height / 2;
            break;
          case Resolution.Quarter:
            textureDescriptor.width = Screen.width / 4;
            textureDescriptor.height = Screen.height / 4;
            break;
          case Resolution.Eighth:
            textureDescriptor.width = Screen.width / 8;
            textureDescriptor.height = Screen.height / 8;
            break;
          case Resolution.Sixteenth:
            textureDescriptor.width = Screen.width / 16;
            textureDescriptor.height = Screen.height / 16;
            break;
        }
        
        if (settings.quality == Quality.HighFidelity)
        {
          material.SetInt(ShaderIDs.Samples, settings.samples);
          material.SetFloat(ShaderIDs.TapeCreaseDistortion, settings.tapeCreaseDistortion);

          material.SetTexture(ShaderIDs.NoiseTexture, noiseTexture);

#if UNITY_2022_1_OR_NEWER
          cmd.Blit(colorBuffer, renderTextureHandle0, material, 1);
          cmd.Blit(renderTextureHandle0, renderTextureHandle1, material, 2);
          cmd.Blit(renderTextureHandle1, colorBuffer);
#else
          Blit(cmd, colorBuffer, renderTextureHandle0, material, 1);
          Blit(cmd, renderTextureHandle0, renderTextureHandle1, material, 2);
          Blit(cmd, renderTextureHandle1, colorBuffer);
#endif
        }
        else
        {
#if UNITY_2022_1_OR_NEWER
          cmd.Blit(colorBuffer, renderTextureHandle0, material);
          cmd.Blit(renderTextureHandle0, colorBuffer);
#else
          Blit(cmd, colorBuffer, renderTextureHandle0, material);
          Blit(cmd, renderTextureHandle0, colorBuffer);
#endif
        }

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
