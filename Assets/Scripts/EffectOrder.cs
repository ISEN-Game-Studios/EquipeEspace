using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOrder : MonoBehaviour
{
    private Animator animator;

    private string order;
    private float timer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void GetOrderState(bool state, float timer, string order)
    {
        this.order = order;
        this.timer = timer;

        if (state)
            animator.SetTrigger("Succes");
        else
            animator.SetTrigger("Failure");
    }

    private void ShowOrder()
    {
        GameManager.ShowOrder(order, timer);
        order = "";
    }
}
