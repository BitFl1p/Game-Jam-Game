using Godot;

public partial class Tree : Mob {
  private static readonly PackedScene PrefabLocation = GD.Load<PackedScene>("res://Prefabs/tree.tscn");
  private PlayerMaster player;
  private int manaTicks = 30;
  private static Tree Create() {
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
    if(tree.player.island.GetTile(tree.GetTilemapPosition()) != Island.PixelType.GRASS) return false;
    tree.player.currentMobs.Add(tree);
    tree.player.island.AddChild(tree); 
    PlayerMaster.Tick += tree.OnTick;
    tree.GlobalPosition = tree.islandPos;
    for (int x = 0; x < 3; x++) for (int y = 0; y < 3; y++) {
      int i = tree.GetTilemapPosition() + x - 1 + 53 * (y - 1);
        if(tree.player.island.GetTile(i) == Island.PixelType.GRASS) tree.player.island.SetTile(i, Island.PixelType.TALL_GRASS);
    }
    return true;
  }

  protected override void Ai() {
    manaTicks--;
    if(manaTicks > 0) return;

    manaTicks = 30;
    player.GainMana(1);
    for (int x = 0; x < 3; x++)
    for (int y = 0; y < 3; y++) {
      int i = GetTilemapPosition() + x - 1 + 53 * (y - 1);
      if(player.island.GetTile(i) == Island.PixelType.GRASS) player.island.SetTile(i, Island.PixelType.TALL_GRASS);
    }
  }
}
