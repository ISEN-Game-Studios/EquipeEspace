using UnityEngine;

public class VerticalViewport : MonoBehaviour
{
    [System.Serializable]
    public class Layout
    {
        public enum SizeType
        {
            Unit,
            Percentage
        }

        public float Height;
        public SizeType HeightSizeType;
        public SpriteRenderer Renderer;
    }

    private new Camera camera;

    [SerializeField] private float viewportWidth;

    [SerializeField] private Layout[] layouts;

    private void Awake()
    {
        camera = Camera.main;
    }

    void Start()
    {
        float ratio = Screen.height / (float)Screen.width;

        float height = viewportWidth * ratio;

        camera.orthographicSize = height / 2f;

        float remainingHeight = 0f;
        foreach (Layout layout in layouts)
            if (layout.HeightSizeType == Layout.SizeType.Unit)
                remainingHeight += layout.Height;

        remainingHeight = Mathf.Clamp(height - remainingHeight, 0, height);

        float nextHeight = -height / 2f;
        foreach (Layout layout in layouts)
        {
            float layoutHeight = layout.Height;

            if (layout.HeightSizeType == Layout.SizeType.Percentage)
                layoutHeight = remainingHeight * layoutHeight / 100f;

            if (layout.Renderer.drawMode == SpriteDrawMode.Simple)
                layout.Renderer.transform.localScale = new Vector3(viewportWidth, layoutHeight);
            else
                layout.Renderer.size = new Vector3(viewportWidth, layoutHeight);

            layout.Renderer.transform.position = new Vector3(0f, nextHeight + layoutHeight / 2f);

            nextHeight += layoutHeight;
        }
    }
}
