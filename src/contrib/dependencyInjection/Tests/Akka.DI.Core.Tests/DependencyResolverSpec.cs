using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akka.Actor;
using Akka.TestKit;
using Xunit;

namespace Akka.DI.Core.Tests
{
    public class DependencyResolverSpec : AkkaSpec
    {

        sealed class TestActorResolver : IDependencyResolver
        {
            private ActorSystem system;

            public TestActorResolver(ActorSystem system)
            {
                if (system == null) throw new ArgumentNullException("system");
                this.system = system;
            }
            public Type GetType(string actorName)
            {
                return typeof(TestActor);
            }

            public Func<ActorBase> CreateActorFactory(Type actorType)
            {
                Func<ActorBase> factory = () =>
                {
                    return (ActorBase)system.ActorOf(Props.Create(actorType));
                };
                return factory;
            }

            public Props Create<TActor>() where TActor : ActorBase
            {
                return system.GetExtension<DIExt>().Props(typeof(TActor));
            }

            public void Release(ActorBase actor)
            {
                actor = null;
            }
        }

        [Fact]
        public void SystemMustHaveARegisteredDIExt()
        {
            Assert.True(false);
        }

    }
}
