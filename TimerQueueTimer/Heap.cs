using System;
using System.Collections.Generic;

namespace TimerQueueTimer
{
    // An implementation of generic heap data structure. T1 is the type of the
    // priority and T2 is the type of items needs to be stored in the heap.
    // The behavior of the heap i.e. min heap or max heap can be defined using
    // IComparer argument in the constructor.
    public class Heap<T1, T2>
    {
        // Represents an entry in the heap.
        class HeapItem
        {
            public T1 Priority { get; set; }
            public T2 Item { get; set; }
        }

        HeapItem[] Collection;
        IComparer<T1> Comparer;
        readonly T2 DefaultValue;
        int LastIndex;

        // Gets the maximum capacity of the heap.
        public int MaxSize
        {
            get
            {
                return Collection.Length;
            }
        }

        // Creates a priority queue of capacity 'maxSize'. The comparer is used
        // by the heap to compare the priorities, the behavior of the heap i.e.
        // work as min heap or max heap is decided by the comparer.
        //
        // MinHeap : int r = comparer.Compare(a, b), r < 0 if a < b, r > 0 if a > b
        // MaxHeap : int r = comparer.Compare(a, b), r < 0 if a > b, r > 0 if a < b
        public Heap(int maxSize, IComparer<T1> comparer, T2 defaultValue)
        {
            Comparer = comparer;
            Collection = new HeapItem[maxSize];
            LastIndex = -1;
            DefaultValue = defaultValue;
        }

        // Given index of a (child) node, gets index of the parent node
        // if the index is of root node then return -1.
        // O(1)
        int GetParentIndex(int index)
        {
            return index <= 0 ? -1 : (int)Math.Floor(((double)index - 1) / 2);
        }

        // Given index of parent node, gets the index of its left child
        // node. If parent has no left child return -1.
        // O(1)
        int GetLeftChildIndex(int index)
        {
            int leftIndex = 2 * index + 1;
            return leftIndex > LastIndex ? -1 : leftIndex;
        }

        // Given index of parent node, gets the index of its right child
        // node. If parent has no right child return -1.
        // O(1)
        int GetRigtChildIndex(int index)
        {
            int rightIndex = 2 * index + 2;
            return rightIndex > LastIndex ? -1 : rightIndex;
        }

        // Exchange the values at the given indices.
        // O(1)
        void Exchange(int index1, int index2)
        {
            HeapItem temp = Collection[index1];
            Collection[index1] = Collection[index2];
            Collection[index2] = temp;
        }

        // Restore the heap property of the balanced tree if it is
        // violated at 'index'.
        // This function requires the subtrees rooted at left
        // and right child node of the node at 'index' to be heap.
        void Heapify(int index)
        {
            int leftIndex = GetLeftChildIndex(index);
            // The structural property of the heap ensure that the nodes
            // at the last level are as left as possible, this means if
            // leftIndex == -1 then 'index' is a leaf node.
            if (leftIndex == -1)
            {
                return;
            }

            int rightIndex = GetRigtChildIndex(index);
            // No right child means there is only left child for node at 'index'
            // and 'index' is at 'lastlevel -1' level
            if (rightIndex == -1)
            {
                if (Comparer.Compare(Collection[leftIndex].Priority,
                    Collection[index].Priority) < 0)
                {
                    Exchange(leftIndex, index);
                }

                return;
            }

            HeapItem minOrMaxItem;
            // Gets child node (left or right) with maximum or minimum priority
            if (Comparer.Compare(Collection[rightIndex].Priority,
                Collection[leftIndex].Priority) < 0)
            {
                minOrMaxItem = Collection[rightIndex];
            }
            else
            {
                minOrMaxItem = Collection[leftIndex];
            }

            // restore the heap property at 'index' if required and apply heapify
            // recursively for the sub tree rooted at modified child node.
            if (Comparer.Compare(minOrMaxItem.Priority,
                Collection[index].Priority) < 0)
            {
                if (minOrMaxItem == Collection[rightIndex])
                {
                    Exchange(rightIndex, index);
                    Heapify(rightIndex);
                    return;
                }
                else
                {
                    Exchange(leftIndex, index);
                    Heapify(leftIndex);
                    return;
                }
            }
        }

        // Returns the item in the top of the heap without removing it.
        // If heap is empty then return the value set as default by user.
        //
        // If this is a min heap then top item will be the item with
        // minimum priority, if its a max heap then top item will be
        // the item with maximum priority.
        // O(1)
        public T2 Peek()
        {
            return LastIndex == -1 ? DefaultValue : Collection[0].Item;
        }

        // Insert an item to heap with a given priority. Throws IndexOutOfRangeException
        // if the heap is full.
        // O(log n)
        public void Push(T1 priority, T2 item)
        {
            if (LastIndex >= MaxSize - 1)
            {
                throw new
                    IndexOutOfRangeException(String.Format("Heap reached its maximum capacity {0}", MaxSize));
            }

            LastIndex++;
            Collection[LastIndex] = new HeapItem
            {
                Priority = priority,
                Item = item
            };

            int index = LastIndex;
            int parentIndex = GetParentIndex(index);
            while (parentIndex != -1 &&
                Comparer.Compare(Collection[index].Priority, Collection[parentIndex].Priority) < 0)
            {
                Exchange(index, parentIndex);
                index = parentIndex;
                parentIndex = GetParentIndex(index);
            }
        }

        // Remove the top element from the heap and return it. If the heap is
        // empty then return the value set as default by user.
        // O(log n)
        public T2 Pop()
        {
            if (LastIndex == -1)
            {
                return DefaultValue;
            }

            HeapItem minOrMaxItem = Collection[0];
            // Index needs to be decremented by 1 before calling Heapify.
            LastIndex--;
            if (LastIndex != -1)
            {
                Collection[0] = Collection[LastIndex + 1];
                Heapify(0);
            }

            return minOrMaxItem.Item;
        }

    }
}
