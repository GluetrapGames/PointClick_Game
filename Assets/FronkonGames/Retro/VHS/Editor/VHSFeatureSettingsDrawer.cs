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
using UnityEngine;
using UnityEditor;
using static FronkonGames.Retro.VHS.Inspector;

namespace FronkonGames.Retro.VHS.Editor
{
  /// <summary> Retro VHS inspector. </summary>
  [CustomPropertyDrawer(typeof(VHS.Settings))]
  public class VHSFeatureSettingsDrawer : Drawer
  {
    private VHS.Settings settings;

    protected override void ResetValues() => settings?.ResetDefaultValues();

    protected override void InspectorGUI()
    {
      settings ??= GetSettings<VHS.Settings>();

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      settings.intensity = Slider("Intensity", "Controls the intensity of the effect [0, 1]. Default 1.", settings.intensity, 0.0f, 1.0f, 1.0f);        

      /////////////////////////////////////////////////
      // VHS.
      /////////////////////////////////////////////////
      Separator();

      settings.quality = (VHS.Quality)EnumPopup("Quality", "Shader quality. Default HighFidelity.", settings.quality, VHS.Quality.HighFidelity);
      if (settings.quality == VHS.Quality.HighFidelity)
      {
        IndentLevel++;
        settings.samples = Slider("Samples", "Number of samples used during calculations in YIQ color space [2, 10]. Default 6.", settings.samples, 2, 10, 6);
        IndentLevel--;
      }

      settings.resolution = (VHS.Resolution)EnumPopup("Resolution", "Final image resolution. Default Quarter (1/4).", settings.resolution, VHS.Resolution.Quarter);

      Label("YIQ color space");
      IndentLevel++;
      settings.yiq.x = Slider("Luma information", "The only component used by black-and-white TV [-2, 2]. Default 0.9.", settings.yiq.x, -2.0f, 2.0f, VHS.Settings.DefaultYIQ.x);
      settings.yiq.y = Slider("In-phase", "Chrominance orange-blue [-2, 2]. Default 1.1.", settings.yiq.y, -2.0f, 2.0f, VHS.Settings.DefaultYIQ.y);
      settings.yiq.z = Slider("Quadrature", "Chrominance purple-green [-2, 2]. Default 1.5.", settings.yiq.z, -2.0f, 2.0f, VHS.Settings.DefaultYIQ.z);
      IndentLevel--;

      settings.shadowTint = ColorField("Shadow tint", "Shadow tint color.", settings.shadowTint, VHS.Settings.DefaultShadowTint);
      IndentLevel++;
      float min = settings.blackLevel;
      float max = settings.whiteLevel;
      MinMaxSlider("Color levels", "White/black levels.", ref min, ref max, 0.0f, 1.0f, 0.0f, 1.0f);
      settings.blackLevel = min;
      settings.whiteLevel = max;
      IndentLevel--;

      settings.tapeCreaseStrength = Slider("Tape crease", "Noise band that also deforms the color [0, 1]. Default 1.", settings.tapeCreaseStrength, 0.0f, 1.0f, 1.0f);
      IndentLevel++;
      settings.tapeCreaseCount = Slider("Count", "Number of bands [0, 50]. Default 8.", settings.tapeCreaseCount, 0.0f, 50.0f, 8.0f);
      settings.tapeCreaseVelocity = Slider("Velocity", "Band speed [-5, 5]. Default 1.2.", settings.tapeCreaseVelocity, -5.0f, 5.0f, 1.2f);
      settings.tapeCreaseNoise = Slider("Noise", "Band noise [0, 1]. Default 0.7.", settings.tapeCreaseNoise, 0.0f, 1.0f, 0.7f);
      if (settings.quality == VHS.Quality.HighFidelity)
        settings.tapeCreaseDistortion = Slider("Distortion", "Band color distortion [0, 1]. Default 0.2.", settings.tapeCreaseDistortion, 0.0f, 1.0f, 0.2f);
      IndentLevel--;

      settings.colorNoise = Slider("Color noise", "Tape noise [0, 1]. Default 0.1.", settings.colorNoise, 0.0f, 1.0f, 0.1f);

      settings.chromaBand = Slider("Chroma band", "In VHS chroma band is a much lower resolution (technically 1/16th) [1, 64]. Default 16.", settings.chromaBand, 1, 64, 16);
      settings.lumaBand = Slider("Luma band", "In VHS the luma band is half of the resolution [1, 16]. Default 2.", settings.lumaBand, 1, 16, 2);

      settings.tapeNoiseHigh = Slider("Tape noise high", "Tape distortion (high frequency) [0, 1]. Default 0.1.", settings.tapeNoiseHigh, 0.0f, 1.0f, 0.1f);
      IndentLevel++;
      settings.tapeNoiseLow = Slider("Low", "Tape distortion (low frequency) [0, 1]. Default 0.1.", settings.tapeNoiseLow, 0.0f, 1.0f, 0.1f);
      IndentLevel--;

      settings.acBeatStrength = Slider("AC beat", "Amount of AC interferrences [0, 1]. Default 0.1.", settings.acBeatStrength, 0.0f, 1.0f, 0.1f);
      IndentLevel++;
      settings.acBeatCount = Slider("Count", "AC interferrences density [0, 1]. Default 0.1.", settings.acBeatCount, 0.0f, 1.0f, 0.1f);
      settings.acBeatVelocity = Slider("Velocity", "AC interferrences velocity [-1, 1]. Default 0.2.", settings.acBeatVelocity, -1.0f, 1.0f, 0.2f);
      IndentLevel--;

      settings.bottomWarpHeight = Slider("Bottom warp", "'Head-switching' noise height [0, 100]. Default 15.", settings.bottomWarpHeight, 0.0f, 100.0f, 15.0f);
      IndentLevel++;
      settings.bottomWarpDistortion = Slider("Distortion", "Distortion strength [-1, 1]. Default 0.1.", settings.bottomWarpDistortion, -1.0f, 1.0f, 0.1f);
      settings.bottomWarpJitterExtent = Slider("Jitter extent", "Extra noise [0, 100]. Default 50.", settings.bottomWarpJitterExtent, 0.0f, 100.0f, 50.0f);
      IndentLevel--;

      settings.vignette = Slider("Vignette", "Vignette effect strength [0, 1]. Default 0.25.", settings.vignette, 0.0f, 1.0f, 0.25f);

      /////////////////////////////////////////////////
      // Color.
      /////////////////////////////////////////////////
      Separator();

      if (Foldout("Color") == true)
      {
        IndentLevel++;

        settings.brightness = Slider("Brightness", "Brightness [-1.0, 1.0]. Default 0.", settings.brightness, -1.0f, 1.0f, 0.0f);
        settings.contrast = Slider("Contrast", "Contrast [0.0, 10.0]. Default 1.", settings.contrast, 0.0f, 10.0f, 1.0f);
        settings.gamma = Slider("Gamma", "Gamma [0.1, 10.0]. Default 1.", settings.gamma, 0.01f, 10.0f, 1.0f);
        settings.hue = Slider("Hue", "The color wheel [0.0, 1.0]. Default 0.", settings.hue, 0.0f, 1.0f, 0.0f);
        settings.saturation = Slider("Saturation", "Intensity of a colors [0.0, 2.0]. Default 1.", settings.saturation, 0.0f, 2.0f, 1.0f);

        IndentLevel--;
      }

      /////////////////////////////////////////////////
      // Advanced.
      /////////////////////////////////////////////////
      Separator();

      if (Foldout("Advanced") == true)
      {
        IndentLevel++;

        settings.affectSceneView = Toggle("Affect the Scene View?", "Does it affect the Scene View?", settings.affectSceneView);
        settings.filterMode = (FilterMode)EnumPopup("Filter mode", "Filter mode. Default Bilinear.", settings.filterMode, FilterMode.Bilinear);
        settings.whenToInsert = (UnityEngine.Rendering.Universal.RenderPassEvent)EnumPopup("RenderPass event",
          "Render pass injection. Default BeforeRenderingPostProcessing.",
          settings.whenToInsert,
          UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingPostProcessing);
        settings.enableProfiling = Toggle("Enable profiling", "Enable render pass profiling", settings.enableProfiling);

        IndentLevel--;
      }
    }
  }
}
