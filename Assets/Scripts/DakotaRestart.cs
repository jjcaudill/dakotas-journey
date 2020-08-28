using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dakota
{
    public class DakotaRestart : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
            }
        }
    }
}
