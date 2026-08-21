using MTG.Core.Abilities;

namespace MTG.Engine.Gameplay;

public class EffectResolver
{
    public void Resolve(IEffect effect, GameContext context, CardInstance targets, CommanderPlayer controller)
    {
        switch (effect)
        {
            case DrawCardsEffect draw:
                controller.DrawCard(draw.Amount);
                break;

            case DealDamageEffect damage:
                //var target = targets?.SingleSelectedTarget;
                //context.ApplyDamage(target, damage.Damage);
                break;

            case DestroyTargetEffect:
                //var permanent = targets?.SingleSelectedPermanent;
                //context.Destroy(permanent);
                break;

            default:
                throw new NotImplementedException($"Effekt {effect.GetType().Name} ist nicht verarbeitbar.");
        }
    }
}
