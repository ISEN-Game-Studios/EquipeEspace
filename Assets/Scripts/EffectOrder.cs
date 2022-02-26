using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOrder : MonoBehaviour
{
    private Animator animator;
    private new SpriteRenderer renderer;

    private string order;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SpriteRenderer rendererParent = transform.parent.GetComponent<SpriteRenderer>();
        renderer.size = rendererParent.size;
    }

    private void Update()
    {
        
    }
    public void GetOrderState(bool state, string order)
    {
        this.order = order;

        if (state)
            animator.SetTrigger("Succes");
        else
            animator.SetTrigger("Failure");
    }

    private void ShowOrder()
    {
        GameManager.ShowOrder(order);
        order = "";
    }

}
