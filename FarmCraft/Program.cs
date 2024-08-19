var server = MinecraftServer
    .CreateBuilder()
    .UseIcon()
    .Build();

server.Start();

Console.ReadLine();

server.Stop();

Console.ReadLine();