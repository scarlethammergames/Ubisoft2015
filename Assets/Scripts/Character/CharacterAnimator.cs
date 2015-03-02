using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour {
	public float _runThreshold = 0.01f;
	public float _sprintThreshold = 10f;

	Animator _animator;
  //DeftPlayerController controller;
	Rigidbody _rb;

	// Use this for initialization
	void Start () {
		_animator = this.GetComponent<Animator>();
		_animator.SetBool("isIdle", true);
    	 _rb = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if(_animator)
		{
			float speed = _rb.GetPointVelocity(Vector3.zero).magnitude;
			if(speed > _runThreshold)
			{
				_animator.SetBool("isIdle", false);
				_animator.SetBool("isRunning", true);
			}
			else
			{
				_animator.SetBool("isIdle", true);
				_animator.SetBool("isRunning", false);
			}
		}
	}
}
