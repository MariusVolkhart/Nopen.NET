# Nope'n.NET!

A Roslyn analyzer which requires that classes be `sealed`, `abstract`, or attributed with
`[Open]`.

>  Design and document for inheritance or else prohibit it
>
> – Item 19, Effective Java, Third Edition

[![Build Status](https://dev.azure.com/MariusVolkhart/Nopen.NET/_apis/build/status/MariusVolkhart.Nopen.NET?branchName=master)](https://dev.azure.com/MariusVolkhart/Nopen.NET/_build/latest?definitionId=3&branchName=master)

## Usage

C# creates new types as open by default which can be dangerous. This analyzer ensures that the
intent to leave a class open is explicitly declared.

```csharp
using Nopen.NET.Attributes;

sealed class Foo {}
abstract class Bar {}
[Open] class Baz {}
```

Non-`sealed`, non-`abstract` classes without the `[Open]` attribute will be marked with an error.

```csharp
class Bad {}
```
```
Bad.cs(1,1): error Nopen: Class 'Bad' is implicitly open. Classes should be explicitly marked sealed, abstract, or [Open].
```

For more information see Item 19 of Effective Java, Third Edition.


## Download

[![NuGet](https://img.shields.io/nuget/v/Nopen.NET.svg)](https://www.nuget.org/packages/Nopen.NET/)

```xml
<ItemGroup>
  <PackageReference Include="Nopen.NET" Version="<version_here>" PrivateAssets="All" />
</ItemGroup>
```

## Credits
Thanks to Jake Wharton and his [Nope'n project](https://github.com/JakeWharton/nopen) for the idea.

## License

    Copyright 2019 Marius Volkhart

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.