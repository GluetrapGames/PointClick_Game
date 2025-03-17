using GlueTrap.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GlueTrap
{
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler,
	IEndDragHandler
{
	public Image image;
	[HideInInspector]
	public Transform parentAfterDrag;
	[HideInInspector]
	public Transform parentBeforeDrag;

	private GameManager _GameManager;
	public InventoryItemData itemData;


	private void Awake()
	{
		_GameManager = Utils.GetGameManager();
	}

	// Start is called before the first frame update
	public void OnBeginDrag(PointerEventData eventData)
	{
		parentAfterDrag = transform.parent;
		parentBeforeDrag = transform.parent;
		transform.SetParent(transform.root);
		transform.SetAsLastSibling();
		image.raycastTarget = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector3 mouseScreenPos =
			_GameManager.m_Camera.ScreenToWorldPoint(Input.mousePosition);
		Vector3 mousePosition = new Vector2(mouseScreenPos.x, mouseScreenPos.y);
		transform.position = mousePosition;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		transform.SetParent(parentAfterDrag);
		parentBeforeDrag.GetComponent<InventorySlot>().item = null;
		image.raycastTarget = true;
	}
}
}