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
                foreach (IMethodSymbol? method in methods.Left.OfType<IMethodSymbol>().OrderBy(m => m.Name))
                {
                    GenerateSimpleTestMethodCalls(mainBody, method);
                }

                // Add theory methods
                foreach (IMethodSymbol method in methods.Right.OfType<IMethodSymbol>().OrderBy(m => m.Name))
                {
                    GenerateTestMethodCallsUsingMemberData(mainBody, method);
                    GenerateTestMethodCallsUsingInlineData(mainBody, method);
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

        private static void GenerateSimpleTestMethodCalls(StringBuilder mainBody, IMethodSymbol? method)
        {
            if (method is not null)
            {
                string containingType = method.ContainingType.ToDisplayString();
                string methodCall = method.IsStatic
                    ? $"{containingType}.{method.Name}();"
                    : $"new {containingType}().{method.Name}();";

                mainBody.AppendLine(methodCall);
#if LOG_PASSING_TEST_NAMES
                        mainBody.AppendLine($"System.Console.WriteLine(\"{containingType}.{method.Name} passed\");");
#endif
            }
        }

        private static void GenerateTestMethodCallsUsingInlineData(StringBuilder mainBody, IMethodSymbol method)
        {
            IEnumerable<AttributeData> inlineAttributeData = method.GetAttributes().Where(attr => attr.AttributeClass?.Name == "InlineDataAttribute");
            foreach (AttributeData inlineDataAttribute in inlineAttributeData)
            {
                var constructorArguments = inlineDataAttribute.ConstructorArguments.FirstOrDefault().Values;

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

#if LOG_PASSING_TEST_NAMES
                            string testPassingMessage = $"{containingType}.{method.Name} ({attributes}) passed";
                            testPassingMessage = testPassingMessage.Replace("\"", "\\\""); // Escape quotes for C# string
                            mainBody.AppendLine($"System.Console.WriteLine(\"{testPassingMessage}\");");
#endif
            }
        }

        private static void GenerateTestMethodCallsUsingMemberData(StringBuilder mainBody, IMethodSymbol method)
        {
            IEnumerable<AttributeData> memberDataAttributes = method.GetAttributes().Where(attr => attr.AttributeClass?.Name == "MemberDataAttribute");
            foreach (AttributeData memberDataAttribute in memberDataAttributes)
            {
                var memberName = memberDataAttribute.ConstructorArguments.FirstOrDefault().Value as string;
                if (memberName is null)
                {
                    continue; // Skip if no member name is provided
                }
                // Assuming the member data is a method returning an IEnumerable<object[]>
                string containingType = method.ContainingType.ToDisplayString();
                string methodCall = method.IsStatic
                    ? $"{containingType}.{method.Name}"
                    : $"new {containingType}().{method.Name}";

                mainBody.AppendLine($"foreach (var data in {containingType}.{memberName}())");
                mainBody.AppendLine("{");

                var testArguments = new string[method.Parameters.Length];
                for (int i = 0; i < method.Parameters.Length; i++)
                {
                    testArguments[i] = $"({method.Parameters[i].Type})data[{i}]";
                }

                mainBody.AppendLine($"    {methodCall}({string.Join(", ", testArguments)});");

#if LOG_PASSING_TEST_NAMES
                string testPassingMessage = $"{containingType}.{method.Name} ({string.Join(", ", testArguments.Select(x => $"{{{x}}}"))}) passed";
                testPassingMessage = testPassingMessage.Replace("\"", "\\\""); // Escape quotes for C# string
                mainBody.AppendLine($"    System.Console.WriteLine($\"{testPassingMessage}\");");
#endif
                mainBody.AppendLine("}");
            }
        }
    }
}
