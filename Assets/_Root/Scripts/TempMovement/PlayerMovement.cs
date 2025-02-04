using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float m_MoveSpeed = 5.0f;
    private Controls _controls;

    private void Update()
    {
        var moveINput = _controls.Player.Move.ReadValue<Vector2>();
        var movementVec = new Vector3(moveINput.x, moveINput.y, 0.0f) * (m_MoveSpeed * Time.deltaTime);


        transform.Translate(movementVec);
    }

    private void OnEnable()
    {
        _controls = new Controls();
        _controls.Player.Enable();
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
        _controls = null;
    }
}
