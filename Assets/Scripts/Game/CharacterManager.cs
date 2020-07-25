using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [HideInInspector] public Joystick joystick;

    private const float MOVE_SPEED = 0.04f;

    private void FixedUpdate()
    {
        if(joystick != null && joystick.Direction != Vector2.zero)
        {
            Move();
        }
    }

    private void Move()
    {
        Vector3 vector = joystick.Direction;
        vector = new Vector3(vector.x, 0, vector.y);
        vector = vector.normalized; //長さ1に正規化

        if (vector == Vector3.zero) return;
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + vector * MOVE_SPEED);
        transform.rotation = Quaternion.LookRotation(vector); //向きを変更する
    }
}
