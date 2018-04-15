using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player;

    [SerializeField]
    private Vector3 m_offset;

    void Start()
    {

    }

    void Update()
    {
        transform.position = m_player.transform.position + m_offset;
    }
}
