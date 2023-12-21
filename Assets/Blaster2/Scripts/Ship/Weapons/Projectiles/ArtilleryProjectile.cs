using UnityEngine;

public class ArtilleryProjectile : EnemyProjectile
{
    public PlayerShip playerShip;
    public float timer = 5;
    public bool fall;
    public Vector3 TargetPos;
    public GameObject warningPrefab;
    private SizeByDistance warning;
    public float distance;
    public override void OnStart()
    {
        base.OnStart();
        timer = 1;
        fall = false;

        if (warning == null)
        {
            GameObject warningGO = Instantiate(warningPrefab);
            warning = warningGO.GetComponent<SizeByDistance>();      
        }

        warning.Setup(transform);
        warning.Hide();
    }

    public override void SetOwner(BaseWeapon baseWeapon)
    {
        base.SetOwner(baseWeapon);
        timer = 1;
        fall = false;
        if (warning == null)
        {
            GameObject warningGO = Instantiate(warningPrefab);
            warning = warningGO.GetComponent<SizeByDistance>();
            warning.Setup(transform);
        }

        warning.Hide();
    }

    public override void Movement()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            fall = true;
        }

        if (!fall)
        {
            playerShip = PlayerManager.GetPlayer();

            if (playerShip != null)
            {
                TargetPos = playerShip.transform.position;
            }
            else
            {
                TargetPos = new Vector3(0, 0, 0);
            }
            warning.transform.position = new Vector3(TargetPos.x, 0, TargetPos.z);

            transform.position += Vector3.up * speed * Time.deltaTime;

            speed = 60;
        }
        else if (fall)
        {
            speed = 120;
            if (TargetPos != null)
            {
                distance = Vector3.Distance(TargetPos, transform.position);

                warning.Show();

                transform.position += transform.forward * speed * Time.deltaTime;

                transform.LookAt(TargetPos);

                if (distance < 2)
                {
                    DestoryNow();
                }
     
            }
        }

    }

}