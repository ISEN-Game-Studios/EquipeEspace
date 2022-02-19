using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceTeam;
public class Cell : Interactable
{
    [SerializeField] private ItemData itemData;

    [Space(5)]

    [SerializeField] private TextMesh textMesh;

    [Space(5)]

    [SerializeField] private GameObject button;

    private TypeButton typeButton;

    private void Awake()
    {
        typeButton = button.GetComponent<TypeButton>();
    }
    public void Start()
    {
        textMesh.text = itemData.name;
    }
    public override void OnStateChange()
    {
        state = (state + 1) % itemData.Values.Length; 
    }
}
