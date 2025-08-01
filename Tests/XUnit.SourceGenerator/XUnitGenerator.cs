using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace XUnit.SourceGenerator
{
    [Generator]
    internal class XUnitGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var factMethods = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "Xunit.FactAttribute",
                    predicate: static (node, _) => node is MethodDeclarationSyntax,
                    transform: static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol((MethodDeclarationSyntax)ctx.TargetNode) as IMethodSymbol)
                .Where(static m => m is not null)
                .Collect();

            var theoryMethods = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "Xunit.TheoryAttribute",
                    predicate: static (node, _) => node is MethodDeclarationSyntax,
                    transform: static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol((MethodDeclarationSyntax)ctx.TargetNode) as IMethodSymbol)
                .Where(static m => m is not null)
                .Collect();

            var allTestMethods = factMethods.Combine(theoryMethods);

            context.RegisterSourceOutput(allTestMethods, static (spc, methods) =>
            {
                var mainBody = new StringBuilder();

                // Generate calls for Fact methods
                foreach (IMethodSymbol? method in methods.Left.OrderBy(m => m.Name))
                {
                    if (method is not null)
                    {
                        string containingType = method.ContainingType.ToDisplayString();
                        string methodCall = method.IsStatic
                            ? $"{containingType}.{method.Name}();"
                            : $"new {containingType}().{method.Name}();";

                        mainBody.AppendLine(methodCall);
                        mainBody.AppendLine($"System.Console.WriteLine(\"{containingType}.{method.Name} passed\");");
                    }
                }

                // Add theory methods
                foreach (IMethodSymbol? method in methods.Right.OrderBy(m => m.Name))
                {
                    if (method is not null)
                    {
                        IEnumerable<AttributeData> attributeData = method.GetAttributes().Where(attr => attr.AttributeClass?.Name == "InlineDataAttribute");
                        foreach (AttributeData dataAttribute in attributeData)
                        {
                            var constructorArguments = dataAttribute.ConstructorArguments.FirstOrDefault().Values;

                            var attributes = new StringBuilder();
                            bool addComma = false;
                            for (int parameterIndex = 0; parameterIndex < method.Parameters.Length; parameterIndex++)
                            {
                                if (addComma)
                                {
                                    attributes.Append(", ");
                                }

                                addComma = true;

                                var parameter = method.Parameters[parameterIndex];
                                var constructorArgument = constructorArguments[parameterIndex];
                                if (!SymbolEqualityComparer.Default.Equals(constructorArgument.Type, parameter.Type))
                                {
                                    attributes.Append($"({constructorArgument.Type})");
                                }

                                attributes.Append(constructorArgument.ToCSharpString());
                            }

                            string containingType = method.ContainingType.ToDisplayString();
                            string methodCall = method.IsStatic
                                ? $"{containingType}.{method.Name}({attributes});"
                                : $"new {containingType}().{method.Name}({attributes});";

                            mainBody.AppendLine(methodCall);

                            string testPassingMessage = $"{containingType}.{method.Name} ({attributes}) passed";
                            testPassingMessage = testPassingMessage.Replace("\"", "\\\""); // Escape quotes for C# string

                            mainBody.AppendLine($"System.Console.WriteLine(\"{testPassingMessage}\");");
                        }
                    }
                }

                var source = $@"
using System;

public static class __GeneratedMain
{{
    public static int Main()
    {{
        {mainBody}

        return 0;
    }}
}}";

                spc.AddSource("SimpleRunner.g.cs", SourceText.From(source, Encoding.UTF8));
            });
        }
    }
}
