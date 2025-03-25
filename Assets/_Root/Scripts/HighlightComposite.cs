using System.Collections.Generic;
using EditorAttributes;
using GlueTrap.Utilities;
using UnityEngine;

namespace GlueTrap
{
public class HighlightComposite : MonoBehaviour
{
	[SerializeField]
	private List<Highlight> _highlights;


	[Button("Highlights")]
	private void Awake()
	{
		Utils.FindChildrenByType<Highlight, Highlight>(transform, _highlights,
			c => c);
	}
}
}