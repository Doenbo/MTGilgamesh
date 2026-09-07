using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MTG.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ComponentRole
{
    Token,
    MeldPart,
    MeldResult,
    ComboPiece
}
