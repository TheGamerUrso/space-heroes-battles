using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PunchWeapon : BaseWeapon
{
    public enum PunchState
    {
        Cooldown,   // Waiting for FireRate before choosing the next punch
        Prepare,    // Showing warning fire behind the punch
        Attacking   // Waiting for the punch animation duration to finish via timer
    }
    public PunchState currentPunchState = PunchState.Cooldown;

    [Header("Punch Parts")]
    public BossDestroyablePart LeftPunch;
    public BossDestroyablePart RightPunch;

    [Header("Attack Settings")]
    [SerializeField] private int attacksBeforeBothPunch = 3;
    [SerializeField] private float warningDuration = 1.5f;
    [SerializeField] private float attackDuration = 1.0f; // Set this to match your animation length in seconds

    private int attackCounter = 0;
    private float stateTimer = 0f;
    private bool isBothAttack = false;
    private int selectedPunchIndex = 0; // 0 = Left, 1 = Right

    public override void Setup(Ship ship)
    {
        base.Setup(ship);
        currentPunchState = PunchState.Cooldown;
        stateTimer = FireRate;
    
    }


    public override void Update()
    {
      
    }

    public override void Shoot()
    {
        // Drive the entire attack cadence purely through state and delta-time subtraction
        switch (currentPunchState)
        {
            case PunchState.Cooldown:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    SelectAndPreparePunch();
                }
                break;

            case PunchState.Prepare:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    ExecutePunchAttack();
                }
                break;

            case PunchState.Attacking:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    // Animation window finished; now trigger the FireRate cooldown for the NEXT punch
                    stateTimer = FireRate;
                    currentPunchState = PunchState.Cooldown;
                }
                break;
        }
    }
    private void SelectAndPreparePunch()
    {
        bool leftAlive = LeftPunch != null && LeftPunch != null && LeftPunch.healthComponent.IsAlive;
        bool rightAlive = RightPunch != null && RightPunch != null && RightPunch.healthComponent.IsAlive;

        if (!leftAlive && !rightAlive) return; // Both punches destroyed

        // Track when to perform a dual-punch attack
        attackCounter++;
        if (attackCounter >= attacksBeforeBothPunch)
        {
            attackCounter = 0;
            isBothAttack = true;
        }
        else
        {
            isBothAttack = false;
            selectedPunchIndex = UnityEngine.Random.Range(0, 2);

            // Fallback if the chosen punch is destroyed
            if (selectedPunchIndex == 0 && !leftAlive && rightAlive) selectedPunchIndex = 1;
            else if (selectedPunchIndex == 1 && !rightAlive && leftAlive) selectedPunchIndex = 0;
        }

        // Show warning indicator (fire behind the punch)
        if (isBothAttack)
        {
            if (leftAlive) LeftPunch.PrepareAttack();
            if (rightAlive) RightPunch.PrepareAttack();
        }
        else
        {
            if (selectedPunchIndex == 0 && leftAlive) LeftPunch.PrepareAttack();
            else if (selectedPunchIndex == 1 && rightAlive) RightPunch.PrepareAttack();
        }

        // Transition to warning state
        currentPunchState = PunchState.Prepare;
        stateTimer = warningDuration;
        AboutToShoot?.Invoke(true);
    }

    private void ExecutePunchAttack()
    {
        AboutToShoot?.Invoke(false);

        // Transition to attacking state and load the timer with the animation's length
        currentPunchState = PunchState.Attacking;
        stateTimer = attackDuration;

        bool leftAlive = LeftPunch != null && LeftPunch != null && LeftPunch.healthComponent.IsAlive;
        bool rightAlive = RightPunch != null && RightPunch != null && RightPunch.healthComponent.IsAlive;

        if (isBothAttack)
        {
            if (leftAlive) LeftPunch.Attack();
            if (rightAlive) RightPunch.Attack();
        }
        else
        {
            if (selectedPunchIndex == 0 && leftAlive) LeftPunch.Attack();
            else if (selectedPunchIndex == 1 && rightAlive) RightPunch.Attack();
        }
    }
}
