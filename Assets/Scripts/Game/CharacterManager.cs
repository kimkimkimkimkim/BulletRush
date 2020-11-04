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
    private const float MAX_FIRE_INTERVAL = 0.1f;

    private bool canFire = true;
    private bool isMove = false;
    private List<Collider> rangeColliderList = new List<Collider>();
    private Animator animator;
    private float damage;
    private float fireInterval;
    private int fireRow;

    private void Start()
    {
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

    public void SetStatus()
    {
        var damageLevel = SaveDataUtil.Status.GetDamageLevel();
        var rateLevel = SaveDataUtil.Status.GetRateLevel();

        damage = MasterRecords.GetDamageStatus(damageLevel);
        var rate = MasterRecords.GetRateStatus(rateLevel);
        var interval = 1 / rate;
        var ratio = MAX_FIRE_INTERVAL / interval;

        if (ratio > 1)
        {
            fireRow = (int)Math.Ceiling(ratio);
            fireInterval = MAX_FIRE_INTERVAL * ((float)fireRow / ratio);
        }
        else
        {
            fireRow = 1;
            fireInterval = interval;
        }
    }

    private void FixedUpdate()
    {
        animator.SetBool(Condition.isAim.ToString(), false);

        if (rangeColliderList.Count != 0)
        { 
            var closestCollider = GetClosestCollider();
            Fire(closestCollider.transform);
        }


        if (joystick != null)
        {
            Move();
            //SetDirectionToAnimator();
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

        if (vector == Vector3.zero)
        {
            isMove = false;
            animator.SetFloat(Condition.forward.ToString(), 0);
            return;
        }

        isMove = true;
        animator.SetFloat(Condition.forward.ToString(), 1);
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + vector * MOVE_SPEED);
        transform.rotation = Quaternion.LookRotation(vector);
    }

    private Vector3 GetBulletPosition(int index, int fireRow) {
        const float POSITION_INTERVAL = 0.1f;

        var isEven = fireRow % 2 == 0;
        var muzzlePosition = _muzzle.transform.position;
        var basePositionX = isEven
            ? muzzlePosition.x - (POSITION_INTERVAL / 2) - (POSITION_INTERVAL * ((fireRow / 2) - 1))
            : muzzlePosition.x - (POSITION_INTERVAL * ((fireRow / 2) - 1));
        var bulletPositionX = basePositionX + (POSITION_INTERVAL * index);

        return new Vector3(bulletPositionX, muzzlePosition.y, muzzlePosition.z);
    }

    private void Fire(Transform target) {
        // 移動中なら向きも変更しない
        if (isMove) return;

        var vector = GetXZPlaneVector(target.position - transform.position);
        transform.rotation = Quaternion.LookRotation(vector);

        if (!canFire) return;
        animator.SetBool(Condition.isAim.ToString(), true);
        canFire = false;
        Observable.Timer(TimeSpan.FromSeconds(fireInterval))
            .Do(_ => canFire = true)
            .Subscribe();

        var direction = GetXZPlaneVector(target.position - _muzzle.transform.position);
        for (var i = 0; i < fireRow; i++)
        {
            GameObject bullet = (GameObject)Instantiate(_bulletPrefab);
            bullet.transform.position = GetBulletPosition(i,fireRow);
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
