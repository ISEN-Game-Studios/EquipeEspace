using UnityEngine;

[RequireComponent(typeof(TextMesh), typeof(MeshRenderer))]
public class InfiniteScrollTextMesh : MonoBehaviour
{
    [SerializeField] private SpriteRenderer container;
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float margin = 0.1f;
    [SerializeField] private float leftPadding = 0.25f;

    private TextMesh textMesh;
    private new MeshRenderer renderer;

    private GameObject[] fakes;

    private void Awake()
    {
        textMesh = GetComponent<TextMesh>();
        renderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        if (textMesh.text.Length > 0)
            SetText(textMesh.text);
    }

    public void SetText(string text)
    {
        GetComponent<TextMesh>().text = text;
        return;
        if (fakes != null)
        {
            foreach (GameObject fake in fakes)
                if (fake)
                    Destroy(fake);
        }

        textMesh.text = text;

        float width = renderer.bounds.size.x;

        if (width <= 0)
            return;

        float containerWidth = container.bounds.size.x;

        int count = (int)(containerWidth / width) + 1;

        fakes = new GameObject[count];

        for (int i = 0; i < count; ++i)
        {
            fakes[i] = Instantiate(gameObject, transform);
        }
        return;

        transform.localPosition = new Vector3(container.bounds.min.x + leftPadding, transform.localPosition.y);

        UpdateFakePositions();
    }

    private void Update()
    {
        return;
        if (fakes != null)
        {
            transform.position -= Vector3.right * scrollSpeed * Time.deltaTime;

            if (renderer.bounds.max.x < container.bounds.min.x)
                transform.localPosition = new Vector3(container.bounds.min.x + margin, transform.localPosition.y);

            UpdateFakePositions();
        }
    }

    private void UpdateFakePositions()
    {
        for (int i = 0; i < fakes.Length; ++i)
            if (fakes[i] != null)
                fakes[i].transform.position = new Vector3((renderer.bounds.max.x + margin) * (i + 1), transform.position.y);
    }
}
