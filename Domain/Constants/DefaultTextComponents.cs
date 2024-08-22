
using System.Drawing;

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
                new() { Text = "!\n\nUsername is not allowed to be longer than " },
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
                new() { Text = "\n\nOur server is not yet available on your version of Minecraft." },
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
                new() { Text = "\n\nYour version of Minecraft is outdated." },
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

    public static TextComponent ServerMaxPlayersOnline(int maxPlayersCount)
    {
        return new()
        {
            Text = "Oops...",
            Color = "#92000A",
            Exta =
            [
                new() { Text = " The server is full." },
                new() { Text = "\n\nThere are already  " },
                new() { Text = $"{maxPlayersCount}/{maxPlayersCount} ", Color = "#05fa09" },
                new() { Text = "players playing on the server."},
                new() { Text = "\nTry to connect " },
                new() { Text = "later", Color = "#50C878" }
            ]
        };
    }

    public static TextComponent ServerOplineModeUnathorizadPlayer()
    {
        return new()
        {
            Text = "Oops...",
            Color = "#92000A",
            Exta =
            [
                new() { Text = "\n\nThis server accepts only licensed clients." },
                new() { Text = "\nPlease buy "},
                new() { Text = "Minecraft ", Color = "#05fa09" },
                new() { Text = "in order to play!" },
            ]
        };
    }

    public static TextComponent DisconnectDueToIssue()
    {
        return new()
        {
            Text = "Oops...",
            Color = "#92000A",
            Exta =
            [
               new() { Text = "\n\nAn error occurred during your game." },
                new() { Text = "\nPlease try to "},
                new() { Text = "reconnect", Color = "#05fa09" },
                new() { Text = "." },
            ]
        };
    }
}
