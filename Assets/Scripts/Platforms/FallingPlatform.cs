using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float m_Delay = 0.5f;
    public float m_RespawnTime = 1.0f;

    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Origin;

    private void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Origin = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {   
        if (collision.gameObject.CompareTag("Player")) {
            StartCoroutine(Drop());
        }
    }

    private IEnumerator Drop()
    {
        yield return new WaitForSeconds(m_Delay);
        m_Rigidbody2D.isKinematic = false;
        yield return new WaitForSeconds(m_RespawnTime);
        m_Rigidbody2D.isKinematic = true;
        transform.position = m_Origin;
    }
}