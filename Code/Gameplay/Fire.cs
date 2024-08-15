using Godot;
using System;
using System.Linq;

public partial class Fire : Creature
{
  private static readonly PackedScene PrefabLocation = GD.Load<PackedScene>("res://Prefabs/fire.tscn");
  private PlayerMaster player;
  public override bool canRegenMana { get; protected init; } = false;
  private int fireTick = 10;
  
  public static Fire Create() {
    Fire fire = PrefabLocation.Instantiate<Fire>();
    fire.player = PlayerMaster.Instance;
    return fire;
  }
  
  private void OnTick() {
    GlobalPosition = islandPos;
    Ai();
  }
  public override bool Place(Vector2 position) {
    Fire fire = Create();
    fire.islandPos = (Vector2I)position;
    fire.player.currentMobs.Add(fire);
    fire.player.island.AddChild(fire); 
    PlayerMaster.Tick += fire.OnTick;
    fire.GlobalPosition = fire.islandPos;
    for (int x = 0; x < 3; x++) for (int y = 0; y < 3; y++) {
      int i = fire.GetTilemapPosition() + x - 1 + 53 * (y - 1);
      if(fire.player.island.GetTile(i) is Island.PixelType.GRASS or Island.PixelType.TALL_GRASS or Island.PixelType.BLOOMING_GRASS) fire.player.island.SetTile(i, Island.PixelType.FIRE);
    }
    fire.player.soundManager.Play(SoundManager.Sound.PLACE_HARSH);
    fire.player.island.RedrawPixels();
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
    fireTick = 60;

    
    int closestMob = -1;
    float distance = 999999;
    for(int i = 0; i < player.currentMobs.Count; i++) {
      if (player.currentMobs[i] is Fire or Stone || (player.currentMobs[i].GlobalPosition - GlobalPosition).Length() > distance) continue;
      distance = (player.currentMobs[i].GlobalPosition - GlobalPosition).Length();
      closestMob = i;
    }

    if (distance < 10 && closestMob != -1) {
      Place(player.currentMobs[closestMob].GlobalPosition);
      player.currentMobs[closestMob].Destroy();
    }
    
    if (player.island.GetTile(GetTilemapPosition()) != Island.PixelType.FIRE) Destroy();
  }
}
