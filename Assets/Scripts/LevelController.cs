using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
	public string m_NextScene;
	// public bool m_SlowFade = false;

    void OnTriggerEnter2D(Collider2D collider)
    {
    	if (!collider.CompareTag("Player"))
    		return;

    	// if (m_SlowFade)
     //        StartCoroutine(FadeScene());
            
		SceneManager.LoadScene(m_NextScene);
    }

    // private IEnumerator FadeScene() 
    // {
    //     yield return new WaitForSeconds(3.0f);
    // }
}
