using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlueTrap
{
public class GraphicsSettings : MonoBehaviour
{
	[SerializeField]
	private Toggle _FullscreenToggle;
	[SerializeField]
	private TMP_Dropdown _ResolutionDropdown;
	[SerializeField]
	private TMP_Dropdown _QualityDropdown;


	private void Start()
	{
		// Clear the options in dropdowns if populated.
		if (_ResolutionDropdown.options.Count > 0)
			_ResolutionDropdown.ClearOptions();
		if (_QualityDropdown.options.Count > 0)
			_QualityDropdown.ClearOptions();


		/*var msg = $"Number of Settings: {QualitySettings.count}\nSettings: ";
		var settings = QualitySettings.names;
		msg = settings.Aggregate(msg,
			(current, setting) => current + $"\n{setting}");
		msg +=
			$"\nCurrent Setting: {settings[QualitySettings.GetQualityLevel()]}";

		Debug.Log(msg);*/
	}
}
}