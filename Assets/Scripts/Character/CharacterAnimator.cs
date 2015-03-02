using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour {
	public float _runThreshold = 0.01f;
	public float _sprintThreshold = 10f;

	Animator _animator;
  	DeftPlayerController _controller;
	Rigidbody _rb;
	string[] _animationBoolParameters = {"isIdle", "isRunning", "isAttacking_Projectile"};

	// Use this for initialization
	void Start () {
		_animator = this.GetComponent<Animator>();
		_animator.SetBool("isIdle", true);
		_animator.SetBool("isRunning", false);
		_controller = this.GetComponent<DeftPlayerController>();
    	_rb = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if(_animator)
		{
			//float speed = _rb.GetPointVelocity(Vector3.zero).magnitude;
			if(_controller.state == PlayerState.walking)
			{
				transition(1);
			}
			else if(_controller.state == PlayerState.aiming){
				transition(2);
			}
			else
			{
				transition(0);
			}
		}
	}

	void transition(int parameterIndex){
		_animator.SetBool(_animationBoolParameters[parameterIndex], true);
		for(int i = 0; i < _animationBoolParameters.Length; ++i){
			if(i != parameterIndex){
				_animator.SetBool(_animationBoolParameters[i], false);
			}
		}
	}
}
