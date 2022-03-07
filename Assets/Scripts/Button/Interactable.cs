using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceTeam;
using Array2DEditor;
using System.Linq;

public abstract class Interactable : MonoBehaviour
{
    public ItemData itemData;

    [Space(5)]

    [SerializeField] protected TextMeshWrapper textMeshWrapper;

    [Space(5)]

    [SerializeField] protected List<TextMesh> textMeshsButtons;

    [Space(5)]

    [SerializeField] protected LayerMask maskClickButton;

    [SerializeField] protected LayerMask maskClickCell;

    protected bool broken;
    protected bool dragging;
    protected TargetJoint2D targetJoint;
    protected Rigidbody2D rgbd;

    private Vector3 startPosition;
    protected virtual void Awake()
    {
        targetJoint = GetComponent<TargetJoint2D>();
        rgbd = GetComponent<Rigidbody2D>();
    }
    protected virtual void Start()
    {
        textMeshWrapper.SetText(itemData.name);

        startPosition = transform.position;

        if (itemData.Shape == Shape.Big)
            textMeshWrapper.transform.localScale = Vector3.one / 2;

        if (textMeshsButtons.Count > 0)
        {
            for (int index = 0; index < itemData.Values.Length; ++index)
            {
                textMeshsButtons[index].text = itemData.Values[index];
            }
        }

        broken = false;
        dragging = false;

        Break();
    }

    protected virtual void Update()
    {

        if (SystemInfo.deviceType == DeviceType.Handheld)
            MobileDrag();
        else if (SystemInfo.deviceType == DeviceType.Desktop)
            DesktopDrag();

    }

    public void Break()
    {
        rgbd.isKinematic = false;
        targetJoint.enabled = false;
        broken = true;

    }

    protected void MobileDrag()
    {
        if(Input.touchCount > 0)
        {

            Touch touch = Input.GetTouch(0);
            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);

            if (Input.touchCount > 1)
            {
                dragging = false;
                targetJoint.enabled = false;
                return;
            }

            if (touch.phase == TouchPhase.Began)
            {

                if (Physics2D.Raycast(pos, Vector3.forward, Mathf.Infinity, maskClickCell).transform == transform)
                {
                    if (broken)
                    {
                        dragging = true;
                        targetJoint.enabled = true;
                    }
                }
                else
                {
                    dragging = false;
                    targetJoint.enabled = false;
                }
            }
            if (touch.phase == TouchPhase.Moved && dragging)
            {
                dragging = true;
                targetJoint.target = pos;
                
            }
            else if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                Release();
            }

        }
    }

    protected void DesktopDrag()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            
            if (Physics2D.Raycast(pos, Vector3.forward, Mathf.Infinity,maskClickCell).transform == transform)
            {
                if (broken)
                {
                    dragging = true;
                    targetJoint.enabled = true;
                }                
            }
            else
            {
                dragging = false;
                targetJoint.enabled = false;
            }
        }
        if (Input.GetMouseButton(0) && dragging)
        {
            targetJoint.target = pos;
        }
        else if (dragging && Input.GetMouseButtonUp(0))
        {
            Release();
        }
    }

    protected void Release()
    {

        dragging = false;
        targetJoint.enabled = false;
        if (Mathf.Abs(rgbd.rotation) <= 5)
        {
            broken = false;
            rgbd.isKinematic = true;
            rgbd.angularVelocity = 0f;
            rgbd.velocity = Vector2.zero;
            transform.rotation = Quaternion.identity;
            rgbd.position = startPosition;
        }
    }
    protected void SendState(int state)
    {
        GameManager.OnStateChange(itemData.ID, state);
    }

}
