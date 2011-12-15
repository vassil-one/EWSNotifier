using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EWSNotifier.Utility
{
    public class NTree<T>
    {
        private List<NTree<T>> children;
        public T Data { get; set; }

        public NTree(T data)
        {
            this.Data = data;
            children = new List<NTree<T>>();
        }

        public void addChild(T data)
        {
            children.Add(new NTree<T>(data));
        }

        public NTree<T> getChild(int i)
        {
            if (children.Count <= i)
                return null;
            return children.ElementAt<NTree<T>>(i);
        }

    }
}
