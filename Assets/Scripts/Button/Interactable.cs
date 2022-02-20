using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceTeam;
using Array2DEditor;

public abstract class Interactable : MonoBehaviour
{
    public ItemData itemData;

    [Space(5)]

    [SerializeField] protected TextMesh textMesh;

    [Space(5)]

    [SerializeField] protected List<TextMesh> textMeshsButtons;

    protected virtual void Start()
    {
        textMesh.text = itemData.name;

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
        Debug.Log(state);
        GameManager.OnStateChange(itemData.ID, state);
    }
}
