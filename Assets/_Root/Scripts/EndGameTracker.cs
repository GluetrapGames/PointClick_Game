using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using EditorAttributes;
using GlueTrap.Utilities;
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

	[SerializeField]
	private GameObject _AlbertPrefab;
	[SerializeField, ReadOnly]
	private Transform _AlbertSpawPoint;
	[SerializeField, ReadOnly]
	public SerializedDictionary<string, bool> _DestroyedItems = new();
	[SerializeField]
	private bool _IsGameOver;
	[SerializeField]
	private bool _Log;
	[SerializeField, ReadOnly]
	private SerializedDictionary<string, BreakableItem> _BreakableItems = new();

	private GameManager _GameManager;


	protected override void Awake()
	{
		base.Awake();
		_GameManager = Utils.GetGameManager();
	}

	private void Update()
	{
		if (_IsGameOver) return;

		TrackEndGameItems();

		if (_BreakableItems.Count != 0)
		{
			foreach ((var id, BreakableItem item) in _BreakableItems)
				if (item.m_DamageState == ItemDamageStates.Broken)
					_DestroyedItems[id] = true;
		}

		// Check if everything that was needed was collected or destroyed.
		var allItemsCollected = false;
		if (_GameManager.m_InventoryManager.m_InventoryItems.Count != 0)
		{
			allItemsCollected = m_EndItemTypes.Values.All(value => value);
			if (!allItemsCollected) return;
		}
		/*var allPlantsDestroyed = false;
		if (_DestroyedItems.Count != 0 && _BreakableItems.Count != 0 &&
		    _GameManager.m_InventoryManager.m_InventoryItems.Count != 0)
		{
			allItemsCollected = m_EndItemTypes.Values.All(value => value);
			if (!allItemsCollected) return;
			if (_DestroyedItems.Count >= _BreakableItems.Count)
			{
				allPlantsDestroyed = _DestroyedItems.Values.All(value => value);
				if (!allPlantsDestroyed) return;
			}
		}*/

		if (!allItemsCollected /*|| !allPlantsDestroyed*/) return;
		_IsGameOver = true;
	}

	private void TrackEndGameItems()
	{
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
	}

	public override void OnSceneChange(Scene scene, LoadSceneMode mode)
	{
		// Get Albert's spawner.
		_AlbertSpawPoint = Utils.FindSpawner("AlbertSpawner");

		// Check for GameOver.
		if (_IsGameOver && SceneManager.GetActiveScene() ==
		    SceneManager.GetSceneByName("Hallway1"))
		{
			Instantiate(_AlbertPrefab, _AlbertSpawPoint.position,
				quaternion.identity);
		}

		// Make sure we are in a gameplay scene.
		if (_GameManager.m_NoneGameplayScenes.Any(noneGameplayScene =>
			    scene.name == noneGameplayScene))
			return;

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
}
}