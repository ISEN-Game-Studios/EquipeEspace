using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Order
{
    public string key;
    public string value;
}

public class ObstacleOrder : MonoBehaviour
{
    private Dictionary<string, string> instructions = new Dictionary<string, string>();

    [SerializeField] private SpriteRenderer parentRenderer;

    [Space(5)]

    [SerializeField] private List<Order> orders;

    [Space(5)]

    [SerializeField] private TextMeshWrapper textWrapper;
    
 
    private Animator animator;
    private new SpriteRenderer renderer;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        foreach(Order instruction in orders)
        {
            instructions.Add(instruction.key, instruction.value);
        }
        renderer.size = parentRenderer.size;
    }

    public void GetOrder(string action)
    {
        string instruction = "";
        if(instructions.ContainsKey(action))
            instruction = instructions[action];

        if (instruction != "")
        {
            textWrapper.SetText(instruction);
            animator.SetTrigger("NewOrder");
        }
        else
        {
            textWrapper.SetText("Geoffrey apprend à coder");
            animator.SetTrigger("NewOrder");
        }
    }
    public void DeleteText()
    {
        textWrapper.SetText("");
    }

    public void Succes()
    {
        animator.SetTrigger("Succes");
    }

    public void Cancel()
    {
        DeleteText();
        animator.SetTrigger("Idle");
    }
}
