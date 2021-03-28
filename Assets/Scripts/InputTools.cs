using UnityEngine;

public enum PlaneOrientation { XY, XZ, ZY }
public static class InputTools
{
    public static Vector3 RayToPlanePosition(Ray ray, float planeValue, PlaneOrientation orientation)
    {
        Vector3 position = new Vector3(planeValue, planeValue, planeValue);
        float planeCalc;
        switch (orientation)
        {
            case PlaneOrientation.XY:
                planeCalc = (planeValue - ray.origin.z) / ray.direction.z;
                position.x = ray.origin.x + ray.direction.x * planeCalc;
                position.y = ray.origin.y + ray.direction.y * planeCalc;
                break;
            case PlaneOrientation.XZ:
                planeCalc = (planeValue - ray.origin.y) / ray.direction.y;
                position.x = ray.origin.x + ray.direction.x * planeCalc;
                position.z = ray.origin.z + ray.direction.z * planeCalc;
                break;
            case PlaneOrientation.ZY:
                planeCalc = (planeValue - ray.origin.x) / ray.direction.x;
                position.z = ray.origin.z + ray.direction.z * planeCalc;
                position.y = ray.origin.y + ray.direction.y * planeCalc;
                break;
        }
        return position;

    }
}
