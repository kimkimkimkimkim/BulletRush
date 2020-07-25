using UnityEngine;
using UnityEngine.EventSystems;

public class FixedFloatingJoystick : Joystick
{
    Vector2 InitPos = Vector2.zero;
    protected override void Start()
    {
        GetComponent<RectTransform>().SetAnchorWithKeepingPosition(0.5f, 0.5f);
        background.SetAnchorWithKeepingPosition(0, 0);
        InitPos = background.anchoredPosition;
        base.Start();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.anchoredPosition = InitPos;
        base.OnPointerUp(eventData);
    }
}