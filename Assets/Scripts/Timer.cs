using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    [SerializeField] private SpriteRenderer sliderSpriteRenderer;

    [SerializeField] private Color colorsStart;
    [SerializeField] private Color colorsEnd;
    private float startWidth;
    private SpriteRenderer spriteRenderer;
    private float timer;
    private float startTime;
    private bool isRunning = false;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sliderSpriteRenderer.color = colorsStart;
        startWidth = spriteRenderer.size.x;
        sliderSpriteRenderer.size = spriteRenderer.size;
    }

    // Update is called once per frame
    void Update()
    {
        if(isRunning)
            DecreaseTimer();
    }

    public void SetTimer(float delay)
    {
        timer = delay;
        startTime = Time.time;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    private void DecreaseTimer()
    {

        float percentage = (Time.time - startTime) / timer;
        if (percentage < 1)
        {

            sliderSpriteRenderer.color = Color.Lerp(colorsStart, colorsEnd, percentage);
            sliderSpriteRenderer.size = new Vector2(startWidth * (1 - percentage), spriteRenderer.size.y);
            sliderSpriteRenderer.transform.position = spriteRenderer.bounds.min + new Vector3(sliderSpriteRenderer.bounds.extents.x, spriteRenderer.bounds.extents.y);

        }
        else
        {
            isRunning = false;
        }
 
    }

}
