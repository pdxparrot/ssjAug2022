using System;
using System.Collections.Generic;
using System.Linq;

namespace pdxpartyparrot.ssjAug2022.Collections
{
    // TODO: this can go away when System.Collections.Generic.PriorityQueue is available
    public class PriorityQueue<T>
    {
        private readonly SortedSet<T> _elements;

        public IComparer<T> Comparer => _elements.Comparer;

        public int Count => _elements.Count;

        public PriorityQueue()
        {
            _elements = new SortedSet<T>();
        }

        public PriorityQueue(IComparer<T> comparer)
        {
            _elements = new SortedSet<T>(comparer);
        }

        public void Enqueue(T element)
        {
            _elements.Add(element);
        }

        public T Peek()
        {
            if(Count < 1) {
                throw new InvalidOperationException("PriorityQueue is empty!");
            }

            return _elements.First();
        }

        public bool TryPeek(out T element)
        {
            element = default;

            if(Count < 1) {
                return false;
            }

            element = _elements.First();
            return true;
        }

        public T Dequeue()
        {
            if(Count < 1) {
                throw new InvalidOperationException("PriorityQueue is empty!");
            }

            var element = _elements.First();
            _elements.Remove(element);
            return element;
        }

        public bool TryDequeue(out T element)
        {
            element = default;

            if(Count < 1) {
                return false;
            }

            element = _elements.First();
            _elements.Remove(element);
            return true;
        }

        public void Clear()
        {
            _elements.Clear();
        }
    }
}
