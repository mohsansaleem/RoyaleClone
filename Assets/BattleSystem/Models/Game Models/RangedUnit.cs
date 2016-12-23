using UnityEngine;
using System.Collections;

namespace BattleSystem
{
    public class RangedUnit : Unit,IRanged
    {
        public RangedUnit(string id, Card card, Player player, Team team, Vector2D position, BattleGrid bg):base(id, card, player, team, position,bg)
        {

        }

        public override void Attack()
        {
            base.Attack();
            FireProjectile();
//            Debug.Log("Projectile Fired");
        }

        public virtual void FireProjectile()
        {
            Projectile proj = new Projectile(this, CurrentTarget.Position, Card.ProjectileSpeed, Card.ProjectileRange,Card.Damage);
            proj.Fire();
        }
    }
}
