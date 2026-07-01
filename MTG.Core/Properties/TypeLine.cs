using MTG.Core.Helper;
using MTG.Core.Types;
using System.Text.RegularExpressions;

namespace MTG.Core.Properties;

public class TypeLine
{
    private static readonly Regex TypeLineRegex = new(
        @"^\s*(?<left>[^—]+)(?:—\s*(?<right>.+))?\s*$",
        RegexOptions.Compiled | RegexOptions.ExplicitCapture
    );

    private string Value { get; set; }

    public HashSet<CardType> CardTypes { get; init; }
    public HashSet<SuperType> SuperTypes { get; init; }
    public HashSet<ArtifactType> ArtifactTypes { get; init; }
    public HashSet<CreatureType> CreatureTypes { get; init; }
    public HashSet<EnchantmentType> EnchantmentTypes { get; init; }
    public HashSet<LandType> LandTypes { get; init; }
    public HashSet<PlaneswalkerType> PlaneswalkerTypes { get; init; }
    public HashSet<SpellType> SpellTypes { get; init; }

    private TypeLine(string typeline)
    {
        Value = typeline;
        CardTypes = [];
        SuperTypes = [];
        ArtifactTypes = [];
        CreatureTypes = [];
        EnchantmentTypes = [];
        LandTypes = [];
        PlaneswalkerTypes = [];
        SpellTypes = [];
    }

    public static Result<TypeLine> Create(string typeline)
    {
        if (string.IsNullOrWhiteSpace(typeline))
            return Result<TypeLine>.Failure("Type line cannot be null or empty!");

        var result = new TypeLine(typeline);

        //Split for Two-Faced Cards
        var faces = typeline.Split(["//"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var face in faces)
        {
            //Match
            var match = TypeLineRegex.Match(face);
            if (!match.Success)
                return Result<TypeLine>.Failure($"Invalid type line format: '{face}'");

            //Left Side
            var leftResult = ParseLeftSide(match.Groups["left"].Value);
            if (leftResult.IsFailure)
                return leftResult.ToFailure<TypeLine>();

            var (cardTypes, superTypes) = leftResult.Value;

            if (cardTypes.Count == 0)
                return Result<TypeLine>.Failure($"No valid primary card type found in: '{face}'");

            result.CardTypes.UnionWith(cardTypes);
            result.SuperTypes.UnionWith(superTypes);


            //Right Side
            var rightGroup = match.Groups["right"];

            var rightResult = ParseRightSide(cardTypes, rightGroup);
            if (rightResult.IsFailure)
                return rightResult.ToFailure<TypeLine>();

            var (artifactTypes, creatureTypes, enchantmentTypes, landTypes, planeswalkerTypes, spellTypes) = rightResult.Value;

            result.ArtifactTypes.UnionWith(artifactTypes);
            result.CreatureTypes.UnionWith(creatureTypes);
            result.EnchantmentTypes.UnionWith(enchantmentTypes);
            result.LandTypes.UnionWith(landTypes);
            result.PlaneswalkerTypes.UnionWith(planeswalkerTypes);
            result.SpellTypes.UnionWith(spellTypes);
        }

        return Result<TypeLine>.Success(result);
    }

    private static Result<(HashSet<CardType> CardTypes, HashSet<SuperType> SuperTypes)> ParseLeftSide(string leftSide)
    {
        var cardTypes = new HashSet<CardType>();
        var superTypes = new HashSet<SuperType>();
        var leftWords = leftSide.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in leftWords)
        {
            var camelWord = Conversions.ToCamelCase(word);

            if (Enum.TryParse<CardType>(camelWord, true, out var cardType))
                cardTypes.Add(cardType);

            else if (Enum.TryParse<SuperType>(camelWord, true, out var superType))
                superTypes.Add(superType);

            else
                return Result<(HashSet<CardType>, HashSet<SuperType>)>.Failure($"Unknown type word found on left side: '{word}'");
        }

        return Result<(HashSet<CardType>, HashSet<SuperType>)>.Success((cardTypes, superTypes));
    }

    private static Result<(
        HashSet<ArtifactType> Artifacts,
        HashSet<CreatureType> Creatures,
        HashSet<EnchantmentType> Enchantments,
        HashSet<LandType> Lands,
        HashSet<PlaneswalkerType> Planeswalkers,
        HashSet<SpellType> Spell
        )>
    ParseRightSide(HashSet<CardType> cardTypes, Group rightGroup)
    {
        var artifactTypes = new HashSet<ArtifactType>();
        var creatureTypes = new HashSet<CreatureType>();
        var enchantmentTypes = new HashSet<EnchantmentType>();
        var landTypes = new HashSet<LandType>();
        var planeswalkerTypes = new HashSet<PlaneswalkerType>();
        var spellTypes = new HashSet<SpellType>();

        if (!rightGroup.Success || string.IsNullOrWhiteSpace(rightGroup.Value))
            return Result<(HashSet<ArtifactType>, HashSet<CreatureType>, HashSet<EnchantmentType>, HashSet<LandType>, HashSet<PlaneswalkerType>, HashSet<SpellType>)>.
                   Success((artifactTypes, creatureTypes, enchantmentTypes, landTypes, planeswalkerTypes, spellTypes));

        var rightWords = rightGroup.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        bool isArtifact = cardTypes.Contains(CardType.Artifact);
        bool isCreatureOrKindred = cardTypes.Contains(CardType.Creature) || cardTypes.Contains(CardType.Kindred);
        bool isEnchantment = cardTypes.Contains(CardType.Enchantment);
        bool isLand = cardTypes.Contains(CardType.Land);
        bool isPlaneswalker = cardTypes.Contains(CardType.Planeswalker);
        bool isSpell = cardTypes.Contains(CardType.Instant) || cardTypes.Contains(CardType.Sorcery);

        foreach (var word in rightWords)
        {
            var camelWord = Conversions.ToCamelCase(word);

            if (isCreatureOrKindred && Enum.TryParse<CreatureType>(camelWord, true, out var creatureType))
                creatureTypes.Add(creatureType);

            else if (isArtifact && Enum.TryParse<ArtifactType>(camelWord, true, out var artifactType))
                artifactTypes.Add(artifactType);

            else if (isEnchantment && Enum.TryParse<EnchantmentType>(camelWord, true, out var enchantmentType))
                enchantmentTypes.Add(enchantmentType);

            else if (isLand && Enum.TryParse<LandType>(camelWord, true, out var landType))
                landTypes.Add(landType);

            else if (isPlaneswalker && Enum.TryParse<PlaneswalkerType>(camelWord, true, out var planeswalkerType))
                planeswalkerTypes.Add(planeswalkerType);

            else if (isSpell && Enum.TryParse<SpellType>(camelWord, true, out var spellType))
                spellTypes.Add(spellType);

            else
                return Result<(HashSet<ArtifactType>, HashSet<CreatureType>, HashSet<EnchantmentType>, HashSet<LandType>, HashSet<PlaneswalkerType>, HashSet<SpellType>)>.
                       Failure($"Unknown type word found on right    side: '{word}'");
        }

        return Result<(HashSet<ArtifactType>, HashSet<CreatureType>, HashSet<EnchantmentType>, HashSet<LandType>, HashSet<PlaneswalkerType>, HashSet<SpellType>)>.
               Success((artifactTypes, creatureTypes, enchantmentTypes, landTypes, planeswalkerTypes, spellTypes));
    }

    //Simple Yes/No Questions wrapped
    public bool IsArtifact() => CardTypes.Contains(CardType.Artifact);
    public bool IsBasic() => SuperTypes.Contains(SuperType.Basic);
    public bool IsBattle() => CardTypes.Contains(CardType.Battle);
    public bool IsCreature() => CardTypes.Contains(CardType.Creature);
    public bool IsHistoric() => SuperTypes.Contains(SuperType.Legendary) ||
                                CardTypes.Contains(CardType.Artifact) ||
                                EnchantmentTypes.Contains(EnchantmentType.Saga);
    public bool IsInstant() => CardTypes.Contains(CardType.Instant);
    public bool IsLand() => CardTypes.Contains(CardType.Land);
    public bool IsLegendary() => SuperTypes.Contains(SuperType.Legendary);
    public bool IsPermanent() => !CardTypes.Contains(CardType.Instant) &&
                                 !CardTypes.Contains(CardType.Sorcery);
    public bool IsPlaneswalker() => CardTypes.Contains(CardType.Planeswalker);

    public override string ToString() => Value;
}
