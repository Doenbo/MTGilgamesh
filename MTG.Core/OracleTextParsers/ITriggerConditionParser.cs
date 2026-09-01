using MTG.Core.Abilities;
using MTG.Core.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.OracleTextParsers;

public interface ITriggerConditionParser
{
    Result<ITriggerCondition> Parse(string rawConditionText);
}
