using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour {

	public float timer;
	public Image avatar;
	public Text countdown;

	public float avatarStartPos;
	public float avatarEndPos;
    private float avStrPos;
    private float avEndPos;
    private float avatarPosDiff;

    private RectTransform avtrans;
    private RectTransform cvtrans;

	private float timeLeft;
	private Vector2 avaPos;

	private void Start() {
		cvtrans = gameObject.GetComponent<RectTransform> ();
        avtrans = gameObject.GetComponent<RectTransform>();

        avaPos = avatar.rectTransform.anchoredPosition;
        avStrPos = avatarStartPos * cvtrans.rect.width;
        avEndPos = avatarEndPos * cvtrans.rect.width;
//        Debug.Log(avaPos);
        avatarPosDiff = avStrPos - avEndPos;

        timeLeft = timer;

        updateAvatar();
	}
	
	// Update is called once per frame
	void Update () {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            timeLeft = 0;
        }
        countdown.text = ((int)timeLeft / 60).ToString().PadLeft(2, '0') + ":" + ((int)timeLeft % 60).ToString().PadLeft(2, '0');
        updateAvatar();
    }

	void FixedUpdate() {
		
	}

    private void updateAvatar()
    {
        avaPos.x = avEndPos + ((timeLeft / timer) * avatarPosDiff) + cvtrans.rect.x;
        avatar.rectTransform.anchoredPosition = avaPos;
    }

	void SetTimeText() {
		//timerText.text = "Time: " + timeLeft.ToString ();
	}
}
