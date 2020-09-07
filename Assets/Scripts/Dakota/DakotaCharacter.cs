using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class DakotaCharacter : MonoBehaviour
{
    public float m_StartingHP = 15.0f;
    public float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    public Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    public LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
    public bool m_AirControl = true;                 // Whether or not a player can steer while jumping;
    public float m_JumpForce = 300f;                  // Amount of force added when the player jumps.
    public bool m_CanDoubleJump = false;                  // Dakota's default Z rotation
    public float m_DoubleJumpForce = 200f;                  // Amount of force added when the player jumps.
    public float m_Reach = 2.0f;                  // A mask determining what is ground to the character
    public Transform m_HoldPoint;                  // Where Dakota's mouth is
    public int m_IdleThreshold = 100;                  // Idle time until Dakota will sit
    public float m_DefaultRotation = 0.0f;                  // Dakota's default Z rotation
    public bool m_CanAttack = true;                  // Dakota's default Z rotation
    public bool m_CanShoot = true;                  // Dakota's default Z rotation
    public float m_AttackDmg = 10.0f;                  // Dakota's default Z rotation
    public float m_AttackReach = 2.0f;                  // Dakota's default Z rotation
    public GameObject m_ProjectileObject;                  // Dakota's default Z rotation
    public Transform m_ShootingPoint;                  // Dakota's default Z rotation

    private bool m_CanMove = true;
    private float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Animator m_Anim;            // Reference to the player's animator component.
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private int m_IdleTime = 0;
    private bool m_Grabbing = false;
    private HingeJoint2D m_BiteJoint;
    private bool m_HasDoubleJumped;
    private bool m_HasAttacked = false;
    private bool m_Invulnerable = false;
    private float m_CurrentHP;

    private void Awake()
    {
        m_CurrentHP = m_StartingHP;
        // Setting up references.
        m_Anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_BiteJoint = GetComponent<HingeJoint2D>();
    }

    private void FixedUpdate()
    {
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                m_Grounded = true;
        }
        m_Anim.SetBool("Ground", m_Grounded);

        // Set the vertical animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        m_IdleTime++;
        if (m_IdleTime > m_IdleThreshold)
            m_Anim.SetBool("Idle", true);
        else
            m_Anim.SetBool("Idle", false);
    }

    public void Attack() {
        if (!m_CanAttack || m_HasAttacked) 
            return;

        m_Anim.SetBool("Attack", true);
        m_HasAttacked = true;

        RaycastHit2D[] hits = Physics2D.RaycastAll(m_HoldPoint.position, Vector2.right * transform.localScale.x, m_Reach);
        foreach (RaycastHit2D hit in hits) {
            if(hit.collider != null && hit.collider.tag != "Player") {
                hit.collider.gameObject.GetComponent<EnemyCharacter>().ApplyDamage(m_AttackDmg, transform.position);
            }
        }

        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown() {
        yield return new WaitForSeconds(0.25f);
        m_HasAttacked = false;
        m_Anim.SetBool("Attack", false);
    }

    public void Shoot() {
        if (!m_CanShoot)
            return;

        m_Anim.SetBool("Shoot", true);
        // Spawn a projectile
        GameObject projectile = Instantiate(m_ProjectileObject, m_ShootingPoint.position, Quaternion.identity) as GameObject; 
        Vector2 direction = new Vector2(transform.localScale.x, 0);
        projectile.GetComponent<ProjectileObject>().Direction(direction); 
        projectile.name = "Borf";
        StartCoroutine(ShootCooldown());
    }

    private IEnumerator ShootCooldown() {
        yield return new WaitForSeconds(0.1f);
        m_Anim.SetBool("Shoot", false);
    }

    public void Move(float move, bool jump)
    {
        if (!m_CanMove || m_HasAttacked)
        {
            return; // Can't move while attacking
        }

        // We will be able to rotate, so get right side up when jumping
        if (!m_Grounded && !m_Grabbing) {
            transform.rotation = Quaternion.identity;
            m_Rigidbody2D.angularVelocity = 0;
        }

        if (m_Grounded)
            m_HasDoubleJumped = false;

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            float abs_move = Mathf.Abs(move);
            if (abs_move > 0.0f)
                m_IdleTime = 0;

            // The Speed animator parameter is set to the absolute value of the horizontal input.
            m_Anim.SetFloat("Speed", abs_move);

            // Move the character
            m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed, m_Rigidbody2D.velocity.y);

            // If the input is moving the player right and the player is facing left...
            if (!m_Grabbing && move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
                // Otherwise if the input is moving the player left and the player is facing right...
            else if (!m_Grabbing && move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }

        // If the player should jump...
        if (jump) {
            if (m_Grabbing)
                LetGo();
            if (m_Grounded && m_Anim.GetBool("Ground")) {
                m_Grounded = false;
                m_IdleTime = 0;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            } else if (m_CanDoubleJump && !m_HasDoubleJumped) {
                m_HasDoubleJumped = true;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_DoubleJumpForce));
            }
        }
    }

    public void Grab() {
        if (m_Grabbing){
            LetGo();
            return;          
        }

        // Check if we are close to a grabable object
        Physics2D.queriesStartInColliders = false; // ignore yourself
        RaycastHit2D hit = Physics2D.Raycast(m_HoldPoint.position, Vector2.right * transform.localScale.x, m_Reach); // is there an object within reach
        if(hit.collider != null && hit.collider.tag == "Grabbable") {
            m_Anim.SetBool("Grab", true);
            m_Grabbing = true;
            // Move Dakota bite point to collision location
            Vector3 hitPoint = hit.point;
            transform.position = transform.position + (hitPoint - m_HoldPoint.position);
            // Start up hinge joint to object
            m_BiteJoint.enabled = true;
            m_BiteJoint.connectedBody = hit.rigidbody;
        }
    } 

    public void LetGo() {
        m_Anim.SetBool("Grab", false);
        m_Grabbing = false;
        m_BiteJoint.enabled = false;
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void Heal(float life)
    {
        m_CurrentHP += life;
        m_CurrentHP = Mathf.Max(m_CurrentHP, m_StartingHP);
    }

    public void ApplyDamage(float damage, Vector3 position) 
    {
        if (!m_Invulnerable)
        {
            m_Anim.SetBool("Hit", true);
            m_CurrentHP -= damage;
            Vector2 damageDir = Vector3.Normalize(transform.position - position) * 40f ;
            m_Rigidbody2D.velocity = Vector2.zero;
            m_Rigidbody2D.AddForce(damageDir * 10);

            if (m_CurrentHP < 0)
            {
                StartCoroutine(RipDakota());
            }
            else
            {
                StartCoroutine(Stun(0.25f));
                StartCoroutine(MakeInvincible(1f));
            }
        }
    }

    private IEnumerator Stun(float time) 
    {
        m_CanMove = false;
        yield return new WaitForSeconds(time);
        m_Anim.SetBool("Hit", false);
        m_CanMove = true;
    }

    private IEnumerator MakeInvincible(float time) 
    {
        m_Invulnerable = true;
        yield return new WaitForSeconds(time);
        m_Invulnerable = false;
    }

    private IEnumerator RipDakota()
    {
        m_Anim.SetBool("Die", true);
        m_CanMove = false;
        m_Invulnerable = true;
        m_CanAttack = false;
        yield return new WaitForSeconds(0.4f);
        m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawLine(m_HoldPoint.position, m_HoldPoint.position + Vector3.right * transform.localScale.x * m_Reach);
    // }
}
