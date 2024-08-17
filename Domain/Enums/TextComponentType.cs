namespace Domain.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TextComponentType
{
    [EnumMember(Value = "text")]
    Text,

    [EnumMember(Value = "translatable")]
    Translatable,

    [EnumMember(Value = "keybind")]
    Keybind,

    [EnumMember(Value = "score")]
    Score,

    [EnumMember(Value = "selector")]
    Selector,

    [EnumMember(Value = "nbt")]
    NBT
}
