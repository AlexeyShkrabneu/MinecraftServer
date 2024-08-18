﻿global using Application.Abstractions.Services;
global using Application.Network.Interaction;
global using Application.Network.Paskage;
global using Application.Network.Paskage.Handlers;
global using Domain.Constants;
global using Domain.Enums;
global using Domain.Models;
global using Domain.Models.Encryption;
global using Domain.Models.MojangAuth;
global using Domain.Models.Text;
global using Domain.Options;
global using Infrastructure.Network.Interaction;
global using Infrastructure.Network.Package.ClientBound;
global using Infrastructure.Network.Package.ClientBound.Common;
global using Infrastructure.Network.Package.ClientBound.Login;
global using Infrastructure.Network.Package.ClientBound.Status;
global using Infrastructure.Network.Package.Handlers;
global using Infrastructure.Network.Package.ServerBound;
global using Infrastructure.Network.Package.ServerBound.Login;
global using Infrastructure.Network.Package.ServerBound.Status;
global using Infrastructure.Services;
global using Microsoft.Extensions.DependencyInjection;
global using Newtonsoft.Json;
global using Org.BouncyCastle.Crypto;
global using Org.BouncyCastle.Crypto.Engines;
global using Org.BouncyCastle.Crypto.Modes;
global using Org.BouncyCastle.Crypto.Parameters;
global using Serilog;
global using System.Formats.Asn1;
global using System.Net;
global using System.Net.Sockets;
global using System.Security.Cryptography;
global using System.Text;
