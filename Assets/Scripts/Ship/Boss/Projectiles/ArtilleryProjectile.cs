using System.Collections;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class ArtilleryProjectile : EnemyProjectile
{
    public Vector3 target;
    public GameObject TargetPrefab;
    public GameObject WarningSignalPrefab;
    private GameObject WarningSignal;

    public float timer = 5;

    public float yVel;

    public bool fall;

    public float distFromTarget;

    public GameObject Player;

    private void OnEnable()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        timer = 4;
        fall = false;
        StartCoroutine(GetTarget());
    }
    public override void SetInitialReference()
    {
        base.SetInitialReference();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            fall = true;
        }
    }

    IEnumerator GetTarget()
    {

        while (!fall)
        {
            target = new Vector3(Player.transform.position.x, -50, Player.transform.position.z) + new Vector3(Random.insideUnitCircle.x * 5, 0, Random.insideUnitCircle.y * 5);
            yield return null;
        }

    }

    public override void Movement()
    {

        if (!fall)
        {
            Vector3 direction = (speed *
                transform.up) + (speed / 2) * transform.forward;
            rigid.MovePosition(transform.position + direction * Time.deltaTime);
        }
        else if (fall)
        {
            Vector3 direction = target - transform.position;
            direction.Normalize();
            Debug.DrawRay(transform.position, direction, Color.green);
            rigid.MovePosition(transform.position + direction * speed * Time.deltaTime);

            if (Vector3.Distance(target, transform.position) < 2)
            {
                DestoryNow();
            }
        }
    }

}