using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour {

  Animator anim;
  DeftPlayerController controller;

	// Use this for initialization
	void Start () {
    anim = this.GetComponent<Animator>();
    anim.SetBool("isIdle", true);
    controller = this.GetComponent<DeftPlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
	  if(anim)
    {
      if(this.controller.state == PlayerState.running || this.controller.state == PlayerState.sprinting || this.controller.state == PlayerState.walking)
      {
        anim.SetBool("isIdle", false);
        anim.SetBool("isRunning", true);
      }
      else
      {
        anim.SetBool("isIdle", true);
        anim.SetBool("isRunning", false);
      }
    }
	}
}
