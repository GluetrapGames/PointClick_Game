
using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    [SequencerCommandGroup("submenu")]
    public class SequencerCommandPlayJohnAnim: SequencerCommand
    {
        private string _AnimType;

        private GameObject _Player;
        private Animator _Animator;

        private GameObject _Albert;
        private Animator _AlbertAnim;

        public void Awake()
        {
            // The type of anim
            _AnimType = GetParameter(0);

            _Player = GameObject.FindGameObjectWithTag("Player");
            _Animator = _Player.GetComponentInChildren<Animator>();

            _Albert = GameObject.Find("Albert(Clone)");
            _AlbertAnim = _Albert.GetComponent<Animator>();
        }

        public void Update()
        {
            // Add any update code here. When the command is done, call Stop().
            // If you've called stop above in Awake(), you can delete this method.

            switch (_AnimType) 
            {
                case "unarmed":
                    PlayUnarmedAttackAnim();
                    break;
                case "armed":
                    PlayArmedAttackAnim();
                    break;
                case "drag":
                    PlayDragAnim();
                    break;
                case "pos":
                    MoveToAttackPosition();
                    break;
                case "al_fall":
                    AlbertFall();
                    break;
                default:
                    Debug.Log("Animation type not recognised");
                    break;
            }

            if (_AnimType == null) Debug.Log("ANIMTYPE NULL --- JohnAnim sequence");
            Stop();
        }

        public void OnDestroy()
        {
            // Add your finalization code here. This is critical. If the sequence is cancelled and this
            // command is marked as "required", then only Awake() and OnDestroy() will be called.
            // Use it to clean up whatever needs cleaning at the end of the sequencer command.
            // If you don't need to do anything at the end, you can delete this method.
        }

        // Plays the John attack animation with his fists.
        private void PlayUnarmedAttackAnim() 
        {
            _Animator.Play("John_Unarmed_Attack");
        }  
        
        // Plays the John attack animation with the crowbar.
        private void PlayArmedAttackAnim() 
        {
            _Animator.Play("John_Armed_Attack");

        }  
        
        // Makes John play the dragging of Albert animation for the cutscene.
        private void PlayDragAnim() 
        {
            _Animator.SetBool("IsInCutScene", true);
            _Animator.Play("John_Albert_Drag");

        }

        // Sets John to the correct position ready for the cutscene.
        private void MoveToAttackPosition() 
        {
            _Player.transform.position = new Vector3(0.5f, 7f, 0f);
        }

        // Play Albert's falling animation
        private void AlbertFall() 
        {
            _AlbertAnim.SetBool("IsAttacked", true);
            _AlbertAnim.Play("Albert_Attacked_Blood");
        }

    }

}


