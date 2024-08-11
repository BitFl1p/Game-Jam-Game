using Godot;

public partial class Tree : Mob {
  private static readonly PackedScene PrefabLocation = GD.Load<PackedScene>("res://Prefabs/tree.tscn");
  private PlayerMaster player;

  private static Tree Create() {
    Tree tree = PrefabLocation.Instantiate<Tree>();
    tree.player = PlayerMaster.Instance;
    return tree;
  }

  public override void _Process(double delta) {
    GlobalPosition = islandPos;
    Ai();
  }

  public override bool Place(Vector2 position) {
    Tree tree = Create();
    tree.islandPos = (Vector2I)position;
    tree.player.currentMobs.Add(tree);
    tree.player.island.AddChild(tree);
    //PlayerMaster.Tick += tree.OnTick;
    return true;
  }

  public override void Ai() {
    //GD.Print("Tree tick at" + islandPos);
  }
}
