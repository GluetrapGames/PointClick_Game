using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideCheck : MonoBehaviour 
{
    private bool _isCollided;
	
    public bool IsCollided
    {
        get {return _isCollided; }
    }
	
    void OnTriggerEnter2D(Collider2D other) 
    {
        Debug.Log(transform.name + " collided with " + other.name);
        if (other.gameObject.CompareTag("Player"))
        {
            _isCollided = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Collision Reset");
        if (other.gameObject.CompareTag("Player"))
        {
            ResetCollision();
        }
    }
    
    public void ResetCollision()
    {
        _isCollided = false;
    }
}
