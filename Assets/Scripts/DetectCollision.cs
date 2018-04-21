using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    public bool enter = false;
    public bool stay = false;

    private void LateUpdate()
    {
        enter = false;
    }

    private void FixedUpdate()
    {
        stay = false;
    }

    private void OnTriggerStay(Collider other)
    {
        stay = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        enter = true;
    }
}
