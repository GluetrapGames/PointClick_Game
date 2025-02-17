using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InteractPopUp : MonoBehaviour
{
    public float interactionRadius = 3f;
    public Transform player;
    public GameObject interactionUI;

    private Text interactionText;

    private void Awake()
    {
        interactionText = interactionUI.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null || interactionUI == null || interactionText == null)
            { return; }

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= interactionRadius)
        {
            interactionText.text = $"Press E to interact with {name}";
            interactionUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
        else 
        {
            interactionUI.SetActive(false);
        }
    }

    private void Interact() 
    {
        Debug.Log($"Interacted with {name}");
    }
}
