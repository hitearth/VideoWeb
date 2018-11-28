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
            result.ForEach(it => it.Path = it.Path.Substring(path.Length).Replace("\\", "/"));

            Console.WriteLine(FNode.getChildNodesJson(result));



        }

        static List<FNode> LoopThrough(DirectoryInfo dir, Func<FileInfo, bool> predicate)
        {
            var result = new List<FNode>();
            foreach (var file in dir.GetFiles())
            {
                if (predicate != null && !predicate(file)) continue;
                result.Add(new FNode { Name = file.Name, Path = dir.FullName, Extension = file.Extension.ToLower() });
            }
            foreach (var item in dir.GetDirectories())
            {
                result.AddRange(LoopThrough(item, predicate));
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

    public class FNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public override string ToString()
        {
            return string.Format("{{\"Name\":\"{0}\", \"Path\":\"{1}\", \"Extension\":\"{2}\"}}", Name, Path, Extension);
        }

        public static string getChildNodesJson(List<FNode> fNodes)
        {
            if (fNodes == null) return "null";
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (var item in fNodes)
            {
                sb.Append(item.ToString() + ",");
            }
            if (fNodes.Count > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }
    }

}
