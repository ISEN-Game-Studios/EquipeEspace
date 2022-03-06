using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceTeam;
using Array2DEditor;

public abstract class Interactable : MonoBehaviour
{
    public ItemData itemData;

    [Space(5)]

    [SerializeField] protected TextMeshWrapper textMeshWrapper;

    [Space(5)]

    [SerializeField] protected List<TextMesh> textMeshsButtons;

    protected bool broken;
    protected bool dragging;
    protected TargetJoint2D targetJoint;
    protected Rigidbody2D rgbd;

    protected virtual void Start()
    {
        textMeshWrapper.SetText(itemData.name);

        if(itemData.Shape == Shape.Big)
            textMeshWrapper.transform.localScale = Vector3.one / 2;

        if (textMeshsButtons.Count > 0)
        {
            for (int index = 0; index < itemData.Values.Length; ++index)
            {
                textMeshsButtons[index].text = itemData.Values[index];
            }
        }

        targetJoint = GetComponent<TargetJoint2D>();
        rgbd = GetComponent<Rigidbody2D>();
        broken = false;
        dragging = false;
    }

    protected virtual void Update()
    {
        if (Input.touchCount != 1)
        {
            dragging = false;
            targetJoint.enabled = false;
            return;
        }

        //if (Input.touchCount > 1)
        //{
        //    dragging = false;
        //    targetJoint.enabled = false;
        //    return;
        //}

        Touch touch = Input.GetTouch(0);
        Vector3 pos = touch.position;
        //Vector2 posM = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (touch.phase == TouchPhase.Began /* Input.GetMouseButtonDown(0)*/)
        {
            if (Physics2D.Raycast(pos, Vector3.forward))
            {
                if (broken)
                    targetJoint.enabled = true;
            }
        }
        if (touch.phase == TouchPhase.Moved /* Input.GetMouseButton(0))*/ && broken)
        {
            dragging = true;
            targetJoint.target = pos;
        }
        else if (dragging && /*Input.GetMouseButtonUp(0)*/(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            dragging = false;
            targetJoint.enabled = false;
            if (Mathf.Abs(rgbd.rotation) <= 5)
            {
                broken = false;
                rgbd.isKinematic = true;
                rgbd.angularVelocity = 0f;
                transform.rotation = Quaternion.identity;
            }
        }
    }

    public void Break()
    {
        rgbd.isKinematic = false;
        broken = true;
    }

    protected void SendState(int state)
    {
        Debug.Log(itemData.Values[state]);
        GameManager.OnStateChange(itemData.ID, state);
    }
}
