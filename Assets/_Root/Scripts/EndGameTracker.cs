using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using EditorAttributes;
using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
public class EndGameTracker : Singleton<EndGameTracker>
{
	[Header("End Game Settings"),
	 SerializedDictionary("Item Type", "Is Collected")]
	public SerializedDictionary<ItemTypes, bool> m_EndItemTypes = new();
	[SceneDropdown]
	public int m_EndScene;
	[SerializeField, ReadOnly]
	public SerializedDictionary<string, bool> _DestroyedItems = new();
	//[SerializeField]
	public bool _IsGameOver;
	public bool m_hasMoney;

	// 0 Money, 1 Crowbar, 2 Journal, 3 Keys, 4 Poetry, 5 Cigarettes, 6 Medicine
	public List<ItemTypes> m_CollectedItems = new();

	[SerializeField]
	private GameObject _AlbertPrefab;
	[SerializeField, ReadOnly]
	private Transform _AlbertSpawPoint;
	[SerializeField]
	private bool _Log;
	[SerializeField, ReadOnly]
	private SerializedDictionary<string, BreakableItem> _BreakableItems = new();
	private bool _albertSpawned;
	private bool _moneySet;

	private GameManager _GameManager;
	private int _cluesFound;

	protected override void Awake()
	{
		base.Awake();
		_GameManager = Utils.GetGameManager();
	}

	private void Update()
	{
		_cluesFound = DialogueLua.GetVariable("Clues_Found").asInt;

		if (_IsGameOver)
		{
			if (!_albertSpawned)
			{
				SpawnAlbert();
				Debug.Log("Albert spawned");
				_albertSpawned = true;
			}

			if (!DialogueLua.GetVariable("HasFinalConvo").asBool) return;
			if (DialogueManager.isConversationActive) return;
			GameObject transition = GameObject.Find("ToCS4");
			var transComp = transition.GetComponent<SceneTransition>();
			_GameManager.calcMoneyMeek();
			transComp.CallFromConversationEnd();
			_IsGameOver = false;

			return;
		}

		if (_cluesFound >= 2 && m_hasMoney)
		{
			GameObject phone = GameObject.Find("Phone");
			phone.GetComponentInChildren<Highlight>().enabled = true;
			phone.GetComponentInChildren<PolygonCollider2D>().enabled = true;
			phone.GetComponentInChildren<SpriteRenderer>().material =
				Resources.Load<Material>("Materials/Outline_Mat");
		}

		TrackEndGameItems();

		// Update any destroyed items.
		if (_BreakableItems.Count != 0)
		{
			foreach ((var id, BreakableItem item) in _BreakableItems)
				if (item.m_DamageState == ItemDamageStates.Broken)
					_DestroyedItems[id] = true;
		}

		var endGame = DialogueLua.GetVariable("Final_Phonecall").asBool;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Debug.LogWarning(DialogueLua.GetVariable("Collected_Item_List")
				.ToString());
		}

		if (!endGame) return;
		Debug.LogWarning("GAME END, SPAWNING ALBERT");
		_IsGameOver = true;
	}


	private void OnEnable()
	{
		_GameManager.m_OnGameReset.AddListener(ResetGame);
	}

	private void OnDisable()
	{
		_GameManager.m_OnGameReset.RemoveListener(ResetGame);
	}

	public override void OnSceneChange(Scene scene, LoadSceneMode mode)
	{
		// Get breakable items and add/update the list.
		var newItems =
			FindObjectsByType<BreakableItem>(FindObjectsSortMode.None);

		// Either update or add new item to list.
		foreach (BreakableItem item in newItems)
		{
			var id = item.m_PersistentID;
			_BreakableItems[id] = item;
		}
	}

	public void SpawnAlbert()
	{
		if (!_IsGameOver)
		{
			Debug.LogError("Can't Spawn Albert!");
			return;
		}

		GameObject albertConvoObject = GameObject.Find("Albert");
		albertConvoObject.GetComponent<DialogueSystemTrigger>().enabled = true;
		albertConvoObject.GetComponent<DialogueSystemEvents>().enabled = true;

		// Get Albert's spawner.
		_AlbertSpawPoint = Utils.FindSpawner("AlbertSpawner");

		// Check for GameOver.
		if (_IsGameOver && SceneManager.GetActiveScene() ==
		    SceneManager.GetSceneByName("DownstairsHallway"))
		{
			Debug.Log("GAME OVER CHECK PASSED");
			GameObject albertObj = Instantiate(_AlbertPrefab,
				_AlbertSpawPoint.position, quaternion.identity);
		}
	}

	private void ResetGame()
	{
		// Reset all tracking variables.
		_IsGameOver = false;
		m_hasMoney = false;
		_albertSpawned = false;
		_moneySet = false;
		_cluesFound = 0;

		// Mark all destroyed items as not destroyed.
		var destroyedItems = _DestroyedItems.ToList();
		foreach (var destroyedItem in destroyedItems)
			_DestroyedItems[destroyedItem.Key] = false;
	}

	private void TrackEndGameItems()
	{
		if (DialogueLua.GetVariable("Money_Collected").asBool && !_moneySet)
		{
			m_hasMoney = true;
			_moneySet = true;
		}

		if (_GameManager.m_InventoryManager.m_InventoryItems.Count <= 0) return;

		Dictionary<ItemTypes, bool> keysToUpdate = new();

		// Collect updates.
		foreach ((var itemName, InventoryItemData data) in _GameManager
			         .m_InventoryManager.m_InventoryItems)
		foreach ((ItemTypes type, var isCollected) in m_EndItemTypes)
			if (data.m_Item.m_Type == type)
			{
				if (_Log) Debug.Log($"{type}: {isCollected}");
				keysToUpdate[type] = data.m_IsCollected;
			}

		// Store changes separately.
		List<KeyValuePair<ItemTypes, bool>> updates = new();
		foreach ((ItemTypes type, var isCollected) in m_EndItemTypes)
			if (keysToUpdate.TryGetValue(type, out var updatedValue) &&
			    isCollected != updatedValue)
			{
				updates.Add(
					new KeyValuePair<ItemTypes, bool>(type, updatedValue));
			}

		// Apply updates.
		foreach (var update in updates)
			m_EndItemTypes[update.Key] = update.Value;

		UpdateDMValues();
	}

	private void UpdateDMValues()
	{
		var collectedItems = "";
		for (var i = 0; i < m_CollectedItems.Count; i++)
			collectedItems =
				collectedItems + $"{m_CollectedItems[i].ToString()}, ";
		;
		DialogueLua.SetVariable("Collected_Item_List", collectedItems);
	}
}
}