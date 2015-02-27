using UnityEngine;

public class GameConfig
{
	public const int NUM_MOVES_IN_TURN = 11;

	public const float RAY_DISTANCE = 50.0f;

    private static int _boardLayerMask = 0;
	private static int _arrowLayerMask = 0;
    private static int _guiLayerMask = 0;

    public static int boardLayerMask
	{
		get
		{
			if (_boardLayerMask == 0) _boardLayerMask = 1 << LayerMask.NameToLayer(LayerNames.BoardLayer);
			return _boardLayerMask;
		}
	}

	public static int arrowLayerMask
	{
		get
		{
			if (_arrowLayerMask == 0) _arrowLayerMask = 1 << LayerMask.NameToLayer(LayerNames.ArrowLayer);
			return _arrowLayerMask;
		}
	}

    public static int uiLayerMask
    {
        get
        {
            if (_guiLayerMask == 0) _guiLayerMask = 1 << LayerMask.NameToLayer(LayerNames.UILayer);
            return _guiLayerMask;
        }
    }
}