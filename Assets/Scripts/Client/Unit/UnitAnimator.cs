using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game;

public class UnitAnimator
{

    private Animator _animator;
 
    public enum UnitAnimationState
    {
        Idle,
        Walk,
        Attack,
        Victory
    }

    private const string TRIGGER_WALK = "Walk";
    private const string TRIGGER_Attack = "Attack";
    private const string TRIGGER_Idle = "Idle";
    private const string TRIGGER_Victory = "Victory";

    /*
	private static int _walkState = Animator.StringToHash("Base Layer.Walk");
	private static int _attackState = Animator.StringToHash("Base Layer.Attack");
	private static int _idleState = Animator.StringToHash("Base Layer.Idle");
	private static int _victoryState = Animator.StringToHash("Base Layer.Victory");

	private int _currentState;
	*/

    private Dictionary<UnitAnimationState, Action> _stateToActionMap;

    public UnitAnimator(Animator animator)
    {
        _animator = animator;

        _stateToActionMap = new Dictionary<UnitAnimationState, Action>();
        _stateToActionMap [UnitAnimationState.Idle] = Idle;
        _stateToActionMap [UnitAnimationState.Walk] = Walk;
        _stateToActionMap [UnitAnimationState.Attack] = Attack;
        _stateToActionMap [UnitAnimationState.Victory] = Victory;
    }

	#region Private 

    private void Walk()
    {
        ResetAll();
        _animator.speed = 1.5f;
        _animator.SetTrigger(TRIGGER_WALK);
    }

    private void Idle()
    {
        ResetAll();
        _animator.SetTrigger(TRIGGER_Idle);
    }

    private void Attack()
    {
        ResetAll();
        _animator.SetTrigger(TRIGGER_Attack);
    }

    private void Victory()
    {
        ResetAll();
        _animator.SetTrigger(TRIGGER_Victory);
    }

    private void ResetAll()
    {
        _animator.speed = 1f;
        _animator.ResetTrigger(TRIGGER_WALK);
        _animator.ResetTrigger(TRIGGER_Attack);
//        _animator.ResetTrigger(TRIGGER_Victory);
        _animator.ResetTrigger(TRIGGER_Idle);
    }

	#endregion



	#region Public Api

    public void SetState(UnitAnimationState state)
    {
//        Debug.Log(state);
        _stateToActionMap [state]();
    }

	#endregion
}
