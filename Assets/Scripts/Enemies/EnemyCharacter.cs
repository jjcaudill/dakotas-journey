using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Make other classes to attach to enemy:
// tracker (moves towards Dakota, takes in transform)
// spawner (spawns prefab and applies a force in direction of dakota, optional)
public class EnemyCharacter : MonoBehaviour
{
    public float m_StartingHP = 10f;                    // The fastest the player can travel in the x axis.
    public bool m_KnockBack = false;
    public bool m_Invulnerable = false;

    public bool m_CanAttack = false;
    public float m_AttackDamage = 5.0f;
    public Transform m_AttackCheck;
    // TODO: Add option to respawn, will probably need enemy prefab to respawn

    private float k_AttackRadius = 0.25f; 
    private Rigidbody2D m_Rigidbody2D;
    private float m_CurrentHP;

    // Start is called before the first frame update
    private void Start()
    {

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_CurrentHP = m_StartingHP;
    }

    private void FixedUpdate()
    {
        if (m_CanAttack) 
            return;

        Collider2D collider = Physics2D.OverlapCircle(m_AttackCheck.position, k_AttackRadius, LayerMask.GetMask("Player"));
        if (collider) {
            collider.gameObject.SendMessage("ApplyDamage", m_AttackDamage);
            collider.gameObject.SendMessage("ApplyForce", m_AttackCheck);
        }
    }

    public void ApplyDamage(float damage) 
    {
        m_CurrentHP -= damage;

        if (!m_Invulnerable && m_CurrentHP < 0)
            Destroy(gameObject);
    }

    public void ApplyForce(Vector3 position)
    {
        if (m_KnockBack)
        {
            Vector2 damageDir = Vector3.Normalize(transform.position - position) * 40f ;
            m_Rigidbody2D.velocity = Vector2.zero;
            m_Rigidbody2D.AddForce(damageDir * 10);   
        }
    }

    // If they come in range
    //  Attack, do damage, stop moving while attacking, then if no longer colliding, reset speed
    private void 

}
