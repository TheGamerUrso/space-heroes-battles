using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey;
using CodeMonkey.Utils;

public class WarningSignWidget : MonoBehaviour
{

    public GameObject followTarget;
    public GameObject Target;
    public GameObject WarningSign;
    public float TTL = 4;

    private void Start()
    {
        Sequence fadeAnim = DOTween.Sequence();
        fadeAnim.Append(WarningSign.GetComponent<Image>().DOFade(0, .25f))
  .Append(WarningSign.GetComponent<Image>().DOFade(1, .25f))
  .Append(WarningSign.GetComponent<Image>().DOFade(0, .25f))
  .Append(WarningSign.GetComponent<Image>().DOFade(1, .25f)).SetLoops(-1);
    }

    public float angle;
    public Vector3 targetDir;
    public LayerMask enemies;
    public Collider[] colliders;

    private void FixedUpdate()
    {
        colliders = Physics.OverlapSphere(followTarget.transform.position, 40, enemies);
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
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(followTarget.transform.position);
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
