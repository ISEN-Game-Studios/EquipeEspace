using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PhsycoRapideLight : MonoBehaviour
{
    [SerializeField]
    private float amplitude = 3f;

    [SerializeField]
    private float speed = 10f;

    private new SpriteRenderer renderer;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        renderer.size = new Vector2(amplitude * Mathf.Sin(speed * Time.deltaTime), renderer.size.y);
    }
}
