using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
	// Properties of projectiles.
	public class ProjectileParams
	{
		public string _projectilePrefabPath;
		public float _projectileRadius;
		public float _projectileSpeed;
		public float _projectileDamage;
		public UnitProjectile.ProjectileType _projectileType;
		public UnitProjectile.MotionType _motionType;
		public System.Type _enemyType;
		public bool _shouldAccelerate;
		public Transform _parent;
        public string OnHitSound;

		#region Metadata values for projectiles that weren't put in metadata. (Should have though!)

		#region Public region...
		public static ProjectileParams GetProjectileParams(string unitKey, float projectileDamage, Transform parent)
		{
			ProjectileParams pParams = null;
			if(_unitProjectileInfoDictionary.ContainsKey(unitKey))
			{
                pParams = GetProjectileParam(_unitProjectileInfoDictionary[unitKey]);
				pParams._projectileDamage = projectileDamage;
				pParams._parent = parent;
			}
			return pParams;
		}

		#endregion

		private static Dictionary<string, ProjectileParams> _unitProjectileInfoDictionary;

		static ProjectileParams()
		{
			_unitProjectileInfoDictionary = new Dictionary<string, ProjectileParams>();
			_unitProjectileInfoDictionary["daisy"] = _daisyParams;
			_unitProjectileInfoDictionary["barley"] = _barleyParams;
			_unitProjectileInfoDictionary["highGuy"] = _highGuyParams;
			_unitProjectileInfoDictionary["hipsterUFO"] = _hipsterUFOParams;
			_unitProjectileInfoDictionary["hellAngel"] = _hellAngelParams;
			_unitProjectileInfoDictionary["agentGRASS"] = _agentGRASSParams;

		}

		private static ProjectileParams _daisyParams = GetProjectileParam(typeof(UnitObject), UnitProjectile.MotionType.Linear, 
		                                                                  "TulipAttack", 2.5f, 8f, UnitProjectile.ProjectileType.Splash);

		private static ProjectileParams _barleyParams = GetProjectileParam(typeof(BuildingObject), UnitProjectile.MotionType.Linear, 
		                                                                   "sniperProjectile", 0.3f, 4f, UnitProjectile.ProjectileType.SingleTarget, true);

		private static ProjectileParams _highGuyParams = GetProjectileParam(typeof(BuildingObject), UnitProjectile.MotionType.Linear, 
		                                                                    "highGuySmoke", 1f, 6f, UnitProjectile.ProjectileType.Splash);

		private static ProjectileParams _hipsterUFOParams = GetProjectileParam(typeof(BuildingObject), UnitProjectile.MotionType.Linear, 
		                                                                       "ufoAttack", 1f, 6f, UnitProjectile.ProjectileType.SingleTarget);

		private static ProjectileParams _hellAngelParams = GetProjectileParam(typeof(BuildingObject), UnitProjectile.MotionType.Parabolic, 
		                                                                      "Bottle", 0.3f, 8f, UnitProjectile.ProjectileType.SingleTarget);

		private static ProjectileParams _agentGRASSParams = GetProjectileParam(typeof(BuildingObject), UnitProjectile.MotionType.Linear, 
		                                                                       "AgentGrassBullet", 0.3f, 10f, UnitProjectile.ProjectileType.SingleTarget);


		private static ProjectileParams GetProjectileParam(System.Type enemyType, UnitProjectile.MotionType motionType, string prefabName, 
		                                                   float projectileRadius, float projectileSpeed, UnitProjectile.ProjectileType projectileType, 
		                                                   bool shouldAccelerate = false)
		{
			return new ProjectileParams
			{
				_enemyType = enemyType,
				_motionType = motionType,
				_projectilePrefabPath = Constants.PROJECTILES_PREFAB_PATH + prefabName,
				_projectileRadius = projectileRadius,
				_projectileSpeed = projectileSpeed,
				_projectileType = projectileType,
				_shouldAccelerate = shouldAccelerate
			};
		}
	
        private static ProjectileParams GetProjectileParam(ProjectileParams projectileParams)
        {
            return new ProjectileParams
            {
                _enemyType = projectileParams._enemyType,
                _motionType = projectileParams._motionType,
                _projectilePrefabPath = projectileParams._projectilePrefabPath,
                _projectileRadius = projectileParams._projectileRadius,
                _projectileSpeed = projectileParams._projectileSpeed,
                _projectileType = projectileParams._projectileType,
                _shouldAccelerate = projectileParams._shouldAccelerate
            };
        }
		#endregion		
	}
}