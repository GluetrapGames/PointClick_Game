using System;
using System.Collections.Generic;
using DG.Tweening;
using EditorAttributes;
using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace GlueTrap
{
public class BreakableItem : MonoBehaviour
{
	public ItemDamageStates m_DamageState => _DamageState;

	public string m_PersistentID => _PersistentID;
	[SerializeField]
	public int _itemMaxHp;
	[SerializeField, ProgressBar(nameof(_itemMaxHp), 0.8f, 0f, 0f)]
	public int _itemHp;
	[SerializeField, PropertyOrder(-1), InlineButton(nameof(GenerateID))]
	private string _PersistentID;
	[SerializeField]
	private bool _Log;
	[SerializeField, ReadOnly]
	private ItemDamageStates _DamageState = ItemDamageStates.Undamaged;
	[SerializeField]
	private ItemTypes _ItemType;
	[SerializeField]
	private ItemTypes _effectiveItemType;
	[SerializeField]
	private Vector3 _afterBreakOffset;
	[SerializeField]
	private List<Sprite> _sprites;
	[SerializeField]
	private EventTypes _EventType;
	[SerializeField]
	private BreakMaterialTypes _BreakMaterial;
	[SerializeField]
	private bool _isTV;

	private InputAction _breakableAction;
	private EndGameTracker _EndGameTracker;
	private GameManager _GameManager;
	private Highlight _highlightRef;
	private HighlightComposite _highlightCompRef;
	private bool _hasAddedToCount;
	private ItemTypes _heldItemType;
	private CollideCheck _ItemCollision;
	private HeldItemSlot _playerHeldItem;
	private PlayerInput _PlayerInput;


	private void Awake()
	{
		// Obtain Game Manager.
		_GameManager = Utils.GetGameManager();
		_highlightCompRef = GetComponentInChildren<HighlightComposite>();
		if(!_highlightCompRef) _highlightRef = GetComponentInChildren<Highlight>();
		_ItemCollision = GetComponent<CollideCheck>();
	}

	private void Start()
	{
		_EndGameTracker = _GameManager.m_EndGameTracker;

		_playerHeldItem = _GameManager.m_InventoryManager.m_HeldItemSlot;
		_PlayerInput = _GameManager.m_Player.GetComponent<PlayerInput>();

		_breakableAction = _PlayerInput.actions["Break"];
		if (_breakableAction == null) Debug.LogError("No break action found");

		// If object is destroyed, set it to a broken state and disable highlighting.
		if (!_EndGameTracker._DestroyedItems.ContainsKey(_PersistentID))
			return;
		_itemHp = 0;
		_DamageState = ItemDamageStates.Broken;
		SpriteSwap(_DamageState);
		
		if(!_highlightRef) Debug.LogWarning("<" + name + "> Highlight has no highlight.");
		if(!_highlightCompRef) Debug.LogWarning("<" + name + "> Highlight has no highlight Composite.");
		
	}

	private void Update()
	{
		if (!_playerHeldItem) return;
		if (_playerHeldItem.playerHeldItem != null)
			_heldItemType = _playerHeldItem.playerHeldItem.m_Item.m_Type;

		if (_ItemCollision.IsCollided)
		{
			if(_highlightRef && !_highlightCompRef) _highlightRef._PlayerColliding = true;
			else if (_highlightCompRef && !_highlightRef) _highlightCompRef._PlayerColliding = true;
		}
		else
		{
			if(_highlightRef && !_highlightCompRef) _highlightRef._PlayerColliding = false;
			else if (_highlightCompRef && !_highlightRef) _highlightCompRef._PlayerColliding = false;
		}
		
		if (_breakableAction.WasPressedThisFrame() &&
		    _ItemCollision.IsCollided && _itemHp > 0)
		{
			if (_Log) Debug.Log("Damage Called");
			Damage();
			AkSoundEngine.SetSwitch("BreakMaterial", _BreakMaterial.ToString(),
				gameObject);
			AkSoundEngine.PostEvent(_EventType.ToString(), gameObject);

			// Course the object to shake on hit.
			transform.DOShakeRotation(0.5f, 11f, 700).Play();
		}
		else if (_breakableAction.WasPressedThisFrame() &&
		         !_ItemCollision.IsCollided)
			Debug.Log("Damage failed to call, no collision detected");

		if (Input.GetKeyDown(KeyCode.Space)) OutputDMValues();
	}

	private void Damage()
	{
		// Normal amount of damage if not held item or held item is ineffective.
		_heldItemType = _playerHeldItem.playerHeldItem != null
			? _playerHeldItem.playerHeldItem.m_Item.m_Type
			: ItemTypes.None;
		if (_heldItemType != _effectiveItemType ||
		    _heldItemType == ItemTypes.None)
		{
			_itemHp--;
			IncreaseEnvDM();
			if (_Log)
				Debug.Log($"{name} took 1 damage! New HP = {_itemHp}");
		}
		else
		{
			_itemHp -= 2;
			IncreaseEnvDM();
			if (_Log)
			{
				Debug.Log(
					$"{name} took 2 damage from effective item " +
					$"{_playerHeldItem.playerHeldItem}! New HP = {_itemHp}");
			}
		}

		_DamageState = _itemHp <= 0
			? ItemDamageStates.Broken
			: ItemDamageStates.Damaged;
		SpriteSwap(_DamageState);
	}

	private void DisableHighlighting()
	{
		// Attempt to get a Highlight component and disable it.
		if (!TryGetComponent(out Highlight highlighter))
		{
			highlighter = GetComponentInChildren<Highlight>();
			if (!highlighter)
			{
				Debug.LogWarning($"{name} has no Highlighter component.");
				return;
			}
		}

		highlighter.Hide();
		highlighter.gameObject.SetActive(false);
	}

	private void GenerateID()
	{
		if (_Log) Debug.Log("Generating ID...");
		_PersistentID = Guid.NewGuid().ToString();
		// Tell Unity to save the modified "_PersistentID" property and not wipe it.
#if UNITY_EDITOR
		EditorUtility.IsDirty(this);
		PrefabUtility.RecordPrefabInstancePropertyModifications(this);
		EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
	}

	private void IncreaseEnvDM()
	{
		var EnvDM = DialogueLua.GetVariable("Env_DM_Meter").asInt;
		DialogueLua.SetVariable("Env_DM_Meter", EnvDM + 2);
	}

	private void OutputDMValues()
	{
		var DialogueDM = DialogueLua.GetVariable("Dialogue_DM_Meter").asInt;
		var EnvDM = DialogueLua.GetVariable("Env_DM_Meter").asInt;
		var roomsEntered = DialogueLua.GetVariable("Rooms_Entered").asInt;
		var tvBroken = DialogueLua.GetVariable("TV_Broken").asBool;
		var itemsBroken = DialogueLua.GetVariable("Items_Broken").asInt;
		var hasBeenUpstairs = _GameManager.m_hasUpstairsCourt;
		var crowbarCollected =
			DialogueLua.GetVariable("Crowbar_Collected").asBool;
		Debug.LogWarning(
			$"Dialogue DM Value: {DialogueDM} - Environment DM Value: {EnvDM} - Rooms Entered: {roomsEntered} - TV Broken: {tvBroken.ToString()} - Items Broken: {itemsBroken.ToString()} - Crowbar Collected: {crowbarCollected.ToString()} - Money: {_GameManager.m_collectedMoney} - EnvAfterMoney: {_GameManager.m_moneyAfterMeek}");
		Debug.LogWarning(
			$"END GAME TRACKING: Money Collected: {DialogueLua.GetVariable("Money_Collected").asString} - Clues Found: {DialogueLua.GetVariable("Clues_Found").asString} - Has Been Upstairs: {hasBeenUpstairs}");
	}

	private void SpriteSwap(ItemDamageStates damageState)
	{
		switch (damageState)
		{
			case ItemDamageStates.Damaged:
				gameObject.GetComponent<SpriteRenderer>().sprite = _sprites[0];
				break;
			case ItemDamageStates.Broken:
				DisableHighlighting();
				gameObject.GetComponent<SpriteRenderer>().sprite = _sprites[1];
				gameObject.GetComponent<BoxCollider2D>().enabled = false;
				gameObject.transform.position -= _afterBreakOffset;
				if (!_EndGameTracker._DestroyedItems.ContainsKey(_PersistentID))
				{
					_GameManager.m_totalItemsDestroyed++;
					DialogueLua.SetVariable("Items_Broken",
						_GameManager.m_totalItemsDestroyed);
				}

				if (_isTV) DialogueLua.SetVariable("TV_Broken", true);
				break;
		}
	}

#if UNITY_EDITOR
	private void Reset()
	{
		// Assign a unique ID if it's empty.
		if (string.IsNullOrEmpty(_PersistentID))
			GenerateID();
	}

	private void OnValidate()
	{
		// Update current Health to new max health.
		_itemHp = _itemMaxHp;
	}
#endif
}
}