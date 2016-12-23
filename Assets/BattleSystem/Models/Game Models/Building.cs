using System.Collections;
using System.Collections.Generic;
using System;

namespace BattleSystem
{
    public class Building : Item
    {
        public string Name;
        protected Item CurrentTarget;
        private int AttackInvokeId;
        public event Action<Item> OnAttack;
        public event Action       OnBuildingDestroy;
//        public Building():base("", null,null, Team.Any, new Vector2D(0,0))
//        {
//            Name = "";
//        }
        
        public Building(string name, string id, Card card, Player player, Team team, Vector2D position, BattleGrid bg):base(id, card, player, team, position,bg)
        {
            Name = name;
            StartAttack();
        }

        public void StartAttack()
        {
            AttackInvokeId = Invoke.Instance.InvokeRepeating(() => {
                LookForEnemies();}, -1, Card.HitSpeed, Card.HitSpeed);
//            UnityEngine.Debug.Log("LookForEnemies: "+Card.HitSpeed);
        }

        public void StopAttack()
        {
            Invoke.Instance.StopInvoke(AttackInvokeId);
        }

        public void LookForEnemies()
        {
//            UnityEngine.Debug.Log("LookForEnemies StartAttack");
            CurrentTarget= this.BattleGrid.FindTargetInRange(this);
//            UnityEngine.Debug.Log("LookForEnemies "+target);
            if (CurrentTarget != null)
            {
                Attack();
                if(OnAttack != null)
                    OnAttack(CurrentTarget);
            }
        }

        protected virtual void Attack()
        {
            UnityEngine.Debug.Log("Attack StartAttack");
        }

        public override void Destroy()
        {
            base.Destroy();
            StopAttack();
            if (OnBuildingDestroy != null)
                OnBuildingDestroy();
        }

    }
}