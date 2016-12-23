using UnityEngine;
using System.Collections;
using Game;
using BattleSystem;

public class BuildingObject : MonoBehaviour
{
    private Animator _animator;
    protected ProjectileParams _projectileParams;
    protected float Damage;

    public static void Spawn(Building building)
    {
        Object BuildingPrefab = Resources.Load(Constants.BUILDINGS_PREFAB_PATH + building.Card.Name);
        GameObject BuildingGO = GameObject.Instantiate(BuildingPrefab, new Vector3(building.Position.x, 0f, building.Position.y), Quaternion.identity) as GameObject;
        BuildingObject BuildingObject = BuildingGO.AddComponent<BuildingObject>();
        building.OnAttack += BuildingObject.Attack;
        building.OnBuildingDestroy += BuildingObject.Die;
        building.OnDamage += BuildingObject.OnDamage;
        BuildingObject.Initialize();
    }

    // Use this for initialization
    private void Initialize()
    {
        Animator [] Animators = transform.GetComponentsInChildren<Animator>();
        for (int i = 0; i < Animators.Length; i++)
        {
            if (Animators [i].gameObject.name == "barley")
            {
                _animator = Animators [i];
                this._projectileParams = ProjectileParams.GetProjectileParams("barley", this.Damage, Animators [i].gameObject.GetComponentInChildren<FXController>().attackTransform);
            }
        }
    }

    public void Attack(Item Target)
    {
        BattleSystemClient.Instance.AddActionForExecution(() => 
        {
            Vector3 _Target = new Vector3(Target.Position.x,0f,Target.Position.y);
            _animator.transform.LookAt(_Target);
            UnitProjectile.Instantiate (_Target, _projectileParams);
            _animator.ResetTrigger("Attack");
            _animator.SetTrigger("Attack");
        });
    }

    protected void OnDamage(float damage)
    {
        //TODO: Show Healthbar
    }

    public void Die()
    {
        BattleSystemClient.Instance.AddActionForExecution(() => 
        {
            Destroy(this.gameObject);
        });
    }
}
