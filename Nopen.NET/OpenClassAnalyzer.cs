using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Nopen.NET
{
  /// <summary>
  /// Roslyn analyzer which reports classes that are implicitly declared as being open for extension.
  /// </summary>
  /// <remarks>
  /// Leaving classes accidently open for extension results in brittle libraries and inflexible APIs. This analyzer
  /// will produce a compiler error when such declarations are found. An error is chosen over a warning as the presence
  /// of this analyzer during compilation can only mean an intent to enforce this behavior, in which case warnings are
  /// too easily ignored.
  /// </remarks>
  /// <seealso cref="OpenAttribute"/>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class OpenClassAnalyzer : DiagnosticAnalyzer
  {
    private const string Title = "Classes should be explicitly marked sealed, abstract, or [Open]";

    internal const string MessageFormat =
      "Class '{0}' is implicitly open. Classes should be explicitly marked sealed, abstract, or [Open].";

    private const string Description = "C# creates new types as open by default which can be dangerous. "
                                       + "This analyzer ensures that the intent to leave a class open is explicitly declared. "
                                       + "For more information see Item 19 of Effective Java, Third Edition, which also applies to C#.";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
      DiagnosticRuleIds.OpenClass,
      Title,
      MessageFormat,
      "NOpen",
      DiagnosticSeverity.Error,
      true,
      Description
    );

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
      // Find implicitly typed variable declarations.
      var declaration = (ClassDeclarationSyntax) context.Node;
      var modifiers = declaration.Modifiers.Select(it => it.Kind()).ToList();

      if (modifiers.Contains(SyntaxKind.StaticKeyword))
      {
        // Static classes cannot be subclassed, so this check does not apply
        return;
      }

      if (modifiers.Contains(SyntaxKind.SealedKeyword) || modifiers.Contains(SyntaxKind.AbstractKeyword))
      {
        // Sealed and abstract are allowed
        return;
      }

      var hasOpenAttribute = context.SemanticModel.GetDeclaredSymbol(declaration).GetAttributes()
        .Any(it => it.AttributeClass.ToString() == typeof(OpenAttribute).FullName);

      if (hasOpenAttribute)
      {
        return;
      }

      context.ReportDiagnostic(Diagnostic.Create(
        Rule,
        declaration.GetLocation(),
        declaration.Identifier.ValueText)
      );
    }
  }
}