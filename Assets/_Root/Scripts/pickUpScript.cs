using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUpScript : MonoBehaviour
{
    public bool isClicked;
    public bool activateVariable;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        isClicked = false;
        activateVariable = false;
    }

    // Update is called once per frame
    void Update()
    {

        // When mouse is clicked, Ray cast to check if the game object space was clicked.
        if (Input.GetMouseButtonDown(0)) 
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
                isClicked = true;
            else 
                isClicked = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the game object has been clicked and the player is within the trigger then hide the object and display alert message.
        if (isClicked) 
        { 
            //Debug.Log("Item collected");
            PixelCrushers.DialogueSystem.DialogueManager.ShowAlert(gameObject.name + " has been collected!");
            activateVariable = true;
        }
    }
}
