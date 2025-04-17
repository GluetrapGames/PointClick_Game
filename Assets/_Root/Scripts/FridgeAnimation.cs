using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GlueTrap
{
    public class FridgeAnimation : MonoBehaviour
    {
        private bool _ConvoPlayed;
        
        private SpriteRenderer _FridgeSprite;
        [SerializeField]
        private Sprite _OpenFridgeSprite;
        [SerializeField]
        private Sprite _CloseFridgeSprite;

        private Vector3 _ClosePosition = new Vector3(3.24f, 3.42f, 9.99f);
        private Vector3 _OpenPosition = new Vector3(2.65f, 3.42f, 9.99f);

        private GameObject _Player;
        public bool isDrinking;

        private Animator _Animator;

        void Awake()
        {
            _ConvoPlayed = false;

            // Get the fridge's sprite component
            _FridgeSprite = GetComponentInChildren<SpriteRenderer>();

            _FridgeSprite.sprite = _CloseFridgeSprite;

            _Player = GameObject.FindGameObjectWithTag("Player");
            _Animator = _Player.GetComponentInChildren<Animator>();

        }

        void Update()
        {
            // Check if the current conversation playing is the fridge convo
            // Play the beer drink animation.
            if (DialogueManager.lastConversationID == 37 && DialogueManager.IsConversationActive && !_ConvoPlayed) 
            {
                _Animator.Play("John_Drink_Beer");
                // Change the fridge sprite and position
                _FridgeSprite.sprite = _OpenFridgeSprite;
                gameObject.transform.position = _OpenPosition;


                // Prevent actions from happening again this frame.
                _ConvoPlayed = true;
            }

            // Reset convo flag and fridge sprite so that it can be played again.
            if (!DialogueManager.isConversationActive)
            {
                // Reset fridge sprite and position
                _FridgeSprite.sprite = _CloseFridgeSprite;
                gameObject.transform.position = _ClosePosition;

                _ConvoPlayed = false; 
            }
        }

    }
}
