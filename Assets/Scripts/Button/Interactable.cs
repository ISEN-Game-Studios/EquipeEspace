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
    }

    protected void SendState(int state)
    {
        Debug.Log(itemData.Values[state]);
        GameManager.OnStateChange(itemData.ID, state);
    }
}
