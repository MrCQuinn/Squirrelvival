using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class LevelController : MonoBehaviour {
    protected new Camera camera;
    protected Canvas canvas;
    protected RectTransform canvasRect;
    protected LevelTimer timer;

    private Transform pauseMenu;
    private Text getReadyText;
    protected class DelayData // collect data related to level start delay in one place
    {
        public int delay = 3;
        public int dPart;
        public float startTime;

        public DelayData()
        {
            dPart = delay / 3;
        }
    }
    protected DelayData delay;

    protected List<ICharacterController> NPCs = new List<ICharacterController>();
    protected int nuts;
    protected UIAcornController acornCtrl;


	public LevelController()
    {

    }

    public virtual int MinimumNuts
    {
        get
        {
            return 0;
        }
    }

    public virtual float LevelTime
    {
        get
        {
            return 0;
        }
    }

    protected virtual void Awake()
    {
        GameObject temp;

        temp = GameObject.FindGameObjectWithTag("UI");
        if (temp == null)
        {
            Debug.Log(this.GetType().Name + " could not find a UI in the scene!");
        }
        else
        {
            canvas = temp.GetComponent<Canvas>();
            canvasRect = canvas.GetComponent<RectTransform>();
            if ((timer = canvas.GetComponent<LevelTimer>()) == null)
            {
                Debug.Log(this.GetType().Name + " could not find a LevelTimer in UI canvas!");
            }
            // initialize timer's time property before the timer runs Start()
            if (timer != null)
            {
                timer.time = LevelTime;
            }
        }
    }

    protected virtual void Start()
    {
        GameObject temp;    // temporary storing place for possibly null objects

        if ((acornCtrl = FindObjectOfType<UIAcornController>()) == null)
        {
            Debug.Log(this.GetType().Name + " could not find a UIAcornController in the scene!");
        }

        temp = GameObject.FindGameObjectWithTag("MainCamera");
        if (temp == null)
        {
            Debug.Log(this.GetType().Name + " could not find a MainCamera in the scene!");
        }
        else
        {
            camera = temp.GetComponent<Camera>();
        }

        temp = GameObject.FindGameObjectWithTag("PauseMenu");
        if (temp == null)
        {
            Debug.Log(this.GetType().Name + " could not find a PauseMenu in the scene!");
        }
        else
        {
            pauseMenu = temp.transform;
            pauseMenu.gameObject.SetActive(false);
        }

        temp = GameObject.FindGameObjectWithTag("ReadyText");
        if (temp == null)
        {
            Debug.Log(this.GetType().Name + " could not find a ReadyText found in the scene!");
        }
        else
        {
            getReadyText = temp.GetComponent<Text>();
        }

        delay = new DelayData();
        delay.startTime = Time.time + delay.delay;
    }

    protected virtual void FixedUpdate()
    {
        if (Time.time < delay.startTime + 1)
        {
            float dTime = Time.time - delay.startTime + delay.delay;
            if (dTime < delay.dPart)
            {
                getReadyTextField = "3";
            }
            else if (dTime < 2 * delay.dPart)
            {
                getReadyTextField = "2";
            }
            else if (dTime < delay.delay)
            {
                getReadyTextField = "1";
            }
            else
            {
                getReadyTextField = "Go!!";
                
                foreach (ICharacterController NPC in NPCs)
                {
                    SquirrelController.setGo();
                    NPC.Begin();
                }
            }
        }
        else
        {
            Destroy(getReadyText);
        }
    }

    protected virtual string getReadyTextField
    {
        set
        {
            if (getReadyText != null)
            {
                getReadyText.text = value;
            }
        }
        get
        {
            if (getReadyText != null)
            {
                return getReadyText.text;
            }
            else
            {
                return "";
            }
        }
    }

    public void RegisterCharacter(ICharacterController NPC)
    {
        NPCs.Add(NPC);
    }

    virtual public void OnAcornCollect(Vector3 worldPosition)
    {
        ++nuts;
        Vector2 pos = camera.WorldToViewportPoint(worldPosition);
        pos = new Vector2(
            ((pos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((pos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        acornCtrl.onNutCollect(pos, nuts);
    }

    virtual public void OnGoldenAcornCollect(Vector3 worldPosition)
    {
        nuts += 20;
        Vector2 pos = camera.WorldToViewportPoint(worldPosition);
        pos = new Vector2(
            ((pos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((pos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        acornCtrl.onGoldenNutCollect(pos, 20, nuts);
    }

    protected virtual void UnPausePlayers()
    {
        SquirrelController.setGo();

        foreach (ICharacterController NPC in NPCs)
        {
            NPC.UnPause();
        }
        /*
		if (dogs != null && dogs.Length > 0) {
			foreach (GameObject dog in dogs) {
				if (dog != null) {
                    dog.SendMessage("setGo");
					dog.SendMessage ("unFreeze");
				}
			}
		}
        */
    }

    protected virtual void PausePlayers()
    {
        SquirrelController.setStop();

        foreach (ICharacterController NPC in NPCs)
        {
            NPC.Pause();
        }
        /*
		if (dogs != null && dogs.Length > 0) {
			foreach (GameObject dog in dogs) {
				dog.SendMessage ("freeze");
			}
		}
        */
    }

    public virtual void Pause()
    {
        pauseMenu.gameObject.SetActive(true);
        PausePlayers();
        Time.timeScale = 0;
    }

    public virtual void UnPause()
    {
        pauseMenu.gameObject.SetActive(false);
        UnPausePlayers();
        Time.timeScale = 1;
    }

    abstract public void SlowTime();
    abstract public void SpeedUp();
    abstract public void GameOver(bool levelWon);
	abstract public void tallyScore();
    abstract public void hitHome();
    abstract public void hometreeOffScreen(bool value);
}
