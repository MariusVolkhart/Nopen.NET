using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace Nopen.NET.Test
{
  internal sealed class OpenClassCodeFixProviderTest : CodeFixVerifier
  {
    [Test]
    public void UnmodifiedClassBecomesSealed()
    {
      const string inViolation = "class B {}";
      const string @fixed = "sealed class B {}";
      VerifyCSharpFix(inViolation, @fixed);
    }

    [TestCase("public ")]
    [TestCase("internal ")]
    [TestCase("private ")]
    [TestCase("protected ")]
    [TestCase("protected internal ")]
    [TestCase("protected private ")]
    public void ModifiedClassBecomesSealed(string modifier)
    {
      var inViolation = $"{modifier}class B {{}}";
      var @fixed = $"{modifier}sealed class B {{}}";
      VerifyCSharpFix(inViolation, @fixed);
    }

    [Test]
    public void PartialClassBecomesSealed()
    {
      const string inViolation = "partial class B {}";
      const string @fixed = "sealed partial class B {}";
      VerifyCSharpFix(inViolation, @fixed);
    }

    protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new OpenClassAnalyzer();

    protected override CodeFixProvider GetCSharpCodeFixProvider() => new OpenClassCodeFixProvider();
  }
}