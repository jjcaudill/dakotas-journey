using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Make other classes to attach to enemy:
// tracker (moves towards Dakota, takes in transform)
// spawner (spawns prefab and applies a force in direction of dakota, optional)
public class Stalker : MonoBehaviour
{
    public Transform m_Target;
    public float m_Speed;
    public float m_Range;
    public bool m_UnlimitedRange = true;
    public Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    public LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    private float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private Rigidbody2D m_Rigidbody2D;
    private float m_OriginalXScale;
    private float m_Velocity;

    private void Start()
    {
        m_OriginalXScale = transform.localScale.x;
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Do nothing if out of range
        Vector3 diff = transform.position - m_Target.position;
        if (!m_UnlimitedRange && diff.magnitude > m_Range) {
            return;
        }

        Vector3 tempScale = transform.localScale;
        tempScale.x = m_OriginalXScale;
        m_Velocity = -1 * m_Speed;
        if (transform.position.x < m_Target.position.x){
            m_Velocity *= -1;
            tempScale.x *= -1;
        }

        transform.localScale = tempScale;

        m_Rigidbody2D.velocity = new Vector2(m_Velocity, m_Rigidbody2D.velocity.y);

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        bool grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject != gameObject) {
                grounded = true;
                break;
            }
        }

        if (!grounded) {
            transform.rotation = Quaternion.identity;
            m_Rigidbody2D.angularVelocity = 0;
        }
    }
}
