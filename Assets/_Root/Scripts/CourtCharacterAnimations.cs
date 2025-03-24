using UnityEngine;
using UnityEngine.UI;

public class CourtCharacterAnimations : MonoBehaviour
{
    private SpriteRenderer image; 
    private Animator animator; 
    public string animationName = "PlayAnimation";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        image = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Ensure that the Image and Animator are properly set in the Inspector
        if (image == null)
        {
            Debug.LogError("Image reference is not assigned!");
        }
        if (animator == null)
        {
            Debug.LogError("Animator reference is not assigned!");
        }
    }

    void Update()
    {
        // Detect Space key press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Trigger the animation
            if (animator != null)
            {
                animator.Play(animationName);
                
            }
        }
    }
}
