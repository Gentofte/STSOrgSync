using Organisation.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Organisation.ReportTool
{
    public class TreeNode<T>
    {
        private readonly T _value;
        private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();

        public TreeNode(T value)
        {
            _value = value;
        }

        public TreeNode<T> this[int i]
        {
            get { return _children[i]; }
        }

        public TreeNode<T> Parent { get; private set; }

        public T Value { get { return _value; } }

        public ReadOnlyCollection<TreeNode<T>> Children
        {
            get { return _children.AsReadOnly(); }
        }

        public TreeNode<T> AddChild(T value)
        {
            var node = new TreeNode<T>(value) { Parent = this };
            _children.Add(node);
            return node;
        }

        public TreeNode<T> AddTreeNodeChild(TreeNode<T> value)
        {
            _children.Add(value);
            return value;
        }

        public TreeNode<T>[] AddChildren(params T[] values)
        {
            return values.Select(AddChild).ToArray();
        }

        public bool RemoveChild(TreeNode<T> node)
        {
            return _children.Remove(node);
        }

        public void Traverse(Action<T> action)
        {
            action(Value);
            foreach (var child in _children)
                child.Traverse(action);
        }

        public IEnumerable<T> Flatten()
        {
            return new[] { Value }.Union(_children.SelectMany(x => x.Flatten()));
        }

        public string PrintPretty(string indent, bool last)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(indent);
            if (last)
            {
                builder.Append("\\-");
                indent += "  ";
            }
            else
            {
                builder.Append("|-");
                indent += "| ";
            }
            builder.AppendLine(this.Value.ToString());

            for (int i = 0; i < Children.Count; i++)
            {
                builder.Append(Children[i].PrintPretty(indent, i == Children.Count - 1));
            }
            return builder.ToString();
        }


        public XElement GetXml(TreeNode<OU> rootNode)
        {
            var xml = new XElement("OU");
            xml.Add(new XElement("Uuid", rootNode.Value.Uuid));
            xml.Add(new XElement("Name", rootNode.Value.Name));

            if (rootNode.Value.Addresses != null)
            {
                foreach (AddressHolder address in rootNode.Value.Addresses)
                {
                    xml.Add(new XElement("Address", address.Value));
                }
            }

            for (int i = 0; i < rootNode.Children.Count; i++)
            {
                xml.Add(GetXml(rootNode.Children[i]));
            }

            return xml;
        }
    }
}
