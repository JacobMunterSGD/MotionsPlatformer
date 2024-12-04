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

    SpriteRenderer sr;

    void Start()
    {
        player = GameObject.Find("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        bombTimer = bombTimerStartValue;

        rb.velocity = playerRB.velocity;

        sr.color = new Color(0, 0, 0);

    }

    void Update()
    {
        if (bombTimer > 0)
        {
            bombTimer -= Time.deltaTime;
            sr.color = new Color(sr.color.r + Time.deltaTime, 0, 0);
        }
        else
        {
            Vector2 bombExplosionVelocityOnPlayer = (
                new Vector2(player.transform.position.x, player.transform.position.y)
                - new Vector2(transform.position.x, transform.position.y)).normalized;

            float distanceBetweenBombandPlayer = Vector2.Distance(
                new Vector2(transform.position.x, transform.position.y),
                new Vector2(player.transform.position.x, player.transform.position.y));

            playerRB.velocity += bombExplosionVelocityOnPlayer
                * (1 / distanceBetweenBombandPlayer)
                * bombPower;

            Destroy(gameObject);
        }
    }
}
