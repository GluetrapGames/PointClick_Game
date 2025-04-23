using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace GlueTrap
{
public class StorageRoomCutscene : MonoBehaviour
{
	[SerializeField]
	private GameObject _DebbiePrefab;
	[SerializeField]
	private Transform _DebbieSpawner;
	[SerializeField]
	private SceneTransition _SceneTransition;
	private bool _BitchMove = true;
	private GameObject _Debbie;
	private bool _DebbieSpawned;
	private DialogueEntry _DialogueEntry;
	private int _DialogueID;

	private GameManager _GameManager;

	private void Awake()
	{
		_GameManager = Utils.GetGameManager();
	}

	private void Start()
	{
		var playerNPC = _GameManager.m_Player.GetComponent<NPCMovement>();

		if (!playerNPC.enabled)
			playerNPC.enabled = true;

		// Start the Player movement path.
		playerNPC.UpdateCellPath();
		playerNPC.Move();
	}

	private void Update()
	{
		if (!_GameManager.m_IsCutScene) return;

		// Keep track of the current dialogue ID.
		if (DialogueManager.IsConversationActive)
		{
			_DialogueEntry = DialogueManager.currentConversationState.subtitle
				.dialogueEntry;
			_DialogueID = _DialogueEntry.id;
		}


		// On a specific conversation node, spawn debbie if she hasn't spawned yet.
		if (DialogueManager.lastConversationID == 66 && _DialogueID == 5)
		{
			if (!_DebbieSpawned)
			{
				_Debbie = Instantiate(_DebbiePrefab, _DebbieSpawner.position,
					Quaternion.identity);
				_DebbieSpawned = true;
				_BitchMove = false;
			}
		}

		// Once Debbie has spawned and is meant to move, have here move.
		if (!_BitchMove && _DebbieSpawned)
		{
			// Grab Debbie's components.
			var debsMovement = _Debbie.GetComponent<NPCMovement>();
			var debsGrid = _Debbie.GetComponent<GridMovement>();
			var debsAnimator = _Debbie.GetComponent<Animator>();

			// If Debbie is moving, change her animation to a moving one.
			if (debsGrid.m_IsMoving)
				debsAnimator.Play("Debbie_Walk_Front");
			else
				debsAnimator.Play("Idle");

			// Update her path and make her move.
			debsMovement.UpdateCellPath();
			debsMovement.Move();

			_BitchMove = true;
		}

		// If the conversation has finished, transition to the last Court scene.
		if (DialogueManager.lastConversationID == 66 &&
		    !DialogueManager.IsConversationActive)
			_SceneTransition.CallFromConversationEnd();
	}
}
}