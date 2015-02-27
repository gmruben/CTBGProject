using UnityEngine;
using System;
using System.Collections;

public class Node : IComparable<Node>
{
	private SquareIndex m_index;
    private int m_fCost;
    private int m_hCost;

    public Node(SquareIndex pIndex, int pFCost, int pHCost)
    {
        m_index = pIndex;
        m_fCost = pFCost;
        m_hCost = pHCost;
    }

    public int CompareTo(Node other)
    {
        if (other == null)
        {
            return 1;
        }

        return FCost.CompareTo(other.FCost);
    }

    #region PROPERTIES

	public SquareIndex index
    {
        get { return m_index; }
        set { m_index = value; }
    }

    public int FCost
    {
        get { return m_fCost; }
        set { m_fCost = value; }
    }

    public int HCost
    {
        get { return m_hCost; }
        set { m_hCost = value; }
    }

    #endregion
}