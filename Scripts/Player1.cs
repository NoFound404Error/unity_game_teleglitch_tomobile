using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : LivingEntity
{
    [Header("Player Properties")]
    public JoystickInput joystick;
    public GameObject playerBody;
    public ParticleSystem deathEffect;

    public bool autoAim;
    public float MoveSpeed;
    public float turnSpeed;

    private float range = 15f;
    private Vector3 _moveVector;
    private Transform _transform;

    private Inventory inventory;
    private Animator playerAnim;
    private Transform target;

    protected override void Start()
    {
        base.Start();
        _transform = transform;
        _moveVector = Vector3.zero;
        playerAnim = transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Animator>();
        inventory = GetComponent<Inventory>();
        
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    private void Update()
    {
        HandleInput();
        if (joystick.trigger == true)
        {
            if (autoAim != true)
            {
                RotateToJoystick();
            }
            DoMove();

            if (inventory.highlightedItem != null)
            {
                if (inventory.highlightedItem.GetComponent<Item>().type == "Gun")
                    playerAnim.SetBool("Walk", true);
                else
                    playerAnim.SetBool("EmptyWalk", true);
            }
        }
        else if (joystick.trigger == false)
        {
            DontMove();
            if (inventory.highlightedItem != null)
            {
                if (inventory.highlightedItem.GetComponent<Item>().type == "Gun")
                    playerAnim.SetBool("Walk", false);
                else
                    playerAnim.SetBool("EmptyWalk", false);
            }
        }

        if (autoAim == true)
        {
            UpdateTarget();
            RotateToEnemy();
        }        
    }

    private void FixedUpdate()
    {
        Move();
    }
    
    public void HandleInput()
    {
        _moveVector = poolInput();
    }

    public Vector3 poolInput()
    {
        float h = joystick.GetHorizontalValue();    // 조이스틱의 x축 입력
        float v = joystick.GetVerticalValue();      // 조이스틱의 y축 입력
        Vector3 moveDir = new Vector3(h, 0, v).normalized;  // 조이스틱의 x,y 입력을 플레이어의 x,z 입력으로 적용한다

        return moveDir;
    }

    private void Move()
    {
        _transform.Translate(_moveVector * MoveSpeed * Time.deltaTime);
        
    }

    public void RotateToJoystick()
    {
        Vector3 moveDir = poolInput();
        Quaternion lookRotation = Quaternion.LookRotation(moveDir);
        Vector3 rotation = Quaternion.Lerp(playerBody.transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        playerBody.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    public void RotateToEnemy()
    {
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(playerBody.transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
            playerBody.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }
    }

    public void DontMove()
    {
        MoveSpeed = 0f;
    }

    public void DoMove()
    {
        MoveSpeed = 7.5f;
    }

    public void SwitchRotate()
    {
        if(autoAim == false)
        {
            autoAim = true;
            return;
        }
        else
        {
            autoAim = false;
            return;
        }
    }
    
    public void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Wall")
        {
            MoveSpeed = 0.5f;
        }
    }

    public override void TakeHit(float damage, RaycastHit hitPoint)
    {
        if (damage >= health)
        {
            Destroy(Instantiate(deathEffect.gameObject, hitPoint.point, Quaternion.FromToRotation(Vector3.forward, hitPoint.point)) as GameObject, 1f);           
        }
        base.TakeHit(damage, hitPoint);
    }


    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)    // 모든 적들을 찾아서
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position); // 적들까지의 거리를 측정하고
            if (distanceToEnemy < shortestDistance)  // 그 거리가 가장 짧은 거리라면
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;               // 현재 가장 가까운 적으로 등록
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)   // 범위안에 적이 있다면
        {
            target = nearestEnemy.transform;    // 타겟 설정
        }
        else
        {                                // 범위를 벗어나면
            target = null;                      // 타겟 해제
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
