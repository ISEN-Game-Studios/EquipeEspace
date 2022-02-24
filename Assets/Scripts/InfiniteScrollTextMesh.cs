using UnityEngine;

[RequireComponent(typeof(TextMesh), typeof(MeshRenderer))]
public class InfiniteScrollTextMesh : MonoBehaviour
{
    [SerializeField] private SpriteRenderer container;

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

        string[] words = text.Split(' ');

        textMesh.text = text;

        float width = renderer.bounds.size.x;

        int lineCount = Mathf.CeilToInt(width / container.bounds.size.x);

        int wordByLine = words.Length / lineCount;

        textMesh.text = "";
        for (int i = 0; i < words.Length; ++i)
            textMesh.text += words[i] + (i % wordByLine == wordByLine - 1 && i != words.Length - 1 ? "\n" : " ");
    }
}
