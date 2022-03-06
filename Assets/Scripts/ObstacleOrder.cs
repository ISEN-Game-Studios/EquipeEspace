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

    [SerializeField] private TextMesh text;
 
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
            text.text = instruction;
            animator.SetTrigger("NewOrder");            
        }
        else
        {
            text.text = "Geoffrey apprend à coder";
            animator.SetTrigger("NewOrder");
        }
    }

    public void Succes()
    {
        animator.SetTrigger("Succes");
    }

    
}
