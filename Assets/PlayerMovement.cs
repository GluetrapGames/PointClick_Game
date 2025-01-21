using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float m_MoveSpeed = 5.0f;
    private Controlls _controlls;

    private void Update()
    {
        var moveINput = _controlls.Player.Move.ReadValue<Vector2>();
        var movementVec = new Vector3(moveINput.x, moveINput.y, 0.0f) * (m_MoveSpeed * Time.deltaTime);


        transform.Translate(movementVec);
    }

    private void OnEnable()
    {
        _controlls = new Controlls();
        _controlls.Player.Enable();
    }

    private void OnDisable()
    {
        _controlls.Player.Disable();
        _controlls = null;
    }
}
