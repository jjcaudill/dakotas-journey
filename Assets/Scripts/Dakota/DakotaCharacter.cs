using System;
using UnityEngine;

namespace Dakota
{
    public class DakotaCharacter : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 300f;                  // Amount of force added when the player jumps.
        [SerializeField] private bool m_AirControl = true;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField] private float m_Reach = 2.0f;                  // A mask determining what is ground to the character
        [SerializeField] private Transform m_HoldPoint;                  // Where Dakota's mouth is
        [SerializeField] private int m_TimeUntilSitting = 10;                  // Idle time until Dakota will sit
        [SerializeField] private float m_DefaultRotation = 0.0f;                  // Dakota's default Z rotation

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
        private int m_IdleTime = 0;
        private bool m_Grabbing = false;
        private HingeJoint2D m_BiteJoint;

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
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
            if (m_IdleTime > m_TimeUntilSitting)
                m_Anim.SetBool("Idle", true);
            else
                m_Anim.SetBool("Idle", false);
        }


        public void Move(float move, bool jump, bool bark)
        {
            m_Anim.SetBool("Bark", bark);
            if (bark)
                return;

            // We will be able to rotate, so get right side up when jumping
            if (!m_Grounded && !m_Grabbing) {
                transform.rotation = Quaternion.identity;
            }

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
            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_IdleTime = 0;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
        }

        public void Grab() {
            Debug.Log("Grabbing...");
            if (m_Grabbing) {
                m_Anim.SetBool("Grab", false);
                m_Grabbing = false;
                // Disable joint
                m_BiteJoint.enabled = false;
                return;
            }

            // Check if we are close to a grabable object
            Physics2D.queriesStartInColliders = false; // ignore yourself
            RaycastHit2D hit = Physics2D.Raycast(m_HoldPoint.position, Vector2.right * transform.localScale.x, m_Reach); // is there an object within reach
            if(hit.collider != null && hit.collider.tag == "Grabbable") {
                m_Anim.SetBool("Grab", true);
                m_Grabbing = true;
                Debug.Log("Grabbed!");
                // Enable joint
                // Set connected rigidbody to hit.rigidbody
                m_BiteJoint.enabled = true;
                m_BiteJoint.connectedBody = hit.rigidbody;
            }
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

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(m_HoldPoint.position, m_HoldPoint.position + Vector3.right * transform.localScale.x * m_Reach);
        }
    }
}
