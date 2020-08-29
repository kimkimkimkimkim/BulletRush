using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using System;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private GameObject _muzzle;
    [SerializeField] private GameObject _range;
    [SerializeField] private GameObject _body;

    [HideInInspector] public Joystick joystick;
    [HideInInspector] public GameManager gameManager;

    private const float MOVE_SPEED = 0.04f;
    private const float BULLET_SPEED = 30f;

    private bool canFire = true;
    private List<Collider> rangeColliderList = new List<Collider>();
    private Animator animator;
    private float damage;
    private float fireInterval;

    private void Start()
    {
        SetStatus();

        animator = GetComponent<Animator>();

        _body.OnTriggerEnterAsObservable()
            .Do(collider =>
            {
                if(collider.gameObject.tag == "Enemy") {
                    gameManager.Defeat();
                }
            })
            .Subscribe();

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

    private void SetStatus()
    {
        var damageLevel = SaveDataUtil.Status.GetDamageLevel();
        var rateLevel = SaveDataUtil.Status.GetRateLevel();

        damage = MasterRecords.GetDamageMB().First(m => m.Level == damageLevel).Value;
        var rate = MasterRecords.GetRateMB().First(m => m.Level == rateLevel).Value;
        fireInterval = 1 / rate;
    }

    private void FixedUpdate()
    {
        if (rangeColliderList.Count != 0)
        { 
            var closestCollider = GetClosestCollider();
            Fire(closestCollider.transform);

            animator.SetBool(Condition.isAim.ToString(), true);
        }
        else 
        {
            animator.SetBool(Condition.isAim.ToString(), false);
        }


        if (joystick != null)
        {
            Move();
            SetDirectionToAnimator();
        }
    }

    private void SetDirectionToAnimator() {
        var forward = transform.forward.normalized;

        animator.SetFloat(Condition.left.ToString(), -joystick.Direction.x);
        animator.SetFloat(Condition.right.ToString(), joystick.Direction.x);
        animator.SetFloat(Condition.forward.ToString(), joystick.Direction.y);
        animator.SetFloat(Condition.backward.ToString(), -joystick.Direction.y);
    }

    private void Move()
    {
        Vector3 vector = joystick.Direction;
        vector = new Vector3(vector.x, 0, vector.y);
        vector = vector.normalized;

        if (vector == Vector3.zero) return;
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + vector * MOVE_SPEED);

        if (!animator.GetBool(Condition.isAim.ToString()))
        {
            transform.rotation = Quaternion.LookRotation(vector);
        }
    }

    private void Fire(Transform target) {
        var vector = GetXZPlaneVector(target.position - transform.position);
        transform.rotation = Quaternion.LookRotation(vector);

        if (!canFire) return;
        canFire = false;
        Observable.Timer(TimeSpan.FromSeconds(fireInterval))
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
                        collider.GetComponent<EnemyManager>().TakeDamage(damage);
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

    private enum Condition { 
        isAim,
        left,
        right,
        forward,
        backward,
    }
}
