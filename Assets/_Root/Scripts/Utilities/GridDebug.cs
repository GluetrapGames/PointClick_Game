using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GlueTrap.Utilities
{
public class GridDebug : MonoBehaviour
{
	public bool m_ToggleDebug;
	public Color m_DebugColour = Color.cyan;

	[SerializeField]
	private Color _DefaultColour = new(1f, 1f, 1f, 0.00f);

	private Tilemap _Navmesh;


	private void Awake()
	{
		if (_Navmesh) return;
		_Navmesh = GameObject.FindGameObjectWithTag("NavMesh")
			.GetComponent<Tilemap>();
	}

	private void Update()
	{
		ApplyDebugColours();
	}

	private void ApplyDebugColours()
	{
		if (!_Navmesh) return;
		_Navmesh.color = m_ToggleDebug ? m_DebugColour : _DefaultColour;
	}

	// Apply changes in play mode.
#if UNITY_EDITOR
	private void Reset()
	{
		if (_Navmesh) return;
		_Navmesh = GameObject.FindGameObjectWithTag("NavMesh")
			.GetComponent<Tilemap>();
	}

	private void OnValidate()
	{
		// Apply changes in the editor when values are changed.
		if (!Application.isPlaying)
			EditorApplication.delayCall += ApplyDebugColours;
	}
#endif
}
}