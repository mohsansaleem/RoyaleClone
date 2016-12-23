using UnityEngine;
using System.Collections;

public class FXController : MonoBehaviour
{
	public enum AttackFXType
	{
		Continuous,
		Discontinuous,
		Lazer
	}

	private AttackFXType attackType;

	public AttackFXType _attackType {
		set {
			attackType = value;

			if (attackType == AttackFXType.Continuous || attackType == AttackFXType.Lazer)
				attackTransform.gameObject.SetActive (false);
			else
				attackTransform.gameObject.SetActive (true);
		}
		get {
			return attackType;
		}
	}

	public enum FXStyle
	{
		Attack,
		Damage,
		Heal,
		Destroy
	}

	public Transform attackTransform;
	public Transform damageTransform;
	public Transform destroyTransform;
	public Transform healTransform;
	public GameObject attackPrefab;
	public GameObject damagePrefab;
	public GameObject destroyPrefab;
	public GameObject healPrefab;

	public void ShowFX (FXStyle type, Vector3? TargetPos = null)
	{
		switch (type) {
		case FXStyle.Attack:
			Transform tran = null;
			if (_attackType == AttackFXType.Continuous /*|| _attackType == AttackFXType.Lazer*/) {

				attackTransform.gameObject.SetActive (true);
				if (attackTransform.childCount == 0) {
					tran = PoolInstantiateParticle (attackPrefab,attackTransform).transform;
//                  Vector3 prefabScale = attackPrefab.transform.localScale;

//                  tran.localScale = prefabScale;
				}

			} else if (_attackType == AttackFXType.Lazer) {
				attackTransform.gameObject.SetActive (true);
				LineRenderer lineRenderer;
				if (attackTransform.childCount == 0) {
                        tran = PoolInstantiateParticle (attackPrefab,attackTransform).transform;
					//                  Vector3 prefabScale = attackPrefab.transform.localScale;
					lineRenderer = tran.GetComponentInChildren<LineRenderer> ();
					//                  tran.localScale = prefabScale;
				} else {
					lineRenderer = attackTransform.GetComponentInChildren<LineRenderer> ();
				}

				lineRenderer.SetPosition (0, attackTransform.position);
				lineRenderer.SetPosition (1, TargetPos.Value);


			} else {
				if (attackPrefab != null) {
                        tran = PoolInstantiateParticle (attackPrefab,attackTransform).transform;
				}
			}

			if (tran != null) {
				tran.SetParent (attackTransform);
				tran.localPosition = Vector3.zero;
				tran.localRotation = Quaternion.identity;
			}
			break;
		case FXStyle.Destroy:
			if (destroyPrefab != null)
				PoolInstantiateParticle (destroyPrefab).transform.position = destroyTransform.position;
			break;
		case FXStyle.Damage:
			if (damagePrefab != null && damageTransform.childCount <= 1)
				PoolInstantiateParticle (damagePrefab).transform.parent = damageTransform;
			break;
		case FXStyle.Heal:
			if (healPrefab == null || healTransform.childCount > 1)
				return;
			GameObject instance = PoolInstantiateParticle (healPrefab);
			instance.transform.parent = healTransform;
			instance.transform.localPosition = Vector3.zero;
			break;
		}
	}

	public void ShowLazerFX (Vector3 target)
	{
		if (target == null)
			return;
		_attackType = AttackFXType.Lazer;
		ShowFX (FXStyle.Attack, target);

		// Vector3 newPos = new Vector3(target.x, attackTransform.position.y, target.z);

//      Oscillator osc = attackTransform.GetComponentInChildren<Oscillator>();
//      osc._fromPosition = attackTransform.position;
//      osc._toPosition = newPos;

//      LineRenderer lineRend = attackTransform.GetComponentInChildren<LineRenderer>();
//      lineRend.SetPosition(0, attackTransform.position);
//      lineRend.SetPosition(1, target);
	}

	public void StopAttackFX ()
	{
		if (_attackType == AttackFXType.Continuous || _attackType == AttackFXType.Lazer)
			attackTransform.gameObject.SetActive (false);
	}
    
	GameObject PoolInstantiateParticle (GameObject _particle, Transform Parent = null)
	{
//		if (ObjectPool.CountPooled (_particle) == 0)
//			ObjectPool.CreatePool (_particle, 1);
//        GameObject ToReturn =  Parent == null ? ObjectPool.Spawn (_particle, transform.position, transform.rotation): ObjectPool.Spawn (_particle, Parent,transform.position, transform.rotation);
		return null;
	}
}
