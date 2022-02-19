using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceTeam;
using Array2DEditor;

public class Goal
{
    public int id;
    public string state;

    public Goal(int id, string state)
    {
        this.id = id;
        this.state = state;
    }
}

public abstract class TypeButton : MonoBehaviour
{
    [SerializeField] protected ItemData itemData;

    [Space(5)]

    [SerializeField] protected TextMesh textMesh;

    [Space(5)]

    [SerializeField] protected List<TextMesh> textMeshsButtons;

    [Space(5)]

    [SerializeField] protected List<Collider2D> colliders;

    [Space(5)]

    [SerializeField] protected List<SpriteRenderer> spriteRenderers;

    [Space(5)]

    [SerializeField] protected Array2DSprite buttonsSprites;

    protected int[] states;
    protected Goal goal;
    protected abstract void ChangeState(int index);

    private void Start()
    {
        textMesh.text = itemData.name;
        states = new int[colliders.Count];

        if (textMeshsButtons.Count > 0)
        {
            for (int index = 0; index < itemData.Values.Length; ++index)
            {
                textMeshsButtons[index].text = itemData.Values[index];
            }
        }
    }

    protected void GetGoal(int id, string state)
    {
        goal = new Goal(id, state);
    }

    protected void SendState(int id, int state)
    {
        if (goal != null && goal.id == id && goal.state == itemData.Values[state])
            GameManager.SendGoalComplete();
    }

    private void Update()
    {

        int index;
        index = CheckForTouch();
        if (index != -1 )
        {
            ChangeState(index);
        }
        index = CheckForTouchMouse();
        if (index != -1)
        {
            ChangeState(index);
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


