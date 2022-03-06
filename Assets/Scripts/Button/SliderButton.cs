using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SliderButton : Interactable
{

    [SerializeField] private List<Transform> sliderValues;

    [Space(5)]

    [SerializeField] private Transform cursor;

    [Space(5)]

    [SerializeField] private Collider2D cursorCollider;

    private Vector3 previousPos;
    private Vector3 actualPos;
    private Vector3 direction;
    private float directionMagnitude;
    private int state;
    private bool isHit;

    protected override void Update()
    {
        base.Update();
        if (broken)
            return;

        if (CheckForTouch() || CheckForTouchMouse())
        {

            if (previousPos != Vector3.zero)
            {
                PlaceCursor(false);

            }

            previousPos = actualPos;

        }
        else if(Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            isHit = false;
            if(previousPos != Vector3.zero)
            {

                previousPos = Vector2.zero;
                PlaceCursor(true);

            }

        }
    }

    private void PlaceCursor (bool clamp)
    {
        direction = sliderValues[sliderValues.Count - 1].position - sliderValues[0].position;
        directionMagnitude = direction.magnitude;

        Vector3 touchDirecion = actualPos - sliderValues[0].position;
        Vector3 cursorVector = Vector3.Project(touchDirecion, direction);

        if (Vector3.Dot(cursorVector, direction) > 0)
        {

            float interval = directionMagnitude / 3f;
            float rounded = Mathf.Round(cursorVector.magnitude / interval) * interval;
            float magnitude = clamp? rounded : cursorVector.magnitude;
            cursorVector = cursorVector.normalized * Mathf.Clamp(magnitude, 0f, directionMagnitude);
            if(clamp)
            {

                float state = Mathf.Round(rounded/directionMagnitude * 3f);
                state = Mathf.Clamp(state,0f,3f);
                SendState((int)state);

            }
            
        }          
        else
            cursorVector = Vector3.zero;

        cursor.position = sliderValues[0].position + cursorVector;
    }

    protected bool CheckForTouch()
    {
        if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved))
        {

            var worldPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPoint = new Vector2(worldPosition.x, worldPosition.y);

            actualPos = worldPosition;

            if (isHit || cursorCollider == Physics2D.OverlapPoint(touchPoint))
            {
                isHit = true;
                return true;
            }
                

        }

        return false;

    }
    protected bool CheckForTouchMouse()
    {
        if (Input.GetMouseButton(0))
        {

            var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPoint = new Vector2(worldPosition.x, worldPosition.y);

            actualPos = worldPosition;

            if (isHit || cursorCollider == Physics2D.OverlapPoint(touchPoint))
            {
                isHit = true;
                return true;
            }
                

        }

        return false;

    }
}
