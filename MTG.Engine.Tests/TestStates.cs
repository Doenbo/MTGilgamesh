using FluentAssertions;
using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Tests.Dummy;
using MTG.Creator;
using MTG.Engine.Cards;
using MTG.Engine.Gameplay;
using MTG.Engine.States;
using MTG.Engine.Tests.Dummy;

namespace MTG.Engine.Tests;

public class TestStates
{
    [Fact]
    public async Task TestSix()
    {
        var cres = CoreDummyFactory.CreatePresetCard(PresetCard.Six); //TODO Dummy (in all tests)
        cres.IsSuccess.Should().BeTrue(cres.Error);

        var dcomm = EngineDummyFactory.CreateCommanderPlayer([cres.Value]);
        dcomm.IsSuccess.Should().BeTrue(dcomm.Error);

        var ci = CardInstance.Create(cres.Value, dcomm.Value);
        ci.IsSuccess.Should().BeTrue(ci.Error);

        var res = cres.Value.MainFace.TryGetComponent<CreatureComponent>(out var cc2);
        res.Should().BeTrue();
        cc2.Should().NotBeNull();

        var cs = ci.Value.GetState<CreatureState>();
        cs.Should().NotBeNull();
        cs.CurrentPower.Should().Be(cc2.Power);
        cs.CurrentToughness.Should().Be(cc2.Toughness);
    }
}
