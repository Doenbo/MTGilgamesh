namespace MTG.Core.Abilities;

public interface IAbilityCost { }
public record TapCost() : IAbilityCost;
public record ManaCostData(ManaCost Mana) : IAbilityCost;
public record PayLifeCost(int Amount) : IAbilityCost;
public record SacrificeCost(string TargetDescription) : IAbilityCost;
public record DiscardCost(int Amount) : IAbilityCost;
