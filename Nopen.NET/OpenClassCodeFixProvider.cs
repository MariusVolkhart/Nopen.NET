using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Nopen.NET
{
  /// <summary>
  /// Provides a code fix to mark implicitly open classes as <c>sealed</c>.
  /// </summary>
  [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OpenClassCodeFixProvider)), Shared]
  public sealed class OpenClassCodeFixProvider : CodeFixProvider
  {
    private const string title = "Make sealed";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticRuleIds.OpenClass);

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      var diagnostic = context.Diagnostics.First();
      var diagnosticSpan = diagnostic.Location.SourceSpan;

      // Find the type declaration identified by the diagnostic.
      var classDeclaration = (ClassDeclarationSyntax) root.FindNode(diagnosticSpan);

      // Register a code action that will invoke the fix.
      context.RegisterCodeFix(CodeAction.Create(
          title,
          c => MakeSealedAsync(context.Document, classDeclaration, c),
          title),
        diagnostic
      );
    }

    private static async Task<Document> MakeSealedAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
    {
      // Make sure that declarations of partial classes get formatted as "sealed partial class" not "partial sealed class"
      var sealedToken = SyntaxFactory.Token(SyntaxKind.SealedKeyword);
      var modifiers = classDeclaration.Modifiers;
      var offset = modifiers.Any(it => it.Kind() == SyntaxKind.PartialKeyword) ? 1 : 0;
      var index = modifiers.Count - offset;
      var newDeclaration = classDeclaration.WithModifiers(modifiers.Insert(index, sealedToken));

      // Replace the old class declaration with the new class declaration.
      var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
      var newRoot = oldRoot.ReplaceNode(classDeclaration, newDeclaration);
      return document.WithSyntaxRoot(newRoot);
    }
  }
}