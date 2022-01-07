using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Proto;
using System.Threading;
using System.Threading.Tasks;

using Shared;

class MyService : IHostedService {
  ILogger logger;
  ActorSystem system;

  public MyService(ILogger<MyService> logger, ActorSystem system, IHostApplicationLifetime lifecycle) {
    this.logger = logger;
    this.system = system;

    lifecycle.ApplicationStarted.Register(OnStarted);
  }

  public Task StartAsync(CancellationToken cancellationToken) {
    return Task.CompletedTask;
  }
  public Task StopAsync(CancellationToken cancellationToken) {
    return Task.CompletedTask;
  }

  void OnStarted() {
    var context = system.Root;

    var props = Props.FromProducer(() => new MyActor(logger));
    var pid = context.SpawnNamed(props, "my-actor");

    logger.LogInformation($"pid {pid}");
  }
}
