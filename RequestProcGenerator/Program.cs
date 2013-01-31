using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RequestProcGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.Error.WriteLine("Needs a file as an argument");
                return;
            }

            var file = new StreamReader(args[0]);
            var levelstack = new Stack<int>();
            
            // Parse
            List<Function> functions = new List<Function>();
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();

                if (string.IsNullOrWhiteSpace(line) || string.IsNullOrEmpty(line))
                    continue;


                var cFunction = new Function();
                cFunction.Name = line.Trim();

                levelstack.Push(GetLevel(line));
                while (GetLevel(line = file.ReadLine()) > levelstack.Peek() || string.IsNullOrWhiteSpace(line) || string.IsNullOrEmpty(line))
                {
                    switch (line.Trim().ToLower())
                    {
                        case "":
                            break;
                        case "description":
                            StringBuilder descriptionBuilder = new StringBuilder();
                            levelstack.Push(GetLevel(line));
                            while (GetLevel(line = file.ReadLine()) > levelstack.Peek() || string.IsNullOrWhiteSpace(line) || string.IsNullOrEmpty(line))
                            {
                                descriptionBuilder.AppendLine(line);
                            }
                            cFunction.Description = descriptionBuilder.ToString();
                            break;
                        case "arguments":
                            while (GetLevel(line = file.ReadLine()) > levelstack.Peek() || string.IsNullOrWhiteSpace(line) || string.IsNullOrEmpty(line))
                            {
                                var spl = line.Split(' ');
                                var type = new DataType();
                                var name = string.Empty;
                                if (spl.Length > 1)
                                {
                                    name = spl.Last();
                                }
                                levelstack.Push(GetLevel(line));
                            }
                            break;
                        case "return":
                        case "returns":
                            break;
                    }
                }
            }
        }

        static int GetLevel(string line)
        {
            return line.Length - line.TrimStart('\t').Length;
        }
    }

    class Function
    {
        public string Name = string.Empty;
        public DataType ReturnType = new DataType();
        public Dictionary<string, DataType> Arguments = new Dictionary<string, DataType>();
        public string Description = string.Empty;

        public string GetParameterSignature()
        {
            return string.Join(", ", from a in Arguments.Keys select new string[] { Arguments[a].TypeName, a });
        }
        public string GetParameterPass()
        {
            return string.Join(", ", Arguments.Keys);
        }
        public string GetMetadataXml()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Description))
            {
                sb.AppendLine("<summary>");
                sb.AppendLine(Description);
                sb.AppendLine("</summary>");
            }
            foreach (var a in from a in Arguments.Keys where !string.IsNullOrEmpty(Arguments[a].Description) select new { Name = a, Argument = Arguments[a] })
            {
                sb.Append("<param name=\"");
                sb.Append(a.Name);
                sb.AppendLine("\">");
                sb.AppendLine(a.Argument.Description);
                sb.AppendLine("</param>");
            }
            if (!string.IsNullOrEmpty(ReturnType.Description))
            {
                sb.AppendLine("<returns>");
                sb.AppendLine(ReturnType.Description);
                sb.AppendLine("</returns>");
            }
            return sb.ToString();
        }
    }

    class DataType
    {
        public string TypeName = "void";
        public string Description = string.Empty;
    }
}
