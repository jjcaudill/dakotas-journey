using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float m_Damage = 0.0f; // Damage the projectile does
    [SerializeField] private int m_TTL = 1000; // Time to live

    private int m_AliveTime;
    private Animator m_Anim;            // Reference to the player's animator component.
    
    // Start is called before the first frame update
    private void Start()
    {
        m_Anim = GetComponent<Animator>();
        m_AliveTime = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_AliveTime > m_TTL) {
        	// Destruct object
            m_Anim.SetBool("Die", true);
        }

        ++m_AliveTime;
    }
}
