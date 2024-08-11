
using System.Threading.Tasks;

public class TestCard : Card {
  public TestCard() {
    ManaCost = 1;
    MiniSpriteID = 0;
    SpriteID = 1;
  }
 
  public override async Task<bool> Play() {
    if (!await PlaceMode(new Human(), 5)) return false;
    player.UseMana(ManaCost);
    return true;
  }
}