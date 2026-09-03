using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Properties;

namespace MTG.Core.Cards;

public interface ICard
{
    //Merged Face Properties
    string FullName { get; }
    string FullTypeLine { get; }

    //Core
    Guid Id { get; }
    string Lang { get; }
    string Layout { get; }


    //Gameplay
    IReadOnlyList<ICardFace> Faces { get; }
    ICardFace MainFace => Faces[0];
    ManaType ColorIdentity { get; }
    Dictionary<Format, Legality> Legalities { get; }

    //Print
    string CollectorNumber { get; }
    string SetName { get; }
    string Set { get; }
    Dictionary<ImageSize, Uri> ImageUris { get; }
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
