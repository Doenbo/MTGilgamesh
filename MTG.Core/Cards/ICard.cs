using MTG.Core.Enums;
using MTG.Core.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MTG.Core.Cards;

public interface ICard
{
    //Merged Face Properties
    public string FullName { get; }
    public string FullTypeLine { get; }

    //Core
    public Guid Id { get; set; }
    public string Lang { get; set; }
    public string Layout { get; set; }


    //Gameplay
    public List<ICardFace> Faces { get; }
    ICardFace MainFace => Faces[0];
    public Dictionary<Format, Legality> Legalities { get; }
    public List<Color> ProducedMana { get; }

    //Print
    public string CollectorNumber { get; }
    public string SetName { get; set; }
    public string Set { get; }
    public Dictionary<ImageSize, Uri> ImageUris { get; }
    public Rarity Rarity { get; }

    //Simple Getter
    public Result<Color> GetCardColorIdentity();

    //Simple Yes/No Checks
    public bool IsPermanent() => MainFace.IsPermanent();
    public bool IsInstant() => MainFace.IsInstant();
    public bool IsLand() => MainFace.IsLand();
    public bool IsMultifaced() => Faces.Count > 1;

    //ToStrings
    public string ToString();
    public string ToStringConsole();

}
