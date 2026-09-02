using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.OracleTextParsers;
using MTG.Core.Properties;
using MTG.Core.Types;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MTG.Core.Cards;

public class CardFaceFactory
{
    public static Result<ICardFace> Create(
        string name,
        TypeLine typeline,
        string? oracleText,
        IEnumerable<ICardComponent> components)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<ICardFace>.Failure("Name can't be null or empty!");

        if (components is null || !components.Any())
            return Result<ICardFace>.Failure("Components can't be null or empty!");

        var cardFace = new CardFace(components)
        {
            Name = name,
            TypeLine = typeline,
            OracleText = oracleText ?? string.Empty
        };

        return Result<ICardFace>.Success(cardFace);
    }

    private class CardFace : ICardFace
    {
        public CardFace(IEnumerable<ICardComponent> components)
        {
            _components.AddRange(components);
        }

        //100% Mandatory Properties
        public required string Name { get; init; }
        public required TypeLine TypeLine { get; init; }
        public required string OracleText { get; init; }

        private readonly List<ICardComponent> _components = [];
        public IReadOnlyList<ICardComponent> Components => _components.AsReadOnly();


        //Simple Yes/No Checks
        public bool IsArtifact() => TypeLine.IsArtifact();
        public bool IsBasic() => TypeLine.IsBasic();
        public bool IsBattle() => TypeLine.IsBattle();
        public bool IsCreature() => TypeLine.IsCreature();
        public bool IsHistoric() => TypeLine.IsHistoric();
        public bool IsInstant() => TypeLine.IsInstant();
        public bool IsLand() => TypeLine.IsLand();
        public bool IsLegendary() => TypeLine.IsLegendary();
        public bool IsPermanent() => TypeLine.IsPermanent();
        public bool IsPlaneswalker() => TypeLine.IsPlaneswalker();

        public bool IsCardType(CardType cardType) => TypeLine.IsCardType(cardType);

        //Component Methods
        public void AddComponent(ICardComponent component)
        {
            ArgumentNullException.ThrowIfNull(component);
            _components.Add(component);
        }

        public void AddComponents(IEnumerable<ICardComponent> components)
        {
            ArgumentNullException.ThrowIfNull(components);
            _components.AddRange(components);
        }

        public bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : class, ICardComponent
        {
            component = _components.OfType<T>().FirstOrDefault();
            return component != null;
        }

        public bool TryGetComponents<T>(out IReadOnlyList<T> components) where T : class, ICardComponent
        {
            var result = _components.OfType<T>().ToList();
            if (result.Count > 0)
            {
                components = result.AsReadOnly();
                return true;
            }

            components = [];
            return false;
        }

        public bool HasComponent<T>() where T : class, ICardComponent
        {
            return _components.OfType<T>().Any();
        }

        //ToStrings
        public override string ToString() => Name;

        public string ToStringConsole()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"/------------------------------------\\");
            if (TryGetComponent<ManaCostComponent>(out var manaComp))
            {
                sb.AppendLine($"|{Name} {manaComp.ManaCost.ToString()}");
            }
            else
            {
                sb.AppendLine($"|{Name}");
            }

            sb.AppendLine($"|{TypeLine}");

            //TODO auslagern?
            var abilities = Components
                .OfType<KeywordAbilitiesComponent>()
                .SelectMany(c => c.Abilities)
                .Select(item => $"{{{item}}}");

            var actions = Components
                .OfType<KeywordActionsComponent>()
                .SelectMany(c => c.Actions)
                .Select(item => $"{{{item}}}");

            var words = Components
                .OfType<AbilityWordsComponent>()
                .SelectMany(c => c.Words)
                .Select(item => $"{{{item}}}");

            var threewords = string.Concat(abilities.Concat(actions).Concat(words));

            if (threewords != string.Empty)
                sb.AppendLine($"|{threewords}");

            if (!string.IsNullOrEmpty(OracleText))
            {
                var oracletext = OracleText.Replace("\n", $"{Environment.NewLine}|");
                sb.AppendLine($"|{oracletext}");
            }

            if (TryGetComponent<CreatureComponent>(out var creatureComp))
            {
                sb.AppendLine($"|({creatureComp.Power}/{creatureComp.Toughness})");
            }

            sb.AppendLine($"\\------------------------------------/");

            return sb.ToString();
        }
    }
}