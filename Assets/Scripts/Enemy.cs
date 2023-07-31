using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("General Info")]
    [SerializeField] protected EnemyType enemyType;

    [Header("Movement")]
    [SerializeField] protected Vector3 targetPosition;

    [Header("Player Detection")]
    [SerializeField] protected float detectionRadius;
    public bool playerDetected;
    public GameObject player;

    [Header("Attacking")]
    [SerializeField] protected bool closeEnoughToAttack;
    [SerializeField] protected bool isAttacking;
    [SerializeField] protected int damage;

    [Header("Health")]
    [SerializeField]protected float Health;
    protected bool isDead;


    protected Coroutine currentAttack;
    protected Coroutine currentMovementDelay;
    protected PlayerController playerController;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField]protected NavMeshAgent navMeshAgent;

    //for damage taking purposes
    protected Shooting shooting;

    public enum EnemyType
    {
        carrot,
        broccoliParent,
        broccoliKid,
        broccoliBaby,
        cabbage
    }


    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        shooting = GameObject.Find("RotatePoint").GetComponent<Shooting>();
        if (enemyType == EnemyType.carrot)
        {
            SetNewDestination();
            currentMovementDelay = StartCoroutine(DestinationChangeDelay());
        }

        else if (enemyType == EnemyType.broccoliParent || enemyType == EnemyType.broccoliKid || enemyType == EnemyType.broccoliBaby||enemyType==EnemyType.cabbage)
        {
            // instantly make the broccolis and cabbages attack towards the player
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, 30f, LayerMask.GetMask("Player"));

            if (playerCollider != null)
            {
                player = playerCollider.gameObject;
                playerController = player.GetComponent<PlayerController>();
                playerDetected = true;
                isAttacking = true;
                navMeshAgent.isStopped = true;


                currentAttack = StartCoroutine(Attack());
            }
        }
        
        if (gameObject.name.Contains("Carrot"))
        {
            Health = 4;
        }
        
    }

    void Update()
    {
        if (!isAttacking && enemyType == EnemyType.carrot)
        {
            DetectPlayer();
        }
        //if (playerDetected)
        //{
        //    navMeshAgent.SetDestination(playerController.gameObject.transform.position);
        //}
    }

    public virtual void SetNewDestination()
    {
        Vector3 newDirection = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f));

        if (newDirection.x > transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        navMeshAgent.SetDestination(newDirection);

    }

    protected virtual IEnumerator DestinationChangeDelay()
    {
        yield return new WaitForSeconds(Random.Range(1.5f, 3.5f));


        navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(2f);

        SetNewDestination();
        navMeshAgent.isStopped = false;

        currentMovementDelay = StartCoroutine(AlternativeDestinationChangeDelay());
    }

    protected virtual IEnumerator AlternativeDestinationChangeDelay()
    {
        yield return new WaitForSeconds(Random.Range(1.5f, 3.5f));


        navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(2f);

        SetNewDestination();
        navMeshAgent.isStopped = false;

        currentMovementDelay = StartCoroutine(DestinationChangeDelay());
    }

    protected virtual void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Player"));

        if (playerCollider != null)
        {
            Debug.Log("player has been detected");

            player = playerCollider.gameObject;
            playerController = player.GetComponent<PlayerController>();
            playerDetected = true;
            isAttacking = true;
            StopCoroutine(currentMovementDelay);
            navMeshAgent.isStopped = true;
            StartCoroutine(SpottingDelayAfterPlayerDetection());

            // increase the speed for upcoming attacks compared to idle
            navMeshAgent.speed = navMeshAgent.speed * 1.7f;
            navMeshAgent.acceleration = navMeshAgent.acceleration * 1.7f;
        }
    }

    protected virtual IEnumerator SpottingDelayAfterPlayerDetection()
    {
        yield return new WaitForSeconds(1f);

        currentAttack = StartCoroutine(Attack());
    }

    protected virtual IEnumerator Attack()
    {
        // delay between each attack, can later be removed if needed
        yield return new WaitForSeconds(1f);

        navMeshAgent.isStopped = false;
        targetPosition = player.transform.position;

        while (isAttacking && !isDead)
        {
            targetPosition = player.transform.position;
            navMeshAgent.SetDestination(targetPosition);

            if (targetPosition.x > transform.position.x)
            {
                spriteRenderer.flipX = true;
            }

            else
            {
                spriteRenderer.flipX = false;
            }

            if (closeEnoughToAttack)
            {
                DealDamage();
                yield break;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    public virtual void DealDamage()
    {
        if (playerController.invincible == false)
        {

            playerController.health -= damage;
            RestartAttack();
            StartCoroutine(playerInvincibility());

        }


        //StartCoroutine(Retreat());

         IEnumerator playerInvincibility()
        {
            playerController.invincible = true;
            yield return new WaitForSeconds(1f);
            playerController.invincible = false;
        }
    }

    public virtual void TakeDamage()
    {
        Health -= 1;
        if (Health <= 0)
        {

                    isDead = true;

                    Destroy(gameObject);

            }
    }


    protected virtual void RestartAttack()
    {
        StopCoroutine(currentAttack);

        isAttacking = true;

        currentAttack = StartCoroutine(Attack());
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isAttacking)
        {
            closeEnoughToAttack = false;
        }
    }

    public virtual void GameOver()
    {
        StopAllCoroutines();
    }
}

