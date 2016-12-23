using UnityEngine;
using System.Collections;
using Game;
using System.Collections.Generic;

public class HighTower : Defense {

    private Animator _animator;
    protected override void FaceTarget()
    {
        if(_currentUnit != null)
            _animator.transform.LookAt(_currentUnit.transform);
    }

    public void Start()
    {
        Animator [] Animators = transform.GetComponentsInChildren<Animator>();
        for(int i = 0 ; i < Animators.Length; i++)
        {
            if(Animators[i].gameObject.name == "barley")
            {
                _animator = Animators[i];
                this._projectileParams = ProjectileParams.GetProjectileParams("barley",this.Damage,Animators[i].gameObject.GetComponentInChildren<FXController>().attackTransform);
            }
        }

    }

    public override void BeforeFireStart ()
    {
//        Debug.LogError("BeforeFireStart ");
        FaceTarget ();
    }

    public override void AttackCurrent()
    {
        //Start Attack Animation
//		AudioManager.instance.PlaySound ("highTower", SoundType.Attack);

		_animator.ResetTrigger("Attack");
        _animator.SetTrigger("Attack");
        base.AttackCurrent();
    }

}
