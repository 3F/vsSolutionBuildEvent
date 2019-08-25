using System.Linq;
using EvMSBuildTest.Stubs;
using net.r_eg.EvMSBuild.Exceptions;
using Xunit;

namespace EvMSBuildTest
{
    public class VarDataTest
    {
        [Fact]
        public void ParseVarAndDataTest1()
        {
            var target = new ToParse();

            string data = "$(var=$(Path.Replace('\\', '/')):project)";

            Assert.Equal("", target.Eval(data));
            Assert.Null(target.AccessToVariables.GetValue("var"));
            Assert.Equal("[E~Path.Replace('\\', '/')~]", target.AccessToVariables.GetValue("var", "project"));
        }

        [Fact]
        public void ParseVarAndDataTest2()
        {
            var target = new ToParse();

            string data = "$(var = $$(Path:project))";

            Assert.Equal("", target.Eval(data));
            Assert.Equal("$(Path:project)", target.AccessToVariables.GetValue("var"));
        }

        [Fact]
        public void ParseVarAndDataTest3()
        {
            var target = new ToParse();

            string data = "$$(var = $$(Path:project))";
            Assert.Equal("$(var = $$(Path:project))", target.Eval(data));
        }

        [Fact]
        public void ParseVarAndDataTest4()
        {
            var target = new ToParse();

            string data = "$$(var = $(Path:project))";
            Assert.Equal("$(var = $(Path:project))", target.Eval(data));
        }

        [Fact]
        public void ParseVarAndDataTest5()
        {
            var target = new ToParse();

            string data = "$(var = $(Path:project))";

            Assert.Equal("", target.Eval(data));
            Assert.Equal("[P~Path~project]", target.AccessToVariables.GetValue("var"));
        }

        [Fact]
        public void ParseVarAndDataTest6()
        {
            var target = new ToParse();

            string data = "$(var=$(Path:project2):project)";

            Assert.True(target.AccessToVariables.Variables.Count() < 1);
            Assert.Equal("", target.Eval(data));
            Assert.Equal("[P~Path~project2]", target.AccessToVariables.GetValue("var", "project"));
            Assert.True(target.AccessToVariables.Variables.Count() == 1);
        }

        [Fact]
        public void ParseVarAndDataTest7()
        {
            var target = new ToUserVariables();

            target.AccessToVariables.SetVariable("var", "project", "is a Windows_NT"); //"$(var.Replace('%OS%', $(OS)):project)";

            string actual = target.Eval("$(var:project)");
            Assert.Equal("is a Windows_NT", actual);
        }

        [Fact]
        public void ParseEscapeAndVarTest1()
        {
            var target = new ToParse();

            /*
                $(p1 = $(Platform))
                $(p2 = $(p1))
                $(p2)
             */
            string data = "$(p1 = $(Platform))$(p2 = $(p1))$(p2)";
            Assert.Equal("[P~Platform~]", target.Eval(data));
            Assert.Equal("[P~Platform~]", target.AccessToVariables.GetValue("p1"));
            Assert.Equal("[P~Platform~]", target.AccessToVariables.GetValue("p2"));
        }

        [Fact]
        public void ParseEscapeAndVarTest2()
        {
            var target = new ToParse();

            /*
                $(p1 = $(Platform))
                $(p2 = $$(p1))
                $(p2)
             */
            string data = "$(p1 = $(Platform))$(p2 = $$(p1))$(p2)";
            Assert.Equal("[P~Platform~]", target.Eval(data));
            Assert.Equal("[P~Platform~]", target.AccessToVariables.GetValue("p1"));
            Assert.Equal("$(p1)", target.AccessToVariables.GetValue("p2"));
        }

        [Fact]
        public void ParseEscapeAndVarTest3()
        {
            var target = new ToParse();

            /*
                $(p1 = $(Platform))
                $(p2 = $$$(p1))
                $(p2)
             */
            string data = "$(p1 = $(Platform))$(p2 = $$$(p1))$(p2)";
            Assert.Equal("$(p1)", target.Eval(data));
            Assert.Equal("[P~Platform~]", target.AccessToVariables.GetValue("p1"));
            Assert.Equal("$$(p1)", target.AccessToVariables.GetValue("p2"));
        }

        [Fact]
        public void ParseEscapeAndVarTest4()
        {
            var target = new ToParse();

            /*
                $(p1 = $(Platform))
                $(p2 = $$$$(p1))
                $(p2)
             */
            string data = "$(p1 = $(Platform))$(p2 = $$$$(p1))$(p2)";
            Assert.Equal("$$(p1)", target.Eval(data));
            Assert.Equal("[P~Platform~]", target.AccessToVariables.GetValue("p1"));
            Assert.Equal("$$$(p1)", target.AccessToVariables.GetValue("p2"));
        }

        [Fact]
        public void ParseEscapeAndVarTest5()
        {
            var target = new ToParse();

            string data = "$(p2 = $(Platform))";
            Assert.Equal("", target.Eval(data));
            Assert.Equal("[P~Platform~]", target.AccessToVariables.GetValue("p2"));
        }

        [Fact]
        public void ParseEscapeAndVarTest6()
        {
            var target = new ToParse();

            string data = "$(p2 = $$(Platform))";
            Assert.Equal("", target.Eval(data));
            Assert.Equal("$(Platform)", target.AccessToVariables.GetValue("p2"));
        }

        [Fact]
        public void ParseEscapeAndVarTest7()
        {
            var target = new ToParse();

            string data = "$(p2 = $$$(Platform))";
            Assert.Equal("", target.Eval(data));
            Assert.Equal("$$(Platform)", target.AccessToVariables.GetValue("p2"));
        }

        [Fact]
        public void ParseEscapeAndVarTest8()
        {
            var target = new ToParse();

            string data = "$(p2 = $$$$(Platform))";
            Assert.Equal("", target.Eval(data));
            Assert.Equal("$$$(Platform)", target.AccessToVariables.GetValue("p2"));
        }

        [Fact]
        public void ParseEscapeAndVarTest9()
        {
            var target = new ToParse();

            /*
                $(p1 = $(Platform))
                $(p1)
             */
            string data = "$(p1 = $(Platform))$(p1)";
            Assert.Equal("[P~Platform~]", target.Eval(data));
            Assert.Equal("[P~Platform~]", target.AccessToVariables.GetValue("p1"));
        }

        [Fact]
        public void ParseEscapeAndVarTest10()
        {
            var target = new ToParse();

            /*
                $(p1 = $(Platform))
                $$(p1)
             */
            string data = "$(p1 = $(Platform))$$(p1)";
            Assert.Equal("$(p1)", target.Eval(data));
            Assert.Equal("[P~Platform~]", target.AccessToVariables.GetValue("p1"));
        }

        [Fact]
        public void ParseStringTest1()
        {
            var target = new ToParse();

            string data = "$(var = \"$(Path:project)\")";

            Assert.Equal("", target.Eval(data));
            Assert.Equal("[P~Path~project]", target.AccessToVariables.GetValue("var"));
        }

        [Fact]
        public void ParseStringTest2()
        {
            var target = new ToParse();

            string data = "$(var = \" mixed $(Path:project) \" )";

            Assert.Equal("", target.Eval(data));
            Assert.Equal(" mixed [P~Path~project] ", target.AccessToVariables.GetValue("var"));
        }

        [Fact]
        public void ParseStringTest3()
        {
            var target = new ToParse();

            Assert.Equal(string.Empty, target.Eval("$(var = \" $$(Path:project) \")"));
            Assert.Equal(" $(Path:project) ", target.AccessToVariables.GetValue("var"));

            Assert.Equal(string.Empty, target.Eval("$(var = ' $$(Path:project) ')"));
            Assert.Equal(" $$(Path:project) ", target.AccessToVariables.GetValue("var"));
        }

        [Fact]
        public void ParseStringTest4()
        {
            var target = new ToParse();

            string data = "$(var = ' $(Path:project) ')";

            Assert.Equal("", target.Eval(data));
            Assert.Equal(" $(Path:project) ", target.AccessToVariables.GetValue("var"));
        }

        [Fact]
        public void ParseStringTest5()
        {
            var target = new ToParse();

            string data = @"$(var = '$(Path.Replace(\'1\', \'2\'))')";

            Assert.Equal("", target.Eval(data));
            Assert.Equal("$(Path.Replace('1', '2'))", target.AccessToVariables.GetValue("var"));
        }

        [Fact]
        public void ParseStringTest6()
        {
            var target = new ToParse();

            string data = "$(var = '$(Path.Replace(\\\"1\\\", \\\"2\\\"))')";

            Assert.Equal("", target.Eval(data));
            Assert.Equal("$(Path.Replace(\\\"1\\\", \\\"2\\\"))", target.AccessToVariables.GetValue("var"));
        }

        [Fact]
        public void ParseStringTest7()
        {
            var target = new ToParse();

            string data = "$(var = \"$(Path.Replace(\\'1\\', \\'2\\'))\")";

            Assert.Equal("", target.Eval(data));
            Assert.Equal("[E~Path.Replace(\\'1\\', \\'2\\')~]", target.AccessToVariables.GetValue("var"));
        }

        [Fact]
        public void ParseStringTest8()
        {
            var target = new ToParse();

            string data = "$(var = \"$(Path.Replace(\\\"1\\\", \\\"2\\\"))\")";

            Assert.Equal("", target.Eval(data));
            Assert.Equal("[E~Path.Replace(\"1\", \"2\")~]", target.AccessToVariables.GetValue("var"));
        }

        [Fact]
        public void ParseStringTest9()
        {
            var target = new ToParse();

            string data = @"$(var = $(Path.Replace(\'1\', '2')))";

            Assert.Equal("", target.Eval(data));
            Assert.Equal(@"[E~Path.Replace(\'1\', '2')~]", target.AccessToVariables.GetValue("var"));
        }

        [Fact]
        public void ParseStringTest10()
        {
            var target = new ToParse();

            string data = "$(var = $(Path.Replace(\\\"1\\\", \"2\")))";

            Assert.Equal("", target.Eval(data));
            Assert.Equal("[E~Path.Replace(\\\"1\\\", \"2\")~]", target.AccessToVariables.GetValue("var"));
        }

        [Fact]
        public void ParseStringTest11()
        {
            var target = new ToParse();
            
            Assert.Equal("", target.Eval("$(name = ' test 12345  -_*~!@#$%^&= :) ')"));
            Assert.Equal(" test 12345  -_*~!@#$%^&= :) ", target.AccessToVariables.GetValue("name"));

            Assert.Equal("", target.Eval("$(name = ' $( -_*~!@#$%^&= :) ')"));
            Assert.Equal(" $( -_*~!@#$%^&= :) ", target.AccessToVariables.GetValue("name"));
        }

        [Fact]
        public void ParseStringTest12()
        {
            var target = new ToParse();

            Assert.Equal(string.Empty, target.Eval("$(name = 'left\\'right')"));
            Assert.Equal("left'right", target.AccessToVariables.GetValue("name"));

            Assert.Equal(string.Empty, target.Eval("$(name = \"left\\'right\")"));
            Assert.Equal("left\\'right", target.AccessToVariables.GetValue("name"));

            Assert.Equal(string.Empty, target.Eval("$(name = 'left\\\"right')"));
            Assert.Equal("left\\\"right", target.AccessToVariables.GetValue("name"));

            Assert.Equal(string.Empty, target.Eval("$(name = \"left\\\"right\")"));
            Assert.Equal("left\"right", target.AccessToVariables.GetValue("name"));
        }

        [Fact]
        public void ParseStringTest13()
        {
            var target = new ToParse();

            Assert.Throws<IncorrectSyntaxException>(() => target.Eval("$(name = 'left'right')"));
            Assert.Throws<IncorrectSyntaxException>(() => target.Eval("$(name = \"left\"right\")"));
        }

        [Fact]
        public void ParseStringTest15()
        {
            var target = new ToParse();
            
            Assert.Equal(string.Empty, target.Eval("$(name   =   \"   left $(Path:project) right  \"   )"));
            Assert.Equal("   left [P~Path~project] right  ", target.AccessToVariables.GetValue("name"));

            Assert.Equal(string.Empty, target.Eval("$(name   =   \"   left \\\"$(Path)\\\" right  \"   )"));
            Assert.Equal("   left \"[P~Path~]\" right  ", target.AccessToVariables.GetValue("name"));

            Assert.Equal(string.Empty, target.Eval("$(name   =   \"   \\'left\\' $(Path:project) 'right'  \"   )"));
            Assert.Equal("   \\'left\\' [P~Path~project] 'right'  ", target.AccessToVariables.GetValue("name"));
        }
    }
}