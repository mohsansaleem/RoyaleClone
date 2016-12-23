using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem
{
    public class Projectile
    {
        public Item Owner;
        public Vector2D Target;
        public float Speed;
        public int Range;
        public float Damage;

        public Projectile(Item owner,Vector2D target,float speed,int range,float damage)
        {
//            if(target == null)
//                throw new System.Exception("Target cant be null for projectile");
//            if(speed < 1)
//                throw new System.Exception("Speed cant be less than zero for projectile");
//            Debug.Log("Projectile");
            Owner = owner;
            Target = target;
            Speed = speed;
            Range = range;
            Damage = damage;
        
        }
        public void Fire()
        {
//            Debug.Log("Projectile Fired "+(Vector2D.Distance(Owner.Position,Target)/Owner.Card.ProjectileSpeed));
                Invoke.Instance.InvokeOnce(() => {
                DoDamage();}, (Vector2D.Distance(Owner.Position,Target)/Owner.Card.ProjectileSpeed));
        }

        private void DoDamage()
        {
            List<Item> Targets = BattleGrid.Instance.FindTargetsinRange(this);
            Debug.Log("DoDamage "+Targets.Count+" "+Range);
            for(int i=0; i<Targets.Count; i++)
            {
                Targets[i].GetDamage(Damage);
            }
        }
    }
}
