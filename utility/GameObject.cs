using Godot;

namespace SpiritualAdventure.utility;

public class GameObject
{
  public EGameObject type { get; }
  public string identifier { get; }
  private object data;

  public GameObject(EGameObject type,string identifier, object data)
  {
    this.type = type;
    this.identifier = identifier;
    this.data = data;
  }
  
  public T GetData<T>()
  {
    return (T)data;
  }
}