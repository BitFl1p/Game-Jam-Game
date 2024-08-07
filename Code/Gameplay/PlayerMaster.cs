using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class PlayerMaster : Node2D {
  
  public enum Mode : byte {
    IDLE, PLACING
  }
  
  [Export] private int maxMana, mana, baseManaRegen;
  [Export] private TextureProgressBar manaBar;
  [Export] public float animSpeed = .1f;
  [Export] private Island island;
  [Export] private PackedScene cardPrefab;
  public Mode playMode;
  private readonly List<Card> hand = new();
  private readonly List<Sprite2D> cardVisuals = new();
  private readonly List<Card> deck = new();
  private readonly List<Mob> currentMobs = new();
  public static event Action<Vector2> MouseClick;
  private Vector2 mousePos;
  
  #region Singleton
  public static PlayerMaster Instance { get; private set; }
  public override void _Ready() {
    if (Instance is not null) QueueFree();
    else Instance = this;
    
    hand.Add(new TestCard());
    hand.Add(new TestCard());
    hand.Add(new TestCard());
    hand.Add(new TestCard());
    hand.Add(new TestCard());
    InitialiseCards();
    
    Tick += OnTick;
  }
  #endregion

  #region Ticker
  [Export] private float tickRate = 0.25f;
  public float tickRateMultiplier;
  private float tick;
  
  public static event Action Tick;
  private void HandleTicks(float delta) {
    tick -= delta * tickRateMultiplier;
    
    if (tick > 0) return;
    Tick?.Invoke();
    tick = tickRate;
  }
  #endregion
  
  public override void _Input(InputEvent @event) {
    switch (@event) {
      case InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } mouseClick:
        MouseClick?.Invoke(mouseClick.Position);
        break;
      case InputEventMouseMotion motion:
        mousePos = motion.Position;
        break;
    }
  }

  private void InitialiseCards() {
    foreach (Sprite2D card in hand.Select(card => cardPrefab.Instantiate<Sprite2D>())) {
      cardVisuals.Add(card);
      AddChild(card);
    }
  }
  private void AddCard(Card card) {
    
  }
  private void RemoveCard(Card card) {
    
  }

  private void UpdateCards() {
    for (int i = 0; i < cardVisuals.Count; i++) {
      cardVisuals[i].Position = cardVisuals[i].Position.Lerp(new Vector2(16 + 8 * i, cardVisuals[i].Position.Y), animSpeed);
      if (cardVisuals[i].Position.X - mousePos.X is < 4 and > -4 && mousePos.Y > 50) {
        cardVisuals[i].Position = cardVisuals[i].Position.Lerp(new Vector2(cardVisuals[i].Position.X, playMode == Mode.IDLE ? 40 : 20), animSpeed);
        cardVisuals[i].Frame = hand[i].SpriteID;
        cardVisuals[i].ZIndex = 100;
      }
      else {
        cardVisuals[i].Position = cardVisuals[i].Position.Lerp(new Vector2(cardVisuals[i].Position.X, playMode == Mode.IDLE ? 68 - mousePos.Y / 8 : 20), animSpeed);
        cardVisuals[i].Frame = hand[i].MiniSpriteID;
        cardVisuals[i].ZIndex = i + 11;
      }
    }
  }
  public override void _Process(double delta) {
    HandleTicks((float)delta);
    UpdateCards();
    
    manaBar.MinValue = 0;
    manaBar.MaxValue = maxMana;
    manaBar.Value = mana;
  }

  public void UseMana(int amount) => mana -= amount;
  
  private void OnTick() {
    mana += baseManaRegen;
    
    Mathf.Clamp(mana, 0, maxMana);
  }
  
}
