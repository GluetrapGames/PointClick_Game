using System;
using System.Linq;
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
		if (_ResolutionDropdown) SetupResolutionDropdown();
		if (_QualityDropdown) SetupQualityDropdown();
		if (_FullscreenToggle) _FullscreenToggle.isOn = Screen.fullScreen;
	}

	private void OnEnable()
	{
		// Subscribe to all the OnValueChange events.
		_QualityDropdown?.onValueChanged.AddListener(delegate
		{
			QualityValueChange(_QualityDropdown);
		});
		_ResolutionDropdown?.onValueChanged.AddListener(delegate
		{
			ResolutionValueChange(_ResolutionDropdown);
		});
		_FullscreenToggle?.onValueChanged.AddListener(WindowModeChange);
	}

	private void OnDisable()
	{
		// Unsubscribe from all the OnValueChange events.
		_QualityDropdown?.onValueChanged.RemoveListener(delegate
		{
			QualityValueChange(_QualityDropdown);
		});
		_ResolutionDropdown?.onValueChanged.RemoveListener(delegate
		{
			ResolutionValueChange(_ResolutionDropdown);
		});
		_FullscreenToggle?.onValueChanged.RemoveListener(WindowModeChange);
	}

	private static void QualityValueChange(TMP_Dropdown qualityDropdown)
	{
		// Update quality level.
		QualitySettings.SetQualityLevel(qualityDropdown.value);
	}

	private static void ResolutionValueChange(TMP_Dropdown resolutionDropdown)
	{
		// Update to the new resolution.
		Resolution newResolution = Screen.resolutions[resolutionDropdown.value];
		Screen.SetResolution(newResolution.width, newResolution.height,
			Screen.fullScreen);
	}

	private void SetupQualityDropdown()
	{
		// Clear the options in dropdown if populated.
		if (_QualityDropdown.options.Count > 0)
			_QualityDropdown.ClearOptions();

		// Add all the quality settings into the dropdown.
		var qualitySettings = QualitySettings.names;
		_QualityDropdown.AddOptions(qualitySettings.ToList());

		// Have the current quality value selected.
		_QualityDropdown.value = QualitySettings.GetQualityLevel();
	}

	private void SetupResolutionDropdown()
	{
		// Clear the options in dropdown if populated.
		if (_ResolutionDropdown.options.Count > 0)
			_ResolutionDropdown.ClearOptions();

		// Add all the supported resolutions to the dropdown.
		var resolutions = Screen.resolutions;
		var resolutionNames = resolutions
			.Select(resolution => resolution.ToString()).ToList();
		_ResolutionDropdown.AddOptions(resolutionNames);

		// Have the current resolution be selected.
		// Try to find the current resolution in the list.
		Resolution currentResolution = Screen.currentResolution;
		var currentIndex = resolutions.ToList().FindIndex(r =>
			r.width == currentResolution.width &&
			r.height == currentResolution.height &&
			Math.Abs(r.refreshRateRatio.value -
			         currentResolution.refreshRateRatio.value) < 0.0001);


		// If failed to find exact resolution, find the next best one.
		if (currentIndex < 0)
		{
			currentIndex = resolutions.ToList().FindIndex(r =>
				r.width == currentResolution.width &&
				r.height == currentResolution.height);
		}

		_ResolutionDropdown.value = currentIndex;
	}

	private static void WindowModeChange(bool isFullscreen)
	{
		Screen.fullScreen = isFullscreen;
	}
}
}