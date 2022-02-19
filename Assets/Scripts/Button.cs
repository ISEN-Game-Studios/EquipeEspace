using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceTeam;
using Array2DEditor;
public class Button : Interactable
{
    [SerializeField] private ItemData itemData;

    [Space(5)]

    [SerializeField] private TextMesh textMesh;

    [Space(5)]

    [SerializeField] private Array2DSprite buttonsSprites;

    [Space(5)]

    [SerializeField] private SpriteRenderer[] button;

    private TypeButton typeButton;

    public void Start()
    {
        textMesh.text = itemData.name;
    }
    public override void OnStateChange()
    {
        state = (state + 1) % itemData.Values.Length; 
    }
}
