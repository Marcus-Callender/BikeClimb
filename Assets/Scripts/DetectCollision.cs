using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    public bool m_collisionDetected = false;
    
    //private void FixedUpdate()
    private void LateUpdate()
    {
        m_collisionDetected = false;
    }
    
    /*private void OnTriggerStay(Collider other)
    {
        m_collisionDetected = true;
    }*/

    private void OnTriggerEnter(Collider other)
    {
        m_collisionDetected = true;
    }
}
