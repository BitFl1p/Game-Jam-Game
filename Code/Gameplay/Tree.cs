using Godot;

public partial class Tree : Creature {
  
  private static readonly PackedScene PrefabLocation = GD.Load<PackedScene>("res://Prefabs/tree.tscn");
  private PlayerMaster player;
  private int manaTicks = 30;
  private int wiltTicks = 20;
  public override bool canRegenMana { get; protected init; } = true;
  public static Tree Create() {
    Tree tree = PrefabLocation.Instantiate<Tree>();
    tree.player = PlayerMaster.Instance;
    return tree;
  }

  private void OnTick() {
    GlobalPosition = islandPos;
    Ai();
  }


  public override bool Place(Vector2 position) {
    Tree tree = Create();
    tree.islandPos = (Vector2I)position;
    if(tree.player.island.GetTile(tree.GetTilemapPosition()) is not (Island.PixelType.GRASS or Island.PixelType.BLOOMING_GRASS or Island.PixelType.TALL_GRASS)) return false;
    if (tree.player.island.GetTile(tree.GetTilemapPosition()) == Island.PixelType.BLOOMING_GRASS) {
      return new BloomingTree().Place(position);
    }
    tree.player.currentMobs.Add(tree);
    tree.player.island.AddChild(tree); 
    PlayerMaster.Tick += tree.OnTick;
    tree.GlobalPosition = tree.islandPos;
    for (int x = 0; x < 3; x++) for (int y = 0; y < 3; y++) {
      int i = tree.GetTilemapPosition() + x - 1 + 53 * (y - 1);
        if(tree.player.island.GetTile(i) == Island.PixelType.GRASS) tree.player.island.SetTile(i, Island.PixelType.TALL_GRASS);
    }
    tree.player.soundManager.Play(SoundManager.Sound.PLACE_SOFT);
    tree.player.island.RedrawPixels();
    return true;
  }

  public override void Destroy() {
    player.currentMobs.Remove(this);
    PlayerMaster.Tick -= OnTick;
    QueueFree();
  }


  protected override void Ai() {
    if (player.island.GetTile(GetTilemapPosition()) is not (Island.PixelType.GRASS or Island.PixelType.TALL_GRASS or Island.PixelType.BLOOMING_GRASS)) { 
      wiltTicks--;
      if (wiltTicks <= 0) Destroy();
    }
    else wiltTicks = 20;
    manaTicks--;
    if(manaTicks > 0) return;

    manaTicks = 120;
    player.GainMana(1, (Vector2I)GlobalPosition);
    for (int x = 0; x < 3; x++)
    for (int y = 0; y < 3; y++) {
      int i = GetTilemapPosition() + x - 1 + 53 * (y - 1);
      if(player.island.GetTile(i) == Island.PixelType.GRASS) player.island.SetTile(i, Island.PixelType.TALL_GRASS);
    }
  }
}
