using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _numText;

    private const float SPEED = 2;
    
    private int num;
    private Vector3 moveDirection;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall")
        { 
            Reflection(collision);
        }
       
    }

    public void SetNum(int num) {
        this.num = num;
        _numText.text = num.ToString();
    }

    public void Move(Vector3 direction)
    {
        moveDirection = direction.normalized;
        GetComponent<Rigidbody>().velocity = direction.normalized * SPEED;
    }

    private void Reflection(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 reflect = Vector3.Reflect(moveDirection, normal);
        Debug.Log($"{reflect.x} {reflect.y} {reflect.z}");
        Move(reflect);
    }
}
