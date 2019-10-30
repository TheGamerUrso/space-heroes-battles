using UnityEngine;

public class ArtilleryProjectile : EnemyProjectile
{
    public GameObject TargetPrefab;
    public Transform[] artilleryTargets;
    public GameObject WarningSignalPrefab;
    private GameObject WarningSignal;
    private Transform TargetToGo;
    private int curTarget;
    public float yVel;

    public bool fall;

    public float distFromTarget;

    public void Targets(Transform[] targets)
    {
        artilleryTargets = targets;
    }

    private void Start()
    {
        curTarget = UnityEngine.Random.Range(0, artilleryTargets.Length);
        foreach (Transform item in artilleryTargets)
        {
            if (PlayerManager.GetPlayer() == null)
            {
                return;
            }
            Player Target = PlayerManager.GetPlayer();

            if (Target == null)
            {
                Target = GameObject.FindObjectOfType<Player>();
            }

            if (Target != null)
            {
                float dist = (item.transform.position - Target.transform.position).magnitude;
                if (dist < 5)
                {
                    TargetToGo = item;
                    WarningSignal = Instantiate(WarningSignalPrefab, item.transform, false);
                }
            }
        }
    }

    public override void Movement()
    {
        yVel = 25;
        if (transform.localPosition.y > 10)
        {
            fall = true;
        }

        if (fall == false)
        {
            Vector3 newPos = (transform.forward * speed) + (transform.up * yVel);
            transform.Translate(newPos * Time.deltaTime);
        }
        else if (fall && TargetToGo != null)
        {
            distFromTarget = (transform.position - TargetToGo.transform.position).magnitude - 30;

            WarningSignal.transform.localScale = Vector3.Lerp(WarningSignal.transform.localScale, new Vector3(distFromTarget, distFromTarget, distFromTarget), .1f * Time.deltaTime);

            WarningSignal.transform.localScale = new Vector3(
               Mathf.Clamp(WarningSignal.transform.localScale.x, 0, 1),
               Mathf.Clamp(WarningSignal.transform.localScale.y, 0, 1),
               Mathf.Clamp(WarningSignal.transform.localScale.z, 0, 1)
                );

            transform.position = Vector3.MoveTowards(transform.position, TargetToGo.transform.position, 1);
        }
        if (TargetToGo != null)
        {
            if ((transform.position - TargetToGo.transform.position).magnitude < .1f)
            {
                GameObject explosion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.BulletExplosion);
                explosion.transform.position = transform.position + Vector3.up * 2;
                explosion.transform.rotation = Quaternion.identity;

                Destroy(WarningSignal);
                gameObject.SetActive(false);
                Destroy(gameObject, 1);
            }
        }
    }
}