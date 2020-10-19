using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _numText;

    [HideInInspector] public EnemyData enemyData;

    private const float SPEED = 2;

    private float health;

    public void Init(float health, Vector3 direction)
    {
        SetNum(health);
        Move(direction);
        SetType();
    }

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

    private void SetNum(float health) {
        this.health = health;
        _numText.text = Math.Ceiling(health).ToString();
    }

    public void TakeDamage(float damage) {
        if (health - damage <= 0)
        {
            GameManager.Instance.AddScore(health);
            Killed();
        }
        else
        {
            
            GameManager.Instance.AddScore(damage);
            SetNum(health - damage);
        }

    }

    private void Killed() {
        enemyData.position = transform.position;
        GameManager.Instance.KillTheEnemy(enemyData);
        Destroy(gameObject);
    }

    private void Move(Vector3 direction)
    {
        enemyData.direction = direction.normalized;
        GetComponent<Rigidbody>().velocity = direction.normalized * SPEED;

    }

    private void Reflection(Collider other)
    {
        Vector3 reflect = Vector3.Reflect(enemyData.direction, GetNormal(other.gameObject.tag));
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

    private void SetType()
    {
        switch (enemyData.enemyType)
        {
            case EnemyType.Normal:
                break;
            case EnemyType.Guard:
                CreateShield();
                break;
            default:
                break;
        }
    }

    private void CreateShield() {
        var shield = (GameObject)Resources.Load("Assets/Enemy/Shield");
        var instance = (GameObject)Instantiate(shield, transform);
        var shieldProperty = instance.GetComponent<ShieldProperty>();
        shieldProperty.Init(this);
    }

}
