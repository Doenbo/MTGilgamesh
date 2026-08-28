namespace MTG.Core.Abilities;

public interface IAbilityCost { }

public record TapCost : IAbilityCost;

public record UntapCost : IAbilityCost;

public record ManaCostData(ManaCost Mana) : IAbilityCost;

public record PayLifeCost(int Amount) : IAbilityCost;

public record SacrificeCost(string TargetDescription, int Amount = 1) : IAbilityCost;

public record SacrificeSelfCost : IAbilityCost;

public record DiscardCardCost(int Amount) : IAbilityCost;

public record DiscardHandCost : IAbilityCost;
