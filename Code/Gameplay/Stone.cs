using Godot;

public partial class Stone : Creature {
  private static readonly PackedScene PrefabLocation = GD.Load<PackedScene>("res://Prefabs/stone.tscn");
  private PlayerMaster player;
  public override bool canRegenMana { get; protected init; } = false;
  private int fireTick = 10;
  
  public static Stone Create() {
    Stone stone = PrefabLocation.Instantiate<Stone>();
    stone.player = PlayerMaster.Instance;
    return stone;
  }
  
  private void OnTick() {
    GlobalPosition = islandPos;
  }
  public override bool Place(Vector2 position) {
    Stone stone = Create();
    stone.islandPos = (Vector2I)position;
    stone.player.currentMobs.Add(stone);
    stone.player.island.AddChild(stone); 
    PlayerMaster.Tick += stone.OnTick;
    stone.GlobalPosition = stone.islandPos;
    stone.player.soundManager.Play(SoundManager.Sound.PLACE_HARSH);
    stone.player.island.RedrawPixels();
    return true;
  }

  public override void Destroy() {
    player.currentMobs.Remove(this);
    PlayerMaster.Tick -= OnTick;
    QueueFree();
  }

  protected override void Ai() { }
}
