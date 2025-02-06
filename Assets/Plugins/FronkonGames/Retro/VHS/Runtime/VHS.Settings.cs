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
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace FronkonGames.Retro.VHS
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Settings. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class VHS
  {
    /// <summary>
    /// Qualities.
    /// </summary>
    public enum Quality
    {
      /// <summary>
      /// Better performance but worse graphic quality. Recommended for old mobile devices.
      /// </summary>
      Performant,
      
      /// <summary> More similar to a real VHS. Default option. </summary>
      HighFidelity,
    }

    /// <summary>
    /// Resolutions.
    /// </summary>
    public enum Resolution
    {
      /// <summary> Same resolution. </summary>
      Same,
      
      /// <summary> Half. </summary>
      Half,
      
      /// <summary> One quarter. Default option. </summary>
      Quarter,
      
      /// <summary> One eighth. </summary>
      Eighth,
      
      /// <summary> One sixteenth. </summary>
      Sixteenth
    }
    
    /// <summary>
    /// Settings.
    /// </summary>
    [Serializable]
    public sealed class Settings
    {
      public Settings() => ResetDefaultValues();

#region Common settings.
      /// <summary> Controls the intensity of the effect [0, 1]. Default 1. </summary>
      /// <remarks> An effect with Intensity equal to 0 will not be executed. </remarks>
      public float intensity = 1.0f;
#endregion

#region VHS settings.
      /// <summary> Shader quality. Default HighFidelity. </summary>
      public Quality quality = Quality.HighFidelity;

      /// <summary> Number of samples used during calculations in YIQ color space [2, 10]. Default 6. </summary>
      /// <remarks> Only available in HighFidelity quality. </remarks>
      public int samples = 6;
      
      /// <summary> Final image resolution. Default Quarter (1/4). </summary>
      public Resolution resolution = Resolution.Quarter;

      /// <summary> YIQ color space (x: luma, y: in-phase, z: quadrature). Check https://en.wikipedia.org/wiki/YIQ </summary>
      public Vector3 yiq = DefaultYIQ;
      
      /// <summary> Shadow tint color. </summary>
      public Color shadowTint = DefaultShadowTint;

      /// <summary> Color levels (white) [0, 1]. Default 1. </summary>
      public float whiteLevel = 1.0f;
      
      /// <summary> Color levels (black) [0, 1]. Default 0. </summary>
      public float blackLevel = 0.0f;

      /// <summary> Noise band that also deforms the color [0, 1]. Default 1. </summary>
      public float tapeCreaseStrength = 1.0f;

      /// <summary> Number of bands [0, 50]. Default 8. </summary>
      public float tapeCreaseCount = 8.0f;

      /// <summary> Band speed [-5, 5]. Default 1.2. </summary>
      public float tapeCreaseVelocity = 1.2f;
      
      /// <summary> Band noise [0, 1]. Default 0.7. </summary>
      public float tapeCreaseNoise = 0.7f;
      
      /// <summary> Band color distortion [0, 1]. Default 0.2. </summary>
      /// <remarks> Only available in HighFidelity quality. </remarks>
      public float tapeCreaseDistortion = 0.2f;
      
      /// <summary> Tape noise [0, 1]. Default 0.1. </summary>
      public float colorNoise = 0.1f;

      /// <summary> In VHS chroma band is a much lower resolution (technically 1/16th) [1, 64]. Default 16. </summary>
      public int chromaBand = 16;
      
      /// <summary> In VHS the luma band is half of the resolution [1, 16]. Default 2. </summary>
      public int lumaBand = 2;

      /// <summary> Tape distortion (high frequency) [0, 1]. Default 0.1. </summary>
      public float tapeNoiseHigh = 0.1f;
      
      /// <summary> Tape distortion (low frequency) [0, 1]. Default 0.1. </summary>
      public float tapeNoiseLow = 0.1f;

      /// <summary> Amount of AC interferrences [0, 1]. Default 0.1. </summary>
      public float acBeatStrength = 0.1f;

      /// <summary> AC interferrences density [0, 1]. Default 0.1. </summary>
      public float acBeatCount = 0.1f;
      
      /// <summary> AC interferrences velocity [-1, 1]. Default 0.2. </summary>
      public float acBeatVelocity = 0.2f;

      /// <summary> 'Head-switching' noise height [0, 100]. Default 15. </summary>
      public float bottomWarpHeight = 15.0f;
      
      /// <summary> Distortion strength [-1, 1]. Default 0.1. </summary>
      public float bottomWarpDistortion = 0.1f;
      
      /// <summary> Extra noise [0, 100]. Default 50. </summary>
      public float bottomWarpJitterExtent = 50.0f;

      /// <summary> Vignette effect strength [0, 1]. Default 0.25. </summary>
      public float vignette = 0.25f;
#endregion

#region Color settings.
      /// <summary> Brightness [-1.0, 1.0]. Default 0. </summary>
      public float brightness = 0.0f;

      /// <summary> Contrast [0.0, 10.0]. Default 1. </summary>
      public float contrast = 1.0f;

      /// <summary>Gamma [0.1, 10.0]. Default 1. </summary>      
      public float gamma = 1.0f;

      /// <summary> The color wheel [0.0, 1.0]. Default 0. </summary>
      public float hue = 0.0f;

      /// <summary> Intensity of a colors [0.0, 2.0]. Default 1. </summary>      
      public float saturation = 1.0f;
#endregion

#region Advanced settings.
      /// <summary> Does it affect the Scene View? </summary>
      public bool affectSceneView = false;

      /// <summary> Filter mode. Default Bilinear. </summary>
      public FilterMode filterMode = FilterMode.Bilinear;
      
      /// <summary> Render pass injection. Default BeforeRenderingPostProcessing. </summary>
      public RenderPassEvent whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;
      
      /// <summary> Enable render pass profiling </summary>
      public bool enableProfiling = false;
#endregion

      public static Vector3 DefaultYIQ = new(0.9f, 1.1f, 1.5f);

      public static Color DefaultShadowTint = new(0.7f, 0.0f, 0.9f);

      /// <summary> Reset to default values. </summary>
      public void ResetDefaultValues()
      {
        intensity = 1.0f;

        samples = 6;
        shadowTint = DefaultShadowTint;
        vignette = 0.25f;
        colorNoise = 0.1f;
        whiteLevel = 1.0f;
        blackLevel = 0.0f;
        chromaBand = 16;
        lumaBand = 2;
        tapeNoiseHigh = 0.1f;
        tapeNoiseLow = 0.1f;
        acBeatVelocity = 0.2f;
        acBeatCount = 0.1f;
        acBeatStrength = 0.1f;
        tapeCreaseStrength = 1.0f;
        tapeCreaseNoise = 0.7f;
        tapeCreaseVelocity = 1.2f;
        tapeCreaseCount = 8.0f;
        tapeCreaseDistortion = 0.2f;
        bottomWarpHeight = 15.0f;
        bottomWarpDistortion = 0.1f;
        bottomWarpJitterExtent = 50.0f;
        yiq = DefaultYIQ;

        brightness = 0.0f;
        contrast = 1.0f;
        gamma = 1.0f;
        hue = 0.0f;
        saturation = 1.0f;

        affectSceneView = false;
        filterMode = FilterMode.Bilinear;
        whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;
        enableProfiling = false;
      }
    }    
  }
}
