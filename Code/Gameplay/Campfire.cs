using Godot;
using System;

public partial class Campfire : Creature {
  private static readonly PackedScene PrefabLocation = GD.Load<PackedScene>("res://Prefabs/campfire.tscn");
  private PlayerMaster player;
  public override bool canRegenMana { get; protected init; } = false;
  private int fireTick = 180;
  
  public static Campfire Create() {
    Campfire campfire = PrefabLocation.Instantiate<Campfire>();
    campfire.player = PlayerMaster.Instance;
    return campfire;
  }
  
  private void OnTick() {
    GlobalPosition = islandPos;
    Ai();
  }
  public override bool Place(Vector2 position) {
    Campfire campfire = Create();
    campfire.islandPos = (Vector2I)position;
    campfire.player.currentMobs.Add(campfire);
    campfire.player.island.AddChild(campfire); 
    PlayerMaster.Tick += campfire.OnTick;
    campfire.GlobalPosition = campfire.islandPos;
    campfire.player.soundManager.Play(SoundManager.Sound.PLACE_SOFT);
    campfire.player.island.RedrawPixels();
    return true;
  }

  public override void Destroy() {
    player.currentMobs.Remove(this);
    PlayerMaster.Tick -= OnTick;
    QueueFree();
  }

  protected override void Ai() {
    fireTick--;
    
    if(fireTick > 0) return;
    Destroy();
    
  }
}
