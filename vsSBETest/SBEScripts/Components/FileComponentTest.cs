using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace vsSBETest
{
    /// <summary>
    ///This is a test class for FileComponentTest and is intended
    ///to contain all FileComponentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileComponentTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest()
        {
            FileComponent target = new FileComponent();
            target.parse("#[File get(\"file\")]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest2()
        {
            FileComponent target = new FileComponent();
            target.parse("File get(\"file\")");
        }

        /// <summary>
        ///A test for parse - stGet
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest3()
        {
            FileComponent target = new FileComponent();
            target.parse("[File get(file)]");
        }

        /// <summary>
        ///A test for parse - stGet
        ///</summary>
        [TestMethod()]
        public void parseTest4()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            Assert.AreEqual("content from file", target.parse("[File get(\"file\")]"));
        }

        /// <summary>
        ///A test for parse - stGet
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ScriptException))]
        public void parseTest5()
        {
            FileComponentAccessor target = new FileComponentAccessor(true);
            target.parse("[File get(\"file\")]");
        }

        /// <summary>
        ///A test for parse - stCall
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest6()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File call(file)]");
        }

        /// <summary>
        ///A test for parse - stCall
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest7()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File out(file)]");
        }

        /// <summary>
        ///A test for parse - stCall
        ///</summary>
        [TestMethod()]
        public void parseTest8()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            Assert.AreEqual("stdout", target.parse("[File call(\"file\")]"));
            Assert.AreEqual("stdout", target.parse("[File call(\"file\", \"args\")]"));
            Assert.AreEqual("stdout", target.parse("[File out(\"file\")]"));
            Assert.AreEqual("stdout", target.parse("[File out(\"file\", \"args\")]"));
            Assert.AreEqual("silent stdout", target.parse("[File scall(\"file\")]"));
            Assert.AreEqual("silent stdout", target.parse("[File scall(\"file\", \"args\")]"));
            Assert.AreEqual("silent stdout", target.parse("[File sout(\"file\")]"));
            Assert.AreEqual("silent stdout", target.parse("[File sout(\"file\", \"args\")]"));
        }

        /// <summary>
        ///A test for parse - stCall
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ScriptException))]
        public void parseTest9()
        {
            FileComponentAccessor target = new FileComponentAccessor(true);
            target.parse("[File call(\"file\")]");
        }

        /// <summary>
        ///A test for parse - stWrite
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest10()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File write(\"file\")]");
        }

        /// <summary>
        ///A test for parse - stWrite
        ///</summary>
        [TestMethod()]
        public void parseTest11()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File write(\"file\"):data]");
            Assert.AreEqual("data", target.parse("[File get(\"file\")]"));

            // append, writeLine, appendLine is identical for current accessor
            // however, also need checking the entry point as for the write() above:

            target.parse("[File append(\"file\"):data]");
            Assert.AreEqual("data", target.parse("[File get(\"file\")]"));
            target.parse("[File writeLine(\"file\"):data]");
            Assert.AreEqual("data", target.parse("[File get(\"file\")]"));
            target.parse("[File appendLine(\"file\"):data]");
            Assert.AreEqual("data", target.parse("[File get(\"file\")]"));
        }

        /// <summary>
        ///A test for parse - stWrite
        ///</summary>
        [TestMethod()]
        public void parseTest12()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File write(\"file\"): multi\nline\" \n 'data'.]");
            Assert.AreEqual(" multi\nline\" \n 'data'.", target.parse("[File get(\"file\")]"));
        }

        /// <summary>
        ///A test for parse - stWrite
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ScriptException))]
        public void parseTest13()
        {
            FileComponentAccessor target = new FileComponentAccessor(true);
            target.parse("[File write(\"file\"):data]");
        }

        /// <summary>
        ///A test for parse - stReplace
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest14()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File replace(file, pattern, replacement)]");
        }

        /// <summary>
        ///A test for parse - stReplace
        ///</summary>
        [TestMethod()]
        public void parseTest15()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File replace(\"file\", \"from\", \"to\")]");
            Assert.AreEqual("content to file", target.parse("[File get(\"file\")]"));
        }

        /// <summary>
        ///A test for parse - stReplace
        ///</summary>
        [TestMethod()]
        public void parseTest16()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File replace.Regexp(\"file\", \"t\\s*from\", \"t to\")]");
            Assert.AreEqual("content to file", target.parse("[File get(\"file\")]"));
        }

        /// <summary>
        ///A test for parse - stReplace
        ///</summary>
        [TestMethod()]
        public void parseTest17()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File replace.Wildcards(\"file\", \"con*from \", \"\")]");
            Assert.AreEqual("file", target.parse("[File get(\"file\")]"));
        }

        /// <summary>
        ///A test for parse - stExists
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest18()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File exists.stub(\"path\")]");
        }

        /// <summary>
        ///A test for parse - stExists
        ///</summary>
        [TestMethod()]
        public void parseTest19()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            string realDir = Path.GetDirectoryName(Assembly.GetAssembly(GetType()).Location);

            Assert.AreEqual(Value.VFALSE, target.parse("[File exists.directory(\"c:\\stubdir\\notexist\")]"));
            Assert.AreEqual(Value.VTRUE, target.parse("[File exists.directory(\"" + realDir + "\")]"));
        }

        /// <summary>
        ///A test for parse - stExists
        ///</summary>
        [TestMethod()]
        public void parseTest20()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            string realFile = Assembly.GetAssembly(GetType()).Location;

            Assert.AreEqual(Value.VFALSE, target.parse("[File exists.file(\"file\")]"));
            Assert.AreEqual(Value.VTRUE, target.parse("[File exists.file(\"" + realFile + "\")]"));
        }

        private class FileComponentAccessor: FileComponent
        {
            public bool throwError = false;
            protected string content = "content from file";

            public FileComponentAccessor(bool throwError = false)
            {
                Settings.setWorkPath("/");
                this.throwError = throwError;
            }

            protected override string readToEnd(string file, Encoding enc, bool detectEncoding, out Encoding current)
            {
                current = Encoding.UTF8;
                if(throwError) {
                    throw new System.IO.FileNotFoundException(String.Format("Some error for '{0}'", file));
                }
                return content;
            }

            protected override void writeToFile(string file, string data, bool append, bool writeLine, Encoding enc)
            {
                if(throwError) {
                    throw new System.IO.IOException(String.Format("Some error for '{0}'", file));
                }
                content = data;
            }

            protected override string run(string file, string args, bool silent, bool stdOut)
            {
                if(throwError) {
                    throw new ComponentException(String.Format("Some error for '{0} {1}'", file, args));
                }
                return String.Format("{0}stdout", silent? "silent ": String.Empty);
            }
        }
    }
}
