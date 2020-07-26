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

    private void OnTriggerEnter(Collider other)
    {
        if(
            other.gameObject.tag == "WallTop" ||
            other.gameObject.tag == "WallRight" ||
            other.gameObject.tag == "WallLeft" ||
            other.gameObject.tag == "WallBottom" )
        {
            Reflection(other);
        }
    }

    public void SetNum(int num) {
        this.num = num;
        _numText.text = num.ToString();
    }

    public void TakeDamage(int damage) {
        if (num - damage <= 0) Killed();

        SetNum(num - damage);
    }

    private void Killed() {
        GameManager.Instance.KillTheEnemy();
        Destroy(gameObject);
    }

    public void Move(Vector3 direction)
    {
        moveDirection = direction.normalized;
        GetComponent<Rigidbody>().velocity = direction.normalized * SPEED;
    }

    private void Reflection(Collider other)
    {
        Vector3 reflect = Vector3.Reflect(moveDirection, GetNormal(other.gameObject.tag));
        Move(reflect);
    }

    private Vector3 GetNormal(string tagName)
    {
        switch (tagName)
        {
            case "WallTop":
                return Vector3.back;
            case "WallRight":
                return Vector3.left;
            case "WallLeft":
                return Vector3.right;
            case "WallBottom":
                return Vector3.forward;
            default:
                return Vector3.zero;
        }
    }
}
