using System.Collections.Generic;

namespace WFC
{
    internal class UniqueStack<T>
    {
        private Stack<T> Stack { get; } = new Stack<T>();
        private HashSet<T> ContainedElements  { get; } = new HashSet<T>();

        internal int Count => Stack.Count;
        
        internal void Push(T element)
        {
            if (ContainedElements.Contains(element))
                return;
            
            ContainedElements.Add(element);
            Stack.Push(element);
        }

        internal T Pop()
        {
            var element = Stack.Pop();
            ContainedElements.Remove(element);
            return element;
        }
        
    }
}