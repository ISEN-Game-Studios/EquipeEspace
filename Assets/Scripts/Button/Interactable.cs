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

    protected bool _broken = true;
    protected bool _dragging = false;
    protected TargetJoint2D _targetJoint;
    protected Rigidbody2D _rigidbody;

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

        _targetJoint = GetComponent<TargetJoint2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _broken = false;
        _dragging = false;
    }

    private void Update()
    {
        //if(Input.touchCount != 1)
        //{
        //    dragging = false;
        //    GetComponentInChildren<TargetJoint2D>().enabled = false;
        //    return;
        //}

        if (Input.touchCount > 1)
        {
            _dragging = false;
            _targetJoint.enabled = false;
            return;
        }

        //Touch touch = Input.GetTouch(0);
        //Vector3 pos = touch.position;
        Vector2 posM = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (/*touch.phase == TouchPhase.Began ||*/ Input.GetMouseButtonDown(0))
        {
            if (Physics2D.Raycast(posM, Vector3.forward))
            {
                if (_broken)
                    _targetJoint.enabled = true;
            }
        }
        if ((/*touch.phase == TouchPhase.Moved ||*/ Input.GetMouseButton(0)) && _broken)
        {
            _dragging = true;
            _targetJoint.target = posM;
        }
        else if (_dragging && Input.GetMouseButtonUp(0)/*(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)*/)
        {
            _dragging = false;
            _targetJoint.enabled = false;
            if (Mathf.Abs(_rigidbody.rotation) <= 5)
            {
                _broken = false;
                _rigidbody.isKinematic = true;
                _rigidbody.angularVelocity = 0f;
                transform.rotation = Quaternion.identity;
            }
        }
    }

    public void Break()
    {
        _rigidbody.isKinematic = false;
    }

    protected void SendState(int state)
    {
        Debug.Log(itemData.Values[state]);
        GameManager.OnStateChange(itemData.ID, state);
    }
}
