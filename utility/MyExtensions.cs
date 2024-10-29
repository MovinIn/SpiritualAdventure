namespace SpiritualAdventure.utility;

using Godot;
using Object = Godot.GodotObject;

public static class MyExtensions
{
    public static T SafelySetScript<T>(this Object obj, Resource resource) where T : Object
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
}