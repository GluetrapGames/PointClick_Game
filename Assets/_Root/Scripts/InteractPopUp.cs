using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

public class InteractPopUp : MonoBehaviour
{
    public float interactionRadius = 3f;
    public Transform player;
    public GameObject interactionUI;

    private Text interactionText;

    [SerializeField]
    private CollideCheck _collisionCheck;
    
    private void Awake()
    {
        interactionText = interactionUI.GetComponentInChildren<Text>();        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(player == null || interactionUI == null || interactionText == null)
            { return; }

        DrawInteractUI();
        
    }

    private void DrawInteractUI()
    {
        if (!interactionUI.GetComponent<InteractionPanel>().isDrawn && _collisionCheck.IsCollided)
        {
            interactionText.text = $"Interact with {name}";
            interactionUI.SetActive(true);
            interactionUI.GetComponent<InteractionPanel>().isDrawn = true;
            interactionUI.GetComponent<InteractionPanel>().drawnBy = gameObject; 
            Debug.Log("Drawing interaction UI, drawn by " + gameObject.name);
        } else if (!interactionUI.GetComponent<InteractionPanel>().isDrawn && !_collisionCheck.IsCollided)
        {
            return;
        } else if (interactionUI.GetComponent<InteractionPanel>().isDrawn && !_collisionCheck.IsCollided)
        {
            if (interactionUI.GetComponent<InteractionPanel>().drawnBy == gameObject)
            {
                interactionUI.GetComponent<InteractionPanel>().isDrawn = false;
                interactionUI.GetComponent<InteractionPanel>().drawnBy = null;
                interactionUI.SetActive(false);
            }
        }
    }
    
    private void Interact() 
    {
        Debug.Log($"Interacted with {name}");
    }
}
