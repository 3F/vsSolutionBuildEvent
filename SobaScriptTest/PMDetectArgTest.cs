using net.r_eg.SobaScript.SNode;
using Xunit;

namespace SobaScriptTest
{
    public class PMDetectArgTest
    {
        [Fact]
        public void DetectArgumentTest1()
        {
            IPM pm = new PM("solution(\"str data\", 'str data2', 12, -12, 1.5, -1.5, STDOUT, TestEnum.SpecialType, mixed * data, true)");
            
            Assert.True(pm.Is(LevelType.Method, "solution"));
            Assert.Equal(10, pm.Levels[0].Args.Length);

            var args = pm.Levels[0].Args;
            Assert.Equal(ArgumentType.StringDouble, args[0].type);
            Assert.Equal("str data", args[0].data);

            Assert.Equal(ArgumentType.StringSingle, args[1].type);
            Assert.Equal("str data2", args[1].data);

            Assert.Equal(ArgumentType.Integer, args[2].type);
            Assert.Equal(12, args[2].data);

            Assert.Equal(ArgumentType.Integer, args[3].type);
            Assert.Equal(-12, args[3].data);

            Assert.Equal(ArgumentType.Double, args[4].type);
            Assert.Equal(1.5, args[4].data);

            Assert.Equal(ArgumentType.Double, args[5].type);
            Assert.Equal(-1.5, args[5].data);

            Assert.Equal(ArgumentType.EnumOrConst, args[6].type);
            Assert.Equal("STDOUT", args[6].data);

            Assert.Equal(ArgumentType.EnumOrConst, args[7].type);
            Assert.Equal("TestEnum.SpecialType", args[7].data);

            Assert.Equal(ArgumentType.Mixed, args[8].type);
            Assert.Equal("mixed * data", args[8].data);

            Assert.Equal(ArgumentType.Boolean, args[9].type);
            Assert.Equal(true, args[9].data);
        }

        [Fact]
        public void DetectArgumentTest2()
        {
            IPM pm = new PM(" solution (1.5, -1.5, 1.5f, -1.5f, 1.5d, -1.5d) ");
            
            Assert.True(pm.Is(LevelType.Method, "solution"));
            Assert.Equal(6, pm.Levels[0].Args.Length);

            var args = pm.Levels[0].Args;
            Assert.Equal(ArgumentType.Double, args[0].type);
            Assert.Equal(1.5d, args[0].data);

            Assert.Equal(ArgumentType.Double, args[1].type);
            Assert.Equal(args[1].data, -1.5d);

            Assert.Equal(ArgumentType.Float, args[2].type);
            Assert.Equal(1.5f, args[2].data);

            Assert.Equal(ArgumentType.Float, args[3].type);
            Assert.Equal(-1.5f, args[3].data);

            Assert.Equal(ArgumentType.Double, args[4].type);
            Assert.Equal(1.5d, args[4].data);

            Assert.Equal(ArgumentType.Double, args[5].type);
            Assert.Equal(-1.5d, args[5].data);
        }

        [Fact]
        public void DetectArgumentTest3()
        {
            IPM pm = new PM(" m77(\"guid\", 12, {\"p1\", {4, \"test\", 8, 'y'}, true}, {false, 'p2'}) ");

            Assert.True(pm.Is(LevelType.Method, "m77"));

            var args = pm.Levels[0].Args;
            Assert.Equal(4, args.Length);

            Assert.Equal(ArgumentType.StringDouble, args[0].type);
            Assert.Equal("guid", args[0].data);

            Assert.Equal(ArgumentType.Integer, args[1].type);
            Assert.Equal(12, args[1].data);

            Assert.Equal(ArgumentType.Object, args[2].type);
            {
                RArgs args2 = (RArgs)args[2].data;
                Assert.Equal(3, args2.Length);

                Assert.Equal(ArgumentType.StringDouble, args2[0].type);
                Assert.Equal("p1", args2[0].data);

                Assert.Equal(ArgumentType.Object, args2[1].type);
                {
                    RArgs args21 = (RArgs)args2[1].data;
                    Assert.Equal(4, args21.Length);

                    Assert.Equal(ArgumentType.Integer, args21[0].type);
                    Assert.Equal(4, args21[0].data);

                    Assert.Equal(ArgumentType.StringDouble, args21[1].type);
                    Assert.Equal("test", args21[1].data);

                    Assert.Equal(ArgumentType.Integer, args21[2].type);
                    Assert.Equal(8, args21[2].data);

                    Assert.Equal(ArgumentType.Char, args21[3].type);
                    Assert.Equal('y', args21[3].data);
                }

                Assert.Equal(ArgumentType.Boolean, args2[2].type);
                Assert.Equal(true, args2[2].data);
            }

            Assert.Equal(ArgumentType.Object, args[3].type);
            {
                RArgs args3 = (RArgs)args[3].data;
                Assert.Equal(2, args3.Length);

                Assert.Equal(ArgumentType.Boolean, args3[0].type);
                Assert.Equal(false, args3[0].data);

                Assert.Equal(ArgumentType.StringSingle, args3[1].type);
                Assert.Equal("p2", args3[1].data);
            }
        }

        [Fact]
        public void DetectArgumentTest4()
        {
            IPM pm = new PM("solution(\"str \\\" data1 \\\" \", \"str \\' data2 \\' \", 'str \\' data3 \\' ', 'str \\\" data4 \\\" ')");
            
            Assert.True(pm.Is(LevelType.Method, "solution"));
            Assert.Equal(4, pm.Levels[0].Args.Length);

            var args = pm.Levels[0].Args;
            Assert.Equal(ArgumentType.StringDouble, args[0].type);
            Assert.Equal("str \" data1 \" ", args[0].data);

            Assert.Equal(ArgumentType.StringDouble, args[1].type);
            Assert.Equal("str \\' data2 \\' ", args[1].data);

            Assert.Equal(ArgumentType.StringSingle, args[2].type);
            Assert.Equal("str ' data3 ' ", args[2].data);

            Assert.Equal(ArgumentType.StringSingle, args[3].type);
            Assert.Equal("str \\\" data4 \\\" ", args[3].data);
        }
    }
}
