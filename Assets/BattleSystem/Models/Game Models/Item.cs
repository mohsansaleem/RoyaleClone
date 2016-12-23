using System;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem
{
    public enum Team
    {
        Any,
        Red,
        Blue
    }

    public enum ItemType
    {
        Building,
        GroundRanged,
        GroundMelee,
        AirRanged,
        AirMelee,
        Defensive,
        GroundDefensive,
        AirDefensive,
        Splash,
        GroundSplash,
        AirSplash
    }
    
    public enum TargetType
    {
        None,
        Any,
        Ground,
        Air,
        Hybrid,
        Defensive,
        Building
    }

    public class Item
    {
        protected BattleGrid BattleGrid;
        public event Action<float> OnDamage;
        public string Id;
        public Team Team;
        private float currentHealth;

        public int Weight;
        public Card Card;
        public Player Player;
        public Vector2D Position;

        public Item(string id, Card card, Player player, Team team, Vector2D position, BattleGrid battleGrid)
        {
            Id = id;
            Card = card;
            Player = player;
            Team = team;
            this.currentHealth = card.HP;
            Position = position;
            BattleGrid = battleGrid;
        }

        public float CurrentHealth
        {
            set
            {
                currentHealth = value;
                UnityEngine.Debug.Log("currentHealth: "+currentHealth);
                if (currentHealth <= 0)
                    Destroy();
            }

            get
            {
                return currentHealth;
            }
        }

        public ItemType ItemType
        {
            get{ return Card.ItemType; }
        }
        public TargetType TargetType
        {
            get{ return Card.TargetType; }
        }


        static public Team GetOpponentTeam(Team team)
        {
            return team == Team.Blue ? Team.Red : Team.Blue;
        }

        public Team GetOpponent
        {
            get
            {
                return GetOpponentTeam(Team);
            }
        }

        public bool IsGround
        {
            get
            {
                return (ItemType == ItemType.GroundRanged || ItemType == ItemType.GroundMelee || ItemType == ItemType.GroundDefensive || ItemType == ItemType.Defensive || ItemType == ItemType.Building);
            }
        }
        
        public bool IsAir
        {
            get
            {
                return (ItemType == ItemType.AirRanged || ItemType == ItemType.AirMelee || ItemType == ItemType.AirDefensive || ItemType == ItemType.Defensive);
            }
        }

        public virtual void GetDamage(float damage)
        {
            UnityEngine.Debug.Log("GetDamage: "+damage +" "+this.Id);
            CurrentHealth -= damage;
            if (OnDamage != null)
            {
                OnDamage(damage);
            }
        }

        public Action<Item> OnItemDestroy;

        public virtual void Destroy()
        {
            UnityEngine.Debug.Log("Destroying: "+this.Id);
            BattleGrid.Map [Position.xi, Position.yi].RemoveItem(this);
//            if(OnItemDestroy != null)
//                OnItemDestroy(this);
        }
    }
}