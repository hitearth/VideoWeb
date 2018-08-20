using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scanfile
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = GetArg(args, "-p");
            if (string.IsNullOrEmpty(path)) path = Environment.CurrentDirectory;
            path = Path.Combine(Environment.CurrentDirectory, path);

            Func<FileInfo, bool> predicate = null;

            var extlimit = GetArg(args, "-e");
            if (!string.IsNullOrEmpty(extlimit))
            {
                var extlimits = extlimit.Split(',');
                predicate = f => extlimits.Contains(f.Extension);
            }            

            StringBuilder sb = new StringBuilder();

            var dir = new DirectoryInfo(path);
            var result = LoopThrough(dir, predicate);

            
            Console.WriteLine(DNode.getChildNodesJson(result));
            


        }

        static List<Node> LoopThrough(DirectoryInfo dir, Func<FileInfo, bool> predicate)
        {
            var result = new List<Node>();
            foreach (var file in dir.GetFiles())
            {
                if (predicate != null && !predicate(file)) continue;
                result.Add(new FNode { Value = file.Name });
            }
            foreach (var item in dir.GetDirectories())
            {
                var childNodes = LoopThrough(item, predicate);
                if (childNodes != null && childNodes.Count > 0)
                {
                    result.Add(new DNode
                    {
                        PathValue = item.Name,
                        childNodes = childNodes
                    });
                }
            }
            return result;
        }

        static string GetArg(string[] args, string name)
        {
            var index = args.ToList().IndexOf(name);
            if (index > -1 && index < args.Length - 1)
            {
                return args[index + 1];
            }
            return null;
        }
    }

    public abstract class Node
    {
    }
    public class FNode : Node
    {
        public string Value { get; set; }
        public override string ToString()
        {
            return string.Format("{{\"value\":\"{0}\"}}", Value);
        }
    }
    public class DNode : Node
    {
        public string PathValue { get; set; }
        public List<Node> childNodes { get; set; }
        public override string ToString()
        {
            return string.Format("{{\"path\":\"{1}\",\"childNodes\":{0}}}", getChildNodesJson(childNodes), PathValue);
        }
        public static string getChildNodesJson(List<Node> childNodes)
        {
            if (childNodes == null) return "null";
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (var item in childNodes)
            {
                sb.Append(item.ToString() + ",");
            }
            if (childNodes.Count > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
