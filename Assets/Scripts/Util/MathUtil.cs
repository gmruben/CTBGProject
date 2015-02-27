using UnityEngine;
using System.Collections;

public class MathUtil
{
	public static float signedAngle(Vector2 v1, Vector2 v2)
	{
		float angle = Vector2.Angle(v1, v2);
		Vector3 cross = Vector3.Cross(v1, v2);
		
		if (cross.z > 0) angle = 360 - angle;
		return angle;
	}

    public static Vector2 lineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
    {
        //Line: Ax + By = C

        //Get A, B, C of first line - points : ps1 to pe1
        float A1 = pe1.y - ps1.y;
        float B1 = ps1.x - pe1.x;
        float C1 = A1 * ps1.x + B1 * ps1.y;

        //Get A, B, C of second line - points : ps2 to pe2
        float A2 = pe2.y - ps2.y;
        float B2 = ps2.x - pe2.x;
        float C2 = A2 * ps2.x + B2 * ps2.y;

        //Get delta and check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
        {
            throw new System.Exception("Lines are parallel");
        }

        //Return the Vector2 intersection point
        float x = (B2 * C1 - B1 * C2) / delta;
        float y = (A1 * C2 - A2 * C1) / delta;

        return new Vector2(x, y);
    }
}