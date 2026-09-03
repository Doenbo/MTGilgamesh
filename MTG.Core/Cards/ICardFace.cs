using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.OracleTextParsers;
using MTG.Core.Properties;
using MTG.Core.Types;
using System.Diagnostics.CodeAnalysis;

namespace MTG.Core.Cards;

public interface ICardFace
{

    //Core
    string Name { get; }
    TypeLine TypeLine { get; }
    IReadOnlyList<ICardComponent> Components { get; }
    string OracleText { get; }


    //Simple Yes/No Checks
    bool IsArtifact();
    bool IsBasic();
    bool IsBattle();
    bool IsCreature();
    bool IsHistoric();
    bool IsInstant();
    bool IsLand();
    bool IsLegendary();
    bool IsPermanent();
    bool IsPlaneswalker();
    bool IsCardType(CardType cardType);

    //Component Methods
    bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : class, ICardComponent;

    bool TryGetComponents<T>(out IReadOnlyList<T> components) where T : class, ICardComponent;

    bool HasComponent<T>() where T : class, ICardComponent;

    //ToStrings
    string ToString();
    string ToStringConsole();
}