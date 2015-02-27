using UnityEngine;
using System.Collections;

public class ColorPalette
{
	public static Color red = new Color(1.0f, 0.1f, 0.1f);
	public static Color blue = new Color(0.1f, 0.1f, 1.0f);

	private static Color[] _frozenColorList;
	private static Color[] _poisonColorList;

	public static void setMeshToColor(GameObject obj, Color color)
	{
		Mesh mesh = obj.GetComponent<MeshFilter>().mesh;

		int length = mesh.vertices.Length;
		Color[] vcolor = new Color[length];

		for (int i = 0; i < length; i++)
		{
			vcolor[i] = color;
		}
		
		mesh.colors = vcolor;
	}

    public static void setHorizontalGradient(GameObject quad, Color color1, Color color2, float normalizedValue)
    {
        Color interpolateColor = (color1 * (1.0f - normalizedValue)) + (color2 * normalizedValue);

        Mesh mesh = quad.GetComponent<MeshFilter>().mesh;
        Color[] vcolor = new Color[4];

        vcolor[0] = color1;
        vcolor[1] = interpolateColor;
        vcolor[2] = interpolateColor;
        vcolor[3] = color1;

        mesh.colors = vcolor;
    }

    public static string ColorToHex(Color color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static Color HexToColor(string hex)
    {
        Color32 color;

        if (hex == "none")
        {
            color = new Color(0, 0, 0, 255);
        }
        else
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            color = new Color32(r, g, b, 255);
        }

        return color;
    }

	public static Color[] frozenColorList
	{
		get
		{
			if (_frozenColorList == null)
			{
				_frozenColorList = new Color[4];

				_frozenColorList[0] = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
				_frozenColorList[1] = new Color(149.0f / 255.0f, 216.0f / 255.0f, 240.0f / 255.0f);
				_frozenColorList[2] = new Color(123.0f / 255.0f, 193.0f / 255.0f, 237.0f / 255.0f);
				_frozenColorList[3] = new Color(081.0f / 255.0f, 155.0f / 255.0f, 228.0f / 255.0f);
			}

			return _frozenColorList;
		}
	}

	public static Color[] poisonColorList
	{
		get
		{
			if (_poisonColorList == null)
			{
				_poisonColorList = new Color[4];
				
				_poisonColorList[0] = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
				_poisonColorList[1] = new Color(189.0f / 255.0f, 174.0f / 255.0f, 198.0f / 255.0f);
				_poisonColorList[2] = new Color(156.0f / 255.0f, 138.0f / 255.0f, 165.0f / 255.0f);
				_poisonColorList[3] = new Color(066.0f / 255.0f, 028.0f / 255.0f, 082.0f / 255.0f);
			}
			
			return _poisonColorList;
		}
	}
}