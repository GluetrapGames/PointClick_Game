using System;
using System.Linq;
using EditorAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlueTrap
{
public class GraphicsSettings : MonoBehaviour
{
	[SerializeField,
	 Tooltip("The way the Fullscreen switching logic is handled.")]
	private FullscreenUIType _FullscreenUIType;
	[SerializeField,
	 EnableField(nameof(_FullscreenUIType), FullscreenUIType.Toggle)]
	private Toggle _FullscreenToggle;
	[SerializeField,
	 EnableField(nameof(_FullscreenUIType), FullscreenUIType.Dropdown)]
	private TMP_Dropdown _FullscreenDropdown;

	[Space, SerializeField]
	private TMP_Dropdown _ResolutionDropdown;
	[SerializeField]
	private TMP_Dropdown _QualityDropdown;


	private void Start()
	{
		if (_ResolutionDropdown) SetupResolutionDropdown();
		if (_QualityDropdown) SetupQualityDropdown();
		switch (_FullscreenUIType)
		{
			case FullscreenUIType.Toggle:
				_FullscreenToggle.isOn = Screen.fullScreen;
				break;
			case FullscreenUIType.Dropdown:
				SetupFullscreenDropdown();
				break;
		}
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

		switch (_FullscreenUIType)
		{
			case FullscreenUIType.Toggle:
				_FullscreenToggle?.onValueChanged.AddListener(
					WindowModeToggleChange);
				break;
			case FullscreenUIType.Dropdown:
				_FullscreenDropdown?.onValueChanged.AddListener(delegate
				{
					WindowModeDropdownChange(_FullscreenDropdown);
				});
				break;
		}
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

		switch (_FullscreenUIType)
		{
			case FullscreenUIType.Toggle:
				_FullscreenToggle?.onValueChanged.RemoveListener(
					WindowModeToggleChange);
				break;
			case FullscreenUIType.Dropdown:
				_FullscreenDropdown?.onValueChanged.RemoveListener(delegate
				{
					WindowModeDropdownChange(_FullscreenDropdown);
				});
				break;
		}
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

	private void SetupFullscreenDropdown()
	{
		// Clear the options in dropdown if populated.
		if (_FullscreenDropdown.options.Count > 0)
			_FullscreenDropdown.ClearOptions();

		// Add all the display modes into the dropdown.
		var displayOptions = Enum.GetNames(typeof(FullScreenMode));

		_FullscreenDropdown.AddOptions(displayOptions.ToList());

		// Get the index of the current fullscreen mode.
		var currentIndex = (int)Screen.fullScreenMode;

		// Have the current display mode value selected.
		_FullscreenDropdown.value = currentIndex;
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

	private static void WindowModeDropdownChange(
		TMP_Dropdown windowModeDropdown)
	{
		// Update to the new display mode.
		Screen.fullScreenMode = (FullScreenMode)windowModeDropdown.value;
	}

	private static void WindowModeToggleChange(bool isFullscreen)
	{
		Screen.fullScreen = isFullscreen;
	}

	private enum FullscreenUIType
	{
		Toggle = 0,
		Dropdown = 1
	}
}
}