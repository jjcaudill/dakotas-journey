using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof (DakotaCharacter))]
public class DakotaController : MonoBehaviour
{
    private DakotaCharacter m_Character;
    private bool m_Jump;
    private bool m_Attack;
    private bool m_Shoot;

    private void Awake()
    {
        m_Character = GetComponent<DakotaCharacter>();
    }


    private void Update()
    {
        if (!m_Jump)
            m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");

        if (!m_Attack)
            m_Attack = CrossPlatformInputManager.GetButtonDown("Fire1");

        if (!m_Shoot)
            m_Shoot = CrossPlatformInputManager.GetButtonDown("Fire2");

        if (CrossPlatformInputManager.GetButtonDown("Fire3"))
        {
            m_Character.Grab();
        }
    }


    private void FixedUpdate()
    {
        // Read the inputs.
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        // Pass all parameters to the character control script.
        if (m_Attack)
            m_Character.Attack();
        else if (m_Shoot)
            m_Character.Shoot();
        else 
            m_Character.Move(h, m_Jump);
            
        m_Jump = false;
        m_Attack = false;
        m_Shoot = false;
    }
}
