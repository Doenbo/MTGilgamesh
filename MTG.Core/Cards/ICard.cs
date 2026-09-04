using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Properties;
using System.Text.Json.Serialization;

namespace MTG.Core.Cards;

public interface ICard
{
    //Merged Face Properties
    string Name { get; }
    string TypeLine { get; }

    //Core
    Guid Id { get; }
    string Lang { get; }
    string Layout { get; }

    //Related Card Objects
    IReadOnlyList<ICard> AllParts { get; }

    //Gameplay
    IReadOnlyList<ICardFace> Faces { get; }
    ICardFace MainFace => Faces[0];
    ManaType ColorIdentity { get; }
    bool IsToken { get; }
    IReadOnlyDictionary<Format, Legality> Legalities { get; }

    //Print
    string CollectorNumber { get; }
    string SetName { get; }
    string Set { get; }
    IReadOnlyDictionary<ImageSize, Uri> ImageUris { get; }
    Rarity Rarity { get; }

    //Simple Yes/No Checks
    bool IsArtifact() => MainFace.IsArtifact();
    bool IsBasic() => MainFace.IsBasic();
    bool IsBattle() => MainFace.IsBattle();
    bool IsCreature() => MainFace.IsCreature();
    bool IsHistoric() => MainFace.IsHistoric();
    bool IsInstant() => MainFace.IsInstant();
    bool IsLand() => MainFace.IsLand();
    bool IsLegendary() => MainFace.IsLegendary();
    bool IsPermanent() => MainFace.IsPermanent();
    bool IsPlaneswalker() => MainFace.IsPlaneswalker();
    bool IsMultifaced() => Faces.Count > 1;

    //ToStrings
    string ToString();
    string ToStringConsole();

}
