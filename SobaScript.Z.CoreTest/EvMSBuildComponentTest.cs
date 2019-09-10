using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.Core;
using Xunit;

namespace SobaScript.Z.CoreTest
{
    public class EvMSBuildComponentTest
    {
        [Fact]
        public void ParseTest1()
        {
            var target = new EvMSBuildComponent(new Soba());

            Assert.Equal(EvMSBuilder.UNDEF_VAL, target.Eval("[$(notRealVariablename)]"));

            Assert.Equal("65536", target.Eval("[$([System.Math]::Pow(2, 16))]"));
        }

        [Fact]
        public void ParseTest2()
        {
            var target = new EvMSBuildComponent(new Soba());

            Assert.Throws<IncorrectSyntaxException>(() => 
                target.Eval("[$()]")
            );

            Assert.Throws<IncorrectSyntaxException>(() => 
                target.Eval("[$(]")
            );

            Assert.Throws<IncorrectSyntaxException>(() => 
                target.Eval("[$(notRealVariablename]")
            );
        }

        [Fact]
        public void ParseTest3()
        {
            var target = new EvMSBuildComponent(new Soba());

            Assert.Equal(Value.Empty, target.Eval("[$(vParseTest3 = \"string123\")]"));
            Assert.Equal(" left 'string123' ) right ", target.Eval("[$([System.String]::Format(\" left '{0}' ) right \", $(vParseTest3)))]"));
            Assert.Equal(" left \"string123\" ) right ", target.Eval("[$([System.String]::Format(' left \"{0}\" ) right ', $(vParseTest3)))]"));
        }

        [Fact]
        public void ParseTest4()
        {
            var target = new EvMSBuildComponent(new Soba());

            Assert.Equal("$(name)", target.Eval("[$$(name)]"));
            Assert.Equal("$$(name)", target.Eval("[$$$(name)]"));
            Assert.Equal("$([System.String]::Format(\" left '{0}' ) right \", $(name)))", target.Eval("[$$([System.String]::Format(\" left '{0}' ) right \", $(name)))]"));
        }
    }
}
