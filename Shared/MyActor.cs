using Microsoft.Extensions.Logging;
using Proto;
using System;
using System.Threading.Tasks;

using Shared;

public class MyActor : IActor {
  ILogger logger;

  public MyActor(ILogger logger) {
    this.logger = logger;
  }

  public async Task ReceiveAsync(IContext context) {
    logger.LogInformation($"ReceiveAsync message {context.Message?.GetType()}");
    logger.LogInformation($"ReceiveAsync sender {context.Sender}");

    var targetAddress = Environment.GetEnvironmentVariable("TARGET_PID");

    if (targetAddress == null) {
      return;
    }

    var targetPid = PID.FromAddress(targetAddress, "my-actor");

    switch (context.Message) {
      case Started _: {
          await Task.Delay(5000);

          context.Send(targetPid, new MessageEnvelope(new Ping(), context.Self));

          break;
        }
      case DeadLetterResponse response: {
          logger.LogInformation($"ReceiveAsync DeadLetterResponse response {response}");
          break;
        }
      case Ping ping: {
          context.Respond(new Pong());
          break;
        }
      case Pong pong: {
          // limit the request to actor -a, to avoid a deadlock
          if (targetPid.Address.EndsWith("-b:80")) {
            var response = await context.RequestAsync<Response>(targetPid, new Request());

            logger.LogInformation($"ReceiveAsync Pong response {response.GetType()}");
          }

          context.Send(targetPid, new MessageEnvelope(new Ping(), context.Self));
          break;
        }
      case Request request: {
          context.Respond(new Response());
          break;
        }
    };
  }
}
