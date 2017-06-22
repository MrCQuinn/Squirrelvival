using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class SquirrelController : MonoBehaviour
{
    public float speed;
    public Sprite squishedSquirrel;
    public GameObject timeberry_particle;
    public static bool go = false;
    public static bool disabled = false;
    public static bool dead = true;

    private Animator animator;
    private float moveHorizontal;
    private float moveVertical;
    bool    moveRight   = false,
            moveLeft    = false,
            moveUp      = false,
            moveDown    = false;
    public static bool inTree;
	private bool onBerries;
    private int count;
	private float timeBerryTimer;
	private AudioSource[] nutSounds;
	private LevelController lvlctrl;



    private GameObject currentTree;
    private SpriteRenderer sr;

    // Use this for initialization
    void Start()
    {
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();

		lvlctrl = FindObjectOfType<LevelController>();

		nutSounds = GetComponents<AudioSource>();

        dead = false;
		onBerries = false;

        inTree = false;
    }

    // Update is called once per frame
    void Update()
    {
        moveLeft = moveRight = moveUp = moveDown = false;


        
        if (go && !disabled)
        {
			
			#if UNITY_ANDROID
				float dx = CrossPlatformInputManager.GetAxis("Horizontal");
				float dy = CrossPlatformInputManager.GetAxis("Vertical");
				if (dx > .3)
				{
					moveRight = true;
				}

				if (dx < -.3)
				{
					moveLeft = true;
				}

				if (dy > .3)
				{
					moveUp = true;
				}

				if (dy < -.3)
				{
					moveDown = true;
				}
			#else
				if (Input.GetKey("right") || Input.GetKey(KeyCode.D))
	            {
	                moveRight = true;
	            }

				if (Input.GetKey("left") || Input.GetKey(KeyCode.A))
	            {
	                moveLeft = true;
	            }

				if (Input.GetKey("up") || Input.GetKey(KeyCode.W))
	            {
	                moveUp = true;
	            }

				if (Input.GetKey("down") || Input.GetKey(KeyCode.S))
	            {
	                moveDown = true;
	            }
			#endif
			move(moveLeft, moveRight, moveUp, moveDown);
        }
    }

    public void move(bool left, bool right, bool up, bool down, float multiplier = 1)
    {
        moveHorizontal = moveVertical = 0;
        if (!dead && !inTree)
        {
            if (right == true && left == false)
            {
                animator.SetTrigger("squirrelRight");
                moveHorizontal += 0.354f;
                if (up == true && down == false)
                {
                    moveVertical += 0.354f;
                }
                else if (down == true && up == false)
                {
                    moveVertical -= 0.354f;
                }
                else
                {
                    moveHorizontal += 0.146f;
                }
            }
            else if (left == true && right == false)
            {
                animator.SetTrigger("squirrelLeft");
                moveHorizontal -= 0.354f;
                if (up == true && down == false)
                {
                    moveVertical += 0.354f;
                }
                else if (down == true && up == false)
                {
                    moveVertical -= 0.354f;
                }
                else
                {
                    moveHorizontal -= 0.146f;
                }
            }
            else if (up == true && down == false && right == left)
            {
                animator.SetTrigger("squirrelUp");
                moveVertical += 0.5f;
            }
            else if (down == true && up == false && right == left)
            {
                animator.SetTrigger("squirrelDown");
                moveVertical -= 0.5f;
            }
            else
            {
                animator.ResetTrigger("squirrelDown");
                animator.SetTrigger("squirrelStop");
            }
        }
        else
        {
            if ((Input.GetKeyDown("up") || Input.GetKeyDown(KeyCode.W)) && !Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + currentTree.GetComponent<BoxCollider2D>().size.y * currentTree.transform.localScale.y), Vector2.up, 1f, LayerMask.GetMask("Obstacles")))
            {
                descendFromTree();
                transform.position = new Vector3(transform.position.x, transform.position.y + currentTree.GetComponent<BoxCollider2D>().size.y * currentTree.transform.localScale.y, 0f);
            } else if ((Input.GetKeyDown("down") || Input.GetKeyDown(KeyCode.S)) && !Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - currentTree.GetComponent<BoxCollider2D>().size.y * currentTree.transform.localScale.y), Vector2.down, 1f, LayerMask.GetMask("Obstacles")))
            {
                descendFromTree();
                transform.position = new Vector3(transform.position.x, transform.position.y - currentTree.GetComponent<BoxCollider2D>().size.y * currentTree.transform.localScale.y, 0f);
            } else if ((Input.GetKeyDown("left") || Input.GetKeyDown(KeyCode.A)) && !Physics2D.Raycast(new Vector2(transform.position.x - currentTree.GetComponent<BoxCollider2D>().size.x * currentTree.transform.localScale.x, transform.position.y), Vector2.left, 1f, LayerMask.GetMask("Obstacles")))
            {
                descendFromTree();
                transform.position = new Vector3(transform.position.x - currentTree.GetComponent<BoxCollider2D>().size.x * currentTree.transform.localScale.x - 0.5f, transform.position.y, 0f);
            } else if ((Input.GetKeyDown("right") || Input.GetKeyDown(KeyCode.D)) && !Physics2D.Raycast(new Vector2(transform.position.x + currentTree.GetComponent<BoxCollider2D>().size.x * currentTree.transform.localScale.x, transform.position.y), Vector2.right, 1f, LayerMask.GetMask("Obstacles")))
            {
                descendFromTree();
                transform.position = new Vector3(transform.position.x + currentTree.GetComponent<BoxCollider2D>().size.x * currentTree.transform.localScale.x + 0.5f, transform.position.y, 0f);
            }


			#if UNITY_ANDROID
				if (up && !Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + currentTree.GetComponent<BoxCollider2D>().size.y * currentTree.transform.localScale.y), Vector2.up, 1f, LayerMask.GetMask("Obstacles")))
				{
					descendFromTree();
					transform.position = new Vector3(transform.position.x, transform.position.y + currentTree.GetComponent<BoxCollider2D>().size.y * currentTree.transform.localScale.y, 0f);
				} else if (down && !Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - currentTree.GetComponent<BoxCollider2D>().size.y * currentTree.transform.localScale.y), Vector2.down, 1f, LayerMask.GetMask("Obstacles")))
				{
					descendFromTree();
					transform.position = new Vector3(transform.position.x, transform.position.y - currentTree.GetComponent<BoxCollider2D>().size.y * currentTree.transform.localScale.y, 0f);
				} else if (left && !Physics2D.Raycast(new Vector2(transform.position.x - currentTree.GetComponent<BoxCollider2D>().size.x * currentTree.transform.localScale.x, transform.position.y), Vector2.left, 1f, LayerMask.GetMask("Obstacles")))
				{
					descendFromTree();
					transform.position = new Vector3(transform.position.x - currentTree.GetComponent<BoxCollider2D>().size.x * currentTree.transform.localScale.x - 0.5f, transform.position.y, 0f);
				} else if (right && !Physics2D.Raycast(new Vector2(transform.position.x + currentTree.GetComponent<BoxCollider2D>().size.x * currentTree.transform.localScale.x, transform.position.y), Vector2.right, 1f, LayerMask.GetMask("Obstacles")))
				{
					descendFromTree();
					transform.position = new Vector3(transform.position.x + currentTree.GetComponent<BoxCollider2D>().size.x * currentTree.transform.localScale.x + 0.5f, transform.position.y, 0f);
				}

			#endif

        }

        moveVertical *= multiplier;
        moveHorizontal *= multiplier;
    }

    void FixedUpdate()
    {
        //Use the two store floats to create a new Vector2 variable movement.
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0);

		if (!dead) {
			transform.position += movement * speed * Time.deltaTime;
		}
        

        // update sorting order
        sr.sortingOrder = (int)((transform.position.y + 0.05) * -10);



		if (onBerries) {
			if (timeBerryTimer > 0) {
				timeBerryTimer -= Time.deltaTime;
			} else {
				speedUp ();
				onBerries = false;
			}
		}
    }

    public void random_pickup()
    {
        int choose = Random.Range(0,3);
		nutSounds [choose].Play ();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            //SetCountText ();
        }

        if (other.gameObject.CompareTag("Acorn"))
        {
            random_pickup();
            lvlctrl.SendMessage("OnAcornCollect", other.gameObject.transform.position);
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Golden Acorn"))
        {
			nutSounds [0].Play ();
            lvlctrl.SendMessage("OnGoldenAcornCollect", other.gameObject.transform.position);
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Car")
        {
            squish();
        }

		if (other.gameObject.tag == "dogSprite" && !inTree)
        {
			other.gameObject.SendMessage("killSquirrel");
			dieByDog ();
        }

		if (other.gameObject.tag == "Timeberry") {
            random_pickup();
            Instantiate(timeberry_particle, other.transform.position, other.transform.rotation);
            other.gameObject.SetActive (false);
			timeBerryAction ();
		}
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
		if (coll.gameObject.tag == "Transformer") {
			shocked ();
		}

        if (coll.gameObject.tag == "Dog")
        {
            coll.gameObject.SendMessage("killSquirrel");

            gameObject.SetActive(false);
        }

		if (coll.gameObject.tag == "dogSprite" && !inTree)
		{
			coll.gameObject.SendMessage("killSquirrel");
			dieByDog ();
		}

		if (coll.gameObject.tag == "HomeTree") {
			lvlctrl.SendMessage ("hitHome");
		}

		if (coll.gameObject.tag == "ClimbTree") {
			//if (moveUp == true && moveDown == false && moveLeft == false && moveRight == false) {
				currentTree = coll.gameObject;
				currentTree.SendMessage ("Climbed");
                climbTree();
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                transform.position = new Vector3(currentTree.GetComponent<BoxCollider2D>().offset.x * currentTree.transform.localScale.x, currentTree.GetComponent<BoxCollider2D>().offset.y * currentTree.transform.localScale.y, 0) + currentTree.transform.position;
			//}
		}

		if (coll.gameObject.tag == "Tutorial1") {
			lvlctrl.SendMessage ("tutorial", 1);
		}
		if (coll.gameObject.tag == "Tutorial2") {
			lvlctrl.SendMessage ("tutorial", 2);
		}
		if (coll.gameObject.tag == "Tutorial3") {
			lvlctrl.SendMessage ("tutorial", 3);
		}
		if (coll.gameObject.tag == "Tutorial4") {
			lvlctrl.SendMessage ("tutorial", 4);
		}
		if (coll.gameObject.tag == "Tutorial5") {
			lvlctrl.SendMessage ("tutorial", 5);
		}
		if (coll.gameObject.tag == "Tutorial6") {
			lvlctrl.SendMessage ("tutorial", 6);
		}
    }

	private void timeBerryAction(){
		timeBerryTimer = 5;
		onBerries = true;
		lvlctrl.SendMessage("SlowTime");
	}

	private void speedUp(){
		lvlctrl.SendMessage("SpeedUp");
	}

	private void climbTree (){
		inTree = true;
		sr.enabled = false;
	}

	private void descendFromTree(){
		inTree = false;
		sr.enabled = true;
		if (currentTree != null) {
			currentTree.SendMessage ("Unclimbed");
		}
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
	}

    public bool isDisabled
    {
        set
        {
            this.GetComponent<BoxCollider2D>().enabled = !value;
            disabled = value;
        }
        get
        {
            return !this.GetComponent<BoxCollider2D>().enabled && disabled;
        }
    }

    internal static void setGo()
    {
		go = true;
    }

	internal static void setStop()
	{
		go = false;
	}

    public static void setDead()
    {
        dead = true;
    }

    public static void restart()
    {
        dead = false;
        go = false;
    }

    private void squish()
    {
		
        dead = true;
		lvlctrl.SendMessage ("GameOver", false);
        animator.SetTrigger("squirrelSquished");
		CameraController.deadSquirrel ();

    }

	private void shocked()
	{

		dead = true;
		lvlctrl.SendMessage ("GameOver", false);
		animator.SetTrigger("squirrelShock");
		CameraController.deadSquirrel ();

	}

	private void dieByDog(){
		dead = true;
		lvlctrl.SendMessage ("GameOver", false);
		gameObject.SetActive(false);
		CameraController.deadSquirrel ();
	}
}