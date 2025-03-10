using Godot;
using System;

public partial class LevelInfoUI : CanvasLayer
{
  private Button close;
  private RichTextLabel title, description;
  
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    close = GetNode<Button>("%Close");
    title = GetNode<RichTextLabel>("%Title");
    description = GetNode<RichTextLabel>("%Description");
    Visible = false;
  }

  public void Display(string titleTxt,string descriptionTxt)
  {
    title.Text = "[center]"+titleTxt+"[/center]";
    description.Text = "[center]"+descriptionTxt+"[/center]";
    Visible = true;
  }

  public void OnClose()
  {
    title.Text = "";
    description.Text = "";
    Visible = false;
  }
}