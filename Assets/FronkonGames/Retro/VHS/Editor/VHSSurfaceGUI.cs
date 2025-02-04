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
using UnityEditor;
using static FronkonGames.Retro.VHS.Inspector;

namespace FronkonGames.Retro.VHS.Editor
{
  /// <summary> Retro VHS Shader GUI. </summary>
  public class VHSSurfaceGUI : SurfaceGUI
  {
    protected override void InspectorGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
      Separator();

      SliderProperty("_Samples", "Number of samples used during calculations in YIQ color space [2, 10]. Default 6.", 6);
      VectorProperty("_YIQ", "YIQ color space (x: luma, y: in-phase, z: quadrature).", VHS.Settings.DefaultYIQ);
      ColorProperty("_ShadowTint", "Shadow tint color.", VHS.Settings.DefaultShadowTint, false, false);

      Label("Color labels");
      IndentLevel++;
      SliderProperty("_BlackLevel", "Color levels (black) [0, 1]. Default 0.", 0.0f);
      SliderProperty("_WhiteLevel", "Color levels (white) [0, 1]. Default 1.", 1.0f);
      IndentLevel--;

      SliderProperty("_TapeCreaseStrength", "Noise band that also deforms the color [0, 1]. Default 1.", 1.0f);
      IndentLevel++;
      SliderProperty("_TapeCreaseCount", "Number of bands [0, 50]. Default 8.", 8);
      SliderProperty("_TapeCreaseVelocity", "Band speed [-5, 5]. Default 1.2.", 1.2f);
      SliderProperty("_TapeCreaseNoise", "Band noise [0, 1]. Default 0.7.", 0.7f);
      SliderProperty("_TapeCreaseDistortion", "Band color distortion [0, 1]. Default 0.2.", 0.2f);
      IndentLevel--;

      SliderProperty("_ColorNoise", "Tape noise [0, 1]. Default 0.1.", 0.1f);

      SliderProperty("_TapeNoiseHigh", "Tape distortion (high frequency) [0, 1]. Default 0.1.", 0.1f);
      IndentLevel++;
      SliderProperty("_TapeNoiseLow", "Tape distortion (low frequency) [0, 1]. Default 0.1.", 0.1f);
      IndentLevel--;

      SliderProperty("_ACBeatStrength", "Amount of AC interferrences [0, 1]. Default 0.1.", 0.1f);
      IndentLevel++;
      SliderProperty("_TapeCreaseCount", "AC interferrences density [0, 1]. Default 0.1.", 0.1f);
      SliderProperty("_TapeCreaseVelocity", "AC interferrences velocity [-1, 1]. Default 0.2.", 0.2f);
      IndentLevel--;

      SliderProperty("_BottomWarpHeight", "'Head-switching' noise height [0, 100]. Default 15.", 15);
      IndentLevel++;
      SliderProperty("_BottomWarpDistortion", "Distortion strength [-1, 1]. Default 0.1.", 0.1f);
      SliderProperty("_BottomWarpJitterExtent", "Extra noise [0, 100]. Default 50.", 50);
      IndentLevel--;

      SliderProperty("_Vignette", "Vignette effect strength [0, 1]. Default 0.25.", 0.25f);

      Separator();

      SliderProperty("_Brightness", "Brightness [-1.0, 1.0]. Default 0.", 0.0f);
      SliderProperty("_Contrast", "Contrast [0.0, 10.0]. Default 1.", 1.0f);
      SliderProperty("_Gamma", "Gamma [0.1, 10.0]. Default 1.", 1.0f);
      SliderProperty("_Hue", "The color wheel [0.0, 1.0]. Default 0.", 0.0f);
      SliderProperty("_Saturation", "Intensity of a colors [0.0, 2.0]. Default 1.", 1.0f);
      SliderProperty("_EmissionStrength", "Emission strength [0.0, 2.0]. Default 0.5.", 0.5f);
    }
  }
}
