using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyTouch : MonoBehaviour
{
	public float m_Damage; // Amount  of damage it does when touched
	public float m_HitCooldown = 0.50f; // Number of seconds before it hits again
	public bool m_DeleteAfterAnyCollision = false;
	public bool m_DeleteAfterPlayerCollision = false;

	private bool m_HasHit = false; // Whether or not enemy is on cooldown

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_HasHit || !collision.gameObject.CompareTag("Player")){
        	if(m_DeleteAfterAnyCollision)
				Destroy(gameObject); 
            return;
        }

		m_HasHit = true;
        collision.gameObject.SendMessage("ApplyDamage", m_Damage);
        collision.gameObject.SendMessage("ApplyForce", transform.position);

		if (m_DeleteAfterAnyCollision || m_DeleteAfterPlayerCollision)
			Destroy(gameObject); 
	
    	StartCoroutine(HitCooldown());
	}

	private IEnumerator HitCooldown() {
        yield return new WaitForSeconds(m_HitCooldown);
        m_HasHit = false;
    }
}
