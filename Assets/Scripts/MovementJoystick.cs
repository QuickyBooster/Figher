using UnityEngine.UIElements;
using UnityEngine;

class MovementJoystick : BaseJoystick
{
    protected override void OnDrag(Vector3 mousePos)
    {
        if (IsTouching)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
            pos.z = 0;
            Vector3 direction = pos - controllerPosition;
            float distance = Vector3.Distance(pos, controllerPosition);
            distance = Mathf.Clamp(distance, 0, 0.4f);
            joystick.transform.localPosition = direction.normalized * distance;

            if (player != null)
            {
                player.OnMoving(Mathf.Atan2(direction.y, direction.x), Time.deltaTime);
            }
        }
    }
}
