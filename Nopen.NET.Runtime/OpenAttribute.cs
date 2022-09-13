using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Nopen.NET
{
  /// <summary>
  /// Denote a class as explicitly open for extension through inheritance.
  /// </summary>
  /// <remarks>
  /// The only three valid states for a class declaration is:
  /// <list type="bullet">
  /// <item><description><c>sealed class Name</c></description></item>
  /// <item><description><c>abstract class Name</c></description></item>
  /// <item><description><c>[Open] class Name</c></description></item>
  /// </list>
  /// </remarks>
  [PublicAPI]
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  [Conditional("Nopen")] // This attribute should be removed during compilation
  public sealed class OpenAttribute : Attribute
  {
    /// <summary>
    /// An optional reason as to why the class is <c>[Open]</c> rather than <c>sealed</c> or <c>abstract</c>.
    /// </summary>
    [CanBeNull]
    public string Reason { get; }

    /// <summary>
    /// Creates a new attribute with an optional reason.
    /// </summary>
    /// <param name="reason"><see cref="Reason"/></param>
    public OpenAttribute(string reason = null)
    {
      Reason = reason;
    }
  }
}