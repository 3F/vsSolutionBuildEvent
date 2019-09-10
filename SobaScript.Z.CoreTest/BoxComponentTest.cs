using System;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Varhead;
using net.r_eg.Varhead.Exceptions;
using SobaScript.Z.CoreTest.Stubs;
using Xunit;

namespace SobaScript.Z.CoreTest
{
    public class BoxComponentTest
    {
        [Fact]
        public void ParseTest1()
        {
            var target = SobaAcs.MakeWithBoxComponent();

            Assert.Throws<SubtypeNotFoundException>(() =>
                target.Eval(@"#[Box notrealnode]")
            );
        }

        [Fact]
        public void RepeatTest1()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithBoxComponent(uvar);

            Assert.Equal
            (
                "ab!ab!ab!ab!", 
                _NoSpaces(
                    @"
                      #[$(i = 0)]#[Box repeat($(i) < 4): $(i = $([MSBuild]::Add($(i), 1)))
                          ab!
                      ]", 
                    target
                )
            );

            Assert.Single(uvar.Variables);
            Assert.Equal("4", uvar.GetValue("i", null));
        }

        [Fact]
        public void RepeatTest2()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithBoxComponent(uvar);

            Assert.Equal
            (
                "ab!ab!ab!", 
                _NoSpaces(
                    @"
                    #[$(i = 2)]#[Box repeat($(i) < 8): $(i = $([MSBuild]::Add($(i), 2)))
                        ab!
                        #[Box operators.sleep(50)]
                    ]", 
                    target
                )
            );

            Assert.Single(uvar.Variables);
            Assert.Equal("8", uvar.GetValue("i", null));
        }

        [Fact]
        public void RepeatTest3()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithBoxComponent(uvar);

            Assert.Equal
            (
                string.Empty, 
                _NoSpaces(@"
                    #[$(i = 2)]#[Box repeat($(i) < 8; true): $(i = $([MSBuild]::Add($(i), 1)))
                        ab!
                    ]", 
                    target
                )
            );

            Assert.Single(uvar.Variables);
            Assert.Equal("8", uvar.GetValue("i", null));
        }

        [Fact]
        public void RepeatTest4()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithBoxComponent(uvar);

            Assert.Equal
            (
                "ab!ab!ab!ab!", 
                _NoSpaces(@"
                    #[$(i = 0)]#[Box repeat($(i) < 4; false): $(i = $([MSBuild]::Add($(i), 1)))
                        ab!
                    ]", 
                    target
                )
            );

            Assert.Single(uvar.Variables);
            Assert.Equal("4", uvar.GetValue("i", null));
        }

        [Fact]
        public void RepeatTest5()
        {
            var target = SobaAcs.MakeWithBoxComponent();

            Assert.Throws<PMArgException>(() =>
                _NoSpaces(@"#[Box repeat(false; false; true): ]", target)
            );

            Assert.Throws<PMArgException>(() =>
                _NoSpaces(@"#[Box repeat(false; 123): ]", target) // int type instead of bool
            );
        }

        [Fact]
        public void RepeatTest6()
        {
            var target = SobaAcs.MakeWithBoxComponent();

            Assert.Throws<PMArgException>(() =>
                _NoSpaces(@"#[Box repeat( ): ]", target)
            );

            Assert.Throws<IncorrectNodeException>(() =>
                _NoSpaces(@"#[Box repeat: ]", target)
            );
        }


        [Fact]
        public void IterateTest1()
        {
            var target = SobaAcs.MakeWithBoxComponent();

            Assert.Throws<IncorrectNodeException>(() =>
                _NoSpaces(@"#[Box iterate: ]", target)
            );
        }

        [Fact]
        public void IterateTest2()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithBoxComponent(uvar);

            Assert.Equal
            (
                "ab!ab!ab!ab!", 
                _NoSpaces(@"
                    #[$(i = 0)]#[Box iterate(;$(i) < 4; ): $(i = $([MSBuild]::Add($(i), 1)))
                        ab!
                    ]", 
                    target
                )
            );

            Assert.Single(uvar.Variables);
            Assert.Equal("4", uvar.GetValue("i", null));
        }

        [Fact]
        public void IterateTest3()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithBoxComponent(uvar);

            Assert.Equal
            (
                "ab!ab!ab!", 
                _NoSpaces(@"
                    #[$(i = -2)]#[Box iterate(;$(i) < 4; i = $([MSBuild]::Add($(i), 2))): 
                        ab!
                    ]", 
                    target
                )
            );

            Assert.Single(uvar.Variables);
            Assert.Equal("4", uvar.GetValue("i", null));
        }

        [Fact]
        public void IterateTest4()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithBoxComponent(uvar);

            Assert.Equal
            (
                "ab!ab!ab!", 
                _NoSpaces(@"
                    #[Box iterate(i = 1; $(i) < 4; i = $([MSBuild]::Add($(i), 1))): 
                        ab!
                    ]", 
                    target
                )
            );

            Assert.Single(target.UVars.Variables);
            Assert.Equal("4", target.UVars.GetValue("i", null));
        }

        [Fact]
        public void IterateTest5()
        {
            var target = SobaAcs.MakeWithBoxComponent();

            Assert.Throws<PMArgException>(() =>
                target.Eval(@"#[Box iterate(i = 1; $(i) < 4; i = $([MSBuild]::Add($(i), 1)); ): ]")
            );

            Assert.Throws<PMArgException>(() =>
                target.Eval(@"#[Box iterate(; i = 1; $(i) < 4; i = $([MSBuild]::Add($(i), 1)) ): ]")
            );
        }

        [Fact]
        public void OpSleepTest1()
        {
            var target = SobaAcs.MakeWithBoxComponent();

            Assert.Throws<ArgumentException>(() =>
                target.Eval(@"#[Box operators.sleep()]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval(@"#[Box operators.notrealProperty]")
            );
        }

        [Fact]
        public void OpSleepTest2()
        {
            var target = SobaAcs.MakeWithBoxComponent();

            var start = DateTime.Now;
            target.Eval(@"#[Box operators.sleep(1000)]");
            var end = DateTime.Now;

            Assert.True((end - start).TotalMilliseconds >= 1000);
        }

        [Fact]
        public void StDataTest1()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithBoxComponent(uvar);

            uvar.SetVariable("p1", null, "v1");
            uvar.Evaluate("p1", null, new EvaluatorBlank(), true);

            Assert.Equal(string.Empty, target.Eval("#[Box data.pack(\"test1\", false): $(p1)#[Box operators.sleep(10)] ]"));
            Assert.Equal(string.Empty, target.Eval("#[Box data.pack(\"test2\", true): $(p1) #[Box operators.sleep(10)] ]"));

            Assert.Equal("$(p1)#[Box operators.sleep(10)]", target.Eval("#[Box data.get(\"test1\", false)]").Trim());
            Assert.Equal("v1", _NoSpaces("#[Box data.get(\"test1\", true)]", target));

            Assert.Equal("v1", _NoSpaces("#[Box data.get(\"test2\", false)]", target));
            Assert.Equal("v1", _NoSpaces("#[Box data.get(\"test2\", true)]", target));
        }

        [Fact]
        public void StDataTest2()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithBoxComponent(uvar);

            target.Eval("#[Box data.pack(\"test1\", false): 123]");

            Assert.Throws<LimitException>(() =>
                target.Eval("#[Box data.pack(\"test1\", true): 123]")
            );
        }

        [Fact]
        public void StDataTest3()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithBoxComponent(uvar);

            Assert.Throws<NotFoundException>(() =>
                target.Eval("#[Box data.get(\"notexists\", false)]")
            );

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("#[Box data.get(\"test1\", false): 123]")
            );
        }

        [Fact]
        public void StDataTest5()
        {
            var uvar = new UVars();
            var target = SobaAcs.MakeWithBoxComponent(uvar);

            uvar.SetVariable("p1", null, "ab!");
            uvar.Evaluate("p1", null, new EvaluatorBlank(), true);

            Assert.Equal(string.Empty, target.Eval("#[Box data.pack(\"test1\", false): $(p1) ]"));
            Assert.Equal(string.Empty, target.Eval("#[Box data.pack(\"test2\", true): $(p1) ]"));

            Assert.Equal("$(p1)$(p1)$(p1)$(p1)", _NoSpaces("#[Box data.clone(\"test1\", 4)]", target));
            Assert.Equal("$(p1)$(p1)$(p1)$(p1)", _NoSpaces("#[Box data.clone(\"test1\", 4, false)]", target));
            Assert.Equal("ab!ab!ab!ab!", _NoSpaces("#[Box data.clone(\"test1\", 4, true)]", target));

            Assert.Equal("ab!ab!", _NoSpaces("#[Box data.clone(\"test2\", 2)]", target));
            Assert.Equal("ab!ab!", _NoSpaces("#[Box data.clone(\"test2\", 2, false)]", target));
            Assert.Equal("ab!ab!", _NoSpaces("#[Box data.clone(\"test2\", 2, true)]", target));

            Assert.Equal(string.Empty, _NoSpaces("#[Box data.clone(\"test2\", 0)]", target));
        }

        [Fact]
        public void StDataTest6()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithBoxComponent(uvar);

            Assert.Throws<NotFoundException>(() =>
                target.Eval("#[Box data.clone(\"notexists\", 4)]")
            );
        }

        [Fact]
        public void StDataTest7()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithBoxComponent(uvar);
            
            Assert.Equal(string.Empty, target.Eval("#[Box data.pack(\"test1\", false): 123 ]"));
            Assert.Equal(string.Empty, target.Eval("#[Box data.free(\"test1\")]"));

            Assert.Throws<NotFoundException>(() =>
                target.Eval("#[Box data.get(\"test1\", false)]")
            );
        }

        [Fact]
        public void StDataTest8()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithBoxComponent(uvar);

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("#[Box data.free(\"test1\"): 123]")
            );
        }

        [Fact]
        public void StDataTest9()
        {
            var uvar = new UVars();
            var target = SobaAcs.MakeWithBoxComponent(uvar);

            Assert.Equal(string.Empty, target.Eval("#[Box data.free(\"test1\")]"));
            Assert.Equal(string.Empty, target.Eval("#[Box data.free(\"test2\")]"));
        }

        private string _NoSpaces(string raw, ISobaScript script)
        {
            return script.Eval(raw).Replace("\r", "").Replace("\n", "").Replace(" ", "");
        }
    }
}
