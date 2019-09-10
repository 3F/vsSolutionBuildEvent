using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.Core;
using net.r_eg.Varhead;
using Xunit;

namespace SobaScript.Z.CoreTest
{
    public class ConditionComponentTest
    {
        [Fact]
        public void ParseTest1()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("#[(true){body1}]")
            );

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[(true)]")
            );
        }

        [Fact]
        public void ParseTest2()
        {
            var target = new ConditionComponent(new Soba());
            Assert.Equal(Value.Empty, target.Eval("[ (true * 1) { body1 } ]"));
            Assert.Equal(" body1 ", target.Eval("[ (!true * 1) { body1 } ]"));
        }

        [Fact]
        public void ParseTest3()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[ (!) { body1 } ]")
            );
        }

        [Fact]
        public void ParseTest6()
        {
            var target = new ConditionComponent(new Soba());
            Assert.Equal(" body1 ", target.Eval("[ (true) { body1 } ]"));
            Assert.Equal(" body1 ", target.Eval("[ (true == 1) { body1 } ]"));
            Assert.Equal(" body1 ", target.Eval("[ (true == true) { body1 } ]"));
            Assert.Equal(" body1 ", target.Eval("[(true){ body1 }]"));
            Assert.Equal("\n body1 \n", target.Eval("[(true){\n body1 \n}]"));
            Assert.Equal(" body1 ", target.Eval("[ (\"true\" == 1) { body1 } ]"));
            Assert.Equal(" body1 ", target.Eval("[ (\"true\") { body1 } ]"));
            Assert.Equal(Value.Empty, target.Eval("[ (false) { body1 } ]"));
            Assert.Equal(Value.Empty, target.Eval("[ (true == 0) { body1 } ]"));
            Assert.Equal(Value.Empty, target.Eval("[ (true == false) { body1 } ]"));
        }

        [Fact]
        public void ParseTest7()
        {
            var target = new ConditionComponent(new Soba());
            Assert.Equal(" body2 ", target.Eval("[ (false) { body1 } else { body2 } ]"));
            Assert.Equal(" body1 ", target.Eval("[ (false == 0) { body1 } else { body2 } ]"));
            Assert.Equal(" body1 ", target.Eval("[ (false == false) { body1 } else { body2 } ]"));
            Assert.Equal(" body1 ", target.Eval("[ (\"false\" == 0) { body1 } else { body2 } ]"));
            Assert.Equal(" body2 ", target.Eval("[ (\"false\") { body1 } else { body2 } ]"));
            Assert.Equal(" body2 ", target.Eval("[(false){ body1 }else{ body2 }]"));
            Assert.Equal("\n body2 \n", target.Eval("[ (false) {\n body1 \n}\nelse {\n body2 \n} ]"));
        }

        [Fact]
        public void ParseTest8()
        {
            var target = new ConditionComponent(new Soba());
            Assert.Equal(" body1 ", target.Eval("[(!false){ body1 }else{ body2 }]"));
            Assert.Equal(Value.Empty, target.Eval("[(!true){ body1 }]"));
        }

        [Fact]
        public void ParseTest9()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Equal(" body1 ", target.Eval("[(str1 === str1){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(\"str1 \"===\" str1\"){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(str1 == str1){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(\"str1 \"==\" str1\"){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(\"str1\"==\" str1 \"){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(str1==\"str1 \"){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(02 == 2){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(02 === 2){ body1 }else{ body2 }]"));
        }

        [Fact]
        public void ParseTest10()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Equal(" body2 ", target.Eval("[(str1 !== str1){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(\"str1 \"!==\" str1\"){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(str1 != str1){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(\"str1 \"!=\" str1\"){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(02 != 2){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(02 !== 2){ body1 }else{ body2 }]"));
        }

        [Fact]
        public void ParseTest11()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Equal(" body1 ", target.Eval("[(Test123Data ~= 12){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(Test123Data ~= \" 12\"){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(Test 123 Data ~= \" 12\"){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(Test 123 Data ~= Data){ body1 }else{ body2 }]"));
        }

        [Fact]
        public void ParseTest12()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Equal(" body2 ", target.Eval("[(1 > 1){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(1 >= 1){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(1 > 01){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(01 >= 1){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(1 < 1){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(1 <= 1){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(1 < 01){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(01 <= 1){ body1 }else{ body2 }]"));
        }

        [Fact]
        public void ParseTest13()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Equal(" body2 ", target.Eval("[(!str1 === str1){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(!\"str1 \"===\" str1\"){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(! str1 == str1){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(! \"str1 \"==\" str1\"){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(!02 == 2){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(!02 === 2){ body1 }else{ body2 }]"));

            Assert.Equal(" body1 ", target.Eval("[(!str1 !== str1){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(!\"str1 \"!==\" str1\"){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(! str1 != str1){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(!\"str1 \"!=\" str1\"){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(!02 != 2){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(! 02 !== 2){ body1 }else{ body2 }]"));

            Assert.Equal(" body2 ", target.Eval("[(!Test123Data ~= 12){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(! Test123Data ~= \" 12\"){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(! Test 123 Data ~= \" 12\"){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(!Test 123 Data ~= Data){ body1 }else{ body2 }]"));

            Assert.Equal(" body1 ", target.Eval("[(!1 > 1){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(!1 >= 1){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(! 1 > 01){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(!01 >= 1){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(! 1 < 1){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(! 1 <= 1){ body1 }else{ body2 }]"));
            Assert.Equal(" body1 ", target.Eval("[(!1 < 01){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(! 01 <= 1){ body1 }else{ body2 }]"));
        }

        [Fact]
        public void ParseTest14()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[(1 > ){ body1 }else{ body2 }]")
            );
        }

        [Fact]
        public void ParseTest15()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[(1 === ){ body1 }else{ body2 }]")
            );
        }

        [Fact]
        public void ParseTest16()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[(1 >= ){ body1 }else{ body2 }]")
            );
        }

        [Fact]
        public void ParseTest17()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Throws<System.FormatException>(() =>
                target.Eval("[(2 > 1test ){ body1 }else{ body2 }]")
            );
        }

        [Fact]
        public void ParseTest18()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Equal(" body1 ", target.Eval("[(ConsoleApplication_1 ^= Console){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(ConsoleApplication_1 ^= Application){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(ConsoleApplication_1 ^= \" Console\"){ body1 }else{ body2 }]"));

            Assert.Equal(" body1 ", target.Eval("[(ConsoleApplication_1 =^ _1){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(ConsoleApplication_1 =^ Console){ body1 }else{ body2 }]"));
            Assert.Equal(" body2 ", target.Eval("[(ConsoleApplication_1 =^ \"_1 \"){ body1 }else{ body2 }]"));
        }

        [Fact]
        public void DisclosureTest1()
        {
            IUVars uv = new UVars();
            uv.SetVariable("test", null, "(data");
            uv.SetVariable("test2", null, "data)");
            uv.SetVariable("test3", null, "true");

            var soba = new Soba(uv);
            soba.Register(new UserVariableComponent(soba));

            var target = new ConditionComponent(soba);
            Assert.Equal("yes", target.Eval("[( #[var test] ~= \"(data\"){yes}else{no}]"));
            Assert.Equal("no", target.Eval("[( #[var test2] ~= \"(data\"){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( (#[var test3]) ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( 1 < 2 && 2 == 2 || (((2 >= 2) && true)) ){yes}else{no}]"));
        }

        [Fact]
        public void DisclosureTest2()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[( 1 < 2 && 2 == 2 || ((2 >= 2) && true)) ){yes}else{no}]")
            );
        }

        [Fact]
        public void DisclosureTest3()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[( 1 < 2 && () || (((2 >= 2) && true)) ){yes}else{no}]")
            );
        }

        [Fact]
        public void DisclosureTest4()
        {
            IUVars uv = new UVars();
            uv.SetVariable("test", null, "data(str)");
            uv.SetVariable("test2", null, "true");
            uv.SetVariable("test3", null, "4");

            var soba = new Soba(uv);
            soba.Register(new UserVariableComponent(soba));

            var target = new ConditionComponent(soba) {
                PostProcessingMSBuild = true
            };

            Assert.Equal("yes", target.Eval("[(  $(test) == \"data(str)\" ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[(  $(test) == \"data(str)\" && 1 < 2){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[(  #[var test] == \"data(str)\" && #[var test2] || #[var test3] == 4){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[(  #[var test] == \"data(str)\" && !#[var test2] || #[var test3] == 4){yes}else{no}]"));
            Assert.Equal("no", target.Eval("[(  #[var test] == \"data(str)\" && !#[var test2] && #[var test3] == 4){yes}else{no}]"));
        }

        [Fact]
        public void InvertTest1()
        {
            var target = new ConditionComponent(new Soba());
            Assert.Equal("no", target.Eval("[( (!2 == 2) && 1 < 2 ){yes}else{no}]"));
            Assert.Equal("no", target.Eval("[( (!2 == 2) && (!1 < 2) ){yes}else{no}]"));
            Assert.Equal("no", target.Eval("[( (!2 == 2) && (!1 > 2) ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[(! (!2 == 2) && (!1 > 2) ){yes}else{no}]"));
            Assert.Equal("no", target.Eval("[( (!2 == 2) && (!1 < 2) || (!true) ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( (!2 == 2) && (!1 < 2) || (true) ){yes}else{no}]"));
        }

        [Fact]
        public void CompositeTest1()
        {
            IUVars uv = new UVars();
            uv.SetVariable("test", null, "(data");

            var soba = new Soba(uv);
            soba.Register(new UserVariableComponent(soba));

            var target = new ConditionComponent(soba);
            Assert.Equal("no", target.Eval("[( #[var test] ~= \"(data && 1 < 2\" ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( #[var test] ~= \"(data\" && 1 < 2 ){yes}else{no}]"));
        }

        [Fact]
        public void CompositeTest2()
        {
            var target = new ConditionComponent(new Soba());

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[( 2 == 2 & 1 < 2 ){yes}else{no}]")
            );

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[( 2 == 2 | 1 < 2 ){yes}else{no}]")
            );
        }

        [Fact]
        public void CompositeTest3()
        {
            var target = new ConditionComponent(new Soba());
            Assert.Equal("yes", target.Eval("[( 2 == 2 && 1 < 2 ){yes}else{no}]"));
            Assert.Equal("no", target.Eval("[( 2 == 2 && 1 > 2 ){yes}else{no}]"));
            Assert.Equal("no", target.Eval("[( 2 != 2 && 1 < 2 ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( 2 == 2 || 1 < 2 ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( 2 == 2 || 1 > 2 ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( 2 != 2 || 1 < 2 ){yes}else{no}]"));
            Assert.Equal("no", target.Eval("[( 2 != 2 || 1 > 2 ){yes}else{no}]"));
        }

        [Fact]
        public void CompositeTest4()
        {
            var target = new ConditionComponent(new Soba());
            Assert.Equal("no", target.Eval("[( 2 == 2 && 1 < 2 && (7 == 4) ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( 2 != 2 && 1 > 2 || (7 != 4) ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( true && ((1 < 2) || (((2 >= 2) && true))) ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( ((1 < 2) || (((2 >= 2) && true))) ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( ((1 < 2) && (((2 >= 2) && true))) ){yes}else{no}]"));
            Assert.Equal("no", target.Eval("[( ((1 < 2) && (((2 > 2) && true))) ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( true && ((1 < 2) || (((2 > 2) && true))) ){yes}else{no}]"));
            Assert.Equal("no", target.Eval("[( true && ((1 < 2) && (((2 > 2) && true))) ){yes}else{no}]"));
        }

        [Fact]
        public void CompositeTest5()
        {
            var target = new ConditionComponent(new Soba());
            Assert.Equal("no", target.Eval("[( (1 < 2 && 2 == 2 &&(((false || 2 >= 2) && (1 && (false) && true)))) ){yes}else{no}]"));
            Assert.Equal("no", target.Eval("[( (1 < 2 && 2 == 2 &&(((false || 2 >= 2) && (1 > 7 && true)))) ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( (1 < 2 && 2 == 2 ||(((2 >= 2) && true))) ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( (1 < 2 && 2 == 2 || (((2 >= 2) && true))) ){yes}else{no}]"));
        }

        [Fact]
        public void CompositeTest6()
        {
            IUVars uv = new UVars();
            uv.SetVariable("test", null, "data1 && data|2");
            uv.SetVariable("test2", null, "data1 || data&2");

            var soba = new Soba(uv);
            soba.Register(new UserVariableComponent(soba));

            var target = new ConditionComponent(soba);
            Assert.Equal("yes", target.Eval("[( #[var test] == \"data1 && data|2\" ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( #[var test2] == \"data1 || data&2\" ){yes}else{no}]"));
        }

        [Fact]
        public void CompositeTest7()
        {
            var target = new ConditionComponent(new Soba());
            Assert.Equal("yes", target.Eval("[( (1 < 2 && 2 == 2 && ( true || ((false || 2 >= 2) && (1 > 7 && true)))) ){yes}else{no}]"));
            Assert.Equal("no", target.Eval("[( (1 < 2 && 2 == 2 && ( true && ((false || 2 >= 2) && (1 > 7 && true)))) ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( (1 < 2 && 2 == 2 || ( true && ((false || 2 >= 2) && (1 > 7 && true)))) ){yes}else{no}]"));
            Assert.Equal("yes", target.Eval("[( (1 < 2 && 2 == 2 && ( true && ((false || 2 >= 2) || (1 > 7 && true)))) ){yes}else{no}]"));
            Assert.Equal("no", target.Eval("[( (1 < 2 && 2 == 2 && ( true && ((false || 2 > 2) || (1 > 7 && true)))) ){yes}else{no}]"));
        }
    }
}
