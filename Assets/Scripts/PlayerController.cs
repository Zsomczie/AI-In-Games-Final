using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Vector2 speed = new Vector2(5, 5);
    public int health = 6;
    public bool invincible;
    private Shooting shooting;

    //variables for animation purposes
    public float horizontalMove;
    public float verticalMove;
    private void Start()
    {
        gameObject.name = "Player";
        shooting = GetComponentInChildren<Shooting>();
    }
    // Update is called once per frame
    void Update()
    {
        //idle animation here
        float inputY = Input.GetAxis("Vertical");
        float inputX = Input.GetAxis("Horizontal");

        if (inputX!=0||inputY!=0)
        {
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        }
        Vector3 movement = new Vector3(speed.x * inputX, speed.y * inputY, 0);

        movement *= Time.deltaTime;
        //walking animation here
        horizontalMove = Mathf.Abs(movement.x);
        verticalMove = movement.y;

        transform.Translate(movement);
        if (health <= 0)
        {
            //dying animation here

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<Enemy>().GameOver();
            }

            Destroy(gameObject);
        }

    }
}
