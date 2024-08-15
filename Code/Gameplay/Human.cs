using Godot;
using System;

public partial class Human : Creature {
  
  private static readonly PackedScene PrefabLocation = GD.Load<PackedScene>("res://Prefabs/human.tscn");
  private PlayerMaster player;
  [Export] private Sprite2D hungerIcon, dyingIcon;
  public override bool canRegenMana { get; protected init; } = false;
  private int deathTimer = 20, hunger = 180, manaTick = 20;
  private bool hasFood;
  private Vector2I direction;
  private RandomNumberGenerator random = new();
  public static Human Create() {
    Human human = PrefabLocation.Instantiate<Human>();
    human.player = PlayerMaster.Instance;
    return human;
  }
  
  private void OnTick() {
    GlobalPosition = islandPos;
    Ai();
    hungerIcon.Visible = hunger < 180;
    dyingIcon.Visible = deathTimer < 20;
    if (hunger > 180) manaTick--;
    if(manaTick > 0) return;
    manaTick = 30;
    player.GainMana(1, islandPos);
  }
  public override bool Place(Vector2 position) {
    Human human = Create();
    human.islandPos = (Vector2I)position;
    if(human.player.island.GetTile(human.GetTilemapPosition()) is not (Island.PixelType.GRASS or Island.PixelType.BLOOMING_GRASS or Island.PixelType.TALL_GRASS or Island.PixelType.DIRT or Island.PixelType.ASH or Island.PixelType.STONE)) return false;
    human.player.currentMobs.Add(human);
    human.player.island.AddChild(human); 
    PlayerMaster.Tick += human.OnTick;
    human.GlobalPosition = human.islandPos;
    human.player.soundManager.Play(SoundManager.Sound.PLACE_SOFT);
    human.player.island.RedrawPixels();
    human.random.Randomize();
    return true;
  }

  public override void Destroy() {
    player.currentMobs.Remove(this);
    PlayerMaster.Tick -= OnTick;
    QueueFree();
  }

  protected override void Ai() {
    if(deathTimer <= 0) Destroy();
    if (hunger < 0 ||
        player.island.GetTile(PlayerMaster.GetTilemapPosition(islandPos + direction * 2)) is not (Island.PixelType.GRASS
          or Island.PixelType.BLOOMING_GRASS
          or Island.PixelType.TALL_GRASS or Island.PixelType.DIRT or Island.PixelType.ASH or Island.PixelType.STONE)) {
      direction = -direction;
      deathTimer--;
    }
    else deathTimer = 20;

    hunger--;
    if (hasFood) {
      if (hunger < 180) {
        float distanceToFire = 9999;
        int closestFire = -1;
        for (int i = 0; i < player.currentMobs.Count; i++) if (player.currentMobs[i] is Fire fire) {
          if ((fire.GlobalPosition - GlobalPosition).Length() >= distanceToFire) continue;
          distanceToFire = (fire.GlobalPosition - GlobalPosition).Length();
          closestFire = i;
        }
        if (closestFire != -1) if (distanceToFire > 8) direction = (Vector2I)(player.currentMobs[closestFire].GlobalPosition - GlobalPosition).Normalized();
          else { hasFood = false; hunger = 360; }
        
        else new Fire().Place(islandPos + new Vector2I(random.RandiRange(-1, 1), random.RandiRange(-1, 1) * 10));
      }
    }
    else {
      if (hunger < 180) {
        float distanceToBunny = 9999;
        int closestBunny = -1;
        for (int i = 0; i < player.currentMobs.Count; i++) if (player.currentMobs[i] is Bunny bunny) {
          if ((bunny.GlobalPosition - GlobalPosition).Length() >= distanceToBunny) continue;
          distanceToBunny = (bunny.GlobalPosition - GlobalPosition).Length();
          closestBunny = i;
        }

        if (closestBunny != -1)
          if (distanceToBunny > 8)
            direction = (Vector2I)(player.currentMobs[closestBunny].GlobalPosition - GlobalPosition).Normalized();
          else {
            player.currentMobs[closestBunny].Destroy();
            hasFood = true;
          }

        else direction = new Vector2I(random.RandiRange(-1, 1), random.RandiRange(-1, 1));
      }
    }

    if(GlobalPosition.X is > 56 or < 6) direction = new Vector2I(GlobalPosition.X > 30 ? -1 : 1, 0);
    if(GlobalPosition.Y is > 42 or < 11) direction = new Vector2I(0, GlobalPosition.Y > 30 ? -1 : 1);
    
    islandPos += direction * 2;
    
  }
}
