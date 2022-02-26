using UnityEngine;

[RequireComponent(typeof(TextMesh), typeof(MeshRenderer))]
public class TextMeshWrapper : MonoBehaviour
{
    [SerializeField] private SpriteRenderer container;
    [SerializeField] private float percentage;

    private TextMesh textMesh;
    private new MeshRenderer renderer;

    private void Awake()
    {
        textMesh = GetComponent<TextMesh>();
        renderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        SetText(textMesh.text);
    }

    public void SetText(string text)
    {
        if (text == null || text.Length < 1)
        {
            textMesh.text = "";
            return;
        }
        text = char.ToUpper(text[0]) + text.Substring(1);

        string[] words = text.Split(' ');
        textMesh.text = text;
        

        float width = renderer.bounds.size.x;

        int lineCount = Mathf.CeilToInt(width / (container.bounds.size.x * percentage));

        if (width > container.bounds.size.x * percentage && lineCount == 1)
            textMesh.anchor = TextAnchor.UpperCenter;
        if (words.Length < 2)
            return;

        int wordByLine = Mathf.CeilToInt(words.Length / (float)lineCount);
        textMesh.text = "";
        for (int i = 0; i < words.Length; ++i)
        {
            textMesh.text += words[i] + (i % wordByLine == wordByLine - 1 && i != words.Length - 1 ? "\n" : (i != words.Length - 1 ? " " : ""));
        }
            
        
    }
}
