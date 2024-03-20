```csharp
var methodName = "WriteLine";
var message = "Hello, World!";

var allTyeps = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(assembly => assembly.GetTypes()).ToList();
// Attempt to find the Console type in the currently loaded assemblies
var consoleType = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(assembly => assembly.GetTypes())
    .FirstOrDefault(type => type.FullName == "System.Console");

if (consoleType != null)
{
    // Attempt to find the specific WriteLine method that accepts a string argument
    var writeLineMethod = consoleType.GetMethod(methodName, new[] { typeof(string) });

    if (writeLineMethod != null)
    {
        // Invoke the method
        writeLineMethod.Invoke(null, new object[] { message });
    }
    else
    {
        Console.WriteLine("Method not found.");
    }
}
else
{
    Console.WriteLine("Console type not found.");
}
```