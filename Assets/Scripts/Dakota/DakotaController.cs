using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Dakota
{
    [RequireComponent(typeof (DakotaCharacter))]
    public class DakotaController : MonoBehaviour
    {
        private DakotaCharacter m_Character;
        private bool m_Jump;
        private bool m_Bark;

        private void Awake()
        {
            m_Character = GetComponent<DakotaCharacter>();
        }


        private void Update()
        {
            if (!m_Jump)
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");

            if (!m_Bark)
                m_Bark = CrossPlatformInputManager.GetButtonDown("Fire1");

            if (CrossPlatformInputManager.GetButtonDown("Fire2"))
            {
                m_Character.Grab();
            }
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            m_Character.Move(h, m_Jump, m_Bark);
            m_Jump = false;
            m_Bark = false;
        }
    }
}
