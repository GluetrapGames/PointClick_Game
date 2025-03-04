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
	[Header("End Game Settings"),
	 SerializedDictionary("Item Type", "Is Collected")]
	public SerializedDictionary<ItemTypes, bool> m_EndItemTypes = new();
	[SceneDropdown]
	public int m_EndScene;
	public GameManager m_GameManager;

	[SerializeField, ReadOnly]
	private readonly SerializedDictionary<string, BreakableItem>
		_BreakableItems = new();


	protected override void Awake()
	{
		base.Awake();
		m_GameManager = FindFirstObjectByType<GameManager>();
	}

	private void Update()
	{
		if (_IsGameOver) return;

		if (m_GameManager.m_InventoryManager.m_InventoryItems.Count != 0)
		{
			List<ItemTypes> keysToUpdate = new();
			foreach ((var itemName, InventoryItemData data) in m_GameManager
				         .m_InventoryManager.m_InventoryItems)
			foreach ((ItemTypes type, var isCollected) in m_EndItemTypes)
				if (data.m_Item.m_Type == type)
				{
					if (_Log) Debug.Log($"{type}: {isCollected}");
					keysToUpdate.Add(type);
				}

			// Update list for every item collected.
			foreach (ItemTypes key in keysToUpdate) m_EndItemTypes[key] = true;
		}

		if (_BreakableItems.Count != 0)
		{
			foreach ((var id, BreakableItem item) in _BreakableItems)
				if (item.m_DamageState == ItemDamageStates.Broken)
					_DestroyedItems[id] = true;
		}

		// Check if everything that was needed was collected or destroyed.
		var allItemsCollected = false;
		var allPlantsDestroyed = false;
		if (_DestroyedItems.Count != 0 && _BreakableItems.Count != 0 &&
		    m_GameManager.m_InventoryManager.m_InventoryItems.Count != 0)
		{
			allItemsCollected = m_EndItemTypes.Values.All(value => value);
			if (!allItemsCollected) return;
			if (_DestroyedItems.Count >= _BreakableItems.Count)
			{
				allPlantsDestroyed = _DestroyedItems.Values.All(value => value);
				if (!allPlantsDestroyed) return;
			}
		}

		if (!allItemsCollected || !allPlantsDestroyed) return;
		_IsGameOver = true;
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
		if (m_GameManager.m_NoneGameplayScenes.Any(noneGameplayScene =>
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