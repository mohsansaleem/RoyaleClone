using UnityEngine;
using System.Collections;
using System;

namespace BattleSystem
{
    // or consider it as any offensive building
    public class RangedBuilding : Building,IRanged
    {
        public float Damage;
        public float DamagePerSec;
        public Action<Item> OnProjectileFire;
//        private item

        public RangedBuilding(string name, string id, Card card, Player player, Team team, Vector2D position, BattleGrid bg):base(name,id, card, player, team, position,bg)
        {
            Damage = card.Damage;
            DamagePerSec = card.DamagePerSec;
        }

        protected override void Attack()
        {
//            base.Attack();
            FireProjectile();

        }
        // or consider it as where target should get damage
        public virtual void FireProjectile()
        {
            Debug.Log("LookForEnemies FireProjectile");
            Projectile proj = new Projectile(this, CurrentTarget.Position, Card.ProjectileSpeed, Card.ProjectileRange,Card.Damage);
            proj.Fire();
            if (OnProjectileFire != null)
                OnProjectileFire(CurrentTarget);
        }
    }
}
