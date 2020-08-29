using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Dakota {

	// [RequireComponent(typeof (DakotaCharacter))]
	public class GrabScript : MonoBehaviour {
        private DakotaCharacter m_Character;
		public bool grabbed;
		RaycastHit2D hit;
		public float reach = 4f;
		public Transform holdpoint;
		public float throwforce;
		public LayerMask notgrabbed;

		// Use this for initialization
		void Start () {
	        // private DakotaCharacter m_Character;
		}
		
		// Want to have grabable and movable. 
		// Moveable: change position to Dakota's holdposition
		// Grabable: change dakota's position to objects
		void Update () {
		
			if(CrossPlatformInputManager.GetButtonDown("Fire2")) {
				if(!grabbed) {
					Physics2D.queriesStartInColliders = false; // ignore yourself
					hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, reach); // is there an object within reach
					if(hit.collider != null && hit.collider.tag == "Grabbable")
						grabbed = true;
				} else {
					grabbed=false;
				}
			}

			// Move object to Dakota's mouth
			if (grabbed)
				hit.collider.gameObject.transform.position = holdpoint.position;
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position,transform.position + Vector3.right * transform.localScale.x * reach);
		}
	}

}
