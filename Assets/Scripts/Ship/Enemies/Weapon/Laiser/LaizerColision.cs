using UnityEngine;

public class LaizerColision : MonoBehaviour
{
    public ParticleSystem m_ParticleSystem;
    public Vector3 m_Directions;
    public float m_Speed;

    public void Initialize(Vector3 direction, Vector3 position, float speed)
    {
        m_Directions = direction;
        transform.position = position;
        m_Speed = speed;
    }

    private void Awake()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    public void Shoot()
    {
        gameObject.SetActive(true);
        m_ParticleSystem.Play();
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameObject.activeSelf)
        {
            gameObject.transform.eulerAngles = m_Directions;

            Vector3 scale = transform.localScale;

            if (m_Directions.y > 0)
            {
                scale -= transform.right * m_Speed * Time.deltaTime;
            }
            else if (m_Directions.y < 0)
            {
                scale += transform.right * m_Speed * Time.deltaTime;
            }

            transform.localScale = scale;

            transform.localScale = new Vector3(2, 2,
                 Mathf.Clamp(transform.localScale.z, -300, 300)
                );
        }
    }

    public void ResetLaizer()
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<IDamagable>() != null && other.GetComponent<PlayerShipElement>() != null)
            other.GetComponent<IDamagable>().TakeDamage(1f);
    }
}