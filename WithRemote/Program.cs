using Microsoft.Extensions.Logging;
using Proto;
using Proto.Remote;
using Proto.Remote.GrpcNet;
using System;
using System.Threading.Tasks;

using Shared;

string? advertisedHost = Environment.GetEnvironmentVariable("ADVERTISED_HOST");
int advertisedPort;
int.TryParse(Environment.GetEnvironmentVariable("ADVERTISED_PORT"), out advertisedPort);

var loggerFactory = LoggerFactory.Create((c) => c
  .SetMinimumLevel(LogLevel.Trace)
  .AddSimpleConsole()
);
Log.SetLoggerFactory(loggerFactory);

var config = GrpcNetRemoteConfig.BindToAllInterfaces(null, 80)
  .WithAdvertisedHost(advertisedHost)
  .WithAdvertisedPort(advertisedPort > 0 ? advertisedPort : null)
  .WithProtoMessages(MyProtosReflection.Descriptor)
  .WithRemoteDiagnostics(true);

var system = new ActorSystem()
  .WithRemote(config);

await system
  .Remote()
  .StartAsync();

var context = system.Root;

var logger = loggerFactory.CreateLogger<MyActor>();

var props = Props.FromProducer(() => new MyActor(logger));
var pid = context.SpawnNamed(props, "my-actor");

logger.LogInformation($"pid {pid}");

await Task.Delay(-1);
