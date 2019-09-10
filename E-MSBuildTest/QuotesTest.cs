using EvMSBuildTest.Stubs;
using net.r_eg.EvMSBuild;
using net.r_eg.Varhead;
using Xunit;

namespace EvMSBuildTest
{
    public class QuotesTest
    {
        [Fact]
        public void QuotesTest1()
        {
            var target = new EvMSBuilderAcs();

            Assert.Equal(string.Empty, target.Eval("$(name = \" $([System.Math]::Pow(2, 16)) \")"));
            Assert.Equal(" 65536 ", target.UVars.GetValue("name", null));

            Assert.Equal(string.Empty, target.Eval("$(name = ' $([System.Math]::Pow(2, 16)) ')"));
            Assert.Equal(" $([System.Math]::Pow(2, 16)) ", target.UVars.GetValue("name", null));
        }

        [Fact]
        public void QuotesTest2()
        {
            var target = new EvMSBuilderAcs();
            Assert.Equal(" left '123' right ", target.Eval("$([System.String]::Format(\" left '{0}' right \", \"123\"))"));
            Assert.Equal(" left '123' ) right ", target.Eval("$([System.String]::Format(\" left '{0}' ) right \", \"123\"))"));

            Assert.Equal(" left \"123\" right ", target.Eval("$([System.String]::Format(' left \"{0}\" right ', '123'))"));
            Assert.Equal(" left \"123\" ) right ", target.Eval("$([System.String]::Format(' left \"{0}\" ) right ', '123'))"));
        }

        [Fact]
        public void QuotesTest3()
        {
            var target = new EvMSBuilderAcs();

            Assert.Equal(string.Empty, target.Eval("$(tpl = \"My version - '%Ver%'\")"));
            Assert.Equal("My version - '%Ver%'", target.UVars.GetValue("tpl", null));

            Assert.Equal(string.Empty, target.Eval("$(ver = '1.2.3')"));
            Assert.Equal("1.2.3", target.UVars.GetValue("ver", null));

            Assert.Equal(string.Empty, target.Eval("$(rev = '2417')"));
            Assert.Equal("2417", target.UVars.GetValue("rev", null));

            Assert.Equal("My version - '1, 2, 3, 2417'", target.Eval("$(tpl.Replace(\"%Ver%\", \"$(ver.Replace('.', ', ')), $(rev)\"))"));
            Assert.Equal("1.2.3 version - '1.2.3.2417'", target.Eval("$(tpl.Replace(\"%Ver%\", \"$(ver).$(rev)\").Replace(\"My\", \"$(ver)\"))"));
        }

        [Fact]
        public void QuotesTest4()
        {
            var target = new EvMSBuilderAcs();

            target.UVars.SetVariable("name", "project", "test123");
            target.UVars.Evaluate("name", "project", new EvaluatorBlank(), true);

            //Assert.Equal("test123", target.parse("$([System.String]::Concat('$(name:project)'))")); //TODO: read note from hquotes
            Assert.Equal("test123", target.Eval("$([System.String]::Concat(\"$(name:project)\"))")); // $([System.DateTime]::Parse(\"$([System.DateTime]::UtcNow.Ticks)\").ToBinary())
        }

        [Fact]
        public void QuotesTest5()
        {
            var target = new EvMSBuilderAcs();

            target.UVars.SetVariable("name", null, "test123");
            target.UVars.Evaluate("name", null, new EvaluatorBlank(), true);

            Assert.Equal("test123", target.Eval("$([System.String]::Concat(\"$(name)\"))"));
            Assert.Equal("test123", target.Eval("$([System.String]::Concat('$(name)'))"));
        }

        [Fact]
        public void QuotesTest6()
        {
            var target = new EvMSBuilderAcs();

            Assert.Equal(string.Empty, target.Eval("$(version = \"1.2.3\")"));
            Assert.Equal(string.Empty, target.Eval("$(tpl = \"My version - $(version), \\\"$(version)\\\", '$(version)' end.\")"));
            Assert.Equal("My version - 1.2.3, \"1.2.3\", '$(version)' end.", target.Eval("$(tpl)"));
        }

        [Fact]
        public void QuotesTest7()
        {
            var uvar    = new UVars();
            var target  = new EvMSBuilder(new EnvStub(), uvar);

            uvar.SetVariable("lp", null, "s1\\dir");
            uvar.Evaluate("lp", null, new EvaluatorBlank(), true);
            Assert.Equal("s1\\dir", uvar.GetValue("lp", null));

            Assert.Equal("\"s1\\dir\\p1.exe\"", target.Eval("\"$(lp)\\p1.exe\""));
            Assert.Equal("'$(lp)\\p2.exe'", target.Eval("'$(lp)\\p2.exe'"));
            Assert.Equal("s1\\dir\\p3.exe", target.Eval("$(lp)\\p3.exe"));
        }

        [Fact]
        public void QuotesTest8()
        {
            var uvar    = new UVars();
            var target  = new EvMSBuilder(new EnvStub(), uvar);

            uvar.SetVariable("lp", null, "'s2\\dir'");
            uvar.Evaluate("lp", null, new EvaluatorBlank(), true);
            Assert.Equal("'s2\\dir'", uvar.GetValue("lp", null));

            Assert.Equal("\"'s2\\dir'\\p1.exe\"", target.Eval("\"$(lp)\\p1.exe\""));
            //Assert.Equal("''s2\\dir'\\p2.exe'", target.parse("'$(lp)\\p2.exe'")); // ? TODO: unspecified for current time
            Assert.Equal("'s2\\dir'\\p3.exe", target.Eval("$(lp)\\p3.exe"));
        }

        [Fact]
        public void QuotesTest9()
        {
            var uvar    = new UVars();
            var target  = new EvMSBuilder(new EnvStub(), uvar);

            uvar.SetVariable("lp", null, "\"s3\\dir\"");
            uvar.Evaluate("lp", null, new EvaluatorBlank(), true);
            Assert.Equal("\"s3\\dir\"", uvar.GetValue("lp", null));

            //Assert.Equal("\"\"s3\\dir\"\\p1.exe\"", target.parse("\"$(lp)\\p1.exe\"")); // ? TODO: unspecified for current time
            Assert.Equal("'$(lp)\\p2.exe'", target.Eval("'$(lp)\\p2.exe'"));
            Assert.Equal("\"s3\\dir\"\\p3.exe", target.Eval("$(lp)\\p3.exe"));
        }

        [Fact]
        public void QuotesTest10()
        {
            var target = new EvMSBuilderAcs();

            target.UVars.SetVariable("name", null, "test123");
            target.UVars.Evaluate("name", null, new EvaluatorBlank(), true);

            Assert.Equal("test123)", target.Eval("$([System.String]::Concat(\"$(name))\"))"));
            Assert.Equal("(test123", target.Eval("$([System.String]::Concat(\"($(name)\"))"));

            Assert.Equal("(test123", target.Eval("$([System.String]::Concat('($(name)'))"));
            Assert.Equal("test123)", target.Eval("$([System.String]::Concat('$(name))'))"));

            Assert.Equal(" left ) test123 ", target.Eval("$([System.String]::Concat(\" left ) $(name) \"))"));
            Assert.Equal(" left ) test123 ", target.Eval("$([System.String]::Concat(' left ) $(name) '))"));

            Assert.Equal(" left () test123 ", target.Eval("$([System.String]::Concat(\" left () $(name) \"))"));
            Assert.Equal(" left () test123 ", target.Eval("$([System.String]::Concat(' left () $(name) '))"));
        }

        [Fact]
        public void QuotesTest11()
        {
            var target = new EvMSBuilderAcs();
            Assert.Equal("simply \"text\" data", target.Eval("simply \"text\" data"));
            Assert.Equal("simply \\\"text\\\" data", target.Eval("simply \\\"text\\\" data"));
            Assert.Equal("simply \\\\\"text\\\\\" data", target.Eval("simply \\\\\"text\\\\\" data"));
            Assert.Equal("simply 'text' data", target.Eval("simply 'text' data"));
            Assert.Equal("simply \'text\' data", target.Eval("simply \'text\' data"));
            Assert.Equal("simply \\'text\\' data", target.Eval("simply \\'text\\' data"));
        }
    }
}