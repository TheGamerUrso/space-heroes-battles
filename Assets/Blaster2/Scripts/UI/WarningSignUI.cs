using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using PrimeTween;

public class WarningSignUI : MonoBehaviour
{
    private GameObject playerTarget;
    private GameObject Target;
    public GameObject WarningSign;
    public float TTL = 4;


    public void Setup(GameObject playerTarget)
    {
        this.playerTarget = playerTarget;
    }

    private void Start()
    {
        Sequence fadeAnim = Sequence.Create(cycles: 1, Sequence.SequenceCycleMode.Rewind);
        fadeAnim.Chain(Tween.Alpha(WarningSign.GetComponent<Image>(), 1, .25f))
        .Chain(Tween.Alpha(WarningSign.GetComponent<Image>(), 0, .25f))
        .Chain(Tween.Alpha(WarningSign.GetComponent<Image>(), 1, .25f));
    }

    public float angle;
    public Vector3 targetDir;
    public LayerMask enemies;
    public Collider[] colliders;

    private void FixedUpdate()
    {
        if (playerTarget == null) return;

        colliders = Physics.OverlapSphere(playerTarget.transform.position, 40, enemies);
        if (colliders.Length > 0)
        {
            var Asteroid = colliders[0].GetComponent<AsteroidCollider>();
            if (Asteroid != null)
            {
                if (colliders[0].gameObject != null)
                {
                    Target = colliders[0].gameObject;
                }
            }
        }
    }

    void LateUpdate()
    {
        if (playerTarget == null) return;

        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(playerTarget.transform.position);
        transform.position = targetPositionScreenPoint;

        if (Target != null)
        {
            bool isOffScreen = targetPositionScreenPoint.x <= 0 || targetPositionScreenPoint.x >= Screen.width || targetPositionScreenPoint.y <= 0 || targetPositionScreenPoint.y >= Screen.height;

            if (isOffScreen)
            {
                WarningSign.SetActive(false);
                Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
                if (cappedTargetScreenPosition.x <= 0) cappedTargetScreenPosition.x = 0f;
                if (cappedTargetScreenPosition.x >= Screen.width) cappedTargetScreenPosition.x = Screen.width;
                if (cappedTargetScreenPosition.y <= 0) cappedTargetScreenPosition.y = 0f;
                if (cappedTargetScreenPosition.y >= Screen.height) cappedTargetScreenPosition.y = Screen.height;

                transform.position = cappedTargetScreenPosition;
            }

            var screenPos = Camera.main.WorldToScreenPoint(Target.transform.position);
            targetDir = screenPos - transform.position;

            angle = UtilsClass.GetAngleFromVector(targetDir);

            transform.eulerAngles = new Vector3(0, 0, angle);


            if (Target.activeInHierarchy)
            {
                WarningSign.SetActive(true);
            }
            else
            {
                WarningSign.SetActive(false);
                Target = null;
            }
        }

    }
}
