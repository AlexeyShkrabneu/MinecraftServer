var server = MinecraftServer
    .CreateBuilder()
    .ConfigureServer(c => c.OnlineMode = true)
    .UseIcon()
    .Build();

server.Start();

Console.ReadLine();

server.Stop();

Console.ReadLine();