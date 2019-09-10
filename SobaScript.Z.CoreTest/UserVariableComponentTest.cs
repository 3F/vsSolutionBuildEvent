using System;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.Core;
using net.r_eg.Varhead;
using net.r_eg.Varhead.Exceptions;
using SobaScript.Z.CoreTest.Stubs;
using Xunit;

namespace SobaScript.Z.CoreTest
{
    public class UserVariableComponentTest
    {
        [Fact]
        public void ParseTest()
        {
            var target = new UserVariableComponent(new Soba());

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("#[var name = value]")
            );
        }

        [Fact]
        public void ParseTest2()
        {
            var target = new UserVariableComponent(new Soba());

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("var name = value")
            );
        }

        [Fact]
        public void ParseTest3()
        {
            var target = new UserVariableComponentEvalStub(new UVars());

            Assert.Throws<DefinitionNotFoundException>(() => 
            {
                Assert.Equal(Value.Empty, target.Eval("[var name = value]"));
                Assert.Equal(Value.Empty, target.Eval("[var -name]"));
                Assert.Equal("[E1:value]", target.Eval("[var name]"));
            });
        }

        [Fact]
        public void ParseTest4()
        {
            var target = new UserVariableComponentEvalStub(new UVars());

            Assert.Throws<IncorrectSyntaxException>(() => 
            {
                Assert.Equal(Value.Empty, target.Eval("[var name = value]"));
                Assert.Equal(Value.Empty, target.Eval("[var -name = value]"));
            });
        }

        [Fact]
        public void ParseTest5()
        {
            var target = new UserVariableComponentEvalStub(new UVars());
            Assert.Equal(Value.Empty, target.Eval("[var name = value]"));
            Assert.Equal(Value.Empty, target.Eval("[var +name]"));
            Assert.Equal(String.Format("[E1:{0}]", UserVariableComponent.UVARIABLE_VALUE_DEFAULT), target.Eval("[var name]"));
        }

        [Fact]
        public void ParseTest6()
        {
            var target = new UserVariableComponentEvalStub(new UVars());

            Assert.Throws<IncorrectSyntaxException>(() => 
            {
                Assert.Equal(Value.Empty, target.Eval("[var name = value]"));
                Assert.Equal(Value.Empty, target.Eval("[var +name = value]"));
            });
        }

        [Fact]
        public void ParseTest7()
        {
            var target = new UserVariableComponentEvalStub(new UVars());

            Assert.Throws<IncorrectSyntaxException>(() => 
            {
                Assert.Equal(Value.Empty, target.Eval("[var name = value]"));
                Assert.Equal(Value.Empty, target.Eval("[var -name = value]"));
            });
        }

        [Fact]
        public void ParseTest8()
        {
            var target = new UserVariableComponentEvalStub(new UVars());

            Assert.Throws<IncorrectSyntaxException>(() => 
            {
                Assert.Equal(Value.Empty, target.Eval("[var + name]"));
            });
        }

        [Fact]
        public void StdTest1()
        {
            var target = new UserVariableComponent(new Soba());

            Assert.Throws<DefinitionNotFoundException>(() => {
                target.Eval("[var name]");
            });
        }

        [Fact]
        public void StdTest2()
        {
            var target = new UserVariableComponentEvalStub(new UVars());
            Assert.Equal(Value.Empty, target.Eval("[var name = value]"));
            Assert.Equal("[E1:value]", target.Eval("[var name]"));
        }

        [Fact]
        public void StdTest3()
        {
            var target = new UserVariableComponentEvalStub(new UVars());
            Assert.Equal(Value.Empty, target.Eval("[var name = line1 \n line2]"));
            Assert.Equal("[E1:line1 \n line2]", target.Eval("[var name]"));
        }

        [Fact]
        public void StdTest4()
        {
            var target = new UserVariableComponentEvalStub(new UVars());
            Assert.Equal(Value.Empty, target.Eval("[var name = value]"));
            Assert.Equal(Value.Empty, target.Eval("[var name = value2]"));
            Assert.Equal("[E1:value2]", target.Eval("[var name]"));
        }
    }
}
