
public class TestCard : Card {
  public TestCard() {
    ManaCost = 10;
    MiniSpriteID = 0;
    SpriteID = 1;
  }
 
  public override async void Play() {
    await PlaceMode(new Human(), 5);
    player.UseMana(ManaCost);
  }
}