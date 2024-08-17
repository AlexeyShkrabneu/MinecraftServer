namespace Domain.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum FontType
{
    [EnumMember(Value = "minecraft:default")]
    Default,

    [EnumMember(Value = "minecraft:uniform")]
    Uniform,

    [EnumMember(Value = "minecraft:alt")]
    Alt,

    [EnumMember(Value = "minecraft:illageralt")]
    Illageralt,
}
