namespace Domain.Constants;

public static class DefaultTextComponents
{
    public static TextComponent UsernameInvalidLength(string userName, int maxPlayerUserNameLength)
    {
        return new()
        {
            Text = "Dear ",
            Exta =
            [
                new() { Text = userName, Color = "#50C878" },
                new() { Text = "!\nUsername is not allowed to be longer than " },
                new() { Text = maxPlayerUserNameLength.ToString(), Color = "#0ABAB5" },
                new() { Text = " characters!" }
            ]
        };
    }

    public static TextComponent ServerVersionIsOutdated(string versionName)
    {
        return new()
        {
            Text = "Oops...",
            Color = "#1CD3A2",
            Exta =
            [
                new() { Text = "\nOur server is not yet available on your version of Minecraft." },
                new() { Text = "\nIn order to play, please use: " },
                new() { Text = $"Minecraft {versionName}", Color = "#0ABAB5" }
            ]
        };
    }

    public static TextComponent ServerVersionIsModern(string versionName)
    {
        return new()
        {
            Text = "Oh-oh...",
            Color = "#1CD3A2",
            Exta =
            [
                new() { Text = "\nYour version of Minecraft is outdated." },
                new() { Text = "\nIn order to play, please update to: " },
                new() { Text = $"Minecraft {versionName}", Color = "#0ABAB5" }
            ]
        };
    }

    public static TextComponent PlayerIsAlreadyOnline(string userName)
    {
        return new()
        {
            Text = userName,
            Color = "#50C878",
            Exta =
            [
                new() { Text = " is already on the server!" },
            ]
        };
    }
}
