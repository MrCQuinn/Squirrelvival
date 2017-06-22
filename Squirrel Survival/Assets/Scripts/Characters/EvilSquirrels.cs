using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilSquirrels : MonoBehaviour, ICharacterController {

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
    private SpriteRenderer sr;

    public float speed;

    private void Awake()
    {
        // register with LevelController
        LevelController lvlctrl;
        if ((lvlctrl = FindObjectOfType<LevelController>()) != null)
        {
            lvlctrl.RegisterCharacter(this);
        }
        else
        {
            Debug.Log(this.GetType().Name + " could not find a LevelController!");
        }
    }

    // Use this for initialization
    void Start()
    {
		animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        startLoc = transform.position;
        bool loop = true;
        arrived = false;
        while (loop)
        {
            num = Random.Range(1, 334);
            acornName = "Acorn (" + num.ToString() + ")";
            if (GameObject.Find(acornName) != null)
            {
                acorn = GameObject.Find(acornName);
                endLoc = acorn.transform.position;
                loop = false;
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
        /*
        Debug.Log(up);
        Debug.Log(down);
        Debug.Log(left);
        Debug.Log(right);
        */
		if (up) {
			animator.SetTrigger ("SquirrelUp");
		}

		if (down) {
			animator.ResetTrigger("SquirrelLeft");
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
                /*
                Debug.Log(up);
                Debug.Log(down);
                Debug.Log(left);
                Debug.Log(right);
                */
                if (up)
                {
                    animator.SetTrigger("SquirrelUp");
                }

                

                if (right)
                {
                    animator.SetTrigger("SquirrelRight");
                }

                if (left)
                {
                    animator.SetTrigger("SquirrelLeft");
                }

				if (down)
				{
					animator.SetTrigger("SquirrelDown");
				}
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

    private void FixedUpdate()
    {
        // update sorting order
        sr.sortingOrder = (int)((transform.position.y + 0.05) * -10);
    }

    public void Begin()
    {

    }

    public void Pause()
    {

    }

    public void UnPause()
    {

    }
}
