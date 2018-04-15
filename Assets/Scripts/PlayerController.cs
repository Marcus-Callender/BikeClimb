﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct SpriteField
{
    [SerializeField]
    public Sprite sprite;

    [SerializeField]
    public float time;

    [HideInInspector, SerializeField]
    public float aggragateTime;
}

[System.Serializable]
struct SpriteAnim
{
    // for easier editing not for use in code
    [SerializeField]
    public string m_name;

    [SerializeField]
    public SpriteField[] m_fields;

    [HideInInspector, SerializeField]
    public float m_totalTime;
}

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody m_rigb;

    [SerializeField]
    private SpriteRenderer m_spriteRend;

    [SerializeField]
    private SpriteAnim[] m_anims;

    [SerializeField]
    private int m_spriteAnimIndex = 0;

    [SerializeField]
    private float m_acceleration;

    [SerializeField]
    private float m_deceleration;

    [SerializeField]
    private float m_maxSpeed;

    private Vector3 m_currentSpeed;
    private float m_animTime = 0.0f;
    private int m_prevSpriteAnimIndex = 0;

    void Start()
    {
        m_currentSpeed = Vector3.zero;

        for (int z = 0; z < m_anims.Length; z++)
        {
            float aggragateTime = 0.0f;

            for (int x = 0; x < m_anims[z].m_fields.Length; x++)
            {
                aggragateTime += m_anims[z].m_fields[x].time;
                m_anims[z].m_fields[x].aggragateTime = aggragateTime;
            }

            m_anims[z].m_totalTime = aggragateTime;
        }
    }

    void Update()
    {
        float targateSpeed = Input.GetAxisRaw("Horizontal") * m_maxSpeed;

        m_currentSpeed = m_rigb.velocity;

        if (targateSpeed > m_currentSpeed.x)
        {
            m_currentSpeed.x += m_acceleration * Time.deltaTime;

            if (Input.GetAxisRaw("Horizontal") == 0.0f)
                m_currentSpeed.x = Mathf.Clamp(m_currentSpeed.x, -m_maxSpeed, 0.0f);
        }
        else if (targateSpeed < m_currentSpeed.x)
        {
            m_currentSpeed.x -= m_acceleration * Time.deltaTime;

            if (Input.GetAxisRaw("Horizontal") == 0.0f)
                m_currentSpeed.x = Mathf.Clamp(m_currentSpeed.x, 0.0f, m_maxSpeed);
        }

        m_currentSpeed.x = Mathf.Clamp(m_currentSpeed.x, -m_maxSpeed, m_maxSpeed);

        m_rigb.velocity = m_currentSpeed;

        if (m_currentSpeed.x != 0.0f)
            m_spriteRend.flipX = m_currentSpeed.x < 0.0f;

        UpdateAnims(targateSpeed);
    }

    void UpdateAnims(float targateSpeed)
    {
        if ((m_currentSpeed.x > 0.0f && targateSpeed < 0.0f) || (m_currentSpeed.x < 0.0f && targateSpeed > 0.0f))
        {
            m_spriteAnimIndex = 1;
        }
        else
        {
            m_spriteAnimIndex = 0;
        }

        if (m_prevSpriteAnimIndex != m_spriteAnimIndex)
        {
            m_animTime = 0.0f;
        }

        m_animTime += Time.deltaTime;

        if (m_animTime > m_anims[m_spriteAnimIndex].m_totalTime)
            m_animTime = 0.0f;

        for (int z = 0; z < m_anims[m_spriteAnimIndex].m_fields.Length; z++)
        {
            if (m_animTime < m_anims[m_spriteAnimIndex].m_fields[z].aggragateTime)
            {
                m_spriteRend.sprite = m_anims[m_spriteAnimIndex].m_fields[z].sprite;
                break;
            }
        }

        m_prevSpriteAnimIndex = m_spriteAnimIndex;
    }
}