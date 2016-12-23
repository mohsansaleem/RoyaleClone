
using System.Collections;

namespace BattleSystem
{
    public class Card
    {
        public string Id;
        public string Name;
        public string Description;
        public float HP;
        public float Damage;
        public float DamagePerSec;
        public float HitSpeed;
        public float ProjectileSpeed;
        public int ProjectileRange;
        /// <summary>
        /// The move speed(Cells Per Sec).
        /// </summary>
        public float MoveSpeed;

        public float Range;
        public float DeployTime;

        /// <summary>
        /// The sight range. Cells at which distance a unit will change its path.
        /// </summary>
        public float SightRange;

        public ItemType ItemType;

        public TargetType TargetType;

        public Card(string id, string name, string des, float hp, float damage, float dps, float hitspeed, float moveSpeed, float range, float deployTime, float sightRange, ItemType itemType, TargetType targetType)
        {
            Id = id;
            Name = name;
            Description = des;
            HP = hp;
            Damage = damage;
            DamagePerSec = dps;
            HitSpeed = hitspeed;
            MoveSpeed = moveSpeed;
            Range = range;
            DeployTime = deployTime;
            SightRange = sightRange;
            ItemType = itemType;
            TargetType = targetType;
        }

        public float SecsPerCellJump
        {
            get
            {
                return 1f / MoveSpeed;
            }
        }

        public override string ToString()
        {
            return string.Format("[Card: Name={0},HitSpeed={1}]", Name, HitSpeed);
        }
    }
}
