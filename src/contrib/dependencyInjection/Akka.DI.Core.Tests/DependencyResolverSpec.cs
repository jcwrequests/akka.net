using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akka.Actor;
using Akka.TestKit;
using Xunit;
using Xunit.Extensions;

namespace Akka.DI.Core.Tests
{
    public class DependencyResolverSpec : AkkaSpec
    {
        
        private class DITestActor : ActorBase
        {
            protected override bool Receive(object message)
            {
                return true;
            }
        }
        sealed class TestActorResolver : IDependencyResolver
        {
            private ActorSystem system;

            public TestActorResolver(ActorSystem system)
            {
                if (system == null) throw new ArgumentNullException("system");
                this.system = system;
                this.system.AddDependencyResolver(this);
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

        [Fact, Trait("DIExt", "DependencyResolverSpec")]
        public void SystemMustHaveARegisteredDIExt()
        {
            using (var system = ActorSystem.Create("MySystem"))
            {
                var propsResolver =
                    new TestActorResolver(system);

                var extenion = system.GetExtension<DIExt>();
                Assert.NotNull(extenion);

            }
           
        }
       [Fact, Trait("DIExt", "DependencyResolverSpec")]
        public void DependencyResolverCanCreatePropsOfAnActor()
        {
            using (var system = ActorSystem.Create("MySystem"))
            {
                var propsResolver =
                    new TestActorResolver(system);

                Props testProps = propsResolver.Create<DITestActor>();
                Assert.NotNull(testProps);

            }

        }
        [Fact, Trait("DIExt", "DependencyResolverSpec")]
        public void DependencyResolverCanCreateAnActorReference()
        {
            using (var system = ActorSystem.Create("MySystem"))
            {
                var propsResolver =
                    new TestActorResolver(system);

                IActorRef testActorRef = system.ActorOf(propsResolver.Create<DITestActor>());
                Assert.NotNull(testActorRef);

            }
       }
       
    }
}
