using Godot;

namespace SpiritualAdventure.ui;

public partial class LevelSelectButton : Button
{
  
  private static PackedScene scene = ResourceLoader.Load<PackedScene>("res://ui/LevelSelectButton.tscn");
  private int level;
  private RichTextLabel title;
  private TextureButton info;
  private MarginContainer infoVisibilityHandle;

  private string titleTxt, descriptionTxt;
  private bool withInfo;

  private const int MaxTitleDisplayCharacters=13;

  public override void _Ready()
  {
    title = GetNode<RichTextLabel>("%Title");
    info = GetNode<TextureButton>("%Info");
    infoVisibilityHandle = GetNode<MarginContainer>("%InfoVisibilityHandle");
    infoVisibilityHandle.Visible = withInfo;
    string shortenedTitle=titleTxt;
    if (titleTxt.Length > MaxTitleDisplayCharacters)
    {
      shortenedTitle=titleTxt.Substring(0, MaxTitleDisplayCharacters)+"...";
    }
    title.Text = "[center]"+shortenedTitle+"[/center]";
  }
  
  public static LevelSelectButton Instantiate(int levelNumber)
  {
    var b = scene.Instantiate<LevelSelectButton>();
    b.level = levelNumber;
    b.SetText("" + levelNumber);
    b.titleTxt = "";
    b.descriptionTxt = "";
    b.withInfo = false;
    return b;
  }

  public LevelSelectButton WithInfo(string titleTxt,string descriptionTxt)
  {
    withInfo = true;
    this.titleTxt = titleTxt;
    this.descriptionTxt= descriptionTxt;
    
    return this;
  }

  public void OnPressed()
  {
    Root.LoadLevel(level);
  }

  public void OnInfoPressed()
  {
    if (!withInfo) return;
    
    Root.DisplayLevelInfo(titleTxt,descriptionTxt);
  }
}