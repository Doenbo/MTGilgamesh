using FluentAssertions;
using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Creator;
using MTG.Engine.Cards;
using MTG.Engine.Gameplay;
using MTG.Engine.States;
using MTG.Engine.Tests.Dummy;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Tests;

public class TestStates
{
    [Fact]
    public async Task Test1()
    {
        var cref = new CardRef() { Name = "Karametra, God of Harvests" };
        var cres = await new CardCreator().GetByExactName(cref); //TODO Dummy (in all tests)
        cres.IsSuccess.Should().BeTrue(cres.Error);

        var dummy = DummyFactory.CreateCommanderPlayer();
        dummy.IsSuccess.Should().BeTrue(dummy.Error);

        var ci = CardInstance.Create(cres.Value, dummy.Value);
        ci.IsSuccess.Should().BeTrue(ci.Error);

        var cc = CreatureComponent.Create("6", "7");
        cc.IsSuccess.Should().BeTrue(cc.Error);

        ci.Value.AddState(new CreatureState(cc.Value));

        var res = cres.Value.MainFace.TryGetComponent<CreatureComponent>(out var cc2);
        res.Should().BeTrue();
        cc2.Should().NotBeNull();

        var cs = ci.Value.GetState<CreatureState>();
        cs.Should().NotBeNull();
        cs.CurrentPower.Should().Be(cc2.Power);
    }
}
