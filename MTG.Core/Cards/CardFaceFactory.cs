using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Properties;
using MTG.Core.Types;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MTG.Core.Cards;

public class CardFaceFactory
{
    public static Result<ICardFace> Create(string name, TypeLine typeline, string? oracleText)
    {
        if (name == null)
            return Result<ICardFace>.Failure("Name can't be null!");

        return Result<ICardFace>.Success(new CardFace() { Name = name, TypeLine = typeline, OracleText = oracleText ?? string.Empty });
    }

    private class CardFace : ICardFace
    {
        private readonly Dictionary<Type, List<ICardComponent>> _components = [];

        public CardFace() { }

        //100% Mandatory Properties
        public required string Name { get; init; }
        public required TypeLine TypeLine { get; init; }
        public required string OracleText { get; init; }

        //Gameplay
        public List<KeywordAbility> KeywordAbilities { get; init; } = []; //https://api.scryfall.com/catalog/keyword-abilities
        public List<KeywordAction> KeywordActions { get; init; } = []; //https://api.scryfall.com/catalog/keyword-actions
        public List<AbilityWord> AbilityWords { get; init; } = []; //https://api.scryfall.com/catalog/ability-words

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

            var type = component.GetType();

            if (!_components.TryGetValue(type, out var list))
            {
                list = [];
                _components[type] = list;
            }

            list.Add(component);
        }

        public void AddComponents(IEnumerable<ICardComponent> components)
        {
            ArgumentNullException.ThrowIfNull(components);

            foreach (var component in components)
            {
                AddComponent(component);
            }
        }

        public bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : class, ICardComponent
        {
            if (_components.TryGetValue(typeof(T), out var list) && list.Count > 0)
            {
                component = (T)list[0];
                return true;
            }

            component = null;
            return false;
        }

        public bool TryGetComponents<T>(out IReadOnlyList<T> components) where T : class, ICardComponent
        {
            if (_components.TryGetValue(typeof(T), out var list) && list.Count > 0)
            {
                components = list.Cast<T>().ToList().AsReadOnly();
                return true;
            }

            components = [];
            return false;
        }

        public bool HasComponent<T>() where T : class, ICardComponent
        {
            return _components.TryGetValue(typeof(T), out var list) && list.Count > 0;
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
            var threewords = string.Empty;
            if (KeywordAbilities.Count != 0)
            {
                var keywords = string.Concat(KeywordAbilities.Select(item => $"{{{item}}}"));
                threewords += keywords;
            }

            if (KeywordActions.Count != 0)
            {
                var keywords = string.Concat(KeywordActions.Select(item => $"{{{item}}}"));
                threewords += keywords;
            }

            if (AbilityWords.Count != 0)
            {
                var keywords = string.Concat(AbilityWords.Select(item => $"{{{item}}}"));
                threewords += keywords;
            }
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