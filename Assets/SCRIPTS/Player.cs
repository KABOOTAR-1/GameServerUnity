using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player :MonoBehaviour
{
    // Start is called before the first frame update
    public int id;
    public string UserName;

  
    private float MoveSpeed = 5f / Constants.TICKS_PER_SEC;
    private bool[] input;
    public void Initialize(int _id, string userName)
    {
        id = _id;
        UserName = userName;
       
        input = new bool[4];
    }

    public void FixedUpdate()
    {
        Vector2 _inputDirection = Vector2.zero;

        if (input[0])
            _inputDirection.y += 1;
        if (input[1])
            _inputDirection.y -= 1;
        if (input[2])
            _inputDirection.x -= 1;
        if (input[3])
            _inputDirection.x += 1;

        Move(_inputDirection);
    }

    private void Move(Vector2 _inputDirection)
    {

        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        transform.position += _moveDirection * MoveSpeed;

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }
    internal void SetInput(bool[] _inputs, Quaternion _rotation)
    {

        input = _inputs;
        transform.rotation = _rotation;
    }
}
