using System;
using EditorAttributes;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
	// Static Animator parameter hashes.
	private static readonly int s_IsMoving = Animator.StringToHash("IsMoving");
	private static readonly int s_HorizontalMovement =
		Animator.StringToHash("HorizontalMovement");
	private static readonly int s_VerticalMovement =
		Animator.StringToHash("VerticalMovement");

	[SerializeField, ReadOnly]
	private AnimationComponents _AnimationComponents;
	[SerializeField, ReadOnly]
	private Vector3 _PlayerPosition;

	private GameManager _GameManager;
	private Vector3 _OldPlayerPosition;


	private void Awake()
	{
		_AnimationComponents = GetAnimationComponents();
		_GameManager = FindFirstObjectByType<GameManager>();
	}

	private void Start()
	{
		_PlayerPosition = _GameManager.m_Player.transform.position;
		_OldPlayerPosition = _PlayerPosition;
	}

	private void Update()
	{
		// Return if not moving.
		if (!_GameManager.m_Player.m_Movement.m_IsMoving)
		{
			_AnimationComponents.m_Animator.SetBool(s_IsMoving, false);
			return;
		}

		_AnimationComponents.m_Animator.SetBool(s_IsMoving, true);

		// Update position and find out the delta.
		_PlayerPosition = _GameManager.m_Player.transform.position;
		Vector3 delta = _PlayerPosition - _OldPlayerPosition;
		delta.Normalize(); //< Makes it between -1 & 1.

		// Flip the sprite horizontally based on the current Player's X direction.
		_AnimationComponents.m_SpriteRenderer.flipX = delta.x switch
		{
			> 0f => false,
			< 0f => true,
			_ => _AnimationComponents.m_SpriteRenderer.flipX
		};

		// Update the animation parameters.
		_AnimationComponents.m_Animator.SetInteger(s_HorizontalMovement,
			(int)delta.x);
		_AnimationComponents.m_Animator.SetInteger(s_VerticalMovement,
			(int)delta.y);

		// Update the old position.
		_OldPlayerPosition = _PlayerPosition;
	}


	/// Obtain the required components for sprite animation from either the
	/// current object or one of its children.
	private AnimationComponents GetAnimationComponents()
	{
		AnimationComponents components;
		Transform chosenObject = transform;

		// Look for the SpriteRenderer first, prioritising parent.
		components.m_SpriteRenderer = GetComponent<SpriteRenderer>();
		if (!components.m_SpriteRenderer)
		{
			// Search for a child with a SpriteRenderer.
			foreach (Transform child in transform)
			{
				var childSpriteRenderer = child.GetComponent<SpriteRenderer>();

				// Continue searching until we find a valid object.
				if (!childSpriteRenderer) continue;
				components.m_SpriteRenderer = childSpriteRenderer;
				chosenObject = child;
				break;
			}
		}

		// If a SpriteRenderer was found, get or add an Animator on the same object.
		if (components.m_SpriteRenderer)
		{
			components.m_Animator = chosenObject.GetComponent<Animator>();
			if (!components.m_Animator)
			{
				components.m_Animator =
					chosenObject.gameObject.AddComponent<Animator>();
			}
		}
		else
		{
			// If no SpriteRenderer was found anywhere, return default components.
			components.m_Animator = null;
		}

		return components;
	}


	[Serializable]
	private struct AnimationComponents
	{
		public Animator m_Animator;
		public SpriteRenderer m_SpriteRenderer;
	}
}