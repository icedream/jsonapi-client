using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

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

            Debug.WriteLine("Input file is \"{0}\"", args.First());

            var file = new StreamReader(args.First());
            var levelstack = new Stack<int>();
            
            // Parse mlst file
            List<Function> functions = new List<Function>();
            List<string> @using = new List<string>();
            @using.Add("JsonApi.Client");
            @using.Add("JsonApi.Client.Classes");
            @using.Add("JsonApi.Client.DataTypes");
            @using.Add("System");
            @using.Add("System.Collections.Generic");
            @using.Add("System.Linq");
            @using.Add("System.Text");
            string @namespace = "JsonApi.Client.Extensions";
            string @class = "CustomRequests";

            Debug.WriteLine("Default imported namespaces: {0}", @using.Count);
            Debug.WriteLine("Default namespace: {0}", @namespace, null); // Yep, laziness found here.
            Debug.WriteLine("Default class: {0}", @class, null);

            string line = file.ReadLine().TrimEnd();
            while (!file.EndOfStream)
            {
                Debug.WriteLine("Current line, expecting new function or compiler directive: Level {1} => {0}", line.Trim(), GetLevel(line));

                if (string.IsNullOrWhiteSpace(line) || string.IsNullOrEmpty(line))
                {
                    Debug.WriteLine("Ignoring because empty.");
                    line = file.ReadLine().TrimEnd();
                    continue;
                }

                if (line.StartsWith("."))
                {
                    // compiler command
                    var lspl = line.Split(' ');
                    var argline = string.Join(" ", lspl.Skip(1));
                    var argarray = lspl.Skip(1);
                    Debug.WriteLine("Compiler command being processed: {0} (with {1} arguments)", lspl[0].ToLower().Substring(1), argarray.Count());
                    line = file.ReadLine().TrimEnd(); // for next processing, we should ignore that from now on.
                    switch (lspl[0].ToLower().Substring(1))
                    {
                        case "using":
                            if (!@using.Contains(argline, StringComparer.OrdinalIgnoreCase))
                            {
                                Debug.WriteLine("Importing {0}", argline);
                                @using.Add(argline);
                            }
                            else
                                Console.Error.WriteLine("Warning: Tried adding \"using {0}\", but namespace has already been imported.", argline, null);
                            break;
                        case "namespace":
                            Debug.WriteLine("Changing to namespace {0}", argline, null);
                            @namespace = argline;
                            break;
                        case "class":
                            Debug.WriteLine("Changing to class {0}", argline, null);
                            @class = argline;
                            break;
                    }
                    continue;
                }

                var cFunction = new Function();
                cFunction.Name = line.Trim();
                Debug.WriteLine("Generating new function: {0}", cFunction.Name, null);

                levelstack.Push(GetLevel(line)); // root level push
                line = file.ReadLine().TrimEnd();
                while (line != null && !(line.TrimEnd() == "") && GetLevel(line) > levelstack.Peek())
                {
                    Debug.WriteLine("Current line, expecting new descriptive keyword: Level {1} => {0}", line.Trim(), GetLevel(line));
                    switch (line.Trim().ToLower())
                    {
                        default:
                            {
                                Console.Error.WriteLine("Warning: Found unknown descriptive keyword \"{0}\", ignoring", line.Trim().ToLower(), null);
                                line = file.ReadLine().TrimEnd();
                            } break;
                        case "description":
                            {
                                Debug.WriteLine("Found description for {0}", cFunction.Name, null);
                                if (!string.IsNullOrEmpty(cFunction.Description))
                                {
                                    throw new Exception(string.Format("Function \"{0}\" already contains a description. Can't define twice.", cFunction.Name));
                                }
                                StringBuilder descriptionBuilder = new StringBuilder();
                                levelstack.Push(GetLevel(line)); // command level push
                                while (GetLevel(line = file.ReadLine()) > levelstack.Peek())
                                {
                                    Debug.WriteLine("Adding 1 line to description");
                                    descriptionBuilder.AppendLine(line.Trim());
                                }
                                levelstack.Pop(); // command level pop
                                cFunction.Description = descriptionBuilder.ToString().Trim();
                                Debug.WriteLine("Description: {0}", cFunction.Description, null);
                            } break;
                        case "arguments":
                            {
                                Debug.WriteLine("Found arguments for {0}", cFunction.Name, null);
                                levelstack.Push(GetLevel(line)); // command level push
                                line = file.ReadLine().TrimEnd();
                                while (GetLevel(line) > levelstack.Peek())
                                {
                                    var spl = line.Trim().Split(' ');
                                    var type = new DataType() { TypeName = spl[0] };
                                    var name = string.Empty;
                                    if (spl.Length > 1)
                                        name = spl.Last();
                                    else
                                    {
                                        name = "arg" + cFunction.Arguments.Count;
                                    }
                                    Debug.WriteLine("Adding argument {0} of type {1}", name, type.TypeName);
                                    StringBuilder descriptionBuilder = new StringBuilder();
                                    levelstack.Push(GetLevel(line)); // argument level push
                                    while (GetLevel(line = file.ReadLine()) > levelstack.Peek())
                                    {
                                        Debug.WriteLine("Adding 1 line to description");
                                        descriptionBuilder.AppendLine(line.Trim());
                                    }
                                    levelstack.Pop(); // argument level pop
                                    type.Description = descriptionBuilder.ToString().Trim();
                                    cFunction.Arguments.Add(name, type);
                                }
                                levelstack.Pop(); // command level pop
                            } break;
                        case "return":
                        case "returns":
                            {
                                var type = new DataType() { TypeName = (line = file.ReadLine()).Trim() };
                                Debug.WriteLine("Found return type {0}", type.TypeName, null);
                                StringBuilder descriptionBuilder = new StringBuilder();
                                levelstack.Push(GetLevel(line)); // command level push
                                while (GetLevel(line = file.ReadLine()) > levelstack.Peek())
                                    descriptionBuilder.AppendLine(line.Trim());
                                levelstack.Pop(); // command level pop
                                type.Description = descriptionBuilder.ToString().Trim();
                                cFunction.ReturnType = type;
                            } break;
                    }
                }
                levelstack.Pop(); // root level pop

                functions.Add(cFunction);
            }

            // using directives
            var stream = Console.OpenStandardOutput();
            var streamwriter = new System.CodeDom.Compiler.IndentedTextWriter(new StreamWriter(stream));
            foreach (string importedNamespace in @using)
                streamwriter.WriteLine("using {0};", importedNamespace);

            // Start namespace
            streamwriter.WriteLine("namespace {0}", @namespace);
            streamwriter.WriteLine("{");
            streamwriter.Indent++;

            // Start class
            streamwriter.WriteLine("public static class {0}", @class);
            streamwriter.WriteLine("{");
            streamwriter.Indent++;

            // Functions
            foreach (var function in functions)
            {
                // Fix return type
                if (function.ReturnType == null)
                    function.ReturnType = new DataType() { TypeName = "void" };
                if (function.ReturnType.TypeName.Equals("boolean", StringComparison.OrdinalIgnoreCase))
                    function.ReturnType.TypeName = "bool";
                
                // Fix argument types
                foreach (var aName in function.Arguments.Keys)
                {
                    var aVal = function.Arguments[aName];
                    if (aVal.TypeName.Equals("boolean", StringComparison.OrdinalIgnoreCase))
                        aVal.TypeName = "bool";
                }

                // Request generation function
                foreach (string l in function.GetMetadataXml().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    streamwriter.WriteLine("/// {0}", l.Trim(), null);
                streamwriter.Flush();
                streamwriter.WriteLine("public static StandardAPIRequest {0}({1})", function.NormalizedName, function.GetParameterSignature());
                streamwriter.Write("{ ");
                streamwriter.Write("return new StandardAPIRequest(\"{0}\"{1});", function.Name, function.Arguments.Count > 0 ? ", " + function.GetParameterPass() : "");
                streamwriter.WriteLine(" }");
                streamwriter.WriteLine();
                streamwriter.Flush();

                // Actual standard API request function
                foreach (string l in function.GetMetadataXml().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    streamwriter.WriteLine("/// {0}", l.Trim(), null);
                streamwriter.Flush();
                streamwriter.WriteLine("public static {2} {0}(this Client client{1})", function.NormalizedName, function.Arguments.Count > 0 ? ", " + function.GetParameterSignature() : "", function.ReturnType == null ? "void" : function.ReturnType.TypeName);
                streamwriter.Write("{ ");
                if(function.ReturnType != null && function.ReturnType.TypeName != "void")
                    streamwriter.Write("return client.Request<{2}>({0}({1}));", function.NormalizedName, function.GetParameterPass(), function.ReturnType.TypeName);
                else
                    streamwriter.Write("client.Request({0}({1}));", function.NormalizedName, function.GetParameterPass());
                streamwriter.WriteLine(" }");
                streamwriter.WriteLine();
                streamwriter.Flush();
            }

            // End class
            streamwriter.Indent--;
            streamwriter.WriteLine("}");

            // End namespace
            streamwriter.Indent--;
            streamwriter.WriteLine("}");

            // Finalize
            streamwriter.Flush();
            streamwriter.Close();
            streamwriter.Dispose();
        }

        static int GetLevel(string line)
        {
            if (line == null)
                return -1;
            return line.TrimEnd().Length - line.TrimEnd().TrimStart('\t').Length;
        }
    }

    class Function
    {
        public string Name = string.Empty;
        public string NormalizedName
        {
            get
            {
                return new string(
                        (from c in (char.ToUpper(Name[0]).ToString() + Name.Substring(1)).ToCharArray()
                            select char.IsSymbol(c) || char.IsWhiteSpace(c) || char.IsSurrogate(c) || char.IsSeparator(c) || char.IsPunctuation(c) || char.IsControl(c) ? '_' : c
                        ).ToArray()
                    );
            }
        }
        public DataType ReturnType = new DataType();
        public Dictionary<string, DataType> Arguments = new Dictionary<string, DataType>();
        public string Description = string.Empty;

        public string GetParameterSignature()
        {
            return string.Join(", ", from a in Arguments.Keys select string.Join(" ", new string[] { Arguments[a].TypeName, a }));
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
                sb.Append("\">");
                sb.Append(a.Argument.Description);
                sb.AppendLine("</param>");
            }
            if (!string.IsNullOrEmpty(ReturnType.Description))
            {
                sb.Append("<returns>");
                sb.Append(ReturnType.Description);
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
