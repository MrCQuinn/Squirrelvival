using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAcornController : MonoBehaviour
{
    public RectTransform acornFlyup;
    public RectTransform acornIcon;
    public NutCountController nutcounter;

    // basic nut flyup params
    private Vector2 acornISizeD;
    private Vector2 acornIPos;
    private bool nutSwell = false;
    private float nutSwStart;
    public float nutSwellDuration;
    public float nutSwellScale;

    // golden nut flyup params
    public float goldenRadialSpeed;
    public float goldenOutwardSpeed;

    private int nutcount = 0;

    private delegate void onComplete(int count);

    public void onNutCollect(Vector2 position, int count)
    {
        StartCoroutine(flyup(position, acornIcon.localPosition, onFlyupComplete, count));
    }

    public void onGoldenNutCollect(Vector2 position, int value, int count)
    {
        float deltaAngle = (2 * Mathf.PI) / value;

        for (int i = 0; i < value; ++i)
        {
            StartCoroutine(gFlyup(position, acornIcon.localPosition, deltaAngle * i, onFlyupComplete, nutcount + i));
        }
    }

    private IEnumerator gFlyup(Vector2 start, Vector2 end, float angle, onComplete done, int count)
    {
        float radius = Vector2.Distance(start, end) - 30;

        RectTransform t = Instantiate<RectTransform>(acornFlyup);
        t.SetParent(this.transform, false);
        t.anchoredPosition = start;

        Vector2 rot = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        t.localPosition = ((Vector2)t.localPosition) + rot;

        bool finished = false;
        bool finishFlyup = false;
        float flyupSpeed = 0;

        while (!finished)
        {
            Vector2 dir = (Vector2)t.localPosition - start;
            dir = Quaternion.Euler(0, 0, goldenRadialSpeed * Time.deltaTime) * dir;
            t.localPosition = dir + start;

            if (!finishFlyup)
            {
                if (Vector2.Distance(start, t.localPosition) < radius)
                {
                    t.localPosition += (Vector3)((Vector2)t.localPosition - start).normalized * goldenOutwardSpeed * Time.deltaTime;
                }
                else
                {
                    finishFlyup = true;
                }
            }
            else
            {
                if (Mathf.Abs(Vector2.Distance(end, t.localPosition)) < 50)
                {
                    finished = true;
                }
            }
            yield return null;
        }

        Destroy(t.gameObject);
        done(count);
    }

    private IEnumerator flyup(Vector2 start, Vector2 end, onComplete done, int count)
    {
        RectTransform t = Instantiate<RectTransform>(acornFlyup);
        t.SetParent(this.transform, false);
        t.anchoredPosition = start;
        // Vector2 vnorm = Vector3.Normalize(acornIcon.localPosition);
        // rb.velocity = Vector3.Normalize(acornIcon.localPosition);
        float i = 1;
        while (t.localPosition.y < acornIcon.localPosition.y - 50)
        {
            t.localPosition = Vector2.MoveTowards(t.localPosition, end, i);
            i += .7f;
            yield return null;
        }
        Destroy(t.gameObject);
        done(count);

    }

    private void onFlyupComplete(int count)
    {
        nutSwell = true;
        nutSwStart = Time.time;
        nutcounter.updateCount(++nutcount);
        /*
        if (count > nutcount)
        {
            nutSwell = true;
            nutSwStart = Time.time;
            nutcounter.updateCount(count);
            nutcount = count;
        }
        */
    }

    // Use this for initialization
    void Start()
    {
        acornISizeD = acornIcon.sizeDelta;
        acornIPos = acornIcon.anchoredPosition;
    }

    private void FixedUpdate()
    {
        if (nutSwell)
        {
            if (Time.time > nutSwStart + nutSwellDuration)
            {
                nutSwell = false;
                acornIcon.sizeDelta = acornISizeD;
                acornIcon.anchoredPosition = acornIPos;
            }
            else
            {
                float scale = ((1 - ((Time.time - nutSwStart) / nutSwellDuration)) * nutSwellScale) + 1;
                acornIcon.sizeDelta = acornISizeD * scale;
                acornIcon.anchoredPosition = acornIPos + ((acornIcon.rect.size - acornISizeD) * .5f);
            }
        }
    }
}
