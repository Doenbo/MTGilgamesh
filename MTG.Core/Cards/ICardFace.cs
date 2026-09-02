using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.OracleTextParsers;
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
    void AddComponent(ICardComponent component);

    void AddComponents(IEnumerable<ICardComponent> components);

    bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : class, ICardComponent;

    bool TryGetComponents<T>(out IReadOnlyList<T> components) where T : class, ICardComponent;

    bool HasComponent<T>() where T : class, ICardComponent;

    //ToStrings
    public string ToString();
    public string ToStringConsole();
}