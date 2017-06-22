using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestLevelController : MonoBehaviour
{
	public UIAcornController acornctrl;
	public new Camera camera;
	public Canvas canvas;
	public LevelTimer timer;
	public Text getReadyText;
	public float time;
	public Text gameOverText;
	public Text restartText;
	public Transform menu;

	public int nutsNeeded;


	private static readonly int delay = 3;
	private static readonly int dPart = delay / 3;
	private RectTransform canvasRect;

	private float startTime;
	private int nuts;
	private GameObject[] dogs;
	private GameObject[] cars;
	private bool gameOver;
	private bool atHome;
	private float atHomeMessageTimer;


	private void Awake()
	{
		// initialize timer's time property before the timer runs Start()
		timer.time = time;
	}
	// Use this for initialization
	private void Start()
	{
		restartText.text = "";
		gameOverText.text = "";
		menu.gameObject.SetActive (false);

		startTime = Time.time + delay;

		canvasRect = canvas.GetComponent<RectTransform>();

		dogs = GameObject.FindGameObjectsWithTag("Dog");
		cars = GameObject.FindGameObjectsWithTag("Car");


		SquirrelController.restart();

		gameOver = false;
		atHome = false;
		// generate border trees

	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
		{
			Pause ();
		}
	}

	private void FixedUpdate()
	{

		if (timer.paused && Time.time >= startTime)
		{
			timer.paused = false;
		}

		//if game is over listen for R to restart level
		if (gameOver) {
			if (Input.GetKeyDown (KeyCode.R))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}


		//if colliding with home tree
		if (atHome) {

			//if you have enough nuts
			if (nuts >= nutsNeeded) {
				//listen for C key to load next level
				if (Input.GetKeyDown (KeyCode.C)) {
					goToNextLevel ();
				}
			} else {
				//display temporary message that you need more nuts
				if (atHomeMessageTimer > 0) {
					atHomeMessageTimer -= Time.deltaTime;
				} else {
					gameOverText.text = "";
					atHome = false;
				}
			}
		}


		if (Time.time < startTime + 1)
		{
			float dTime = Time.time - startTime + delay;
			if (dTime < dPart)
			{
				getReadyText.text = "3";
			}
			else if (dTime < 2 * dPart)
			{
				getReadyText.text = "2";
			}
			else if (dTime < delay)
			{
				getReadyText.text = "1";
			}
			else
			{
				getReadyText.text = "Go!!";
				activatePlayers ();
			}
		}
		else
		{
			Destroy(getReadyText);
		}
	}

	public void onNutCollect(Vector3 worldPosition)
	{
		++nuts;
		Vector2 pos = camera.WorldToViewportPoint(worldPosition);
		pos = new Vector2(
			((pos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
			((pos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
		acornctrl.onNutCollect(pos, nuts);
	}

	public void onGoldenAcornCollect(Vector3 worldPosition)
	{
		StartCoroutine(goldenAcornCoroutine(20, worldPosition));
	}

	public IEnumerator goldenAcornCoroutine(int nutValue, Vector3 worldPosition)
	{
		for (int i = 0; i < nutValue; ++i)
		{
			onNutCollect(worldPosition);
			yield return new WaitForSeconds(0.05f);
		}
	}

	public void SlowTime(){
		if (dogs != null && dogs.Length > 0) {
			foreach (GameObject dog in dogs) {
				dog.SendMessage ("slowDown");
			}
		}

		if (cars != null && cars.Length > 0) {
			foreach (GameObject car in cars) {
				car.SendMessage ("slowDown");
			}
		}

		timer.SendMessage ("slowDown");

	}

	public void SpeedUp(){
		if (dogs != null && dogs.Length > 0) {
			foreach (GameObject dog in dogs) {
				dog.SendMessage ("speedUp");
			}
		}

		if (cars != null && cars.Length > 0) {
			foreach (GameObject car in cars) {
				car.SendMessage ("speedUp");
			}
		}

		timer.SendMessage ("speedUp");

	}


	public void GameOver(bool levelWon)
	{
		gameOver = true;

		SquirrelController.setDead ();

		if (levelWon) {
			gameOverText.text = "You Win!!";
			restartText.text = "Press 'C' to Continue";
		} else {
			gameOverText.text = "Game Over!";
			restartText.text = "Press 'R' for Restart";
		}
	}

	public void hitHome(){
		atHome = true;
		if (nuts >= nutsNeeded) {
			deactivatePlayers ();
			if (!gameOver) {
				GameOver (true);
			}
		} else {
			gameOverText.text = "Not enough nuts!!";
			atHomeMessageTimer = 4;
			//restartText.text = "";
		}
	}

	private void activatePlayers(){
		SquirrelController.setGo();
		if (dogs != null && dogs.Length > 0) {
			foreach (GameObject dog in dogs) {
				dog.SendMessage ("setGo");
			}
		}
	}

	private void deactivatePlayers(){
		SquirrelController.setStop();
		if (dogs != null && dogs.Length > 0) {
			foreach (GameObject dog in dogs) {
				dog.SendMessage ("setStop");
			}
		}
	}

	private void goToNextLevel(){
		Scene scene = SceneManager.GetActiveScene();
		if (scene.name == "Park level") 
		{
			SceneManager.LoadScene("Suburb level");
		}
		if (scene.name == "Suburb level") 
		{
			SceneManager.LoadScene("Highway level");
		}
		if (scene.name == "Highway level") 
		{
			SceneManager.LoadScene("Forest level");
		}
		if (scene.name == "Forest level") 
		{
			SceneManager.LoadScene("Tree Farm level");
		}
		if (scene.name == "Tree Farm level") 
		{
			SceneManager.LoadScene("Win scene");
		}
	}

	public void Pause()
	{
		if (menu.gameObject.activeInHierarchy == false) {
			menu.gameObject.SetActive (true);
			deactivatePlayers ();
			Time.timeScale = 0;
		} else {
			menu.gameObject.SetActive (false);
			activatePlayers ();
			Time.timeScale = 1;
		}
	}
}
