using System;
using Godot;
using SpiritualAdventure.levels;
using SpiritualAdventure.utility;

namespace SpiritualAdventure.ui;

public static class CommandParser
{
  public static void Parse(string command)
  {
    string[] parts=command.Replace("/", "").Trim().Split(" ");
    if (parts.Length == 0)
    {
      return;
    }

    try
    {
      switch (parts[0])
      {
        case "teleport":
        case "tp":
          if (!Level.InGame()) return;

          Vector2 position = GameUnitUtils.Vector2(int.Parse(parts[1]), int.Parse(parts[2]));

          if (Level.currentCameraMode == Level.CameraMode.Player)
          {
            Level.player.Position = position;
          }
          else if (Level.currentCameraMode == Level.CameraMode.Ghost)
          {
            Level.ghostCamera.Position = position;
          }
          else if (Level.currentCameraMode == Level.CameraMode.Cutscene)
          {
            Level.cutsceneCamera.Position = position;
          }

          break;
      }
    }
    catch (Exception e)
    {
      GD.PushError(e.Message,e.StackTrace);
    }
  }
}