using UnityEngine;
using System;
using System.Collections;

public class BinaryHeap<T> where T : IComparable<T>
{
    private T[] binaryHeap;
    private int numberOfItems;

    public BinaryHeap(int numberOfElements)
    {
        binaryHeap = new T[numberOfElements];
        numberOfItems = 1;
    }

    public void add(T item)
    {
        binaryHeap[numberOfItems] = item;
        int bubbleIndex = numberOfItems;

        while (bubbleIndex != 1)
        {
            int parentIndex = bubbleIndex / 2;
            
            if (binaryHeap[bubbleIndex].CompareTo(binaryHeap[parentIndex]) <= 0)
            {
                T tmpValue = binaryHeap[parentIndex];
                binaryHeap[parentIndex] = binaryHeap[bubbleIndex];
                binaryHeap[bubbleIndex] = tmpValue;
                bubbleIndex = parentIndex;
            }
            else
            {
                break;
            }
        }

        numberOfItems++;
    }

    public T remove()
    {
        numberOfItems--;
        T returnItem = binaryHeap[1];
        binaryHeap[1] = binaryHeap[numberOfItems];
        int swapItem = 1, parent = 1;

        do
        {
            parent = swapItem;
            if ((2 * parent + 1) <= numberOfItems)
            {
                // Both children exist
                if (binaryHeap[parent].CompareTo(binaryHeap[2 * parent]) >= 0)
                {
                    swapItem = 2 * parent;
                }

                if (binaryHeap[swapItem].CompareTo(binaryHeap[2 * parent + 1]) >= 0)
                {
                    swapItem = 2 * parent + 1;
                }
            }
            else if ((2 * parent) <= numberOfItems)
            {
                // Only one child exists
                if (binaryHeap[parent].CompareTo(binaryHeap[2 * parent]) >= 0)
                {
                    swapItem = 2 * parent;
                }
            }

            // One if the parent's children are smaller or equal, swap them
            if (parent != swapItem)
            {
                T tmpIndex = binaryHeap[parent];
                binaryHeap[parent] = binaryHeap[swapItem];
                binaryHeap[swapItem] = tmpIndex;
            }
        }
        while (parent != swapItem);

        return returnItem;
    }

    public void updateItemAtIndex(int index)
    {
        int bubbleIndex = index;

        while (bubbleIndex != 1)
        {
            int parentIndex = bubbleIndex / 2;

            if (binaryHeap[bubbleIndex].CompareTo(binaryHeap[parentIndex]) <= 0)
            {
                T tmpValue = binaryHeap[parentIndex];
                binaryHeap[parentIndex] = binaryHeap[bubbleIndex];
                binaryHeap[bubbleIndex] = tmpValue;
                bubbleIndex = parentIndex;
            }
            else
            {
                break;
            }
        }
    }

    public int count
    {
		//HACK: Don't know if this is correct, but it seems to work with the null problem
        get { return numberOfItems - 1; }
    }

    public T[] items
    {
        get { return binaryHeap; }
    }
}
