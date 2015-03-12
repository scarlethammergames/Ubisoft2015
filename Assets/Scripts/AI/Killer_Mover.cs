using UnityEngine;
using System.Collections;

public class Killer_Mover : AI_Mover {

	protected StatusUpdate myStatus;


	// Use this for initialization
	void Start () {

		//setting agent
		this.agent = GetComponent<NavMeshAgent> ();

		myStatus = GetComponentInChildren<StatusUpdate> ();

		this.prevWaypoint = this.waypoint;
			
	}

	
	// Update is called once per frame
	void Update () 
	{

		move ();
		
	}


	protected void OnCollisionEnter(Collision other)
	{
		
		if (other.gameObject.tag.Equals ("projectile") || other.gameObject.tag.Equals ("Projectile"))
		{
			
			Destroy (other.gameObject);
			
			if(this.Health <= 0)
			{
				
				Destroy (gameObject);
				
				return;
				
			}	
			
			this.Health -= damageTaken;
			
		}
		
	}


	protected override void react()
	{

		updateWaypoint (GameObject.FindGameObjectWithTag ("Player").gameObject.transform);

	}


	public override void isInterested()
	{
		
		this.interested = true;

		myStatus.updateText (true);
		
		react ();
		
	}


	public void notInterested()
	{
		
		this.interested = false;

		myStatus.updateText (false);
		
		this.waypoint = this.prevWaypoint;

	}

}
