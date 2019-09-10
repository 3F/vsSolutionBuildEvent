using System;
using System.Linq;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.SobaScript.Z.Core;
using net.r_eg.Varhead;
using Xunit;

namespace SobaScriptTest
{
    public class LoaderTest
    {
        private static IUVars uvars = new UVars();

        [Fact]
        public void RegisterUnregisterTest1()
        {
            var soba = new Soba(new UVars());

            Assert.Empty(soba.Registered);
            Assert.Empty(soba.Components);
            Assert.Empty(new Inspector(soba).Root);

            Assert.True(soba.Register(new TryComponent(soba)));

            Assert.Single(soba.Registered);
            Assert.Single(soba.Components);
            Assert.Single(new Inspector(soba).Root);

            Assert.True(soba.Register(new CommentComponent()));
            Assert.True(soba.Register(new BoxComponent(soba)));

            Assert.Equal(3, soba.Registered.Count());
            Assert.Equal(3, soba.Components.Count());
            Assert.Equal(3, new Inspector(soba).Root.Count());

            Assert.True(soba.Unregister(new CommentComponent()));

            Assert.Equal(2, soba.Registered.Count());
            Assert.Equal(2, soba.Components.Count());
            Assert.Equal(2, new Inspector(soba).Root.Count());

            var c1 = new ConditionComponent(soba);
            Assert.True(soba.Register(c1));
            Assert.True(soba.Register(new UserVariableComponent(soba)));
            Assert.True(soba.Register(new EvMSBuildComponent(soba)));

            Assert.True(soba.Unregister(c1));

            Assert.Equal(4, soba.Registered.Count());
            Assert.Equal(4, soba.Components.Count());
            Assert.Equal(4, new Inspector(soba).Root.Count());

            soba.Unregister();

            Assert.Empty(soba.Registered);
            Assert.Empty(soba.Components);
            Assert.Empty(new Inspector(soba).Root);
        }

        [Fact]
        public void RegisterUnregisterTest2()
        {
            var soba = new Soba(new UVars());

            soba.Register(new TryComponent(soba));
            soba.Register(new CommentComponent());
            soba.Register(new BoxComponent(soba));

            Assert.Throws<ArgumentNullException>(() => soba.Unregister(null));
        }

        [Fact]
        public void RegisterUnregisterTest3()
        {
            var soba = new Soba(new UVars());

            Assert.True(soba.Register(new TryComponent(soba)));
            Assert.True(soba.Register(new EvMSBuildComponent(soba)));
            Assert.True(soba.Register(new BoxComponent(soba)));

            Assert.False(soba.Register(new TryComponent(soba)));
            Assert.False(soba.Register(new EvMSBuildComponent(soba)));
            Assert.False(soba.Register(new BoxComponent(soba)));
        }

        [Fact]
        public void GetComponentTest1()
        {
            var soba = new Soba(new UVars());

            Assert.True(soba.Register(new TryComponent(soba)));
            Assert.True(soba.Register(new EvMSBuildComponent(soba)));
            Assert.True(soba.Register(new BoxComponent(soba)));

            Assert.Equal(typeof(TryComponent), soba.GetComponent<TryComponent>().GetType());
            Assert.Equal(typeof(BoxComponent), soba.GetComponent<BoxComponent>().GetType());

            Assert.Null(soba.GetComponent<UserVariableComponent>());
        }

        [Fact]
        public void GetComponentTest2()
        {
            var soba = new Soba(new UVars());

            Assert.True(soba.Register(new TryComponent(soba)));
            Assert.True(soba.Register(new EvMSBuildComponent(soba)));
            Assert.True(soba.Register(new BoxComponent(soba)));

            Assert.Equal(typeof(TryComponent), soba.GetComponent(typeof(TryComponent)).GetType());
            Assert.Equal(typeof(BoxComponent), soba.GetComponent(typeof(BoxComponent)).GetType());

            Assert.Null(soba.GetComponent(typeof(UserVariableComponent)));
        }
    }
}
