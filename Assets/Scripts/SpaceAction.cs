using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "SpaceAction")]
[System.Serializable]
public class SpaceAction : ScriptableObject
{
    [Tooltip("button or horizontalButton or verticalButton or rotateButton or onOffbutton")]
    public TypeOfButton typeOfButton;
    [Tooltip("name of the action")]
    public string nameInstruction;
    [System.NonSerialized] 
    public int value = 0;
    [Tooltip("pour les bouttons en on/off ou juste à activer, 0 = désactivé et 1 = activé ")]
    public int maxValue;
    [System.NonSerialized] 
    public bool isUsed = false;
    [System.NonSerialized]
    public int valueIntended = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum TypeOfButton
{
    button = 0,
    horizontalButton = 1,
    verticalButton = 2,
    rotateButton = 3,
    onOffButton = 4
}
