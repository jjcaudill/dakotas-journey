using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneryObject : MonoBehaviour
{
	public bool CanScroll = false;
	public bool CanParallax = false;
	public float ObjectSize;
	public float ParallaxSpeed;

	private Transform m_TransformCamera;
	private Transform[] m_Layers;
	private float m_Viewzone = 15;
	private int m_LeftIndex;
	private int m_RightIndex;
	private float m_LastCameraX;

    // Start is called before the first frame update
    private void Start()
    {
        m_TransformCamera = Camera.main.transform;
        m_LastCameraX = m_TransformCamera.position.x;
        m_Layers = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
    		m_Layers[i] = transform.GetChild(i);

    	m_LeftIndex = 0;
    	m_RightIndex = m_Layers.Length - 1;
    }	

    private void ScrollLeft()
    {
    	int lastRight = m_RightIndex;
        // m_Layers[m_RightIndex].position = Vector3.right * (m_Layers[m_LeftIndex].position.x - ObjectSize); // TODO: this always sets y as 0
    	m_Layers[m_RightIndex].position = new Vector3(m_Layers[m_LeftIndex].position.x - ObjectSize, m_Layers[m_LeftIndex].position.y, 0); // TODO: this always sets y as 0
    	m_LeftIndex = m_RightIndex;
    	m_RightIndex--;
    	if (m_RightIndex < 0)
    		m_RightIndex = m_Layers.Length - 1; 
    }

    private void ScrollRight()
    {
		int lastLeft = m_LeftIndex;
    	// m_Layers[m_LeftIndex].position = Vector3.right * (m_Layers[m_RightIndex].position.x + ObjectSize); // TODO: this always sets y as 0
        m_Layers[m_LeftIndex].position = new Vector3(m_Layers[m_RightIndex].position.x + ObjectSize, m_Layers[m_RightIndex].position.y, 0); // TODO: this always sets y as 0
    	m_RightIndex = m_LeftIndex;
    	m_LeftIndex++;
    	if (m_LeftIndex >= m_Layers.Length)
    		m_LeftIndex = 0; 
    }

    private void Update()
    {
    	if (CanParallax) {
	    	float deltaX = m_TransformCamera.position.x - m_LastCameraX;
	    	transform.position += Vector3.right * (deltaX * ParallaxSpeed);
	        m_LastCameraX = m_TransformCamera.position.x;
    	}

        if (CanScroll) {
	    	if (m_TransformCamera.position.x < (m_Layers[m_LeftIndex].transform.position.x + m_Viewzone))
	    		ScrollLeft(); 

	    	if (m_TransformCamera.position.x > (m_Layers[m_RightIndex].transform.position.x - m_Viewzone)) 
	    		ScrollRight(); 
        }
    }
}
