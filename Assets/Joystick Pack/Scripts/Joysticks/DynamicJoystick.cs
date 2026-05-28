using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicJoystick : Joystick
{
    public float MoveThreshold
    {
        get { return moveThreshold; }
        set { moveThreshold = Mathf.Abs(value); }
    }

    [SerializeField] private float moveThreshold = 1;

    private Vector2 originalPosition;

    protected override void Start()
    {
        MoveThreshold = moveThreshold;
        base.Start();

        originalPosition = background.anchoredPosition;
        background.gameObject.SetActive(true);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = originalPosition;
        background.gameObject.SetActive(true);

        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.anchoredPosition = originalPosition;
        background.gameObject.SetActive(true);

        base.OnPointerUp(eventData);
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        
        base.HandleInput(magnitude, normalised, radius, cam);

        background.anchoredPosition = originalPosition;
    }
}