using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct BinaryHeap<T> where T : struct, IComparable<T>
{
    public NativeArray<T> heapElements;
    public int currentHeapCount;   //The current amount of elements in the heap


    public BinaryHeap(int _heapSize, Allocator _allocatorType)
    {
        this.heapElements = new NativeArray<T>(_heapSize, _allocatorType);
        this.currentHeapCount = 0;
    }

    /// <summary>
    /// Removes the top element from the heap, and sorts remaining elements.
    /// </summary>
    public void PopElement()
    {
        heapElements[0] = heapElements[currentHeapCount - 1];
        currentHeapCount -= 1;
        SortElementsDown();
    }
    /// <summary>
    /// Adds new element to the heap, and sorts remaining elements.
    /// </summary>
    public void PushElement(T _element)
    {
        currentHeapCount += 1;
        heapElements[currentHeapCount - 1] = _element;
        SortElementsUp();
    }

    public T PeekElement()
    {
        return heapElements[0];
    }

    public void Dispose()
    {
        heapElements.Dispose();
    }

    






    //Private Methods----------------------------------------------------

    private int GetLeftChildIndex(int _index) => (_index * 2) + 1;
    private int GetRightChildIndex(int _index) => (_index * 2) + 2;
    private int GetParentIndex(int _index) => (_index - 1) / 2;
    private bool ElementHasChildren(int _index)
    {
        if (HasLeftChild(_index) == false) { return false; }
        if (HasRightChild(_index) == false) { return false; }
        return true;
    }
    private bool HasLeftChild(int _index)
    {
        if (GetLeftChildIndex(_index) < currentHeapCount) { return true; }
        return false;
    }
    private bool HasRightChild(int _index)
    {
        if (GetRightChildIndex(_index) < currentHeapCount) { return true; }
        return false;
    }
    private void SwapElements(int _indexOne, int _indexTwo)
    {
        T indexOneElement = heapElements[_indexOne];
        heapElements[_indexOne] = heapElements[_indexTwo];
        heapElements[_indexTwo] = indexOneElement;
    }
    private void SortElementsUp()
    {
        int index = currentHeapCount - 1;   //Selects the very last index

        while (index > 0 && heapElements[index].CompareTo(heapElements[GetParentIndex(index)]) < 0)
        {
            SwapElements(index, GetParentIndex(index));
            index = GetParentIndex(index);
        }
    }

    private void SortElementsDown()
    {
        int index = 0;
        while (ElementHasChildren(index))
        {
            int smallestChildIndex = GetLeftChildIndex(index);
            if (HasRightChild(index))
            {
                if (heapElements[smallestChildIndex].CompareTo(heapElements[GetRightChildIndex(index)]) > 0)
                {
                    smallestChildIndex = GetRightChildIndex(index);
                }
            }
            if (heapElements[index].CompareTo(heapElements[smallestChildIndex]) > 0)
            {
                SwapElements(index, smallestChildIndex);
                index = smallestChildIndex;
            }
            else
            {
                break;
            }    
        }
    }
}


