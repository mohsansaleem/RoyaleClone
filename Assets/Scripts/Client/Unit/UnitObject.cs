using UnityEngine;
using System.Collections;
using BattleSystem;

public class UnitObject : MonoBehaviour
{
    protected UnitAnimator Animator;
    protected NavMeshAgent Agent;
    public static void Spawn(Unit unit)
    {
//        Debug.Log("Spawning Unit");
        Object UnitPrefab = Resources.Load(Constants.UNITS_PREFAB_PATH + unit.Card.Name);
        GameObject UnitGO = GameObject.Instantiate(UnitPrefab, new Vector3(unit.Position.x, 0f, unit.Position.y),Quaternion.identity) as GameObject;
        UnitObject UnitObject = UnitGO.AddComponent<UnitObject>();
        unit.OnUnitMove += UnitObject.Move;
        unit.OnUnitAttack += UnitObject.Attack;
        unit.OnUnitDestroy += UnitObject.Die;
        unit.OnDamage += UnitObject.OnDamage;

        UnitObject.Initialize();
//        Debug.Log("Spawned Unit");
    }

    public void Initialize()
    {
        Animator = new UnitAnimator(GetComponent<Animator>());
        Agent = GetComponent<NavMeshAgent>();
        Invoke("StartWalking", 1f);
    }

    protected void StartWalking()
    {
        Animator.SetState(UnitAnimator.UnitAnimationState.Walk);
    }

    protected void Move(Cell NewCell)
    {
        SetDestination(new Vector3(NewCell.x,0f,NewCell.y));
    }

    protected void Attack()
    {
        BattleSystemClient.Instance.AddActionForExecution(() => {
//            Debug.Log("Attack");
            Animator.SetState(UnitAnimator.UnitAnimationState.Attack);
        });
    }

    protected void OnDamage(float damage)
    {
        //TODO: Show Healthbar
    }

    protected void SetDestination(Vector3 dest)
    {
        BattleSystemClient.Instance.AddActionForExecution(() => {
            StartWalking();
            Agent.SetDestination(dest);
        });
    }

    protected void Die()
    {
        BattleSystemClient.Instance.AddActionForExecution(() => {
            Destroy(this.gameObject);
        });

    }
}
