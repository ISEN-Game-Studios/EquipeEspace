using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ViewManager : MonoBehaviour
{
    private new SpriteRenderer renderer;

    [SerializeField]
    private SpriteRenderer ship;

    [SerializeField]
    private ParticleSystem fireParticles;

    private Vector3 shipPosition;
    private float fireStartLifeTime;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void Completion(float completion, float fire)
    {
        shipPosition = renderer.bounds.min + new Vector3(renderer.bounds.size.x * completion, renderer.bounds.extents.y);

        fireStartLifeTime = fire * 10f;
    }

    private void Update()
    {
        ship.transform.position = Vector3.Lerp(ship.transform.position, shipPosition, 10f * Time.deltaTime);

        var main = fireParticles.main;
        main.startLifetime = Mathf.Lerp(main.startLifetime.constant, fireStartLifeTime, 10f * Time.deltaTime);
    }
}
