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
        private Vector3 _OldPosition;

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
            if (DialogueManager.lastConversationID == 37 && DialogueManager.IsConversationActive && !_ConvoPlayed) 
            {
                // Get the player's position before the animation starts
                _OldPosition = _Player.transform.position;

                _Player.transform.position -= new Vector3(0f, 1.3f, 0f);

                // Bring the fridge infront of the player.
                _FridgeSprite.sortingOrder = 6;

                // Play the drinking animation
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
                // Set the fridge back to behind the player.
                _FridgeSprite.sortingOrder = 1;

                // Reset fridge sprite and position
                _FridgeSprite.sprite = _CloseFridgeSprite;
                gameObject.transform.position = _ClosePosition;

                _ConvoPlayed = false; 
            }
        }

    }
}
