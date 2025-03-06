using System;
using UnityEngine;

namespace GlueTrap
{
public class InteractionPanel : MonoBehaviour
{
	[NonSerialized]
	public GameObject drawnBy;
	[NonSerialized]
	public bool isDrawn = false;
}
}