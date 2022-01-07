using Grpc.HealthCheck;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Remote;
using Proto.Remote.GrpcNet;
using System;

using Shared;

var builder = WebApplication.CreateBuilder(args);

string advertisedHost = builder.Configuration["ADVERTISED_HOST"];
int advertisedPort;
int.TryParse(builder.Configuration["ADVERTISED_PORT"], out advertisedPort);

builder.Host.ConfigureHostOptions((options) => {
  options.ShutdownTimeout = TimeSpan.FromSeconds(5);
});
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole();
builder.Services.AddGrpc((options) => {
  options.MaxReceiveMessageSize = null;
  options.EnableDetailedErrors = true;
});
builder.WebHost.ConfigureKestrel((context, options) => {
  options.ListenAnyIP(80, (listenOptions) => {
    listenOptions.Protocols = HttpProtocols.Http2;
    // listenOptions.DisableAltSvcHeader = true;
    // listenOptions.UseHttps();
  });
});
var config = GrpcNetRemoteConfig.BindToAllInterfaces(null, 80)
  .WithAdvertisedHost(advertisedHost)
  .WithAdvertisedPort(advertisedPort > 0 ? advertisedPort : null)
  .WithProtoMessages(MyProtosReflection.Descriptor)
  .WithRemoteDiagnostics(true);
builder.Services.AddRemote(config);
builder.Services.AddHostedService<MyService>();

var app = builder.Build();
app.UseRouting();
app.UseProtoRemote();

await app.RunAsync();
