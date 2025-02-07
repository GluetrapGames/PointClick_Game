using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridDebug : MonoBehaviour
{
	public bool m_ToggleDebug;

	[SerializeField]
	private Tilemap m_GroundTilemap;
	[SerializeField]
	private Color m_DebugColour = Color.cyan;
	[SerializeField]
	private Color m_DefaultColour = new(1f, 1f, 1f, 0.00f);

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
		if (m_GroundTilemap == null) return;
		m_GroundTilemap.color = m_ToggleDebug ? m_DebugColour : m_DefaultColour;
	}
}