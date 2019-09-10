using EvMSBuildTest.Stubs;
using net.r_eg.EvMSBuild;
using Xunit;

namespace EvMSBuildTest
{
    public class ParserTest
    {
        [Fact]
        public void BasicParseTest1()
        {
            var target = new EvMSBuilderAcs();

            string actual   = target.Eval("FooBar");
            string expected = "FooBar";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BasicParseTest2()
        {
            var target = new ToParse();

            Assert.Equal("[P~Path~]", target.Eval("$(Path)"));
            Assert.Equal("$(Path)", target.Eval("$$(Path)"));
            Assert.Equal("$$(Path)", target.Eval("$$$(Path)"));
            Assert.Equal("(Path)", target.Eval("(Path)"));
        }

        [Fact]
        public void BasicParseTest3()
        {
            var target = new ToParse();

            Assert.Equal("[P~Path~project]", target.Eval("$(Path:project)"));
            Assert.Equal("$(Path:project)", target.Eval("$$(Path:project)"));
        }

        [Fact]
        public void BasicParseTest4()
        {
            var target = new ToParse();

            Assert.Equal
            (
                "[E~[System.DateTime]::UtcNow.Ticks~]", 
                target.Eval("$([System.DateTime]::UtcNow.Ticks)")
            );

            Assert.Equal
            (
                "[E~Path.Replace('\\\\', '/')~]", 
                target.Eval("$(Path.Replace('\\\\', '/'))")
            );
        }

        [Fact]
        public void BasicParseTest5()
        {
            var target = new ToParse();

            Assert.Equal
            (
                "[E~Path.Replace('\\', '/')~]", 
                target.Eval("$(Path.Replace('\\', '/'))")
            );

            Assert.Equal
            (
                "[E~ProjectDir.Replace('\\', '/')~client]", 
                target.Eval("$(ProjectDir.Replace('\\', '/'):client)")
            );
        }

        [Fact]
        public void BasicParseTest6()
        {
            var target = new EvMSBuilderAcs();

            string actual = target.Eval("$$(Path.Replace('\', '/'):project)");
            string expected = "$(Path.Replace('\', '/'):project)";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BasicParseTest7()
        {
            var target = new ToParse();

            Assert.Equal
            (
                "[E~ProjectDir.Replace(\"str1\", \"str2\")~]", 
                target.Eval("$(ProjectDir.Replace(\"str1\", \"str2\"))")
            );

            Assert.Equal
            (
                "[E~ProjectDir.Replace('str1', 'str2')~]",
                target.Eval("$(ProjectDir.Replace('str1', 'str2'))")
            );
        }

        [Fact]
        public void BasicParseTest8()
        {
            string data = "$(var.Method('~str~', $(OS:$($(data):project2)), \"~str2~\"):project)";
            string actual = new ToParse().Eval(data);
            Assert.Equal("[E~var.Method('~str~', [P~OS~[E~[P~data~]~project2]], \"~str2~\")~project]", actual);
        }

        [Fact]
        public void ParseWrappingTest1()
        {
            string data = "$($(ProjectDir.Replace('\\', '/'):client))";
            string actual = new ToParse().Eval(data);
            Assert.Equal("[E~[E~ProjectDir.Replace('\\', '/')~client]~]", actual);
        }

        [Fact]
        public void ParseWrappingTest2()
        {
            string data = "$($(ProjectDir.Replace('\\', '/')))";
            string actual = new ToParse().Eval(data);
            Assert.Equal("[E~[E~ProjectDir.Replace('\\', '/')~]~]", actual);
        }

        [Fact]
        public void ParseWrappingTest3()
        {
            string data = "$($(var.Method('str', $(OS))):$(var.Method('str2', $(SO))))";
            string actual = new ToParse().Eval(data);
            Assert.Equal("[E~[E~var.Method('str', [P~OS~])~]:[E~var.Method('str2', [P~SO~])~]~]", actual);
        }

        [Fact]
        public void ParseWrappingTest4()
        {
            var target = new ToParse();

            string data = "$($(Path:project))";
            string actual = target.Eval(data);
            Assert.True("[E~[P~Path~project]~]" == actual || "[P~[P~Path~project]~]" == actual);
        }

        [Fact]
        public void ParseNestedTest1()
        {
            var target = new ToParse();

            string data = @"$(var.Method('str1', $(OS:$($(data.Replace('\', '/')):project2)), 'str2'):project)";
            Assert.Equal(@"[E~var.Method('str1', [E~OS:[E~[E~data.Replace('\', '/')~]~project2]~], 'str2')~project]", target.Eval(data));
        }

        [Fact]
        public void ParseNestedTest2()
        {
            var target = new ToParse();

            string data = @"$(var.Method('str1', $(OS:$($(data.Method2):project2)), 'str2'):project)";
            Assert.Equal(@"[E~var.Method('str1', [P~OS~[E~[E~data.Method2~]~project2]], 'str2')~project]", target.Eval(data));
        }

        [Fact]
        public void ParseNestedTest3()
        {
            var target = new ToParse();

            string data = "$(var.Method('str1', $(OS:$($([System.DateTime]::Parse(\"27.03.2015\").ToBinary()):project2)), 'str2'):project)";
            Assert.Equal("[E~var.Method('str1', [E~OS:[E~[E~[System.DateTime]::Parse(\"27.03.2015\").ToBinary()~]~project2]~], 'str2')~project]", target.Eval(data));
        }

        [Fact]
        public void ParseNestedTest4()
        {
            var target = new ToParse();

            string data = "$(var.Method('str1', $(OS:$($(test.Ticks):project2)), \\\"str2\\\"):project)";
            Assert.Equal("[E~var.Method('str1', [P~OS~[E~[E~test.Ticks~]~project2]], \\\"str2\\\")~project]", target.Eval(data));
        }

        [Fact]
        public void ParseNestedTest5()
        {
            var target = new ToParse();

            string data     = "$(var.Method('str1', $(OS:$($(data):project2)), \\\"str2\\\"):project)";
            string actual   = target.Eval(data);
            Assert.True("[E~var.Method('str1', [P~OS~[E~[P~data~]~project2]], \\\"str2\\\")~project]" == actual
                            || "[E~var.Method('str1', [P~OS~[P~[P~data~]~project2]], \\\"str2\\\")~project]" == actual);
        }

        [Fact]
        public void ParseNestedTest6()
        {
            var target = new ToParse();

            string data = "$([System.DateTime]::Parse(\"01.01.2000\").ToBinary())";
            Assert.Equal("[E~[System.DateTime]::Parse(\"01.01.2000\").ToBinary()~]", target.Eval(data));
        }

        [Fact]
        public void ParseNestedTest7()
        {
            var target = new ToParse();

            string data = "$([System.DateTime]::Parse(\" left $([System.DateTime]::UtcNow.Ticks) right\").ToBinary())";
            Assert.Equal("[E~[System.DateTime]::Parse(\" left [E~[System.DateTime]::UtcNow.Ticks~] right\").ToBinary()~]", target.Eval(data));
        }

        [Fact]
        public void ParseNestedTest8()
        {
            var target = new ToParse();

            string data = "$([System.DateTime]::Parse(' left $([System.DateTime]::UtcNow.Ticks) right').ToBinary())";
            Assert.Equal("[E~[System.DateTime]::Parse(' left $([System.DateTime]::UtcNow.Ticks) right').ToBinary()~]", target.Eval(data));
        }

        [Fact]
        public void ParseNestedTest9()
        {
            var target = new ToParse();

            string data = "$([System.TimeSpan]::FromTicks($([MSBuild]::Subtract($([System.DateTime]::UtcNow.Ticks), $([System.DateTime]::Parse('27.03.2015').ToBinary())))).TotalMinutes.ToString(\"0\"))";
            Assert.Equal("[E~[System.TimeSpan]::FromTicks([E~[MSBuild]::Subtract([E~[System.DateTime]::UtcNow.Ticks~], [E~[System.DateTime]::Parse('27.03.2015').ToBinary()~])~]).TotalMinutes.ToString(\"0\")~]", target.Eval(data));
        }
    }
}