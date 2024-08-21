var server = MinecraftServer
    .CreateBuilder()
    .ConfigureServer(x => x.OnlineMode = true)
    .UseIcon()
    .Build();

server.Start();

Console.ReadLine();

server.Stop();

Console.ReadLine();