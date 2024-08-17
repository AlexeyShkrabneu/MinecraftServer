namespace Domain.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum HoverEventActionType
{
    [EnumMember(Value = "show_text")]
    ShowText,

    [EnumMember(Value = "show_item")]
    ShowItem,

    [EnumMember(Value = "show_entity")]
    ShowEntity
}
