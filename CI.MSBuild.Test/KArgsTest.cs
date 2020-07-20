using System;
using Xunit;
using System.Linq;
using net.r_eg.vsSBE.CI.MSBuild;
using System.Collections.Generic;

namespace CI.MSBuild.Test
{
    public class KArgsTest
    {
        [Fact]
        public void ThisTest1()
        {
            var raw = @"Conari.sln /p:nowarn=1702;NU5119;1591;1701 /l:CI.MSBuild.dll;lib=D:\bin\;cultureUI=en-US /v:m /t:Rebuild;Pub1 /p:Configuration=Debug;Platform=x64 /m:7";

            var kargs = new KArgs(raw.Split(' '));

            Assert.Equal(3, kargs[KArgType.CIM].Count);

            Assert.True(kargs[KArgType.CIM].ContainsKey("CI.MSBuild.dll"));
            Assert.Null(kargs[KArgType.CIM]["CI.MSBuild.dll"]);
            Assert.Equal(@"D:\bin\", kargs[KArgType.CIM]["lib"]);
            Assert.Equal("en-US", kargs[KArgType.CIM]["cultureUI"]);

            Assert.Equal(3, kargs[KArgType.Common].Count);

            Assert.True(kargs[KArgType.Common].ContainsKey("Conari.sln"));
            Assert.Null(kargs[KArgType.Common]["Conari.sln"]);
            Assert.True(kargs[KArgType.Common].ContainsKey("/v:m"));
            Assert.Null(kargs[KArgType.Common]["/v:m"]);
            Assert.True(kargs[KArgType.Common].ContainsKey("/m:7"));
            Assert.Null(kargs[KArgType.Common]["/m:7"]);

            Assert.Equal(3, kargs[KArgType.Properties].Count);

            Assert.True(kargs[KArgType.Properties].ContainsKey("nowarn"));
            Assert.Equal("1702;NU5119;1591;1701", kargs[KArgType.Properties]["nowarn"]);
            Assert.Equal("Debug", kargs[KArgType.Properties]["Configuration"]);
            Assert.Equal("x64", kargs[KArgType.Properties]["Platform"]);

            Assert.Equal(1, kargs[KArgType.Targets].Count);

            Assert.True(kargs[KArgType.Targets].ContainsKey("Rebuild;Pub1"));
            Assert.Null(kargs[KArgType.Targets]["Rebuild;Pub1"]);
        }

        [Fact]
        public void ThisTest2()
        {
            var raw = @"Conari.sln -property:hello=world -p:nowarn=1702;NU5119;1591;1701 /l:CI.MSBuild.dll /logger:cultureUI=en-US /v:m /t:Rebuild;Pub1 /property:Configuration=Debug /p:Platform=x64 /m:7";

            var kargs = new KArgs(raw.Split(' '));

            Assert.Equal(4, kargs[KArgType.Properties].Count);

            Assert.Equal("world", kargs[KArgType.Properties]["hello"]);
            Assert.True(kargs[KArgType.Properties].ContainsKey("nowarn"));
            Assert.Equal("1702;NU5119;1591;1701", kargs[KArgType.Properties]["nowarn"]);
            Assert.Equal("Debug", kargs[KArgType.Properties]["Configuration"]);
            Assert.Equal("x64", kargs[KArgType.Properties]["Platform"]);

            Assert.Equal(2, kargs[KArgType.CIM].Count);

            Assert.True(kargs[KArgType.CIM].ContainsKey("CI.MSBuild.dll"));
            Assert.Null(kargs[KArgType.CIM]["CI.MSBuild.dll"]);
            Assert.Equal("en-US", kargs[KArgType.CIM]["cultureUI"]);

            Assert.Null(kargs[(KArgType)int.MaxValue]);
            Assert.Equal(KArgType.CIM, KArgType.Loggers);
        }

        [Fact]
        public void GetKeysTest1()
        {
            var raw = @"Conari.sln /p:nowarn=1702;NU5119;1591;1701 /l:CI.MSBuild.dll;lib=D:\bin\;cultureUI=en-US /v:m /t:Rebuild;Pub1 /p:Configuration=Debug;Platform=x64 /m:7";

            var kargs = new KArgs(raw.Split(' '));

            Assert.Equal(3, kargs.GetKeys(KArgType.CIM, null).Count());
            Assert.Equal("CI.MSBuild.dll", kargs.GetKeys(KArgType.CIM, null).ElementAt(0));
            Assert.Equal("lib", kargs.GetKeys(KArgType.CIM, null).ElementAt(1));
            Assert.Equal("cultureUI", kargs.GetKeys(KArgType.CIM, null).ElementAt(2));

            Assert.Equal(2, kargs.GetKeys(KArgType.CIM, true).Count());
            Assert.Equal("lib", kargs.GetKeys(KArgType.CIM, true).ElementAt(0));
            Assert.Equal("cultureUI", kargs.GetKeys(KArgType.CIM, true).ElementAt(1));

            Assert.Single(kargs.GetKeys(KArgType.CIM, false));
            Assert.Equal("CI.MSBuild.dll", kargs.GetKeys(KArgType.CIM, false).ElementAt(0));
        }

        [Fact]
        public void GetKeysTest2()
        {
            var raw = @"Conari.sln /p:nowarn=1702;NU5119;1591;1701 /l:CI.MSBuild.dll;lib=D:\bin\;cultureUI=en-US /v:m /t:Rebuild;Pub1 /p:Configuration=Debug;Platform=x64 /m:7";

            var kargs = new KArgs(raw.Split(' '));

            Assert.Equal(3, kargs.GetKeys(KArgType.Common, null).Count());
            Assert.Equal("Conari.sln", kargs.GetKeys(KArgType.Common, null).ElementAt(0));
            Assert.Equal("/v:m", kargs.GetKeys(KArgType.Common, null).ElementAt(1));
            Assert.Equal("/m:7", kargs.GetKeys(KArgType.Common, null).ElementAt(2));

            Assert.Empty(kargs.GetKeys(KArgType.Common, true));

            Assert.Equal(3, kargs.GetKeys(KArgType.Common, false).Count());
            Assert.Equal("Conari.sln", kargs.GetKeys(KArgType.Common, false).ElementAt(0));
            Assert.Equal("/v:m", kargs.GetKeys(KArgType.Common, false).ElementAt(1));
            Assert.Equal("/m:7", kargs.GetKeys(KArgType.Common, false).ElementAt(2));
        }

        [Fact]
        public void GetKeysTest3()
        {
            var raw = @"Conari.sln /p:nowarn=1702;NU5119;1591;1701 /l:CI.MSBuild.dll;lib=D:\bin\;cultureUI=en-US /v:m /t:Rebuild;Pub1 /p:Configuration=Debug;Platform=x64 /m:7";

            var kargs = new KArgs(raw.Split(' '));

            Assert.Equal(3, kargs.GetKeys(KArgType.Properties, null).Count());
            Assert.Equal("nowarn", kargs.GetKeys(KArgType.Properties, null).ElementAt(0));
            Assert.Equal("Configuration", kargs.GetKeys(KArgType.Properties, null).ElementAt(1));
            Assert.Equal("Platform", kargs.GetKeys(KArgType.Properties, null).ElementAt(2));

            Assert.Empty(kargs.GetKeys(KArgType.Properties, false));

            Assert.Equal(3, kargs.GetKeys(KArgType.Properties, true).Count());
            Assert.Equal("nowarn", kargs.GetKeys(KArgType.Properties, true).ElementAt(0));
            Assert.Equal("Configuration", kargs.GetKeys(KArgType.Properties, true).ElementAt(1));
            Assert.Equal("Platform", kargs.GetKeys(KArgType.Properties, true).ElementAt(2));
        }

        [Fact]
        public void GetKeysTest4()
        {
            var raw = @"Conari.sln /p:nowarn=1702;NU5119;1591;1701 /l:CI.MSBuild.dll;lib=D:\bin\;cultureUI=en-US /v:m /t:Rebuild;Pub1 /p:Configuration=Debug;Platform=x64 /m:7";

            var kargs = new KArgs(raw.Split(' '));

            Assert.Single(kargs.GetKeys(KArgType.Targets, null));
            Assert.Equal("Rebuild;Pub1", kargs.GetKeys(KArgType.Targets, null).ElementAt(0));

            Assert.Empty(kargs.GetKeys(KArgType.Targets, true));

            Assert.Single(kargs.GetKeys(KArgType.Targets, false));
            Assert.Equal("Rebuild;Pub1", kargs.GetKeys(KArgType.Targets, false).ElementAt(0));

            Assert.Null(kargs.GetKeys((KArgType)int.MaxValue, null));
            Assert.Null(kargs.GetKeys((KArgType)int.MaxValue, true));
            Assert.Null(kargs.GetKeys((KArgType)int.MaxValue, false));
        }

        [Fact]
        public void GetCountTest1()
        {
            var raw = @"Conari.sln /p:nowarn=1702;NU5119;1591;1701 /l:CI.MSBuild.dll;lib=D:\bin\;cultureUI=en-US /v:m /t:Rebuild;Pub1 /p:Configuration=Debug;Platform=x64 /m:7";

            var kargs = new KArgs(raw.Split(' '));

            Assert.Equal(kargs.GetCount(KArgType.CIM), kargs[KArgType.CIM].Count);
            Assert.Equal(kargs.GetCount(KArgType.Common), kargs[KArgType.Common].Count);
            Assert.Equal(kargs.GetCount(KArgType.Loggers), kargs[KArgType.Loggers].Count);
            Assert.Equal(kargs.GetCount(KArgType.Properties), kargs[KArgType.Properties].Count);
            Assert.Equal(kargs.GetCount(KArgType.Targets), kargs[KArgType.Targets].Count);

            Assert.Equal(0, kargs.GetCount((KArgType)int.MaxValue));
        }

        [Fact]
        public void ExistsTest1()
        {
            var raw = @"Conari.sln /p:nowarn=1702;NU5119;1591;1701 /v:m /p:Configuration=Debug;Platform=x64 /m:7";

            var kargs = new KArgs(raw.Split(' '));

            Assert.False(kargs.Exists(KArgType.CIM));
            Assert.True(kargs.Exists(KArgType.Common));
            Assert.False(kargs.Exists(KArgType.Loggers));
            Assert.True(kargs.Exists(KArgType.Properties));
            Assert.False(kargs.Exists(KArgType.Targets));

            Assert.False(kargs.Exists((KArgType)int.MaxValue));
        }

        [Fact]
        public void CtorTest1()
        {
            Assert.Throws<ArgumentNullException>(() => new KArgs(null));
        }

        public static IEnumerable<object[]> GetRaws()
        {
            yield return new[] { new string[0] };
            yield return new[] { new[] { string.Empty } };
            yield return new[] { new[] { " " } };
            yield return new[] { new[] { " ", "   " } };

            yield return new[] { new[] { "/p:  " } };
            yield return new[] { new[] { "/t:  " } };
            yield return new[] { new[] { "/p:  ", "/t:  " } };
            yield return new[] { new[] { "/p:=  ", "/t:=  " } };
            yield return new[] { new[] { "/p: =  ", "/t: =  " } };

            yield return new[] { new[] { "  /p:  " } };
            yield return new[] { new[] { "  /t:  " } };
            yield return new[] { new[] { "  /p:  ", " /t:  " } };
            yield return new[] { new[] { "  /p:=  ", " /t:=  " } };
            yield return new[] { new[] { "  /p: =  ", " /t: =  " } };
        }

        [Theory]
        [MemberData(nameof(GetRaws))]
        public void CtorTest2(string[] raw)
        {
            var kargs = new KArgs(raw);

            Assert.Empty(kargs.GetKeys(KArgType.Common));
            Assert.Empty(kargs[KArgType.Common]);

            Assert.Empty(kargs[KArgType.Properties]);
            Assert.Empty(kargs[KArgType.Targets]);
            Assert.Empty(kargs[KArgType.CIM]);
            Assert.Empty(kargs[KArgType.Loggers]);

            Assert.Null(kargs[(KArgType)int.MaxValue]);
        }
    }
}
