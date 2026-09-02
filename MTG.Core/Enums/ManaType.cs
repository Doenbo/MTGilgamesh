namespace MTG.Core.Enums;

[Flags]
public enum ManaType : byte
{
    None = 0,          // 0
    White = 1 << 0,    // 1
    Blue = 1 << 1,     // 2
    Black = 1 << 2,    // 4
    Red = 1 << 3,      // 8
    Green = 1 << 4,    // 16
    Colorless = 1 << 5 // 32
}