namespace Domain.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum ClickEventActionType
{
    [EnumMember(Value = "open_url")]
    OpenUrl,

    [EnumMember(Value = "run_command")]
    RunCommand,

    [EnumMember(Value = "suggest_command")]
    SuggestCommand,

    [EnumMember(Value = "change_page")]
    ChangePage,

    [EnumMember(Value = "copy_to_clipboard")]
    CopyToClipboard
}
