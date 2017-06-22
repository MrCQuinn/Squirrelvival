using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilSquirrels : MonoBehaviour {

    private Vector3 startLoc;
    private Vector3 endLoc;
    private int num;
    private string acornName;
    private GameObject acorn;
    private bool arrived;
    private bool left = false;
    private bool right = false;
    private bool up = false;
    private bool down = false;
	private Animator animator;

    public float speed;
    // Use this for initialization
    void Start()
    {
		animator = GetComponent<Animator>();
        startLoc = transform.position;
        bool loop = true;
        arrived = false;
        while (loop)
        {
            num = Random.Range(1, 334);
            acornName = "Acorn (" + num.ToString() + ")";
            Debug.Log(acornName);
            if (GameObject.Find(acornName) != null)
            {
                acorn = GameObject.Find(acornName);
                endLoc = acorn.transform.position;
                loop = false;
                Debug.Log("Object acquired");
            }
        }
        float diffX = transform.position.x - acorn.transform.position.x;
        float diffY = transform.position.y - acorn.transform.position.y;
        if (diffX > 0)
        {
            if (diffY > 0)
            {
                if (diffX > diffY)
                {
                    left = true;
                }
                else
                {
                    down = true;
                }
            }
            else
            {
                if (diffX > -diffY)
                {
                    left = true;
                }
                else
                {
                    up = true;
                }
            }
        }
        else
        {
            if (diffY > 0)
            {
                if (-diffX > diffY)
                {
                    right = true;
                }
                else
                {
                    down = true;
                }
            }
            else
            {
                if (-diffX > -diffY)
                {
                    right = true;
                }
                else
                {
                    up = true;
                }
            }
        }

		if (up) {
			animator.SetTrigger ("SquirrelUp");
		}

		if (down) {
			animator.SetTrigger ("SquirrelDown");
		}

		if (right) {
			animator.SetTrigger ("SquirrelRight");
		}

		if (left) {
			animator.SetTrigger ("SquirrelLeft");
		}



    }
        // Update is called once per frame
    void Update () {
        float step = speed * Time.deltaTime;
        if (!arrived)
        {
            transform.position = Vector3.MoveTowards(transform.position, endLoc, step);
            if(transform.position == endLoc)
            {
                arrived = true;
                bool temp = left;
                left = right;
                right = temp;
                temp = up;
                up = down;
                down = temp;
                Destroy(acorn);
            }
        } else
        {
            transform.position = Vector3.MoveTowards(transform.position, startLoc, step);
            if(transform.position == startLoc)
            {
                Destroy(gameObject);
            }
        }
    }
}
