using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIG.Infrastructure.Extensions
{
    public static class ExNode
    {
        public static IEnumerable<Node> Descendents(this Node node)
        {
            if (node == null) throw new ArgumentNullException("node");
            return node.Children.SelectDeep(n => n.Children);
        }
    }
}
