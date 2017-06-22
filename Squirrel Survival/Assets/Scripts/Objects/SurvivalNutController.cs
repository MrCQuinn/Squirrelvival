using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalNutController : MonoBehaviour {

    public GameObject acorn;

	void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag != "Dog" && other.gameObject.tag != "Car")
        {
            LevelController lvlctrl = FindObjectOfType<LevelController>();
            lvlctrl.SendMessage("nutSpawn");
            if(other.gameObject.tag == "Squirrel")
            {
                SquirrelController sqr = FindObjectOfType<SquirrelController>();
                sqr.random_pickup();
                lvlctrl.SendMessage("OnAcornCollect", gameObject.transform.position);
                LevelTimer timer = FindObjectOfType<LevelTimer>();
                if(timer != null)
                {
                    timer.SendMessage("addTime");
                }
                
            }
            Destroy(gameObject);
        }
    }
}
