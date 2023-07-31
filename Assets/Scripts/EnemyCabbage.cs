using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCabbage : Enemy
{

    [Header("Movement")]
    [SerializeField]public float moveSpeed =50f;
    [SerializeField]public Vector3 lastVelocity;
    [SerializeField] private bool isRolling;


    [Header("Attacking")]
    public bool isRecharging;

    // general private variables
    private Coroutine restartCoroutine;

    [Header("For Script References Only")]
    public Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Health = 5;
    }
    void Update()
    {
        if (!isAttacking)
        {
            DetectPlayer();
        }

        lastVelocity = rb.velocity;

        if (isAttacking && isRolling && !isRecharging && rb.velocity.magnitude < 0.8f)
        {
            isRolling = false;

            if (restartCoroutine == null)
            {
                StartCoroutine(RestartAttack());
            }
        }
    }

    public override void SetNewDestination()
    {
        
        targetPosition = player.transform.position - transform.position;
        rb.AddForce(targetPosition * Time.deltaTime * moveSpeed * 150f);
        StartCoroutine(IsRollingDelay());
        Debug.Log("new path set");

    }

    protected override void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Player"));

        if (playerCollider != null)
        {
            Debug.Log("player has been detected");

            // spotting animation here!
            player = playerCollider.gameObject;
            playerController = player.GetComponent<PlayerController>();
            playerDetected = true;
            isAttacking = true;
            StartCoroutine(SpottingDelayAfterPlayerDetection());
        }
    }

    protected override IEnumerator SpottingDelayAfterPlayerDetection()
    {
        yield return new WaitForSeconds(2f);

        SetNewDestination();
    }

    private IEnumerator IsRollingDelay()
    {
        yield return new WaitForSeconds(0.2f);

        isRolling = true;
        yield return new WaitForSeconds(1f);
        rb.velocity = Vector3.zero;
    }

    public override void DealDamage()
    {
        playerController.health -= damage;
        playerController.invincible = true;
        StartCoroutine(RemoveInvincible());
    }
    private IEnumerator RemoveInvincible() 
    {
        yield return new WaitForSeconds(2f);
        playerController.invincible = false;
    }
    private new IEnumerator RestartAttack()
    {
        isRecharging = true;
        Debug.Log("isRecharging");


        // possible idle animation between attacks here!!

        yield return new WaitForSeconds(1f);

        SetNewDestination();

        isRecharging = false;
    }

    public override void GameOver()
    {
        Vector2 stopPosition = transform.position;
        transform.position = stopPosition;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isAttacking)
        {
            closeEnoughToAttack = true;
            DealDamage();
        }

        if (collision.gameObject.name.Contains("Bullet"))
        {
            TakeDamage();
            Destroy(collision.gameObject);
        }
    }
    public override void TakeDamage()
    {
        Health -= 1;
        if (Health<=0)
        {
            isDead = true;

            Destroy(gameObject);
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isAttacking)
        {
            closeEnoughToAttack = false;
        }
    }
}