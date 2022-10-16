using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace Nopen.NET.Test
{
  public class OpenClassAnalyzerTest : DiagnosticVerifier
  {

    [TestCase("public ")]
    [TestCase("internal ")]
    [TestCase("private ")]
    [TestCase("protected ")]
    [TestCase("protected internal ")]
    [TestCase("protected private ")]
    [TestCase("")] // Implicit internal
    public void VisibilityDoesNotImpactAnalysisOfTopLevelClasses(string visibility)
    {
      var test = $"{visibility}class C {{ }}";
      var expected = new DiagnosticResult
      {
        Id = DiagnosticRuleIds.OpenClass,
        Message = string.Format(OpenClassAnalyzer.MessageFormat, "C"),
        Severity = DiagnosticSeverity.Error,

        // Column 1 because the whole class declaration is the node of interest
        Locations = new[] {new DiagnosticResultLocation("Test0.cs", 1, 1)}
      };

      VerifyCSharpDiagnostic(test, expected);

      test = $"{visibility}sealed class C {{ }}";
      VerifyCSharpDiagnostic(test);
    }

    [TestCase("public ")]
    [TestCase("internal ")]
    [TestCase("private ")]
    [TestCase("protected ")]
    [TestCase("protected internal ")]
    [TestCase("protected private ")]
    [TestCase("")] // Implicit internal
    public void VisibilityDoesNotImpactAnalysisOfInnerClasses(string visibility)
    {
      var test = $@"
static class C
{{
    {visibility}class Inner {{ }}
}}";

      var expected = new DiagnosticResult
      {
        Id = DiagnosticRuleIds.OpenClass,
        Message = string.Format(OpenClassAnalyzer.MessageFormat, "Inner"),
        Severity = DiagnosticSeverity.Error,

        // Column 1 because the whole class declaration is the node of interest
        Locations = new[] {new DiagnosticResultLocation("Test0.cs", 4, 5)}
      };

      VerifyCSharpDiagnostic(test, expected);

      test = $@"
static class C
{{
    {visibility}sealed class Inner {{ }}
}}";
      VerifyCSharpDiagnostic(test);
    }

    [TestCase("static")]
    [TestCase("abstract")]
    [TestCase("sealed")]
    public void TopLevelClassesWithExemptModifiersDoNotProduceError(string modifier)
    {
      var test = $"{modifier} class C {{ }}";
      VerifyCSharpDiagnostic(test);
    }

    [TestCase("static")]
    [TestCase("abstract")]
    [TestCase("sealed")]
    public void InnerClassesWithExemptModifiersDoNotProduceError(string modifier)
    {
      var test = $@"
static class C
{{
    {modifier} class Inner {{ }}
}}";

      VerifyCSharpDiagnostic(test);
    }

    [TestCase("static")]
    [TestCase("abstract")]
    [TestCase("sealed")]
    public void ExemptModifiersWorkWithPartialClasses(string modifier)
    {
      var test = $@"
{modifier} partial class C {{ }}
{modifier} partial class C {{ }}
";

      VerifyCSharpDiagnostic(test);
    }

    [Test]
    public void OpenClassWithAttributeDoesNotProduceError()
    {
      var test = $@"
using {typeof(OpenAttribute).Namespace};

[Open]
class C
{{ }}
";

      VerifyCSharpDiagnostic(test);
    }

    [Test]
    public void RecordsGetAnalyzed()
    {
      var test = "record R { }";
      var expected = new DiagnosticResult
      {
        Id = DiagnosticRuleIds.OpenClass,
        Message = string.Format(OpenClassAnalyzer.MessageFormat, "R"),
        Severity = DiagnosticSeverity.Error,

        // Column 1 because the whole record declaration is the node of interest
        Locations = new[] {new DiagnosticResultLocation("Test0.cs", 1, 1)}
      };

      VerifyCSharpDiagnostic(test, expected);

      test = "sealed Record R {{ }}";
      VerifyCSharpDiagnostic(test);
    }

    protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new OpenClassAnalyzer();
  }
}