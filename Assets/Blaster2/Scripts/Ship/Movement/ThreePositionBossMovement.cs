using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ThreePositionBossMovement : BossEnemyMovement
{
    [SerializeField] private float switchInterval = 3f;
    [SerializeField] private float lerpSpeed = 3f;
    public override void Move()
    {
        if (Positions == null || Positions.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= switchInterval)
        {
            timer = 0f;
            currentPos = (currentPos + 1) % Positions.Length;
        }

        targetPosition = Positions[currentPos];
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
    }
}
