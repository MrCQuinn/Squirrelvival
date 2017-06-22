using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Dog : MonoBehaviour {

	//private GameObject squirrel;
	public float speed;

	private bool squirrelAlive;
	private bool gameStarted; 

	private Animator animator;

	//a* stuff
	public Transform target;
	private Seeker seeker; 
	public Path path;
	public float nextWaypointDistance = 3;
	private int currentWaypoint = 0;
	private bool chasing;

	// Use this for initialization
	void Start () {
		// squirrel = GameObject.FindGameObjectWithTag ("Squirrel");
		squirrelAlive = true;
		gameStarted = false;

		//Get a component reference to the Player's animator component
		animator = GetComponent<Animator>();


		//a* stuff
		seeker = GetComponent<Seeker>();
		chasing = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (squirrelAlive && gameStarted) {

//			if (Vector2.Distance (squirrel.transform.position, transform.position) < 7f) {
//				float dx = squirrel.transform.position.x - transform.position.x;
//				float dy = squirrel.transform.position.y - transform.position.y;
//
//
//				if (dx > 0) {
//					animator.SetTrigger ("dogRight");
//				} else if (dx < 0) {
//					animator.SetTrigger ("dogLeft");
//				} 
//
//				Vector3 movement = new Vector3 (dx, dy, 0);
//				transform.position += movement * speed * Time.deltaTime;
//			} else {
//				animator.SetTrigger ("dogIdle");
//			}

			if (Vector2.Distance (target.transform.position, transform.position) < 7f && chasing == false) {
				chasing = true;
				seeker.StartPath (transform.position, target.position, onPathComplete);
			}

			if (chasing) {
				if (path == null) {
					//We have no path to move after yet
					animator.SetTrigger ("dogIdle");
					return;
				}

				if (currentWaypoint >= path.vectorPath.Count) {
					Debug.Log ("End Of Path Reached");
					chasing = false;
					return;
				}

				//Direction to the next waypoint
				Vector3 dir = (path.vectorPath [currentWaypoint] - transform.position).normalized;


				//for animation
				if (dir.x > 0) {
					animator.SetTrigger ("dogRight");
				} else if (dir.x < 0) {
					animator.SetTrigger ("dogLeft");
				} 

				dir *= speed * Time.fixedDeltaTime;
				this.gameObject.transform.Translate (dir);

				//Check if we are close enough to the next waypoint
				//If we are, proceed to follow the next waypoint
				if (Vector3.Distance (transform.position, path.vectorPath [currentWaypoint]) < nextWaypointDistance) {
					currentWaypoint++;
					return;
				}
			} else {
				animator.SetTrigger ("dogIdle");
			}
		}


	}

	public void onPathComplete(Path p){
		if (!p.error) {
			path = p;
			currentWaypoint = 0;
		}
	}

	void setGo(){
		gameStarted = true;
	}

	void killSquirrel(){
		squirrelAlive = false;
		animator.SetTrigger ("dogKill");
	}
		
}
