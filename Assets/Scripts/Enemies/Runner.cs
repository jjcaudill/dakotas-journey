using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
    public Transform m_Target;
    public float m_Speed;
    public float m_Range;
    public bool m_UnlimitedRange = true;
    public Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    public LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
    public float m_SpeedUpRate = 1.0f; // Rate at which Speed overtakes current velocity [0,1] 

    private float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private Rigidbody2D m_Rigidbody2D;

    private void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Do nothing if out of range
        Vector3 diff = transform.position - m_Target.position;
        if (!m_UnlimitedRange && diff.magnitude > m_Range) {
            return;
        }

        m_Rigidbody2D.velocity = new Vector2((m_Speed * (m_SpeedUpRate) + m_Rigidbody2D.velocity.x * (1.0f - m_SpeedUpRate)), m_Rigidbody2D.velocity.y);

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
