using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Properties;
using MTG.Core.Types;
using System.Diagnostics.CodeAnalysis;

namespace MTG.Core.Cards;

public interface ICardFace
{
    public ICardComponent[] DebugComponents { get; }

    //100% Mandatory Properties
    public string Name { get; }
    public TypeLine TypeLine { get; }

    //Gameplay
    public List<KeywordAbility> KeywordAbilities { get; } //https://api.scryfall.com/catalog/keyword-abilities
    public List<KeywordAction> KeywordActions { get; } //https://api.scryfall.com/catalog/keyword-actions
    public List<AbilityWord> AbilityWords { get; } //https://api.scryfall.com/catalog/ability-words
    public string OracleText { get; }

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
    public bool IsCardType(CardType cardType);

    //Component Methods
    
    /// <summary>
    /// Fügt eine einzelne Komponente unter ihrem konkreten Laufzeittyp hinzu.
    /// </summary>
    void AddComponent(ICardComponent component);

    /// <summary>
    /// Fügt eine Sammlung von Komponenten hinzu.
    /// </summary>
    void AddComponents(IEnumerable<ICardComponent> components);

    /// <summary>
    /// Versucht, die erste/einzige Komponente vom Typ T abzurufen.
    /// </summary>
    bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : class, ICardComponent;

    /// <summary>
    /// Versucht, alle Komponenten vom Typ T abzurufen.
    /// </summary>
    bool TryGetComponents<T>(out IReadOnlyList<T> components) where T : class, ICardComponent;

    /// <summary>
    /// Prüft, ob mindestens eine Komponente vom Typ T vorhanden ist.
    /// </summary>
    bool HasComponent<T>() where T : class, ICardComponent;

    //ToStrings
    public string ToString();
    public string ToStringConsole();
}