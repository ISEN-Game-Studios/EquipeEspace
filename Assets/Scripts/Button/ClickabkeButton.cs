using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickabkeButton : TypeButton
{
    protected override void ChangeState(int index)
    {
        states[index] = Mathf.Abs(states[index] - 1);
        spriteRenderers[index].sprite = buttonsSprites.GetCell(index, states[index]);
        SendState(index);
        StartCoroutine(Reset(index));
    }

    private IEnumerator Reset(int index)
    {
        yield return new WaitForSeconds(1f);
        states[index] = Mathf.Abs(states[index] - 1);
        spriteRenderers[index].sprite = buttonsSprites.GetCell(index, states[index]);
    }
}
