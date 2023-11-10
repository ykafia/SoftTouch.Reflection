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

        var code1 = new CodeWriter();

        code1.WriteLine("namespace hello1;");
        context.AddSource("hello1.g.cs", code1.ToString());
        code1.WriteLine("namespace hello1;");
        context.AddSource("hello1.g.cs", code1.ToString());

        foreach (var c in structsWithAttribute)
        {
            var code = new CodeWriter();

            code.WriteLine("namespace hello2;");
            context.AddSource("hello2.g.cs",code.ToString());
        }
    }
}
