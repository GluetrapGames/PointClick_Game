using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private bool _BitchMove = false;

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
            }
            
            var dialogueID = _DialogueEntry.id;

            // ...
            // After some logic, condition, or time, spawn debbie.
            if (DialogueManager.lastConversationID == 66 && dialogueID == 1)
            {
                if (!_DebbieSpawned)
                {
                    _Debbie = Instantiate(_DebbiePrefab, _DebbieSpawner.position, Quaternion.identity);
                    _DebbieSpawned = true;
                }
            }

            if (!_BitchMove)
            {
                // ...
                // Grab Debbie's components.
                NPCMovement debsMovement = _Debbie.GetComponent<NPCMovement>();
                Animator debsAnimator = _Debbie.GetComponent<Animator>();

                // ...
                // After a condition or logic, play specific animation.
                debsAnimator.Play("Debbie_Walk_Front");

                // ...
                // After a condition or logic, move Debbie.
                debsMovement.Move();

                _BitchMove = true;
            }
            
        }
    }
}
