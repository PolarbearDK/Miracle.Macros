Miracle.Macros
==============

Expand macros in strings.

Sample usage:
```csharp
var macroString = "Hello ${Location}";
Console.WriteLine(macroString.ExpandMacros(new { Location = "World"});
```

Macros has the form: 
````
${Property}
````

Nested properties are supported:
````
${Property.SubProperty}
${Property.SubProperty.SubProperty2}
ect.
````

Composite formatting can be applied using:
````
${Property:format}
${Property.SubProperty:format}
````
