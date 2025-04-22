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

	private GameObject _InvCanvas;

	private GameManager _GameManager;
	public InventoryItemData itemData;


	private void Awake()
	{
		_GameManager = Utils.GetGameManager();
		_InvCanvas = GameObject.FindGameObjectWithTag("Inventory");
	}

	// Start is called before the first frame update
	public void OnBeginDrag(PointerEventData eventData)
	{
		parentAfterDrag = transform.parent;
		parentBeforeDrag = transform.parent;
		transform.SetParent(_InvCanvas.transform);
		transform.SetAsLastSibling();
		image.raycastTarget = false;
	}

	public void OnDrag(PointerEventData eventData)
	{

		Vector3 mouseScreenPos = Input.mousePosition;
		mouseScreenPos.z = 10;
		transform.position = mouseScreenPos;
		
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		transform.SetParent(parentAfterDrag);
		parentBeforeDrag.GetComponent<InventorySlot>().item = null;
		image.raycastTarget = true;
	}
}
}