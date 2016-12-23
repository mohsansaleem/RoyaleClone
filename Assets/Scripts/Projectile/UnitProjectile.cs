using UnityEngine;
using System;
using System.Collections;
 
namespace Game
{
    [RequireComponent (typeof(Rigidbody))]
    [RequireComponent (typeof(Collider))]
    public class UnitProjectile : MonoBehaviour
    {
        //	Whether the projectile deal damage to a specific target/unit or in area
        public enum ProjectileType
        {
            SingleTarget,
            Splash
        }


        //	Whether the projectile follows parabolic/linear path 
        public enum MotionType
        {
            Linear,
            Parabolic
        }

        private const string PROJECTILE_PATH = "Projectiles/";
        private const string _projectilePrefabName = "Projectile";


        private Transform _parent;
        protected Transform _target;
        protected float _damage;
        protected float _speed = 10f;
//		private float _startSpeed = 0f;
        protected float _radius = 1f;
        protected Type _type;
        protected MotionType _motionType;
        protected ProjectileType _projectileType;
        protected string OnHitSound;

        private bool _shouldAccelerate = false;
        private float _currentSpeed = 3;

        //===========================================================================================
        //	Projectile Factory Method
//        private static Type GetUnitFromFactory(ProjectileType projectileType)
//        {
//            switch (projectileType)
//            {
//                case ProjectileType.Splash:
//                    return typeof(SplashProjectile);
//                default :
//                    return typeof(SingleTargetProjectile);
//            }
//        }
        //===========================================================================================



        //===========================================================================================
        //	Instantiation code for projectile objects
        public static UnitProjectile Instantiate(Transform target, ProjectileParams projectileParams)
        {
            if (target == null)
                return null;
			
            //GameObject projectileObject = GameObject.Instantiate(Resources.Load(projectileParams._projectilePrefabPath)) as GameObject;
            GameObject Prefab = Resources.Load<GameObject>(Constants.PROJECTILES_PREFAB_PATH+"Bullet");
            GameObject projectileObject = ObjectPool.Spawn(Prefab);
            UnitProjectile unitProjectile = projectileObject.AddComponent<UnitProjectile>();

//			unitProjectile.transform.position = projectileParams._parent.position;
//			unitProjectile.transform.position = new Vector3(unitProjectile.transform.position.x, 0.5f, unitProjectile.transform.position.z); //offset from ground :s
            unitProjectile._damage = projectileParams._projectileDamage;
            unitProjectile._type = projectileParams._enemyType;
            unitProjectile._radius = projectileParams._projectileRadius;
            unitProjectile._motionType = projectileParams._motionType;
            unitProjectile._parent = projectileParams._parent;
            unitProjectile._speed = projectileParams._projectileSpeed;
            unitProjectile._shouldAccelerate = projectileParams._shouldAccelerate;
            unitProjectile._target = target;
            unitProjectile._projectileType = projectileParams._projectileType;
            unitProjectile.OnHitSound = projectileParams.OnHitSound;
            unitProjectile.Fire();
            return unitProjectile;
        }
        //===========================================================================================


        //===========================================================================================
        //
        public static UnitProjectile Instantiate(Vector3 target, ProjectileParams projectileParams)
        {
            GameObject Prefab = Resources.Load(Constants.PROJECTILES_PREFAB_PATH+"Bullet") as GameObject;
            GameObject projectileObject = ObjectPool.Spawn(Prefab);
            UnitProjectile unitProjectile = projectileObject.AddComponent<UnitProjectile>();
            unitProjectile._damage = projectileParams._projectileDamage;
            unitProjectile._type = projectileParams._enemyType;
            unitProjectile._radius = projectileParams._projectileRadius;
            unitProjectile._motionType = projectileParams._motionType;
            unitProjectile._parent = projectileParams._parent;
            unitProjectile._speed = projectileParams._projectileSpeed;
            unitProjectile._shouldAccelerate = projectileParams._shouldAccelerate;
            unitProjectile.gotoPos = target;
            unitProjectile._projectileType = projectileParams._projectileType;
            unitProjectile.OnHitSound = projectileParams.OnHitSound;
            unitProjectile.Fire();
            return unitProjectile;
        }
        //===========================================================================================


        //===========================================================================================
        //	Monobehavior : Start
        protected virtual void Fire()
        {
            if (_parent != null)
            {
//                if (_parent.GetComponent<Collider>() != null)
//                    Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), _parent.GetComponent<Collider>());
                transform.position = _parent.position;

//				Debug.Log(string.Format("transform.position = {0}, _parent.position = {1}", transform.position, _parent.position));
            }
//            gameObject.GetComponent<SphereCollider>().radius = _radius;
        }
        //===========================================================================================


//        protected abstract void OnReachTarget();
//        protected abstract void OnReachTargetPositionWithoutTarget();

        public Action OnReachTargetPos;

        private Vector3 gotoPos = Vector3.one;
        private Vector3 startPos = Vector3.one;


        //===========================================================================================
        //	Monobehavior : FixedUpdate
        protected void FixedUpdate()
        {
            if (_motionType == MotionType.Linear)
                PerformLinearTranslation();
            else
                PerformParabolicTranslation();
        }
        //===========================================================================================

        float distance;
        float currentDistanceRemaining;
        float step;
        float StartOffset;
        //===========================================================================================
        //	Moves projectiles in a parabolic path
        private void PerformParabolicTranslation()
        {
            if (true)
            {
                if (_currentSpeed <= _speed)
                    _currentSpeed += 0.5f;
            } else
                _currentSpeed = _speed;

            if (gotoPos == Vector3.one)
            {
                if (_target == null)
                {
                    ObjectPool.Recycle(gameObject);
                    return;
                }
                else
                    gotoPos = _target.position;
            }

            if (startPos.Equals( Vector3.one))
            {
                if(_parent == null)
                {
                    ObjectPool.Recycle(gameObject);
                    return;
                }
                startPos = _parent.transform.position;
                StartOffset = startPos.y;
                transform.position = startPos;
            }
				
            if (gotoPos != Vector3.one && startPos != Vector3.one)
            {
                distance = Vector3.Distance(gotoPos, startPos);
                currentDistanceRemaining = Vector3.Distance(gotoPos, transform.position);

                {
                    StartOffset -= 0.05f;
                    StartOffset = Mathf.Max(StartOffset,0f);
                }
                
                transform.position = new Vector3(transform.position.x, (distance / 3) * Mathf.Sin(Mathf.PI * currentDistanceRemaining / distance) + StartOffset, transform.position.z);
                if(currentDistanceRemaining <= 0.01f)
                {
//                    OnReachTargetPositionWithoutTarget();
                    if (OnReachTargetPos != null)
                    {
                        OnReachTargetPos();
                    }
                    //gotoPos = Vector3.one;
                } else
                {
                    step = _currentSpeed * Time.deltaTime;
					
                    transform.position = Vector3.MoveTowards(transform.position, gotoPos, step);
                }

            }
        }
        //===========================================================================================



        //===========================================================================================
        //	Move projectiles linearly
        private void PerformLinearTranslation()
        {
            if (_shouldAccelerate)
            {
                if (_currentSpeed <= _speed)
                    _currentSpeed *= 1.2f;
            } else
                _currentSpeed = _speed;

            if (_target != null)
            {
                if (transform.position != _target.position)
                {
                    gotoPos = _target.position;
                    transform.LookAt(_target.position);
                    float step = _currentSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, _target.position, step);
                } else
                {
//                    OnReachTarget();
                    if (OnReachTargetPos != null)
                    {
                        OnReachTargetPos();
                    }
                }
            } else
            {
                if (gotoPos != Vector3.one)
                {
                    if (transform.position == gotoPos)
                    {
//                        OnReachTargetPositionWithoutTarget();
                        if (OnReachTargetPos != null)
                        {
                            OnReachTargetPos();
                        }
                        gotoPos = Vector3.one;
                    } else
                    {
                        transform.LookAt(gotoPos);
                        float step = _currentSpeed * Time.deltaTime;

                        transform.position = Vector3.MoveTowards(transform.position, gotoPos, step);
                    }
                }
                else
                {
                    ObjectPool.Recycle(gameObject);
                }
            }
        }
        //===========================================================================================

        void OnDisable()
        {
//            switch (_projectileType)
//            {
//                case ProjectileType.Splash:
//                    Destroy(gameObject.GetComponent<SplashProjectile>());
//                    break;
//                default :
//                    Destroy(gameObject.GetComponent<SingleTargetProjectile>());
//                    break;
//            }

        }
    }
}