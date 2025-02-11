using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridDebug : MonoBehaviour
{
	public bool m_ToggleDebug;
	public Color m_DebugColour = Color.cyan;

	[SerializeField]
	private Tilemap _groundTilemap;
	[SerializeField]
	private Color _defaultColour = new(1f, 1f, 1f, 0.00f);


	private void Update()
	{
		ApplyDebugColours();
	}

	// Apply changes in play mode.
#if UNITY_EDITOR
	private void OnValidate()
	{
		// Apply changes in the editor when values are changed.
		if (!Application.isPlaying)
			EditorApplication.delayCall += ApplyDebugColours;
	}
#endif

	private void ApplyDebugColours()
	{
		if (_groundTilemap == null) return;
		_groundTilemap.color = m_ToggleDebug ? m_DebugColour : _defaultColour;
	}
}