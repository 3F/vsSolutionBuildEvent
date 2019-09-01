using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.SNode;
using Xunit;

namespace SobaScriptTest
{
    public class PMDetectTest
    {
        [Fact]
        public void DetectTest1()
        {
            IPM pm = new PM("solution.path(\"sln file\").Last");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.Method, pm.Levels[1].Type);
            Assert.Equal("path", pm.Levels[1].Data);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[1].Args[0].type);
            Assert.Equal("sln file", pm.Levels[1].Args[0].data);

            Assert.Equal(LevelType.Property, pm.Levels[2].Type);
            Assert.Equal("Last", pm.Levels[2].Data);

            Assert.Equal(LevelType.RightOperandEmpty, pm.Levels[3].Type);
        }

        [Fact]
        public void DetectTest2()
        {
            IPM pm = new PM("solution.current.FirstBy(true, 0, \"raw\")");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.Property, pm.Levels[1].Type);
            Assert.Equal("current", pm.Levels[1].Data);

            Assert.Equal(LevelType.Method, pm.Levels[2].Type);
            Assert.Equal("FirstBy", pm.Levels[2].Data);
            Assert.Equal(ArgumentType.Boolean, pm.Levels[2].Args[0].type);
            Assert.Equal(true, pm.Levels[2].Args[0].data);
            Assert.Equal(ArgumentType.Integer, pm.Levels[2].Args[1].type);
            Assert.Equal(0, pm.Levels[2].Args[1].data);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[2].Args[2].type);
            Assert.Equal("raw", pm.Levels[2].Args[2].data);

            Assert.Equal(LevelType.RightOperandEmpty, pm.Levels[3].Type);
        }

        [Fact]
        public void DetectTest3()
        {
            IPM pm = new PM(".solution");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.RightOperandEmpty, pm.Levels[1].Type);
        }

        [Fact]
        public void DetectTest4()
        {
            Assert.Throws<IncorrectSyntaxException>(() => new PM(".solution."));
        }

        [Fact]
        public void DetectTest5()
        {
            IPM pm = new PM(". solution . data = true");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);
            Assert.Equal(LevelType.Property, pm.Levels[1].Type);
            Assert.Equal("data", pm.Levels[1].Data);

            Assert.Equal(LevelType.RightOperandStd, pm.Levels[2].Type);
            Assert.Equal(" true", pm.Levels[2].Data);
        }

        [Fact]
        public void DetectTest6()
        {
            IPM pm = new PM("solution.data : 123 ");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);
            Assert.Equal(LevelType.Property, pm.Levels[1].Type);
            Assert.Equal("data", pm.Levels[1].Data);

            Assert.Equal(LevelType.RightOperandColon, pm.Levels[2].Type);
            Assert.Equal(" 123 ", pm.Levels[2].Data);
        }

        [Fact]
        public void DetectTest7()
        {
            IPM pm = new PM("solution = ");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.RightOperandStd, pm.Levels[1].Type);
            Assert.Equal(" ", pm.Levels[1].Data);

            pm = new PM("solution =");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.RightOperandStd, pm.Levels[1].Type);
            Assert.Equal("", pm.Levels[1].Data);
        }

        [Fact]
        public void DetectTest8()
        {
            IPM pm = new PM("solution() : ");

            Assert.Equal(LevelType.Method, pm.Levels[0].Type);
            Assert.Empty(pm.Levels[0].Args);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.RightOperandColon, pm.Levels[1].Type);
            Assert.Equal(" ", pm.Levels[1].Data);

            pm = new PM("solution(123) :456");

            Assert.Equal(LevelType.Method, pm.Levels[0].Type);
            Assert.Equal(ArgumentType.Integer, pm.Levels[0].Args[0].type);
            Assert.Equal(123, pm.Levels[0].Args[0].data);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.RightOperandColon, pm.Levels[1].Type);
            Assert.Equal("456", pm.Levels[1].Data);
        }

        [Fact]
        public void DetectTest9()
        {
            IPM pm = new PM("left.solution(\" test , args\", 123, true, ' n1 , n2 ').right");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("left", pm.Levels[0].Data);

            Assert.Equal(LevelType.Method, pm.Levels[1].Type);
            Assert.Equal(4, pm.Levels[1].Args.Length);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[1].Args[0].type);
            Assert.Equal(" test , args", pm.Levels[1].Args[0].data);
            Assert.Equal(ArgumentType.Integer, pm.Levels[1].Args[1].type);
            Assert.Equal(123, pm.Levels[1].Args[1].data);
            Assert.Equal(ArgumentType.Boolean, pm.Levels[1].Args[2].type);
            Assert.Equal(true, pm.Levels[1].Args[2].data);
            Assert.Equal(ArgumentType.StringSingle, pm.Levels[1].Args[3].type);
            Assert.Equal(" n1 , n2 ", pm.Levels[1].Args[3].data);
            Assert.Equal("solution", pm.Levels[1].Data);

            Assert.Equal(LevelType.Property, pm.Levels[2].Type);
            Assert.Equal("right", pm.Levels[2].Data);

            Assert.Equal(LevelType.RightOperandEmpty, pm.Levels[3].Type);
        }

        [Fact]
        public void DetectTest10()
        {
            Assert.Throws<IncorrectSyntaxException>(() => new PM("solution(123, ).right"));
            Assert.Throws<IncorrectSyntaxException>(() => new PM("solution(, 123).right"));
        }

        [Fact]
        public void DetectTest11()
        {
            IPM pm = new PM("solution=left.right");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.RightOperandStd, pm.Levels[1].Type);
            Assert.Equal("left.right", pm.Levels[1].Data);
        }

        [Fact]
        public void DetectTest12()
        {
            IPM pm = new PM("solution.path(\"D:/app/name.sln\").projectBy(\"4262A1DC\")");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.Method, pm.Levels[1].Type);
            Assert.Equal("path", pm.Levels[1].Data);
            Assert.Single(pm.Levels[1].Args);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[1].Args[0].type);
            Assert.Equal("D:/app/name.sln", pm.Levels[1].Args[0].data);

            Assert.Equal(LevelType.Method, pm.Levels[2].Type);
            Assert.Equal("projectBy", pm.Levels[2].Data);
            Assert.Single(pm.Levels[2].Args);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[2].Args[0].type);
            Assert.Equal("4262A1DC", pm.Levels[2].Args[0].data);
        }

        [Fact]
        public void DetectTest13()
        {
            IPM pm = new PM("solution.path(\"D:/app/name.sln\", \"test\").projectBy(\"4262A1DC\")");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.Method, pm.Levels[1].Type);
            Assert.Equal("path", pm.Levels[1].Data);
            Assert.Equal(2, pm.Levels[1].Args.Length);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[1].Args[0].type);
            Assert.Equal("D:/app/name.sln", pm.Levels[1].Args[0].data);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[1].Args[1].type);
            Assert.Equal("test", pm.Levels[1].Args[1].data);

            Assert.Equal(LevelType.Method, pm.Levels[2].Type);
            Assert.Equal("projectBy", pm.Levels[2].Data);
            Assert.Single(pm.Levels[2].Args);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[2].Args[0].type);
            Assert.Equal("4262A1DC", pm.Levels[2].Args[0].data);
        }

        [Fact]
        public void DetectTest14()
        {
            IPM pm = new PM("left.solution(\" test , (args)\", 123, true, ' (n1) , n2) ').right");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("left", pm.Levels[0].Data);

            Assert.Equal(LevelType.Method, pm.Levels[1].Type);
            Assert.Equal(4, pm.Levels[1].Args.Length);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[1].Args[0].type);
            Assert.Equal(" test , (args)", pm.Levels[1].Args[0].data);
            Assert.Equal(ArgumentType.Integer, pm.Levels[1].Args[1].type);
            Assert.Equal(123, pm.Levels[1].Args[1].data);
            Assert.Equal(ArgumentType.Boolean, pm.Levels[1].Args[2].type);
            Assert.Equal(true, pm.Levels[1].Args[2].data);
            Assert.Equal(ArgumentType.StringSingle, pm.Levels[1].Args[3].type);
            Assert.Equal(" (n1) , n2) ", pm.Levels[1].Args[3].data);
            Assert.Equal("solution", pm.Levels[1].Data);

            Assert.Equal(LevelType.Property, pm.Levels[2].Type);
            Assert.Equal("right", pm.Levels[2].Data);

            Assert.Equal(LevelType.RightOperandEmpty, pm.Levels[3].Type);
        }

        [Fact]
        public void DetectTest15()
        {
            IPM pm = new PM("solution.path(\"D:/app/(name)).sln\", \"test\").projectBy(\"426(2)A1DC\") : \"test 123\", 'true'");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.Method, pm.Levels[1].Type);
            Assert.Equal("path", pm.Levels[1].Data);
            Assert.Equal(2, pm.Levels[1].Args.Length);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[1].Args[0].type);
            Assert.Equal("D:/app/(name)).sln", pm.Levels[1].Args[0].data);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[1].Args[1].type);
            Assert.Equal("test", pm.Levels[1].Args[1].data);

            Assert.Equal(LevelType.Method, pm.Levels[2].Type);
            Assert.Equal("projectBy", pm.Levels[2].Data);
            Assert.Single(pm.Levels[2].Args);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[2].Args[0].type);
            Assert.Equal("426(2)A1DC", pm.Levels[2].Args[0].data);

            Assert.Equal(LevelType.RightOperandColon, pm.Levels[3].Type);
            Assert.Equal(" \"test 123\", 'true'", pm.Levels[3].Data);
        }

        [Fact]
        public void DetectTest16()
        {
            IPM pm = new PM("solution = \"te()st 123\"), 'true'");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.RightOperandStd, pm.Levels[1].Type);
            Assert.Equal(" \"te()st 123\"), 'true'", pm.Levels[1].Data);

            pm = new PM("solution : \"te()st 123\"), 'true'");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.RightOperandColon, pm.Levels[1].Type);
            Assert.Equal(" \"te()st 123\"), 'true'", pm.Levels[1].Data);
        }

        [Fact]
        public void DetectTest17()
        {
            IPM pm = new PM("solution.path(\"\\\"D:/app/(name)).sln\\\"\", \"\\\"test\\\"\", '\\\"12 34\\\"').projectBy(\"\\\"426(2)A1DC\\\"\") : \"\\\"test 123\\\"\", '\\\"true\\\"'");

            Assert.Equal(LevelType.Property, pm.Levels[0].Type);
            Assert.Equal("solution", pm.Levels[0].Data);

            Assert.Equal(LevelType.Method, pm.Levels[1].Type);
            Assert.Equal("path", pm.Levels[1].Data);
            Assert.Equal(3, pm.Levels[1].Args.Length);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[1].Args[0].type);
            Assert.Equal("\"D:/app/(name)).sln\"", pm.Levels[1].Args[0].data);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[1].Args[1].type);
            Assert.Equal("\"test\"", pm.Levels[1].Args[1].data);
            Assert.Equal(ArgumentType.StringSingle, pm.Levels[1].Args[2].type);
            Assert.Equal("\\\"12 34\\\"", pm.Levels[1].Args[2].data);

            Assert.Equal(LevelType.Method, pm.Levels[2].Type);
            Assert.Equal("projectBy", pm.Levels[2].Data);
            Assert.Single(pm.Levels[2].Args);
            Assert.Equal(ArgumentType.StringDouble, pm.Levels[2].Args[0].type);
            Assert.Equal("\"426(2)A1DC\"", pm.Levels[2].Args[0].data);

            Assert.Equal(LevelType.RightOperandColon, pm.Levels[3].Type);
            Assert.Equal(" \"\\\"test 123\\\"\", '\\\"true\\\"'", pm.Levels[3].Data);
        }
    }
}
