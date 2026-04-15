using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class MousePoint : MonoBehaviour
{
    public static MousePoint instance;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private LayerMask targetLayer;

    [SerializeField]
    private DecalProjector projector;

    private bool isOverUI;

    public DecalProjector Projector { get { return projector; } }

    public bool IsOverUI => isOverUI;

    public Camera MyCamera => _camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {

        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current == null) return;

        isOverUI = EventSystem.current.IsPointerOverGameObject();

        if (_camera == null) return;

        Vector3 mouse = Input.mousePosition;

        Ray ray = _camera.ScreenPointToRay(mouse);

        if (Physics.Raycast(ray,out RaycastHit hit, 1000f, targetLayer))
        {
            transform.position = hit.point;
        }
    }

}
