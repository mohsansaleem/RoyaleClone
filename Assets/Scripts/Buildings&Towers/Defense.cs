using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//using Game.Metadata;

namespace Game
{
	//Base class for all offensive buildings...
	[System.Serializable]
	public class Defense : BuildingObject
	{
//		protected UnitDetector _unitDetector;
//		private List<UnitObject> _unitsInRange;
		private UnitObject currentUnit;
        

        
//        public override List<OptionsData> getOptions()
//        {
//            List<OptionsData> temp = new List<OptionsData>();
//            switch (this.CurrentState)
//            {
//                case State.creating:
//                    temp.Add(new OptionsData(BannerButtons.Cancel));
//                    temp.Add(new OptionsData(BannerButtons.FinishNow,this.FinishNowGems));
//                    break;
//                case State.upgrading:
//                    temp.Add(new OptionsData(BannerButtons.Cancel));
//                    temp.Add(new OptionsData(BannerButtons.FinishNow,this.FinishNowGems));
//                    break;
//                case State.created:
//                    if(this.Level !=10)
//                    {
//                        temp.Add(new OptionsData(BannerButtons.Upgrade,this.UpgradeCost,this.CostType));
//                    }
//                    break;
//            }
//            temp.Add(new OptionsData(BannerButtons.Info));
//            return temp;
//        }
//
//		// Metadata Values //
//
//		public virtual float Damage {
//			get {
//				return _model.buildingsLevels [this.Level.ToString ()].dmgPerAttack;
//			}
//		}
//

//		public virtual UnitProjectile.ProjectileType ProjectileType {
//			get {
//				switch (_model.buildingsLevels [this.Level.ToString ()].projectileType) {
//				case "CurvedSingleTarget":
//					{
//						return UnitProjectile.ProjectileType.SingleTarget;
//					}
//				case "CurvedSplash":
//					{
//						return UnitProjectile.ProjectileType.Splash;
//					}
//				case "LinearSingleTarget":
//					{
//						return UnitProjectile.ProjectileType.SingleTarget;
//					}
//				case "LinearSplash":
//					{
//						return UnitProjectile.ProjectileType.Splash;
//					}
//				default:
//					return UnitProjectile.ProjectileType.Splash;
//				}
//			}
//		}
//
//
//		public virtual UnitProjectile.MotionType MotionType {
//			get {
//				switch (_model.buildingsLevels [this.Level.ToString ()].projectileType) {
//				case "CurvedSingleTarget":
//					{
//						return UnitProjectile.MotionType.Parabolic;
//					}
//				case "CurvedSplash":
//					{
//						return UnitProjectile.MotionType.Parabolic;
//					}
//				case "LinearSingleTarget":
//					{
//						return UnitProjectile.MotionType.Linear;
//					}
//				case "LinearSplash":
//					{
//						return UnitProjectile.MotionType.Linear;
//					}
//				default:
//					return UnitProjectile.MotionType.Linear;
//				}
//			}
//		}

		protected UnitObject _currentUnit {
			get {
				return currentUnit;
			}
			set {
				if (currentUnit != null && value != null)
					CancelInvoke ("AttackIfPossible");
                
				currentUnit = value;
                
				if (currentUnit == null) {
					CancelInvoke ("AttackIfPossible");
				} else {
					InvokeRepeating ("AttackIfPossible", 0f, 1.6f);
				}
			}
		}
        
		//public static LayerMask _ignoreBuildingMask = 

		protected ProjectileParams _projectileParams;

//
//		public virtual void RegisterActionForUnitDetection ()
//		{
//			_unitDetector = gameObject.GetComponentInChildren<UnitDetector> ();
//			_unitDetector._unitEnterAction += OnUnitEnteredRange;
//			_unitDetector._unitExitAction += OnUnitExitedRange;
//			UnitObject._onDestroyEvent += OnUnitDestroyed;
//            
//			_projectileParams = new ProjectileParams
//            {
//
//                _enemyType = typeof(UnitObject),
//                _motionType = MotionType,
//                _projectileDamage = Damage, //Value should come from model
//                _projectilePrefabPath = GameConstant.PREFAB_PATH + "Projectiles/Projectile",
//                _projectileRadius = 1.0f,
//                _projectileSpeed = 10f,
//                _projectileType = ProjectileType,
//                _parent = transform
//            };
//		}
//        
//		public AttackVector AttackVector {
//			get {
//				AttackVector attackVector = AttackVector.Ground;
//				if (_model.buildingsLevels ["1"].attackVector == AttackVector.Air.ToString ())
//					attackVector = AttackVector.Air;
//				else if (_model.buildingsLevels ["1"].attackVector == AttackVector.Both.ToString ())
//					attackVector = AttackVector.Both;
//
//				return attackVector;
//			}
//		}
//        
//		public virtual void OnUnitEnteredRange (UnitObject unit)
//		{
////            Debug.LogError("OnUnitEnteredRange");
////            if (unit.Player == this.Player)
////                return;
//            
//			if (!_unitsInRange.Contains (unit))
//				_unitsInRange.Add (unit);
//            
//			if (_currentUnit == null)
//            {
//				_currentUnit = GetNextUnit ();
//                FaceTarget();
//            }
//		}
//        
//		public virtual void OnUnitExitedRange (UnitObject unit)
//		{
//			if (_unitsInRange.Contains (unit))
//				_unitsInRange.Remove (unit);
//            
//			if (_currentUnit == unit) {
//				_currentUnit = GetNextUnit ();
//                FaceTarget();
//			}
//		}

        protected virtual void  FaceTarget()
        {

        }

		protected bool _canAttack = true;

		public virtual void AllowAttack ()
		{
			Debug.Log ("AllowAttack");
			_canAttack = true;
		}

		public virtual void DisableAttack ()
		{
			Debug.Log ("DisableAttack");
			_canAttack = false;

		}

		private void AttackIfPossible ()
		{
			if (_canAttack)
				AttackCurrent ();
		}


        public virtual void BeforeFireStart()
        {
            
        }

		public virtual void AttackCurrent ()
		{
            BeforeFireStart();
			UnitProjectile.Instantiate (_currentUnit.transform, _projectileParams);
		}
        
//		public virtual UnitObject GetNextUnit ()
//		{
//			if (_unitsInRange.Count > 0)
//				return _unitsInRange [0];
//			return null;
//		}
//        
//		public virtual void OnUnitDestroyed (Destructable aUnit)
//		{
//			UnitObject unit = aUnit as UnitObject;
//            
//			if (unit as UnitObject == null)
//				return;
//            
//			if (_unitsInRange.Contains (unit))
//				_unitsInRange.Remove (unit);
//            
//			if (_currentUnit == unit) {
//				_currentUnit = GetNextUnit ();
//			}
//		}
//
//		public override void OnDestroy ()
//		{
//			UnitObject._onDestroyEvent -= OnUnitDestroyed;
//			base.OnDestroy ();
//		}
//		
//		#region OnTap
//		// -------------------- Building selection OnTap -------------------- 
//		protected override void OnTap (TapGesture gesture)
//		{
////			Debug.Log("Tap detected: " + this.transform.root.name);
//			if (gesture.Selection != this.gameObject || !GameManager.instance.IsModeEquals (GameManager.Mode.Construction, GameManager.Mode.ViewMode))
//				return;
//			
//			base.OnTap (gesture);
//		}
//		#endregion OnTap
//		
//		#region OnDrag
//		// -------------------- Building movement OnDrag -------------------- 
//		protected override void OnDrag (DragGesture gesture)
//		{
//			base.OnDrag (gesture);
//		}
//		#endregion OnDrag
	}
}
