using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using System;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private GameObject _muzzle;
    [SerializeField] private GameObject _range;

    [HideInInspector] public Joystick joystick;

    private const float MOVE_SPEED = 0.04f;
    private const float BULLET_SPEED = 15f;
    private const float FIRE_INTERVAL = 0.5f;

    private bool canFire = true;
    private int attack = 1;
    private List<Collider> rangeColliderList = new List<Collider>();

    private void Start()
    {
        _range.OnTriggerEnterAsObservable()
            .Do(collider =>
            {
            if (collider.gameObject.tag == "Enemy")
            {
                rangeColliderList.Add(collider);
                collider.gameObject.OnDestroyAsObservable()
                    .Do(_ => {
                        if (rangeColliderList.Contains(collider))
                        {
                            rangeColliderList.Remove(collider);
                        }
                    })
                    .Subscribe();
                }
            })
            .Subscribe();
        _range.OnTriggerExitAsObservable()
            .Do(collider =>
            {
                if(collider.gameObject.tag == "Enemy" && rangeColliderList.Contains(collider))
                {
                    rangeColliderList.Remove(collider);
                }
            })
            .Subscribe();
    }

    private void Update()
    {
        if(rangeColliderList.Count != 0)
        {
            var closestCollider = GetClosestCollider();
            Fire(closestCollider.transform);
        }
    }

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
        vector = vector.normalized;

        if (vector == Vector3.zero) return;
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + vector * MOVE_SPEED);
        transform.rotation = Quaternion.LookRotation(vector);
    }

    private void Fire(Transform target) {
        if (!canFire) return;
        canFire = false;
        Observable.Timer(TimeSpan.FromSeconds(FIRE_INTERVAL))
            .Do(_ => canFire = true)
            .Subscribe();

        var direction = GetXZPlaneVector(target.position - _muzzle.transform.position);
        GameObject bullet = (GameObject)Instantiate(_bulletPrefab);
        bullet.transform.position = _muzzle.transform.position;
        bullet.transform.rotation = Quaternion.LookRotation(direction);
        bullet.GetComponent<Rigidbody>().velocity = direction.normalized * BULLET_SPEED;

        bullet.OnTriggerEnterAsObservable()
            .Do(collider =>
            {
                switch (collider.gameObject.tag)
                {
                    case "WallTop":
                    case "WallRight":
                    case "WallLeft":
                    case "WallBottom":
                        Destroy(bullet);
                        break;
                    case "Enemy":
                        collider.GetComponent<EnemyManager>().TakeDamage(attack);
                        Destroy(bullet);
                        break;
                    default:
                        break;
                }
            })
            .Subscribe();
    }

    private Vector3 GetXZPlaneVector(Vector3 vector)
    {
        Vector3 newVector = new Vector3(vector.x, 0, vector.z);
        return newVector;
    }

    private Collider GetClosestCollider()
    {
        Collider closestCollider = null;

        foreach(Collider col in rangeColliderList)
        {
            if (closestCollider == null)
            {
                closestCollider = col;
                continue;
            }

            if(
                (col.transform.position - transform.position).magnitude <
                (closestCollider.transform.position - transform.position).magnitude)
            {
                closestCollider = col;
            }
        };
        return closestCollider;
    }
}
