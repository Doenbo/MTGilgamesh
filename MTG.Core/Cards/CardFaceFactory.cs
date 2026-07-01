using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Properties;
using System.Text;

namespace MTG.Core.Cards;

public class CardFaceFactory
{
    public static Result<ICardFace> Create(string name, TypeLine typeline)
    {
        if (name == null)
            return Result<ICardFace>.Failure("Name, Set and CollectionNumber can't be null!");

        return Result<ICardFace>.Success(new CardFace() { Name = name, TypeLine = typeline });
    }

    private class CardFace : ICardFace
    {
        private readonly Dictionary<Type, ICardComponent> _components = [];

        public CardFace() { }

        //100% Mandatory Properties
        public required string Name { get; init; }
        public required TypeLine TypeLine { get; init; }

        //Gameplay
        public List<KeywordAbility> KeywordAbilities { get; set; } = []; //https://api.scryfall.com/catalog/keyword-abilities
        public List<KeywordAction> KeywordActions { get; set; } = []; //https://api.scryfall.com/catalog/keyword-actions
        public List<AbilityWord> AbilityWords { get; set; } = []; //https://api.scryfall.com/catalog/ability-words
        public string? OracleText { get; set; }

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

        //Component Methods
        public void AddComponent<T>(T component) where T : class, ICardComponent
        {
            _components[typeof(T)] = component;
        }

        public bool HasComponent<T>() where T : class, ICardComponent
            => _components.ContainsKey(typeof(T));

        public T? GetComponent<T>() where T : class, ICardComponent
        {
            return _components.TryGetValue(typeof(T), out var component) ? component as T : null;
        }

        public bool TryGetComponent<T>(out T component) where T : class, ICardComponent
        {
            if (_components.TryGetValue(typeof(T), out var comp))
            {
                component = (T)comp;
                return true;
            }

            component = null!;
            return false;
        }

        public string ToStringConsole()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"/------------------------------------\\");
            if (TryGetComponent<ManaComponent>(out var manaComp))
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