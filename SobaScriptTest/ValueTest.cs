using System;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.SNode;
using Xunit;

namespace SobaScriptTest
{
    public class ValueTest
    {
        [Fact]
        public void ToBooleanTest()
        {
            Assert.True(Value.toBoolean("1"));
            Assert.True(Value.toBoolean("true"));
            Assert.True(Value.toBoolean("TRUE"));
            Assert.True(Value.toBoolean("True"));

            Assert.False(Value.toBoolean("0"));
            Assert.False(Value.toBoolean("false"));
            Assert.False(Value.toBoolean("FALSE"));
            Assert.False(Value.toBoolean("False"));

            Assert.True(Value.toBoolean(" 1 "));
            Assert.True(Value.toBoolean(" true "));
            Assert.False(Value.toBoolean(" 0 "));
            Assert.False(Value.toBoolean(" false "));
        }

        [Fact]
        public void ToBooleanTest2()
        {
            Assert.Throws<IncorrectSyntaxException>(() => Value.toBoolean("TruE"));
            Assert.Throws<IncorrectSyntaxException>(() => Value.toBoolean("FalsE"));
            Assert.Throws<IncorrectSyntaxException>(() => Value.toBoolean("-true"));
            Assert.Throws<IncorrectSyntaxException>(() => Value.toBoolean("true."));
            Assert.Throws<IncorrectSyntaxException>(() => Value.toBoolean(""));
        }

        [Fact]
        public void FromTest()
        {
            Assert.Equal("false", Value.from(false));
            Assert.Equal("true", Value.from(true));
        }

        [Fact]
        public void CmpTest()
        {
            Assert.True(Value.cmp(" test", " test", "==="));
            Assert.True(Value.cmp(" tESt ", " tESt ", "==="));
            Assert.True(Value.cmp("", "", "==="));
            Assert.True(Value.cmp("12", "12", "==="));
            Assert.False(Value.cmp("test", " test", "==="));
            Assert.False(Value.cmp("test ", " test", "==="));
            Assert.False(Value.cmp("tESt", " test", "==="));
            Assert.False(Value.cmp(" ", "", "==="));
            Assert.False(Value.cmp("12", "12 ", "==="));
        }

        [Fact]
        public void CmpTest2()
        {
            Assert.False(Value.cmp(" test", " test", "!=="));
            Assert.False(Value.cmp(" tESt ", " tESt ", "!=="));
            Assert.False(Value.cmp("", "", "!=="));
            Assert.False(Value.cmp("12", "12", "!=="));
            Assert.True(Value.cmp("test", " test", "!=="));
            Assert.True(Value.cmp("test ", " test", "!=="));
            Assert.True(Value.cmp("tESt", " test", "!=="));
            Assert.True(Value.cmp(" ", "", "!=="));
            Assert.True(Value.cmp("12", "12 ", "!=="));
        }

        [Fact]
        public void CmpTest3()
        {
            Assert.True(Value.cmp("test-12M-word", "12M", "~="));
            Assert.True(Value.cmp("", "", "~="));
            Assert.True(Value.cmp("test ", " ", "~="));
            Assert.True(Value.cmp(" ", "", "~="));
            Assert.True(Value.cmp("123", "2", "~="));
            Assert.False(Value.cmp("test-12M-word", "12m", "~="));
            Assert.False(Value.cmp("test-12M-word", " ", "~="));
            Assert.False(Value.cmp("test-12M-word", "0", "~="));
        }

        [Fact]
        public void CmpTest4()
        {
            Assert.True(Value.cmp(" test", " test", "=="));
            Assert.True(Value.cmp(" tESt ", " tESt ", "=="));
            Assert.True(Value.cmp("", "", "=="));
            Assert.True(Value.cmp("12", "12", "=="));
            Assert.True(Value.cmp(" 12", "12 ", "=="));
            Assert.True(Value.cmp("00012", "12", "=="));
            Assert.True(Value.cmp("00012", "012", "=="));
            Assert.False(Value.cmp("test", " test", "=="));
            Assert.False(Value.cmp("test ", " test", "=="));
            Assert.False(Value.cmp("tESt", " test", "=="));
            Assert.False(Value.cmp(" ", "", "=="));
            Assert.False(Value.cmp("120", "12 ", "=="));
            Assert.True(Value.cmp("true", "1", "=="));
            Assert.True(Value.cmp("false", "0", "=="));
        }

        [Fact]
        public void CmpTest5()
        {
            Assert.False(Value.cmp(" test", " test", "!="));
            Assert.False(Value.cmp(" tESt ", " tESt ", "!="));
            Assert.False(Value.cmp("", "", "!="));
            Assert.False(Value.cmp("12", "12", "!="));
            Assert.False(Value.cmp(" 12", "12 ", "!="));
            Assert.False(Value.cmp("00012", "12", "!="));
            Assert.False(Value.cmp("00012", "012", "!="));
            Assert.True(Value.cmp("test", " test", "!="));
            Assert.True(Value.cmp("test ", " test", "!="));
            Assert.True(Value.cmp("tESt", " test", "!="));
            Assert.True(Value.cmp(" ", "", "!="));
            Assert.True(Value.cmp("120", "12 ", "!="));
            Assert.False(Value.cmp("true", "1", "!="));
            Assert.False(Value.cmp("false", "0", "!="));
        }

        [Fact]
        public void CmpTest6()
        {
            Assert.Throws<FormatException>(() => Value.cmp(" test", " test", ">"));
            Assert.Throws<FormatException>(() => Value.cmp("", "", ">"));
            Assert.Throws<FormatException>(() => Value.cmp("t2", "1", ">"));
        }

        [Fact]
        public void CmpTest7()
        {
            Assert.True(Value.cmp("2", "1", ">"));
            Assert.True(Value.cmp("2", "01", ">"));
            Assert.False(Value.cmp("1", "2", ">"));
            Assert.False(Value.cmp("2", "2", ">"));
        }

        [Fact]
        public void CmpTest8()
        {
            Assert.Throws<FormatException>(() => Value.cmp(" test", " test", ">="));
            Assert.Throws<FormatException>(() => Value.cmp("", "", ">="));
            Assert.Throws<FormatException>(() => Value.cmp("t2", "1", ">="));
        }

        [Fact]
        public void CmpTest9()
        {
            Assert.True(Value.cmp("2", "1", ">="));
            Assert.True(Value.cmp("2", "01", ">="));
            Assert.True(Value.cmp("2", "2", ">="));
            Assert.True(Value.cmp("2", "002", ">="));
            Assert.False(Value.cmp("1", "2", ">="));
        }

        [Fact]
        public void CmpTest10()
        {
            Assert.Throws<FormatException>(() => Value.cmp(" test", " test", "<"));
            Assert.Throws<FormatException>(() => Value.cmp("", "", "<"));
            Assert.Throws<FormatException>(() => Value.cmp("t2", "1", "<"));
        }

        [Fact]
        public void CmpTest11()
        {
            Assert.False(Value.cmp("2", "1", "<"));
            Assert.False(Value.cmp("2", "01", "<"));
            Assert.False(Value.cmp("2", "2", "<"));
            Assert.True(Value.cmp("1", "2", "<"));
            Assert.True(Value.cmp("001", "2", "<"));
        }

        [Fact]
        public void CmpTest12()
        {
            Assert.Throws<FormatException>(() => Value.cmp(" test", " test", "<="));
            Assert.Throws<FormatException>(() => Value.cmp("", "", "<="));
            Assert.Throws<FormatException>(() => Value.cmp("t2", "1", "<="));
        }

        [Fact]
        public void CmpTest13()
        {
            Assert.False(Value.cmp("2", "1", "<="));
            Assert.False(Value.cmp("2", "01", "<="));
            Assert.True(Value.cmp("2", "002", "<="));
            Assert.True(Value.cmp("1", "2", "<="));
            Assert.True(Value.cmp("2", "2", "<="));
        }

        [Fact]
        public void CmpTest14()
        {
            Assert.Throws<IncorrectSyntaxException>(() => Value.cmp("2", "1", "test"));
            Assert.Throws<IncorrectSyntaxException>(() => Value.cmp("2", "1", ""));
            Assert.Throws<IncorrectSyntaxException>(() => Value.cmp("2", "1", "> ="));
        }

        [Fact]
        public void PackTest1()
        {
            object data = (object)new object[] { "str", 123, -1.4, true, "str2", new object[] { 1.2f, "str2", false }, -24.574 };
            Assert.Equal("{\"str\", 123, -1.4, true, \"str2\", {1.2f, \"str2\", false}, -24.574}", Value.pack(data));
        }

        [Fact]
        public void PackTest2()
        {
            object data = (object)new object[] { "str", 't', new object[] { 'a', 'b', 'c' }, true };
            Assert.Equal("{\"str\", 't', {'a', 'b', 'c'}, true}", Value.pack(data));
        }

        [Fact]
        public void PackTest3()
        {
            Assert.Equal(" test ", Value.pack(" test "));
            Assert.Equal(string.Empty, Value.pack(string.Empty));
            Assert.Null(Value.pack(null));
        }

        [Fact]
        public void PackTest4()
        {
            object data = new object[] { "str", 123, -1.4, true, 'n', new object[] { 1.2f, "str2", 'y', false }, -24.574 };

            IPM pm = new PM(string.Format("left({0})", Value.pack(data)));
            
            Argument[] raw = (Argument[])pm.Levels[0].Args;
            Assert.Single(raw);

            Assert.Equal(ArgumentType.Object, raw[0].type);
            {
                Argument[] args = (Argument[])raw[0].data;
                Assert.Equal(7, args.Length);

                Assert.Equal(ArgumentType.StringDouble, args[0].type);
                Assert.Equal("str", args[0].data);

                Assert.Equal(ArgumentType.Integer, args[1].type);
                Assert.Equal(123, args[1].data);

                Assert.Equal(ArgumentType.Double, args[2].type);
                Assert.Equal(-1.4, args[2].data);

                Assert.Equal(ArgumentType.Boolean, args[3].type);
                Assert.Equal(true, args[3].data);

                Assert.Equal(ArgumentType.Char, args[4].type);
                Assert.Equal('n', args[4].data);

                Assert.Equal(ArgumentType.Object, args[5].type);
                {
                    Argument[] args5 = (Argument[])args[5].data;
                    Assert.Equal(4, args5.Length);

                    Assert.Equal(ArgumentType.Float, args5[0].type);
                    Assert.Equal(1.2f, args5[0].data);

                    Assert.Equal(ArgumentType.StringDouble, args5[1].type);
                    Assert.Equal("str2", args5[1].data);

                    Assert.Equal(ArgumentType.Char, args5[2].type);
                    Assert.Equal('y', args5[2].data);

                    Assert.Equal(ArgumentType.Boolean, args5[3].type);
                    Assert.Equal(false, args5[3].data);
                }

                Assert.Equal(ArgumentType.Double, args[6].type);
                Assert.Equal(-24.574, args[6].data);
            }            
        }

        [Fact]
        public void PackTest5()
        {
            object data = (object)new object[] { 1.4, 1.4f, 1.4d, -1.4, -1.4f, -1.4d };

            IPM pm = new PM(string.Format("left({0})", Value.pack(data)));

            Argument[] args = (Argument[])pm.Levels[0].Args[0].data;
            Assert.Equal(6, args.Length);
            
            Assert.Equal(ArgumentType.Double, args[0].type);
            Assert.Equal(1.4d, args[0].data);

            Assert.Equal(ArgumentType.Float, args[1].type);
            Assert.Equal(1.4f, args[1].data);

            Assert.Equal(ArgumentType.Double, args[2].type);
            Assert.Equal(1.4d, args[2].data);

            Assert.Equal(ArgumentType.Double, args[3].type);
            Assert.Equal(-1.4d, args[3].data);

            Assert.Equal(ArgumentType.Float, args[4].type);
            Assert.Equal(-1.4f, args[4].data);

            Assert.Equal(ArgumentType.Double, args[5].type);
            Assert.Equal(-1.4d, args[5].data);
        }

        [Fact]
        public void PackTest6()
        {
            object data = new object[] { "str", new object[] { 1, 'y', new object[] { -12.457f, new object[] { 12 } } }, true };
            Assert.Equal("{\"str\", {1, 'y', {-12.457f, {12}}}, true}", Value.pack(data));
        }
    }
}
