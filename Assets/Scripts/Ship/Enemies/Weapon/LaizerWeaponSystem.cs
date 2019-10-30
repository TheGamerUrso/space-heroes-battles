using System.Collections;
using UnityEngine;

public class LaizerWeaponSystem : MonoBehaviour
{
    [SerializeField] private GameObject m_Target;
    private Coroutine m_LaizerCoroutiner;
    public float m_Countdown;
    public float m_Delay;

    public ParticleSystem m_ParticleSystem;
    private bool m_FollowPlayerToggle;

    private Vector3 m_TempPosition;

    private Vector3[] m_Directions =
    {
        new Vector3(0,90,0),
        new Vector3(0,-90,0),
    };

    private Vector3[] m_ListOfLeftSidePositions = {
        new Vector3(-180,200,150),
        new Vector3(-180,200,100),
        new Vector3(-180,200,50),
        new Vector3(-180,200,0)
    };

    private Vector3[] m_ListOfRightSidePositions = {
        new Vector3(160,200,125),
        new Vector3(160,200,75),
        new Vector3(160,200,25)
    };

    public GameObject LaierObject;

    public LineRenderer m_LineRenderer;
    public BoxCollider m_BoxCollider;

    public float m_LaserBeamSize;
    public float m_LaserBeamWidth;

    public GameObject m_Explostion;
    private RaycastHit hit;
    private bool HitExplode = false;
    public float dist = 0;

    public bool HitingSomeone;

    //===========================================================================
    private void Start()
    {
        m_Countdown = m_Delay;
        m_LineRenderer.SetPosition(0, transform.position);

        m_LaserBeamWidth = 0;
    }

    //===========================================================================
    private void Update()
    {
        if (m_Target != null)
        {
            Shoot();

            if (m_FollowPlayerToggle)
            {
                m_TempPosition.z = m_Target.transform.position.z;
                transform.position = m_TempPosition;
            }
        }

        m_LineRenderer.SetPosition(0, transform.position);

        Vector3 test = transform.position + transform.forward * m_LaserBeamSize;

        m_LineRenderer.widthMultiplier = m_LaserBeamWidth;

        if (!HitingSomeone)
        {
            m_LineRenderer.SetPosition(1, test);
            m_Explostion.SetActive(false);
        }

        m_BoxCollider.enabled = false;
        if (m_LaserBeamWidth <= 10)
        {
            m_LaserBeamWidth += 5f * Time.deltaTime;
        }
        else
        {
            m_BoxCollider.enabled = true;
        }

        if (m_Target != null && m_Target.GetComponent<Player>() != null)
        {
            m_Target = null;
        }

        if (m_Target == null)
        {
            HitingSomeone = false;
        }
    }

    //===========================================================================
    public void Shoot()
    {
        if (m_Target == null)
        {
            m_Countdown = 1;
            return;
        }

        float damage = 1f;
        m_Target.GetComponent<IDestroyable>().TakeDamage(damage);

        HitingSomeone = false;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider.tag == Constants.PLAYTERTAG)
            {
                HitingSomeone = true;

                m_LineRenderer.widthMultiplier = m_LaserBeamWidth;
                Vector3 pos;
                pos = transform.position + (transform.forward * Vector3.Magnitude(transform.position - hit.point));
                m_LineRenderer.SetPosition(1, pos);
                m_Explostion.SetActive(true);
                m_Explostion.transform.position = pos;
            }

            m_LaizerCoroutiner = null;

            if (m_LaizerCoroutiner == null)
            {
                m_LaizerCoroutiner = StartCoroutine("Laizer");
            }
            else if (m_LaizerCoroutiner != null)
            {
                if (m_Countdown >= 0)
                {
                    m_Countdown -= Time.deltaTime;
                }
            }
        }
    }

    //===========================================================================
    public void SetTarget(GameObject target)
    {
        m_Target = target;
    }

    //===========================================================================
    private IEnumerator Laizer()
    {
        while (m_Target != null)
        {
            while (m_Countdown <= 0)
            {
                yield return null;
            }

            int num = Random.Range(0, m_Directions.Length);
            switch (num)
            {
                case 0:
                    m_TempPosition = m_ListOfLeftSidePositions[Random.Range(0, m_ListOfLeftSidePositions.Length)];
                    m_TempPosition.z = m_Target.transform.position.z;
                    transform.position = m_TempPosition;
                    break;

                case 1:
                    m_TempPosition = m_ListOfRightSidePositions[Random.Range(0, m_ListOfRightSidePositions.Length)];
                    m_TempPosition.z = m_Target.transform.position.z;
                    transform.position = m_TempPosition;
                    break;

                default:
                    break;
            }

            LaierObject.GetComponent<LaizerColision>().Initialize(m_Directions[num], transform.position, 250);

            yield return new WaitForSeconds(Random.Range(1, 3));

            LaierObject.GetComponent<LaizerColision>().Shoot();

            yield return new WaitForSeconds(Random.Range(1, 3));

            LaierObject.GetComponent<LaizerColision>().ResetLaizer();
            m_Countdown = m_Delay;
        }
    }

    //===========================================================================
    public void OnTriggerStay(Collider other)
    {
        GameObject obj = other.gameObject;
        string gameobjectTag = obj.tag;
        HitingSomeone = false;
        m_Target = null;
        if (gameobjectTag == Constants.PLAYTERTAG)
        {
            m_Target = obj;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;
        string gameobjectTag = obj.tag;

        if (gameobjectTag == Constants.PLAYTERTAG)
        {
            m_Target = null;
        }
    }

    //====================================================================================================
    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        string gameobjectTag = obj.tag;

        if (gameobjectTag == Constants.PLAYTERTAG)
        {
        }
    }

    private IEnumerator PlayEffect(RaycastHit position)
    {
        GameObject explosion = Instantiate(m_Explostion, position.point, Quaternion.identity);
        Destroy(explosion, 1f);
        yield return new WaitForSeconds(1f);
        HitExplode = false;
    }
}