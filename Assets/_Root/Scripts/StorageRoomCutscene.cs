using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap
{
    public class StorageRoomCutscene : MonoBehaviour
    {
        [SerializeField]
        private GameObject _DebbiePrefab;
        [SerializeField]
        private Transform _DebbieSpawner;

        private GameManager _GameManager;
        private bool _DebbieSpawned;
        private GameObject _Debbie;

        private DialogueEntry _DialogueEntry;

        private bool _BitchMove = true;
        private int dialogueID;

        private void Awake()
        {
            _GameManager = Utils.GetGameManager();
        }

        private void Update()
        {

            if (!_GameManager.m_IsCutScene) return;

            if (DialogueManager.IsConversationActive)
            {
                _DialogueEntry = DialogueManager.currentConversationState.subtitle.dialogueEntry;
                dialogueID = _DialogueEntry.id;
            }


            // ...
            // After some logic, condition, or time, spawn debbie.
            if (DialogueManager.lastConversationID == 66 && dialogueID == 5)
            {
                if (!_DebbieSpawned)
                {
                    _Debbie = Instantiate(_DebbiePrefab, _DebbieSpawner.position, Quaternion.identity);
                    _DebbieSpawned = true;
                   _BitchMove = false;
                }
            }

            if (!_BitchMove && _DebbieSpawned)
            {
                // ...
                // Grab Debbie's components.
                NPCMovement debsMovement = _Debbie.GetComponent<NPCMovement>();
                GridMovement debsGrid = _Debbie.GetComponent<GridMovement>();
                Animator debsAnimator = _Debbie.GetComponent<Animator>();

                // ...
                // After a condition or logic, play specific animation.
                if (debsGrid.m_IsMoving)
                    debsAnimator.Play("Debbie_Walk_Front");
                else
                    debsAnimator.Play("Idle");

                // ...
                // After a condition or logic, move Debbie.
                debsMovement.UpdateCellPath();
                debsMovement.Move();

                _BitchMove = true;
            }

            if (DialogueManager.lastConversationID == 66 && !DialogueManager.IsConversationActive) 
            {
                SceneManager.LoadScene("CourtScene 4");
            }
            
        }
    }
}
