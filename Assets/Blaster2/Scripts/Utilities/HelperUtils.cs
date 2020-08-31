using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public static class HelperUtils
{
    public static bool CheckAnimationByName(Animator animator, string AnimationHashName)
    {
        if (animator != null)
        {
            var info = GetAnimatorStateInfo(animator);

            if (info.shortNameHash == Animator.StringToHash(AnimationHashName))
            {
                Debug.Log("Entering");
                return true;
            }
        }

        return false;
    }

    public static AnimatorStateInfo GetAnimatorStateInfo(Animator animator, int index = 0)
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(index);
        return info;
    }


    public static GameObject GetClosest(string Tag, Vector3 position, float maxRange, LayerMask targetLayer)
    {
        Collider[] Enemies;
        Enemies = Physics.OverlapSphere(position, maxRange, targetLayer);
        GameObject enemyTarget = null;

        foreach (Collider enemy in Enemies)
        {
            ITargetable targetable = enemy.GetComponent<ITargetable>();
            if (targetable != null)
            {
                if (targetable.Targetable)
                {
                    float dist = Vector3.Distance(position, targetable.target.transform.position);
                    if (dist <= maxRange)
                    {
                        if (enemyTarget == null)
                        {
                            enemyTarget = targetable.target;
                        }
                        else
                        {
                            if (Vector3.Distance(position, targetable.target.transform.position) < Vector3.Distance(position, enemyTarget.transform.position))
                            {
                                enemyTarget = targetable.target;
                            }
                        }
                    }
                }
            }    
        }
        return enemyTarget;
    }
    public static float GetAngleFromVectorIn3D(Vector3 diff)
    {
        diff.Normalize();
        float rot_y = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
        return rot_y;
    }

    public static Vector3 GetVector3FromAngleInt(int angle)
    {
        //angle = 0 -> 360;
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static Vector3 GetVectorFromAngle(float angle)
    {
        //angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public static float GetAngleFromVectorFloat3D(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public static void SetGameObjectParent(Transform transform, string TargetName)
    {
        GameObject holderParent = GameObject.Find("DynamicObjects");

        if (holderParent == null)
        {
            holderParent = new GameObject("DynamicObjects");
        }

        GameObject holder = GameObject.Find(TargetName);

        if (holder == null)
        {
            holder = new GameObject(TargetName);
            holder.transform.SetParent(holderParent.transform);
        }

        transform.SetParent(holder.transform);
    }

    //Clamp Angles
    public static float ClampAngle(float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle, 360);
        min = Mathf.Repeat(min, 360);
        max = Mathf.Repeat(max, 360);
        bool inverse = false;
        var tmin = min;
        var tangle = angle;
        if (min > 180)
        {
            inverse = !inverse;
            tmin -= 180;
        }
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        var result = !inverse ? tangle > tmin : tangle < tmin;
        if (!result)
            angle = min;

        inverse = false;
        tangle = angle;
        var tmax = max;
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        if (max > 180)
        {
            inverse = !inverse;
            tmax -= 180;
        }

        result = !inverse ? tangle < tmax : tangle > tmax;
        if (!result)
            angle = max;
        return angle;
    }

    public static void LerpVector(this Transform origin, Transform Target, float offspec, float followSpeed)
    {
        origin.position = Vector3.Lerp(
            origin.position,
            Target.position + new Vector3(0, 0, offspec),
            followSpeed *
            Time.deltaTime);
    }

    public static void GetInputAxis(out float vertical, out float horizontal)
    {
        horizontal = Input.GetAxis(Constants.HorizontalAxisStringKey);
        vertical = Input.GetAxis(Constants.VerticalAxisStringKey);
    }

    public static void SetVectorToZero(this Vector3 vector3)
    {
        vector3 = Vector3.zero;
    }

    public static Vector3 GrabMousePositionOnScreen(Vector3 mousPos, Transform targetPos)
    {
        //Grab the current mouse position on the screen
        return
              Camera.main.ScreenToWorldPoint(new Vector3(mousPos.x,
              mousPos.y,
              mousPos.z -
              Camera.main.transform.position.z));
    }

    public static void AddParentToGameObject(this Transform origin, string TargetName)
    {
        Transform parentTransform = GameObject.Find(TargetName).transform;
        if (parentTransform = null)
        {
            return;
        }
        origin.SetParent(parentTransform);
    }

    public static bool GameObjectIsOutOfCameraVision(this Transform transform, float maxZ)
    {
        if (transform.position.z <= maxZ)
        {
            return true;
        }
        return false;
    }

    public static bool GameObjectIsOutOfCameraVisionOnXandYAxis(this Transform transform, int dir)
    {
        if (dir == 0)
        {
            if (transform.localPosition.x < -50 && transform.localPosition.z < -25)
            {
                return true;
            }
        }
        else if (dir == 1)
        {
            if (transform.localPosition.x > 50 && transform.localPosition.z < -25)
            {
                return true;
            }
        }
        return false;
    }

    public static bool CheckOutOfCamera(this Transform transform, bool Loop)
    {
        if (transform.position.z < Constants.m_ZMin)
        {
            if (Loop)
            {
                var NewPosition = new Vector3(UnityEngine.Random.Range(Constants.m_XMin, Constants.m_XMax), 0, Constants.m_ZMax);
                transform.position = NewPosition;
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}

public static class RotationExtentionUtilities
{
    public static void UpdateShipRotation(this Transform transform, Vector3 LookRotation)
    {

        transform.rotation = Quaternion.LookRotation(LookRotation);
    }

    //====================================================================================================
    public static void RotateObject(this Transform transform, float degree)
    {
        Vector3 rotationVector = transform.rotation.eulerAngles;

        rotationVector.y = degree;

        transform.rotation = Quaternion.Euler(rotationVector);
    }

    public static void RotateObjectTowardMouse(this Transform transform, Vector3 mousePosition)
    {
        transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2((mousePosition.y - transform.position.y),
        (mousePosition.x - transform.position.x)) * Mathf.Rad2Deg - 90);
    }
}


public static class Vector3Extention
{
    public static Vector3 GetClampVector3(this Transform transform)
    {
        return new Vector3(
    Mathf.Clamp(transform.position.x, Constants.m_XMin, Constants.m_XMax),
    transform.position.y,
    transform.position.z);
    }

    public static Vector3 GetClampedVector3(float x = 0, float y = 0, float z = 0)
    {
        return new Vector3(x, y, z);
    }

    public static void ClampTransformPosition(this Transform target, Vector2 xAxis, Vector2 yAxis, Vector2 zAxis)
    {
        target.position = new Vector3(
            Mathf.Clamp(target.position.x,
           xAxis.x,
           xAxis.y),
            0,
            Mathf.Clamp(target.position.z,
            zAxis.x,
          zAxis.y));
    }
}

public static class TransformExtention
{
    /// <summary>
    /// Transform Tool to get all children and ignore self
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Transform[] GetChildrenAsList(this Transform transform)
    {
        List<Transform> childrenInTransform = new List<Transform>();
        foreach (Transform item in transform)
        {
            childrenInTransform.Add(item);
        }
        return childrenInTransform.ToArray();
    }
}
/// <summary>
/// Enum Help Functions
/// </summary>
public static class EnumUtil
{
    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static IEnumerable<T> GetNames<T>()
    {
        return Enum.GetNames(typeof(T)).Cast<T>();
    }
}

public static class AnimUtil
{
    public static float GetAnimatorClipLength(Animator animator)
    {
        Animator animComp = animator;
        return animComp.GetCurrentAnimatorStateInfo(0).length;
    }

    public static float GetSpecificAnimatorClipLength(Animator anim, string ID)
    {
        float animationLength = 0;

        if (string.IsNullOrEmpty(ID))
        {
            return animationLength;
        }

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (ID == clip.name)
            {
                animationLength = clip.length;
            }
        }
        return animationLength;
    }
}
