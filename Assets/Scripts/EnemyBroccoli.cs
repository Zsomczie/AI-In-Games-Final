using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBroccoli : Enemy
{
    [Header("For Broccoli Only")]
    [SerializeField] protected GameObject kidPrefab;
    [SerializeField] protected GameObject babyPrefab;
    [SerializeField] protected Transform kidSpawnPoint;
    protected string currentState;
    private void Start()
    {
        if (gameObject.name.Contains("Parent"))
        {
            Health = 6;
            currentState = "Kid";
        }
        else if (gameObject.name.Contains("Kid"))
        {
            Health = 3;
            currentState = "Baby";
        }
        else if (gameObject.name.Contains("Baby"))
        {
            Health = 1;
            currentState = "Dying";
        }
    }
    protected void SpawnNextBroccoliState(string state, int index)
    {
        GameObject newBroccoli = new GameObject();

        switch (state)
        {

            case "kid":
                if (index == 0)
                {
                    newBroccoli = Instantiate(kidPrefab, kidSpawnPoint.GetChild(0).transform.position, Quaternion.identity);
                }

                else
                {
                    newBroccoli = Instantiate(kidPrefab, kidSpawnPoint.GetChild(1).transform.position, Quaternion.identity);
                }

                newBroccoli.GetComponentInChildren<SpriteRenderer>().sortingOrder = 3;

                break;
            case "baby":
                if (index == 0)
                {
                    newBroccoli = Instantiate(babyPrefab, kidSpawnPoint.GetChild(0).transform.position, Quaternion.identity);
                }

                else
                {
                    newBroccoli = Instantiate(babyPrefab, kidSpawnPoint.GetChild(1).transform.position, Quaternion.identity);
                }

                newBroccoli.GetComponentInChildren<SpriteRenderer>().sortingOrder = 4;

                break;
            default:
                Debug.LogError("No next state for " + gameObject.name + " could be spawned!");
                break;
        }
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
    public override void TakeDamage()
    {
        Health -= 1;
        if (Health <= 0)
        {
            switch (enemyType)
            {
                case EnemyType.broccoliParent:
                    int kidsToSpawn = 2;
                    for (int i = 0; i < kidsToSpawn; i++)
                    {
                        SpawnNextBroccoliState("kid", i);
                    }

                    isDead = true;

                    Destroy(gameObject);
                    break;

                case EnemyType.broccoliKid:
                    int babysToSpawn = 2;
                    for (int i = 0; i < babysToSpawn; i++)
                    {
                        SpawnNextBroccoliState("baby", i);
                    }


                    isDead = true;


                    Destroy(gameObject);
                    break;

                case EnemyType.broccoliBaby:

                    isDead = true;


                    Destroy(gameObject);
                    break;

                default:
                    Debug.LogError("No enemy type assigned for " + gameObject.name + ", can't be destroyed!");
                    break;
            }

        }
    }
}
