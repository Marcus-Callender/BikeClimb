using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_BIKE_STATE
{
    IDLE,
    SKID,
    CROUCH,
    JUMPING,
    WHEELE_UP,
    WHEELE_DOWN,
    LEAPING,
    WALL_RIDING
}

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

    [SerializeField]
    public bool m_StayOnLastFrame;
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
    private float m_wallAcceleration;

    [SerializeField]
    private float m_maxSpeed;

    [SerializeField]
    private float m_hopVelocity = 5.0f;

    [SerializeField]
    private float m_leapVelocity = 7.5f;

    [SerializeField]
    private DetectCollision m_colliderRight;

    [SerializeField]
    private DetectCollision m_colliderLeft;

    [SerializeField]
    private DetectCollision m_colliderTop;

    [SerializeField]
    private DetectCollision m_colliderBottom;

    private Vector3 m_currentSpeed;
    private float m_animTime = 0.0f;
    private int m_prevSpriteAnimIndex = 0;

    private Vector3 m_previousPos;

    void Start()
    {
        m_previousPos = transform.position;

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
        m_currentSpeed = m_rigb.velocity;

        float targateSpeed = Input.GetAxisRaw("Horizontal") * m_maxSpeed;

        if ((m_colliderLeft.enter || m_colliderRight.enter) && (!m_colliderTop.stay && !m_colliderBottom.stay))
        {
            m_spriteAnimIndex = (int)E_BIKE_STATE.WALL_RIDING;
            m_rigb.useGravity = false;
        }

        if (m_spriteAnimIndex == (int)E_BIKE_STATE.WALL_RIDING)
        {
            if (!m_colliderLeft.stay && !m_colliderRight.stay)
            {
                m_spriteAnimIndex = (int)E_BIKE_STATE.LEAPING;
                m_rigb.useGravity = true;
            }

            m_currentSpeed.y += m_wallAcceleration * Time.deltaTime;
            //m_currentSpeed.y = m_acceleration * Time.deltaTime;

            if (Input.GetButtonDown("Jump"))
            {
                m_currentSpeed = new Vector3(m_colliderLeft.stay ? m_acceleration : -m_acceleration, m_leapVelocity);
            }

            if (Mathf.Abs((transform.position - m_previousPos).magnitude) < 0.01f)
            {
                Vector3 tmp = transform.position;
                tmp.x += m_colliderLeft.stay ? 0.001f : -0.001f;
                transform.position = tmp;
            }
        }
        else
        {
            if (Input.GetAxisRaw("Vertical") < -0.9f)
            {
                targateSpeed = 0.0f;
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (IsWheeleing())
                {
                    m_spriteAnimIndex = (int)E_BIKE_STATE.LEAPING;
                    m_currentSpeed.y = m_leapVelocity;
                }
                else
                {
                    m_currentSpeed.y = m_hopVelocity;
                }
            }

            if (Dir(targateSpeed) != 0.0f)
            {
                m_currentSpeed.x += m_acceleration * Dir(targateSpeed) * Time.deltaTime;
            }

            if (Dir(targateSpeed) != Dir(m_currentSpeed.x) && Dir(m_currentSpeed.x) != 0.0f)
            {
                float oldDir = Dir(m_currentSpeed.x);

                m_currentSpeed.x += m_deceleration * -Dir(m_currentSpeed.x) * Time.deltaTime;

                if (oldDir > 0.0f)
                {
                    m_currentSpeed.x = Mathf.Clamp(m_currentSpeed.x, 0.0f, m_maxSpeed);
                }
                else
                {
                    m_currentSpeed.x = Mathf.Clamp(m_currentSpeed.x, -m_maxSpeed, 0.0f);
                }
            }
        }

        m_currentSpeed.x = Mathf.Clamp(m_currentSpeed.x, -m_maxSpeed, m_maxSpeed);

        m_rigb.velocity = m_currentSpeed;

        if ((m_currentSpeed.x != 0.0f) && (m_spriteAnimIndex != (int)E_BIKE_STATE.WALL_RIDING))
            m_spriteRend.flipX = m_currentSpeed.x < 0.0f;

        m_previousPos = transform.position;

        UpdateAnims(targateSpeed);
    }

    void UpdateAnims(float targateSpeed)
    {
        if (m_spriteAnimIndex == (int)E_BIKE_STATE.WALL_RIDING)
        {

        }
        else if (m_spriteAnimIndex == (int)E_BIKE_STATE.LEAPING)
        {
            if (m_colliderRight.enter || m_colliderLeft.enter || m_colliderBottom.enter)
            {
                m_spriteAnimIndex = (int)E_BIKE_STATE.IDLE;
            }
        }
        else if (Input.GetAxisRaw("Vertical") < 0.9f && (m_spriteAnimIndex == (int)E_BIKE_STATE.WHEELE_UP || m_spriteAnimIndex == (int)E_BIKE_STATE.WHEELE_DOWN))
        {
            m_spriteAnimIndex = (int)E_BIKE_STATE.WHEELE_DOWN;
        }
        else if (Input.GetAxisRaw("Vertical") > 0.9f && (m_currentSpeed.x > m_maxSpeed * 0.75f || m_currentSpeed.x < -m_maxSpeed * 0.75f))
        {
            m_spriteAnimIndex = (int)E_BIKE_STATE.WHEELE_UP;
        }
        else if (Input.GetAxisRaw("Vertical") < -0.9f)
        {
            m_spriteAnimIndex = (int)E_BIKE_STATE.CROUCH;
        }
        else if ((m_currentSpeed.x > 0.0f && targateSpeed < 0.0f) || (m_currentSpeed.x < 0.0f && targateSpeed > 0.0f))
        {
            m_spriteAnimIndex = (int)E_BIKE_STATE.SKID;
        }
        else
        {
            m_spriteAnimIndex = (int)E_BIKE_STATE.IDLE;
        }

        if (m_prevSpriteAnimIndex != m_spriteAnimIndex)
        {
            m_animTime = 0.0f;
        }

        m_animTime += Time.deltaTime;

        if (m_animTime > m_anims[m_spriteAnimIndex].m_totalTime && !m_anims[m_spriteAnimIndex].m_StayOnLastFrame)
        {
            if (m_spriteAnimIndex == (int)E_BIKE_STATE.WHEELE_DOWN)
            {
                m_spriteAnimIndex = (int)E_BIKE_STATE.IDLE;
            }

            m_animTime = 0.0f;
        }

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

    private bool IsWheeleing()
    {
        return m_spriteAnimIndex == (int)E_BIKE_STATE.WHEELE_UP && m_animTime > m_anims[(int)E_BIKE_STATE.WHEELE_UP].m_totalTime;
    }

    private float Dir(float z)
    {
        return z > 0.0f ? 1.0f : (z < 0.0f ? -1.0f : 0.0f);
    }
}
