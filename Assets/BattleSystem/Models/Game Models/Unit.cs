using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BattleSystem
{
    public class Unit: Item
    {
        // Events
        public event Action<Cell> OnUnitMove;
        public event Action OnUnitAttack;
        public event Action OnUnitDestroy;

        private List<Cell> Path = new List<Cell>();

        protected Item CurrentTarget;
        protected int MovementInvokeId = -1;
        protected int AttackInvokeId = -1;

        public Unit(string id, Card card, Player player, Team team, Vector2D position, BattleGrid bg):base(id, card, player, team, position,bg)
        {
            Card = card;
            CurrentHealth = card.HP;

            this.BattleGrid = bg;

            Vector2D LanePos = BattleGrid.FindNearestLanePos(position);

            GetPathTo(LanePos);
            AppendLanePath();
            StartMoving();
        }

        public float TotalHealth
        {
            get
            {
                return Card.HP;
            }
        }

        #region Movement
        public void StartMoving()
        {
            MovementInvokeId = Invoke.Instance.InvokeRepeating(() => {
                MoveOneCell();}, -1, Card.SecsPerCellJump, Card.SecsPerCellJump);
        }
            
        public void StopMoving()
        {
            Invoke.Instance.StopInvoke(MovementInvokeId);
        }

        private void FollowPath()
        {
            if (HasPath)
            {
                if (!MoveAhead)
                {
                    Debug.LogError("Path Blocked");
                }
            } else
            {
                AppendLanePath();
            }
            Invoke.Instance.ChangePeriod(MovementInvokeId,this.Card.MoveSpeed);
        }

        private void MoveOneCell()
        {
            if (HasTargetInRange)
            {
                Attack();

            } else if (HasNewTargetInSight)
            {
                GetPathTo(CurrentTarget.Position);
                FollowPath();
            } else
            {
                FollowPath();    
            }
        }

        private void GetPathTo(Vector2D Destination)
        {
            BattleGrid.InsertInQueue(this.Position, Destination, this, (vecList) => 
            {
                Path.Clear();
                BattleGrid.VectorsToCells(vecList, Path);
                PrintPath();
            });
        }

        private bool HasPath
        {
            get
            {
                return Path != null && Path.Count > 1;
            }
        }

        private bool MoveAhead
        {
            get
            {
                if( Path [0].MoveToCell(this, Path [1]))
                {
                    Path.RemoveAt(0);
                    if (OnUnitMove != null)
                    {
                        OnUnitMove(Path [1]);
                    }
                    return true;
                }
                return false;
            }
        }

        #endregion
        #region Attack
        public virtual void StartAttack()
        {
            StopMoving();
            AttackInvokeId = Invoke.Instance.InvokeRepeating(() => {
                Attack();}, -1, 0f, Card.HitSpeed);
        }

        public virtual void StopAttack()
        {
            Invoke.Instance.StopInvoke(AttackInvokeId);
        }

        public virtual void Attack()
        {
            Invoke.Instance.ChangePeriod(MovementInvokeId,this.Card.HitSpeed);
            if (OnUnitAttack != null)
            {
                OnUnitAttack();
            }
        }

        private bool HasTargetInRange
        {
            get
            {
                var target = this.BattleGrid.FindTargetInRange(this);
                
                if (target != null)
                {
                    CurrentTarget = target;
                    return true;
                }
                return false;
            }
        }

        private bool HasNewTargetInSight
        {
            get
            {
                var target = this.BattleGrid.FindTargetInSight(this);
                if (target != null)
                {
//                    if (CurrentTarget == null || CurrentTarget != target) // if target moves
                    {
                        CurrentTarget = target;
                        return true;
                    }
                }
                return false;
            }
        }
        #endregion
        private void PrintPath()
        {
            if (Path != null)
            {
                System.Text.StringBuilder PathString = new System.Text.StringBuilder(string.Empty);
                for (int i=0; i<Path.Count; i++)
                {
                    PathString.Append(Path [i].ToString());
                }
                Debug.Log(PathString.ToString());
            }
        }

        private void PrintPath(List<Vector2D> vecs)
        {
            if (vecs != null)
            {
                System.Text.StringBuilder PathString = new System.Text.StringBuilder(string.Empty);
                for (int i=0; i<vecs.Count; i++)
                {
                    PathString.Append(vecs [i].ToString());
                }
                Debug.Log(PathString.ToString());
            }
        }

        private void AppendLanePath()
        {
            if (BattleGrid.Map [Position.xi, Position.yi].IsLeftLane || BattleGrid.Map [Position.xi, Position.yi].IsRightLane)
                return;

            int LeftLaneIndex = 0;
            for (int i=0; i< BattleGrid.LeftLane.Count; i++)
            {
                if (Path [Path.Count - 1] == BattleGrid.LeftLane [i])
                {
                    LeftLaneIndex = i;
                    break;
                }
            }
            //TODO: Complete this for right lane
            Path.AddRange(Path [0].IsLeftLane ? BattleGrid.LeftLane.GetRange(LeftLaneIndex + 1, (BattleGrid.LeftLane.Count - LeftLaneIndex - 1)) : BattleGrid.LeftLane.GetRange(LeftLaneIndex + 1, (BattleGrid.LeftLane.Count - LeftLaneIndex - 1)));
        }

        public override void Destroy()
        {
            base.Destroy();
            StopAttack();
            StopMoving();
            if (OnUnitDestroy != null)
                OnUnitDestroy();
        }
    }
}