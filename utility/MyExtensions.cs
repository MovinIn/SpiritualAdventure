using System;

namespace SpiritualAdventure.utility;

using Godot;
using Object = Godot.GodotObject;

public static class MyExtensions
{
    public static T SafelySetScript<T>(this Object obj, Variant resource) where T : Object
    {
        var godotObjectId = obj.GetInstanceId();
        // Replaces old C# instance with a new one. Old C# instance is disposed.
        obj.SetScript(resource);
        // Get the new C# instance
        return Object.InstanceFromId(godotObjectId) as T;
    }

    public static T SafelySetScript<T>(this Object obj, string resource) where T : Object
    {
        return SafelySetScript<T>(obj, ResourceLoader.Load(resource));
    }
    
    public static TR Method<TR>(this Type t, string method, object obj = null, params object[] parameters) 
      => (TR)t.GetMethod(method)?.Invoke(obj, parameters);
}