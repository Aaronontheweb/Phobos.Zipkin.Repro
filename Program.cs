using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using OpenTracing.Util;
using Petabridge.Tracing.Zipkin;
using Phobos.Actor;
using Phobos.Actor.Configuration;
using Phobos.Tracing.Scopes;

namespace Phobos.Zipkin.Repro
{
    class MyActor : UntypedActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        protected override void OnReceive(object message)
        {
            using (var trace = Context.GetInstrumentation().Tracer.BuildSpan("Foo").StartActive())
            {
                _log.Info("Tracer is of type [{0}]",  Context.GetInstrumentation().Tracer.GetType());
                _log.Info("Received {0}", message);
            }
        }
    }
    
    class Program
    {
        static async Task Main(string[] args)
        {
            var url = "http://localhost:9411";
            var tracer = new ZipkinTracer(new ZipkinTracerOptions(url, "AaronsApp", debug:true){ ScopeManager = new ActorScopeManager()});
            //GlobalTracer.Register(tracer);
            var phobosSetup = PhobosSetup.Create(builder => builder.WithTracing(t => t.SetTracer(tracer)))
                .WithSetup(BootstrapSetup.Create().WithActorRefProvider(PhobosProviderSelection.Local));

            var actorSystem = ActorSystem.Create("Zipkin", phobosSetup);

            var actor = actorSystem.ActorOf(Props.Create(() => new MyActor()));
            
            actorSystem.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1), actor, "hit", ActorRefs.NoSender);

            await actorSystem.WhenTerminated;
        }
    }
}
