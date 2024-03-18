using System;
using UnityEngine;

class RotationJoystick : BaseJoystick
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

            // Calculate the angle of rotation
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            if (player != null)
            {
                // Pass the angle to the OnRotate method
                player.OnRotate(angle);
            }
        }
    }
}
