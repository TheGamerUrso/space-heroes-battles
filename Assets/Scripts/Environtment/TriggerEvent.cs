using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    public GameObject Event;
    public GameObject Explosion;
    public bool Right;
    private bool Activated;
    public float Timer;
    public float EventCountdown;

    private void Update()
    {
        if (Activated)
        {
            if (Timer > 0)
            {
                Timer -= Time.deltaTime;
            }
            else
            {
                Event.gameObject.transform.eulerAngles = Vector3.zero;
                Event.GetComponent<Animator>().SetTrigger("Reset");
                Explosion.SetActive(false);
            }
        }
    }
    public void ResetEvent()
    {
        Event.gameObject.transform.eulerAngles = Vector3.zero;
        Event.GetComponent<Animator>().SetTrigger("Reset");
        Explosion.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {


        if (other.gameObject.tag == "Player")
        {
            if (!Activated)
            {
                Activated = true;
                Timer = EventCountdown;
                if (Right)
                {
                    Event.GetComponent<Animator>().SetTrigger("FallRight");
                }
                else
                {
                    Event.GetComponent<Animator>().SetTrigger("Destroy");
                }

                Explosion.SetActive(true);
               // Explosion.GetComponent<ParticleSystem>().Play();
            }
        }

    }
}
