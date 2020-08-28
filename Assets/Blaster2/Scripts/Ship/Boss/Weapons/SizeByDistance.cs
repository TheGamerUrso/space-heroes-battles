using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeByDistance : MonoBehaviour
{
    public Transform Target;
    public float curDistance;
    public Vector3 offset;

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Setup(Transform target)
    {
        Target = target;
    }

    public float minimumDistanceScale;
    public float maximumDistanceScale;
    public float minimumDistance;
    public float maximumDistance;
    public float norm;

    void Update()
    {
        if (Target != null && Target.gameObject.activeInHierarchy)
        {
            curDistance = (transform.position - Target.transform.position).magnitude;
            norm = (curDistance - minimumDistance) / (maximumDistance - minimumDistance);
            norm = Mathf.Clamp01(norm);

            var minScale = Vector3.one * maximumDistanceScale;
            var maxScale = Vector3.one * minimumDistanceScale;

            transform.localScale = Vector3.Lerp(maxScale, minScale, norm);
            // curDistance = Vector3.Distance(Target.transform.position, transform.position + offset);
            //curDistance = Mathf.Clamp(curDistance, 0, 2);
            // transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(curDistance, curDistance, curDistance), .1f);
        }
        else
        {
            Hide();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + offset, Vector3.one);
    }
}
