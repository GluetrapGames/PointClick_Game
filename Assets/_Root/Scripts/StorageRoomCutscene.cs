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
	private Animator _DebbieAnimator;

	private bool _DebbieMove = true;
	private bool _DebbieSpawned;
	private DialogueEntry _DialogueEntry;
	private GameManager _GameManager;
	private GameObject _Debbie;
	private GridMovement _DebbieGrid;
	private int _DialogueID;

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
		if (DialogueManager.lastConversationID == 66 && _DialogueID == 3)
		{
			if (!_DebbieSpawned)
			{
				_Debbie = Instantiate(_DebbiePrefab, _DebbieSpawner.position,
					Quaternion.identity);
				Debug.Log($"{_Debbie} has spawned!");

				// Grab Debbie's components.
				_DebbieGrid = _Debbie.GetComponent<GridMovement>();
				_DebbieAnimator = _Debbie.GetComponent<Animator>();

				_DebbieSpawned = true;
				_DebbieMove = false;
			}
		}


		// If the conversation has finished, transition to the last Court scene.
		if (DialogueManager.lastConversationID == 66 &&
		    !DialogueManager.IsConversationActive)
			_SceneTransition.CallFromConversationEnd();

		// Logic past here can only happen is Debbie has spawned in.
		if (!_DebbieSpawned) return;

		// If Debbie is moving, change her animation to a moving one.
		if (_DebbieGrid.m_IsMoving)
		{
			Debug.Log("Playing Walkin");
			_DebbieAnimator.Play("Debbie_Walk_Front");
		}
		else
		{
			Debug.Log("Playing Idel");
			_DebbieAnimator.Play("Idle");
		}

		// Once Debbie has spawned and is meant to move, have here move.
		if (!_DebbieMove)
		{
			var debsMovement = _Debbie.GetComponent<NPCMovement>();

			// Update her path and make her move.
			debsMovement.UpdateCellPath();
			debsMovement.Move();

			_DebbieMove = true;
		}
	}
}
}