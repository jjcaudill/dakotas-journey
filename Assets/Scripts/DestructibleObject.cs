using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private float m_StartingHP = 10f;                    // The fastest the player can travel in the x axis.
    private float m_CurrentHP;
    private bool m_Dying;
    private Animator m_Anim;            								  // Reference to the player's animator component.

    // Start is called before the first frame update
    private void Start()
    {
        m_CurrentHP = m_StartingHP;
        m_Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
		m_Anim.SetBool("Hit", false);
        if (m_CurrentHP < 0)
            m_Anim.SetBool("Die", true);
    }

    public void Hit(float hit)
    {   
        m_CurrentHP -= hit;
    	m_Anim.SetBool("Hit", true);
    }
}
