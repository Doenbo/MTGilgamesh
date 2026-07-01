using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.Cards;

public interface ICardFace
{
    //100% Mandatory Properties
    public string Name { get; }
    public TypeLine TypeLine { get; }

    //Gameplay
    public List<KeywordAbility> KeywordAbilities { get; } //https://api.scryfall.com/catalog/keyword-abilities
    public List<KeywordAction> KeywordActions { get; } //https://api.scryfall.com/catalog/keyword-actions
    public List<AbilityWord> AbilityWords { get; } //https://api.scryfall.com/catalog/ability-words
    public string? OracleText { get; set; }

    //Simple Yes/No Checks
    public bool IsArtifact();
    public bool IsBasic();
    public bool IsBattle();
    public bool IsCreature();
    public bool IsHistoric();
    public bool IsInstant();
    public bool IsLand();
    public bool IsLegendary();
    public bool IsPermanent();
    public bool IsPlaneswalker();

    //Component Methods
    void AddComponent<T>(T component) where T : class, ICardComponent;
    bool HasComponent<T>() where T : class, ICardComponent;
    T? GetComponent<T>() where T : class, ICardComponent;
    bool TryGetComponent<T>(out T component) where T : class, ICardComponent;

    //ToStrings
    public string ToString() => Name;
    public string ToStringConsole();
}