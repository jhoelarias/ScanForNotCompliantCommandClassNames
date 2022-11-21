using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScanForNotCompliantCommandClassNames.Data;

namespace ScanForNotCompliantCommandClassNames;

internal class Program
{
    private static void Main(string[] args)
    {
        // This is neccesary because GetReferencedAssemblies only returns the Assemblies being used.
        Console.WriteLine("Not compliant class names");
        var instance = new Class1();
        foreach (var item in CheckForMissingCommandSuffix())
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("Classes with command suffix outside Commands namespace");
        foreach (var item in CheckForCommandsOutsideNamespace())
        {
            Console.WriteLine(item);
        }
    }

    public static IList<string> CheckForMissingCommandSuffix()
    {
        var result = new List<string>();
        foreach (var assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
        {
            Assembly assembly = Assembly.Load(assemblyName);
            List<string> notCompliant = assembly
                .GetTypes()
                .Where(t => t.IsClass && t.Namespace != null && t.Namespace.StartsWith($"{assemblyName.Name}.Commands") && !t.Name.EndsWith("Command"))
                .Select(t => t.Name)?
                .ToList();
            if (notCompliant.Count > 0)
                result.AddRange(notCompliant);
        }
        return result;
    }

    public static IList<string> CheckForCommandsOutsideNamespace()
    {
        var result = new List<string>();
        foreach (var assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
        {
            Assembly assembly = Assembly.Load(assemblyName);
            List<string> notCompliant = assembly
                .GetTypes()
                .Where(t => t.IsClass && t.Namespace != null && !t.Namespace.Contains($"{assemblyName.Name}.Commands") && t.Name.EndsWith("Command"))
                .Select(t => t.Name)?
                .ToList();
            if (notCompliant.Count > 0)
                result.AddRange(notCompliant);
        }
        return result;
    }
}