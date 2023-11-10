using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SoftTouch.Reflection.Generator;

[Generator]
public class ReflectGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }


    public void Execute(GeneratorExecutionContext context)
    {
        // Code generation goes here
        var classesWithAttribute = context.Compilation.SyntaxTrees
                .SelectMany(st => st.GetRoot()
                        .DescendantNodes()
                        .Where(n => n is ClassDeclarationSyntax)
                        .Select(n => n as ClassDeclarationSyntax)
                        .Where(r => r != null && r.AttributeLists
                            .SelectMany(al => al.Attributes)
                            .Any(a => a.Name.GetText().ToString() == "Reflectable")));
        var structsWithAttribute = context.Compilation.SyntaxTrees
                .SelectMany(st => st.GetRoot()
                        .DescendantNodes()
                        .Where(n => n is StructDeclarationSyntax)
                        .Select(n => n as StructDeclarationSyntax)
                        .Where(r => r != null && r.AttributeLists
                            .SelectMany(al => al.Attributes)
                            .Any(a => a.Name.GetText().ToString() == "Reflectable")));


        foreach (var s in structsWithAttribute)
        {
            var nspace = s.SyntaxTree.GetRoot().DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>().First();
            var usings = s.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();
            var code = new CodeWriter();

            var propertiesWithGetters =
                s.Members.OfType<PropertyDeclarationSyntax>()
                .Where(x => x.AccessorList != null && x.AccessorList.GetText().ToString().Contains("get"))
                .Where(x => x.GetText().ToString().Contains("public "))
                .Select(x => (x.Type, x.Identifier));
            var propertiesWithGettersAndSetters =
                s.Members.OfType<PropertyDeclarationSyntax>()
                .Where(x => x.AccessorList != null && x.AccessorList.GetText().ToString().Contains("get"))
                .Where(x => x.AccessorList != null && x.AccessorList.GetText().ToString().Contains("set"))
                .Where(x => x.GetText().ToString().Contains("public "))
                .Select(x => (x.Type, x.Identifier));

            foreach (var u in usings)
                code.WriteLine($"using {u.Name};");

            code.WriteLine("using SoftTouch.Reflection.Core;")
                .WriteEmptyLines(2)
                .WriteLine($"namespace {nspace.Name};")
                .WriteEmptyLines(1);



            code
                .WriteLine($"public partial struct {s.Identifier} : IReflectable")
                .OpenBlock();

            code.WriteLine($"public static string[] Getters {{ get; }} = {{{string.Join(", ", propertiesWithGetters.Select(x => $"\"{x.Identifier}\""))}}};");
            code.WriteLine($"public static string[] Setters {{ get; }} = {{{string.Join(", ", propertiesWithGettersAndSetters.Select(x => $"\"{x.Identifier}\""))}}};");

            code.WriteLine("public static bool HasProperty(string name) => Getters.Contains(name) || Setters.Contains(name);");


            code
                .WriteLine("public T Get<T>(string propertyName)")
                .OpenBlock();
            bool first = true;
            foreach (var (t, p) in propertiesWithGetters)
            {
                var condition = first ? "if" : "else if";
                code.WriteLine($"{condition}(propertyName == \"{p}\" && {p} is T _tmp_{p})")
                    .WriteLine($"    return _tmp_{p};");
                first = false;
            }

            code
                .WriteLine($"else throw new Exception($\"Cannot find property {{propertyName}} of type {{typeof(T)}}\");")
                .CloseBlock();


            code.WriteLine("public void Set<T>(string propertyName, T value)")
                .OpenBlock();
            first = true;
            foreach (var (t, p) in propertiesWithGettersAndSetters)
            {
                var condition = first ? "if" : "else if";
                code.WriteLine($"{condition}(propertyName == \"{p}\" && value is {t} _tmp_{p})")
                    .WriteLine($"    {p} = _tmp_{p};");
                first = false;
            }
            code
                .WriteLine($"else throw new Exception($\"Cannot find property {{propertyName}} of type {{typeof(T)}}\");")
                .CloseAllBlocks();

            context.AddSource($"{s.Identifier}.g.cs", code.ToString());
        }
        foreach (var c in classesWithAttribute)
        {
            var nspace = c.SyntaxTree.GetRoot().DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>().First();
            var usings = c.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();
            var code = new CodeWriter();

            var propertiesWithGetters =
                c.Members.OfType<PropertyDeclarationSyntax>()
                .Where(x => x.AccessorList != null && x.AccessorList.GetText().ToString().Contains("get"))
                .Where(x => x.GetText().ToString().Contains("public "))
                .Select(x => (x.Type, x.Identifier));
            var propertiesWithGettersAndSetters =
                c.Members.OfType<PropertyDeclarationSyntax>()
                .Where(x => x.AccessorList != null && x.AccessorList.GetText().ToString().Contains("get"))
                .Where(x => x.AccessorList != null && x.AccessorList.GetText().ToString().Contains("set"))
                .Where(x => x.GetText().ToString().Contains("public "))
                .Select(x => (x.Type, x.Identifier));

            foreach (var u in usings)
                code.WriteLine($"using {u.Name};");

            code.WriteLine("using SoftTouch.Reflection.Core;")
                .WriteEmptyLines(2)
                .WriteLine($"namespace {nspace.Name};")
                .WriteEmptyLines(1);



            code
                .WriteLine($"public partial class {c.Identifier} : IReflectable")
                .OpenBlock();

            code.WriteLine($"public static string[] Getters {{ get; }} = {{{string.Join(", ", propertiesWithGetters.Select(x => $"\"{x.Identifier}\""))}}};");
            code.WriteLine($"public static string[] Setters {{ get; }} = {{{string.Join(", ", propertiesWithGettersAndSetters.Select(x => $"\"{x.Identifier}\""))}}};");

            code.WriteLine("public static bool HasProperty(string name) => Getters.Contains(name) || Setters.Contains(name);");


            code
                .WriteLine("public T Get<T>(string propertyName)")
                .OpenBlock();
            bool first = true;
            if (propertiesWithGetters.Any())
            {
                foreach (var (t, p) in propertiesWithGetters)
                {
                    var condition = first ? "if" : "else if";
                    code.WriteLine($"{condition}(propertyName == \"{p}\" && {p} is T _tmp_{p})")
                        .WriteLine($"    return _tmp_{p};");
                    first = false;
                }

                code
                    .WriteLine($"else throw new Exception($\"Cannot find property {{propertyName}} of type {{typeof(T)}}\");")
                    .CloseBlock();
            }
            else
                code
                    .WriteLine($"throw new Exception($\"Cannot find property {{propertyName}} of type {{typeof(T)}}\");")
                    .CloseBlock();
            code.WriteLine("public void Set<T>(string propertyName, T value)")
                .OpenBlock();
            first = true;
            if (propertiesWithGettersAndSetters.Any())
            {
                foreach (var (t, p) in propertiesWithGettersAndSetters)
                {
                    var condition = first ? "if" : "else if";
                    code.WriteLine($"{condition}(propertyName == \"{p}\" && value is {t} _tmp_{p})")
                        .WriteLine($"    {p} = _tmp_{p};");
                    first = false;
                }
                code
                    .WriteLine($"else throw new Exception($\"Cannot find property {{propertyName}} of type {{typeof(T)}}\");")
                    .CloseAllBlocks();
            }
            else
                code
                    .WriteLine($"throw new Exception($\"Cannot find property {{propertyName}} of type {{typeof(T)}}\");")
                    .CloseAllBlocks();

            context.AddSource($"{c.Identifier}.g.cs", code.ToString());
        }
    }
}
