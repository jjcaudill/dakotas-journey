using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectileObject : MonoBehaviour
{
    public float m_Damage = 0.0f; // Damage the projectile does
    public int m_TTL = 1000; // Time to live
    public float m_Speed = 10.0f;

    public Vector2 m_Direction;
    private int m_AliveTime;
    private Animator m_Anim;            // Reference to the player's animator component.

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("ProjectileObject");
        m_Anim = GetComponent<Animator>();
        m_AliveTime = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_AliveTime > m_TTL) {
        	// Destruct object
            Destroy(gameObject); 
            // m_Anim.SetBool("Die", true);
        }

        GetComponent<Rigidbody2D>().velocity = m_Direction * m_Speed;
        ++m_AliveTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Ignore it hitting Dakota
            return;

        Debug.Log("OnCollisionEnter2D");
        collision.gameObject.GetComponent<EnemyCharacter>().ApplyDamage(m_Damage, transform.position);

        Destroy(gameObject); 
    }

    public void Direction(Vector2 direction)
    {
        m_Direction = direction;
    }
}
