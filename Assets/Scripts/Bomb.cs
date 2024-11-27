using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    GameObject player;
    Rigidbody2D playerRB;

    Rigidbody2D rb;

    float bombTimer;
    public float bombTimerStartValue;
    public float bombPower;

    void Start()
    {
        player = GameObject.Find("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();

        bombTimer = bombTimerStartValue;

        rb.velocity = playerRB.velocity;

    }

    void Update()
    {
        if (bombTimer > 0)
        {
            bombTimer -= Time.deltaTime;
        }
        else
        {
            Vector2 bombExplosionVelocityOnPlayer = (new Vector2(player.transform.position.x, player.transform.position.y) - new Vector2(transform.position.x, transform.position.y)).normalized;

            float distanceBetweenBombandPlayer = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.transform.position.x, player.transform.position.y));

            playerRB.velocity += bombExplosionVelocityOnPlayer * (1 / distanceBetweenBombandPlayer) * bombPower;

            Destroy(gameObject);
        }
    }
}
