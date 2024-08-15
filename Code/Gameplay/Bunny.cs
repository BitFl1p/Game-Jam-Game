using Godot;
using System;

public partial class Bunny : Creature {
  private static readonly PackedScene PrefabLocation = GD.Load<PackedScene>("res://Prefabs/bunny.tscn");
  private PlayerMaster player;
  public override bool canRegenMana { get; protected init; } = false;
  private int directionChangeRate = 0, deathTimer = 20;
  private Vector2I direction;
  private RandomNumberGenerator random = new();
  public static Bunny Create() {
    Bunny bunny = PrefabLocation.Instantiate<Bunny>();
    bunny.player = PlayerMaster.Instance;
    return bunny;
  }
  
  private void OnTick() {
    GlobalPosition = islandPos;
    Ai();
  }
  public override bool Place(Vector2 position) {
    Bunny bunny = Create();
    bunny.islandPos = (Vector2I)position;
    if(bunny.player.island.GetTile(bunny.GetTilemapPosition()) is not (Island.PixelType.GRASS or Island.PixelType.BLOOMING_GRASS or Island.PixelType.TALL_GRASS or Island.PixelType.DIRT or Island.PixelType.ASH or Island.PixelType.STONE)) return false;
    bunny.player.currentMobs.Add(bunny);
    bunny.player.island.AddChild(bunny); 
    PlayerMaster.Tick += bunny.OnTick;
    bunny.GlobalPosition = bunny.islandPos;
    bunny.player.soundManager.Play(SoundManager.Sound.PLACE_SOFT);
    bunny.player.island.RedrawPixels();
    bunny.random.Randomize();
    return true;
  }

  public override void Destroy() {
    player.currentMobs.Remove(this);
    PlayerMaster.Tick -= OnTick;
    QueueFree();
  }

  protected override void Ai() {
    if(deathTimer <= 0) Destroy();
    if (player.island.GetTile(PlayerMaster.GetTilemapPosition(islandPos + direction)) is not (Island.PixelType.GRASS
        or Island.PixelType.BLOOMING_GRASS
        or Island.PixelType.TALL_GRASS or Island.PixelType.DIRT or Island.PixelType.ASH or Island.PixelType.STONE)) {
      direction = -direction;
      directionChangeRate = 5;
      deathTimer--;
      return;
    }
    deathTimer = 20;
    if(GlobalPosition.X is > 56 or < 6) direction = new Vector2I(GlobalPosition.X > 30 ? -1 : 1, 0);
    if(GlobalPosition.Y is > 42 or < 11) direction = new Vector2I(0, GlobalPosition.Y > 30 ? -1 : 1);
    
    islandPos += direction;
    FlipH = direction.X > 0;
    directionChangeRate--;
    if(directionChangeRate > 0) return;
    directionChangeRate = 5;

    direction = new Vector2I(random.RandiRange(-1, 1), random.RandiRange(-1, 1));
  }
}
