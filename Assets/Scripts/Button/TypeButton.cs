using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceTeam;
using Array2DEditor;



public abstract class TypeButton : Interactable
{

    [SerializeField] protected List<SpriteRenderer> spriteRenderers;

    [Space(5)]

    [SerializeField] protected Array2DSprite buttonsSprites;

    [Space(5)]

    [SerializeField] protected List<Collider2D> colliders;
    protected int[] states;

    protected abstract void ChangeState(int index);
    private void Awake()
    {        
        states = new int[colliders.Count];
    }


    private void Update()
    {

        int index;
        index = CheckForTouch();
        if (index != -1 )
        {
            ChangeState(index);
        }
        else
        {
            index = CheckForTouchMouse();
            if (index != -1)
            {
                ChangeState(index);
            }
        }
        

    }

    protected int CheckForTouch()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            var worldPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPoint = new Vector2(worldPosition.x, worldPosition.y);

            for(int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i] == Physics2D.OverlapPoint(touchPoint))
                    return i;
            }

        }

        return -1;

    }
    protected int CheckForTouchMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {

            var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPoint = new Vector2(worldPosition.x, worldPosition.y);

            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i] == Physics2D.OverlapPoint(touchPoint))
                    return i;
            }

        }

        return -1;

    }

}


