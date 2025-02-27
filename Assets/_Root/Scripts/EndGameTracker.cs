using System.Collections.Generic;
using System.Linq;
using _Root.Scripts.Utilities;
using AYellowpaper.SerializedCollections;
using EditorAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameTracker : PersistantSingleton<EndGameTracker>
{
	public GameManager m_GameManager;
	[Header("End Game Settings"),
	 SerializedDictionary("Item Type", "Is Collected")]
	public SerializedDictionary<string, bool> m_EndItemTypes = new();
	public int m_PlantsInGame;
	[ReadOnly]
	public int m_PlantsDestroyed;
	[SceneDropdown]
	public int m_EndScene;

	[SerializeField, ReadOnly]
	private SerializedDictionary<string, BreakableItem> _BreakableItems = new();
	[SerializeField, ReadOnly]
	public SerializedDictionary<string, bool> _DestroyedItems = new();
	[SerializeField, ReadOnly]
	private Transform _WorldObject;
	[SerializeField]
	private bool _IsGameOver;
	[SerializeField]
	private GameObject _AlbertPrefab;
	[SerializeField]
	private Transform _AlbertSpawnLocation;


	private void Update()
	{
		if (_IsGameOver) return;

		if (m_GameManager.m_InventoryManager.m_InventoryItems.Count != 0)
		{
			List<string> keysToUpdate = new();
			foreach ((var itemName, InventoryItemData data) in m_GameManager
				         .m_InventoryManager.m_InventoryItems)
			foreach (var (type, isCollected) in m_EndItemTypes)
				if (data.m_Item.m_Type == type)
				{
					Debug.Log($"{type}: {isCollected}");
					keysToUpdate.Add(type);
				}

			// Update list for every item collected.
			foreach (var key in keysToUpdate) m_EndItemTypes[key] = true;
		}

		if (_BreakableItems.Count != 0)
		{
			foreach ((var id, BreakableItem item) in _BreakableItems)
				if (item._damageState == BreakableItem._itemStates.Broken)
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
		//SceneManager.LoadScene(m_EndScene);
		// Spawn Albert.
	}


	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (_IsGameOver && SceneManager.GetActiveScene() ==
		    SceneManager.GetSceneByName("Hallway1"))
			Instantiate(_AlbertPrefab, _AlbertSpawnLocation.position,
				quaternion.identity);


		// Try to obtain the Game Manager.
		var gameManager = FindFirstObjectByType<GameManager>();
		if (!gameManager)
		{
			Debug.LogWarning("Cannot find 'Game Manager' object in the scene.");
			return;
		}

		m_GameManager = gameManager;

		// Try to obtain the World GameObject.
		GameObject worldObject = GameObject.FindWithTag("World");
		if (!worldObject)
		{
			Debug.LogWarning("Cannot find 'World' object in the scene.");
			return;
		}

		_WorldObject = worldObject.transform;

		var newItems = new List<BreakableItem>();
		Utils.FindChildrenByType<BreakableItem, BreakableItem>(
			_WorldObject, newItems, c => c);

		// Either update or add new item to list.
		foreach (BreakableItem item in newItems)
		{
			var id = item.m_PersistentID;
			_BreakableItems[id] = item;
		}
	}
}