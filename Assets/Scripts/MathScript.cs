using UnityEngine;
using System.Collections;

public class MathScript : MonoBehaviour
{
    public static float getDistance(Vector2 init, Vector2 current)
    {
        Vector2 difference;
        difference.x = current.x - init.x;
        difference.y = current.y - init.y;
        return Mathf.Sqrt((difference.x * difference.x) + (difference.y * difference.y));
    }

    public static float getAIAngle(Vector2 init, Vector2 current)
    {
        Vector2 difference = current - init;
        return difference.x == 0 ? difference.y * 90 : difference.x < 0 ? 180 : 0;
    }

    public static float getAngle(Vector2 init, Vector2 current)
    {
        Vector2 difference = current - init;
        float angle = Mathf.Atan2(difference.y, difference.x) * 180 / Mathf.PI;
        angle = angle < 0 ? 360 + angle : angle;
        angle += 45;
        angle = (int)angle % 360;
        angle = (int)angle / 90;
        angle *= 90;
        return angle;
    }
}
