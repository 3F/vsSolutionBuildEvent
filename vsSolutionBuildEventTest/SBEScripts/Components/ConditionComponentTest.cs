﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.SBEScripts.Components;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    /// <summary>
    ///This is a test class for ConditionComponentTest and is intended
    ///to contain all ConditionComponentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConditionComponentTest
    {
        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba()); //mockEnv.Object, mockUVar.Object
            target.parse("#[(true){body1}]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest2()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());
            target.parse("[(true)]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest4()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());
            Assert.AreEqual(Value.Empty, target.parse("[ (true * 1) { body1 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (!true * 1) { body1 } ]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest5()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());
            target.parse("[ (!) { body1 } ]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest6()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());
            Assert.AreEqual(" body1 ", target.parse("[ (true) { body1 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (true == 1) { body1 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (true == true) { body1 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[(true){ body1 }]"));
            Assert.AreEqual("\n body1 \n", target.parse("[(true){\n body1 \n}]"));
            Assert.AreEqual(" body1 ", target.parse("[ (\"true\" == 1) { body1 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (\"true\") { body1 } ]"));
            Assert.AreEqual(Value.Empty, target.parse("[ (false) { body1 } ]"));
            Assert.AreEqual(Value.Empty, target.parse("[ (true == 0) { body1 } ]"));
            Assert.AreEqual(Value.Empty, target.parse("[ (true == false) { body1 } ]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest7()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());
            Assert.AreEqual(" body2 ", target.parse("[ (false) { body1 } else { body2 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (false == 0) { body1 } else { body2 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (false == false) { body1 } else { body2 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (\"false\" == 0) { body1 } else { body2 } ]"));
            Assert.AreEqual(" body2 ", target.parse("[ (\"false\") { body1 } else { body2 } ]"));
            Assert.AreEqual(" body2 ", target.parse("[(false){ body1 }else{ body2 }]"));
            Assert.AreEqual("\n body2 \n", target.parse("[ (false) {\n body1 \n}\nelse {\n body2 \n} ]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest8()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());
            Assert.AreEqual(" body1 ", target.parse("[(!false){ body1 }else{ body2 }]"));
            Assert.AreEqual(Value.Empty, target.parse("[(!true){ body1 }]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest9()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());

            Assert.AreEqual(" body1 ", target.parse("[(str1 === str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(\"str1 \"===\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(str1 == str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(\"str1 \"==\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(\"str1\"==\" str1 \"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(str1==\"str1 \"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(02 == 2){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(02 === 2){ body1 }else{ body2 }]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest10()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());

            Assert.AreEqual(" body2 ", target.parse("[(str1 !== str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(\"str1 \"!==\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(str1 != str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(\"str1 \"!=\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(02 != 2){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(02 !== 2){ body1 }else{ body2 }]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest11()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());

            Assert.AreEqual(" body1 ", target.parse("[(Test123Data ~= 12){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(Test123Data ~= \" 12\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(Test 123 Data ~= \" 12\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(Test 123 Data ~= Data){ body1 }else{ body2 }]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest12()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());

            Assert.AreEqual(" body2 ", target.parse("[(1 > 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(1 >= 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(1 > 01){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(01 >= 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(1 < 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(1 <= 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(1 < 01){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(01 <= 1){ body1 }else{ body2 }]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest13()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());

            Assert.AreEqual(" body2 ", target.parse("[(!str1 === str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(!\"str1 \"===\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(! str1 == str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(! \"str1 \"==\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(!02 == 2){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(!02 === 2){ body1 }else{ body2 }]"));

            Assert.AreEqual(" body1 ", target.parse("[(!str1 !== str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(!\"str1 \"!==\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(! str1 != str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(!\"str1 \"!=\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(!02 != 2){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(! 02 !== 2){ body1 }else{ body2 }]"));

            Assert.AreEqual(" body2 ", target.parse("[(!Test123Data ~= 12){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(! Test123Data ~= \" 12\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(! Test 123 Data ~= \" 12\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(!Test 123 Data ~= Data){ body1 }else{ body2 }]"));

            Assert.AreEqual(" body1 ", target.parse("[(!1 > 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(!1 >= 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(! 1 > 01){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(!01 >= 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(! 1 < 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(! 1 <= 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(!1 < 01){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(! 01 <= 1){ body1 }else{ body2 }]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest14()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());
            target.parse("[(1 > ){ body1 }else{ body2 }]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest15()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());
            target.parse("[(1 === ){ body1 }else{ body2 }]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest16()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());
            target.parse("[(1 >= ){ body1 }else{ body2 }]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void parseTest17()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());
            target.parse("[(2 > 1test ){ body1 }else{ body2 }]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest18()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUVars>();
            ConditionComponent target = new ConditionComponent(new Soba());

            Assert.AreEqual(" body1 ", target.parse("[(ConsoleApplication_1 ^= Console){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(ConsoleApplication_1 ^= Application){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(ConsoleApplication_1 ^= \" Console\"){ body1 }else{ body2 }]"));

            Assert.AreEqual(" body1 ", target.parse("[(ConsoleApplication_1 =^ _1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(ConsoleApplication_1 =^ Console){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(ConsoleApplication_1 =^ \"_1 \"){ body1 }else{ body2 }]"));
        }

        /// <summary>
        ///A test for parse->disclosure
        ///</summary>
        [TestMethod()]
        public void disclosureTest1()
        {
            IUVars uv = new UVars();
            uv.SetVariable("test", null, "(data");
            uv.SetVariable("test2", null, "data)");
            uv.SetVariable("test3", null, "true");

            ConditionComponent target = new ConditionComponent(new Soba(uv));
            Assert.AreEqual("yes", target.parse("[( #[var test] ~= \"(data\"){yes}else{no}]"));
            Assert.AreEqual("no", target.parse("[( #[var test2] ~= \"(data\"){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( (#[var test3]) ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( 1 < 2 && 2 == 2 || (((2 >= 2) && true)) ){yes}else{no}]"));
        }

        /// <summary>
        ///A test for parse->disclosure
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void disclosureTest2()
        {
            ConditionComponent target = new ConditionComponent(new Soba());
            target.parse("[( 1 < 2 && 2 == 2 || ((2 >= 2) && true)) ){yes}else{no}]");
        }

        /// <summary>
        ///A test for parse->disclosure
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void disclosureTest3()
        {
            ConditionComponent target = new ConditionComponent(new Soba());
            target.parse("[( 1 < 2 && () || (((2 >= 2) && true)) ){yes}else{no}]");
        }

        /// <summary>
        ///A test for parse->disclosure
        ///</summary>
        [TestMethod()]
        public void disclosureTest4()
        {
            IUVars uv = new UVars();
            uv.SetVariable("test", null, "data(str)");
            uv.SetVariable("test2", null, "true");
            uv.SetVariable("test3", null, "4");

            ConditionComponent target = new ConditionComponent(new Soba(uv));
            target.PostProcessingMSBuild = true;

            Assert.AreEqual("yes", target.parse("[(  $(test) == \"data(str)\" ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[(  $(test) == \"data(str)\" && 1 < 2){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[(  #[var test] == \"data(str)\" && #[var test2] || #[var test3] == 4){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[(  #[var test] == \"data(str)\" && !#[var test2] || #[var test3] == 4){yes}else{no}]"));
            Assert.AreEqual("no", target.parse("[(  #[var test] == \"data(str)\" && !#[var test2] && #[var test3] == 4){yes}else{no}]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void invertTest1()
        {
            ConditionComponent target = new ConditionComponent(new Soba());
            Assert.AreEqual("no", target.parse("[( (!2 == 2) && 1 < 2 ){yes}else{no}]"));
            Assert.AreEqual("no", target.parse("[( (!2 == 2) && (!1 < 2) ){yes}else{no}]"));
            Assert.AreEqual("no", target.parse("[( (!2 == 2) && (!1 > 2) ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[(! (!2 == 2) && (!1 > 2) ){yes}else{no}]"));
            Assert.AreEqual("no", target.parse("[( (!2 == 2) && (!1 < 2) || (!true) ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( (!2 == 2) && (!1 < 2) || (true) ){yes}else{no}]"));
        }

        /// <summary>
        ///A test for parse->composite
        ///</summary>
        [TestMethod()]
        public void compositeTest1()
        {
            IUVars uv = new UVars();
            uv.SetVariable("test", null, "(data");

            ConditionComponent target = new ConditionComponent(new Soba(uv));
            Assert.AreEqual("no", target.parse("[( #[var test] ~= \"(data && 1 < 2\" ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( #[var test] ~= \"(data\" && 1 < 2 ){yes}else{no}]"));
        }

        /// <summary>
        ///A test for parse->composite
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void compositeTest2()
        {
            ConditionComponent target = new ConditionComponent(new Soba());
            target.parse("[( 2 == 2 & 1 < 2 ){yes}else{no}]");
        }

        /// <summary>
        ///A test for parse->composite
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void compositeTest3()
        {
            ConditionComponent target = new ConditionComponent(new Soba());
            target.parse("[( 2 == 2 | 1 < 2 ){yes}else{no}]");
        }

        /// <summary>
        ///A test for parse->composite
        ///</summary>
        [TestMethod()]
        public void compositeTest4()
        {
            ConditionComponent target = new ConditionComponent(new Soba());
            Assert.AreEqual("yes", target.parse("[( 2 == 2 && 1 < 2 ){yes}else{no}]"));
            Assert.AreEqual("no", target.parse("[( 2 == 2 && 1 > 2 ){yes}else{no}]"));
            Assert.AreEqual("no", target.parse("[( 2 != 2 && 1 < 2 ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( 2 == 2 || 1 < 2 ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( 2 == 2 || 1 > 2 ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( 2 != 2 || 1 < 2 ){yes}else{no}]"));
            Assert.AreEqual("no", target.parse("[( 2 != 2 || 1 > 2 ){yes}else{no}]"));
        }

        /// <summary>
        ///A test for parse->composite
        ///</summary>
        [TestMethod()]
        public void compositeTest5()
        {
            ConditionComponent target = new ConditionComponent(new Soba());
            Assert.AreEqual("no", target.parse("[( 2 == 2 && 1 < 2 && (7 == 4) ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( 2 != 2 && 1 > 2 || (7 != 4) ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( true && ((1 < 2) || (((2 >= 2) && true))) ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( ((1 < 2) || (((2 >= 2) && true))) ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( ((1 < 2) && (((2 >= 2) && true))) ){yes}else{no}]"));
            Assert.AreEqual("no", target.parse("[( ((1 < 2) && (((2 > 2) && true))) ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( true && ((1 < 2) || (((2 > 2) && true))) ){yes}else{no}]"));
            Assert.AreEqual("no", target.parse("[( true && ((1 < 2) && (((2 > 2) && true))) ){yes}else{no}]"));
        }

        /// <summary>
        ///A test for parse->composite
        ///</summary>
        [TestMethod()]
        public void compositeTest6()
        {
            ConditionComponent target = new ConditionComponent(new Soba());
            Assert.AreEqual("no", target.parse("[( (1 < 2 && 2 == 2 &&(((false || 2 >= 2) && (1 && (false) && true)))) ){yes}else{no}]"));
            Assert.AreEqual("no", target.parse("[( (1 < 2 && 2 == 2 &&(((false || 2 >= 2) && (1 > 7 && true)))) ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( (1 < 2 && 2 == 2 ||(((2 >= 2) && true))) ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( (1 < 2 && 2 == 2 || (((2 >= 2) && true))) ){yes}else{no}]"));
        }

        /// <summary>
        ///A test for parse->composite
        ///</summary>
        [TestMethod()]
        public void compositeTest7()
        {
            IUVars uv = new UVars();
            uv.SetVariable("test", null, "data1 && data|2");
            uv.SetVariable("test2", null, "data1 || data&2");

            ConditionComponent target = new ConditionComponent(new Soba(uv));
            Assert.AreEqual("yes", target.parse("[( #[var test] == \"data1 && data|2\" ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( #[var test2] == \"data1 || data&2\" ){yes}else{no}]"));
        }

        /// <summary>
        ///A test for parse->composite
        ///</summary>
        [TestMethod()]
        public void compositeTest8()
        {
            ConditionComponent target = new ConditionComponent(new Soba());
            Assert.AreEqual("yes", target.parse("[( (1 < 2 && 2 == 2 && ( true || ((false || 2 >= 2) && (1 > 7 && true)))) ){yes}else{no}]"));
            Assert.AreEqual("no", target.parse("[( (1 < 2 && 2 == 2 && ( true && ((false || 2 >= 2) && (1 > 7 && true)))) ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( (1 < 2 && 2 == 2 || ( true && ((false || 2 >= 2) && (1 > 7 && true)))) ){yes}else{no}]"));
            Assert.AreEqual("yes", target.parse("[( (1 < 2 && 2 == 2 && ( true && ((false || 2 >= 2) || (1 > 7 && true)))) ){yes}else{no}]"));
            Assert.AreEqual("no", target.parse("[( (1 < 2 && 2 == 2 && ( true && ((false || 2 > 2) || (1 > 7 && true)))) ){yes}else{no}]"));
        }
    }
}
