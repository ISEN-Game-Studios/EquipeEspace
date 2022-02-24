using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettableButton : TypeButton
{
    protected override void ChangeState(int index)
    {
        
        states[index] = Mathf.Abs(states[index] - 1);
        spriteRenderers[index].sprite = buttonsSprites.GetCell(index, states[index]);
        SendState(states[index]);
    }

}
