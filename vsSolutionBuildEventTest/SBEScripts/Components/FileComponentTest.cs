using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Components;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    /// <summary>
    ///This is a test class for FileComponentTest and is intended
    ///to contain all FileComponentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileComponentTest
    {
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest()
        {
            FileComponent target = new FileComponent();
            target.parse("#[File get(\"file\")]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest2()
        {
            FileComponent target = new FileComponent();
            target.parse("File get(\"file\")");
        }

        /// <summary>
        ///A test for parse - stGet
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(PMArgException))]
        public void stGetParseTest1()
        {
            FileComponent target = new FileComponent();
            target.parse("[File get(file)]");
        }

        /// <summary>
        ///A test for parse - stGet
        ///</summary>
        [TestMethod()]
        public void stGetParseTest2()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            Assert.AreEqual("content from file", target.parse("[File get(\"file\")]"));
        }

        /// <summary>
        ///A test for parse - stGet
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(UnspecSobaScriptException))]
        public void stGetParseTest3()
        {
            FileComponentAccessor target = new FileComponentAccessor(true);
            target.parse("[File get(\"file\")]");
        }

        /// <summary>
        ///A test for parse - stCall
        ///</summary>
        [TestMethod()]
        public void stCallParseTest1()
        {
            var target = new FileComponent();

            try {
                target.parse("[File call(file)]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[File out(file)]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - stCall
        ///</summary>
        [TestMethod()]
        public void stCallParseTest3()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            Assert.AreEqual("stdout", target.parse("[File call(\"file\")]"));
            Assert.AreEqual("stdout", target.parse("[File call(\"file\", \"args\")]"));
            Assert.AreEqual("silent stdout", target.parse("[File scall(\"file\")]"));
            Assert.AreEqual("silent stdout", target.parse("[File scall(\"file\", \"args\")]"));
            Assert.AreEqual("silent stdout", target.parse("[File sout(\"file\")]"));
            Assert.AreEqual("silent stdout", target.parse("[File sout(\"file\", \"args\")]"));
        }

        /// <summary>
        ///A test for parse - stCall
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(PMArgException))]
        public void stCallParseTest5()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File call(\"file\", \"args\", \"10\")]");
        }

        /// <summary>
        ///A test for parse - stCall
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(PMArgException))]
        public void stCallParseTest6()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File call(\"file\", 10)]");
        }

        /// <summary>
        ///A test for parse - stCall
        ///</summary>
        [TestMethod()]
        public void stCallParseTest7()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            Assert.AreEqual("stdout", target.parse("[File call(\"file\", \"args\", 0)]"));
            Assert.AreEqual("silent stdout", target.parse("[File scall(\"file\", \"args\", 15)]"));
            Assert.AreEqual("silent stdout", target.parse("[File sout(\"file\", \"args\", 10)]"));
        }

        /// <summary>
        ///A test for parse - stCall
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void stCallParseTest8()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File call(\"file\", \"args\", )]");
        }

        /// <summary>
        ///A test for parse - stWrite
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void stWriteParseTest1()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File write(\"file\")]");
        }

        /// <summary>
        ///A test for parse - stWrite
        ///</summary>
        [TestMethod()]
        public void stWriteParseTest2()
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
        public void stWriteParseTest3()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File write(\"file\"): multi\nline\" \n 'data'.]");
            Assert.AreEqual(" multi\nline\" \n 'data'.", target.parse("[File get(\"file\")]"));
        }

        /// <summary>
        ///A test for parse - stWrite
        ///</summary>
        [TestMethod()]
        public void stWriteParseTest5()
        {
            var target = new FileComponent();

            try {
                target.parse("[File write(\"file\", true):data]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[File write(\"file\", true, true):data]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[File write(\"file\", \"true\", \"true\", \"utf-8\"):data]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[File append(\"file\", true, true, \"utf-8\"):data]");
                Assert.Fail("4");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[File appendLine(\"file\", true, true, \"utf-8\"):data]");
                Assert.Fail("5");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[File writeLine(\"file\", true, true, \"utf-8\"):data]");
                Assert.Fail("6");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - stReplace
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(PMArgException))]
        public void stReplaceParseTest1()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File replace(file, pattern, replacement)]");
        }

        /// <summary>
        ///A test for parse - stReplace
        ///</summary>
        [TestMethod()]
        public void stReplaceParseTest2()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File replace(\"file\", \"from\", \"to\")]");
            Assert.AreEqual("content to file", target.parse("[File get(\"file\")]"));
        }

        /// <summary>
        ///A test for parse - stReplace
        ///</summary>
        [TestMethod()]
        public void stReplaceParseTest3()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File replace.Regexp(\"file\", \"t\\s*from\", \"t to\")]");
            Assert.AreEqual("content to file", target.parse("[File get(\"file\")]"));
        }

        /// <summary>
        ///A test for parse - stReplace
        ///</summary>
        [TestMethod()]
        public void stReplaceParseTest4()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File replace.Wildcards(\"file\", \"con*from \", \"\")]");
            Assert.AreEqual("file", target.parse("[File get(\"file\")]"));
        }

        /// <summary>
        ///A test for parse - stReplace
        ///</summary>
        [TestMethod()]
        public void stReplaceParseTest5()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File replace.Regex(\"file\", \"t\\s*from\", \"t to\")]");
            Assert.AreEqual("content to file", target.parse("[File get(\"file\")]"));
        }

        /// <summary>
        ///A test for parse - stExists
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void stExistsParseTest1()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            target.parse("[File exists.stub(\"path\")]");
        }

        /// <summary>
        ///A test for parse - stExists
        ///</summary>
        [TestMethod()]
        public void stExistsParseTest2()
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
        public void stExistsParseTest3()
        {
            FileComponentAccessor target = new FileComponentAccessor();

            Assert.AreEqual(Value.VFALSE, target.parse("[File exists.directory(\"System32\", false)]"));
            //Assert.AreEqual(Value.VFALSE, target.parse("[File exists.directory(\"" + realDir + "\", true)]"));
            Assert.AreEqual(Value.VTRUE, target.parse("[File exists.directory(\"System32\", true)]"));
        }

        /// <summary>
        ///A test for parse - stExists
        ///</summary>
        [TestMethod()]
        public void stExistsParseTest4()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            string realFile = Assembly.GetAssembly(GetType()).Location;

            Assert.AreEqual(Value.VFALSE, target.parse("[File exists.file(\"file\")]"));
            Assert.AreEqual(Value.VTRUE, target.parse("[File exists.file(\"" + realFile + "\")]"));
        }

        /// <summary>
        ///A test for parse - stExists
        ///</summary>
        [TestMethod()]
        public void stExistsParseTest5()
        {
            FileComponentAccessor target = new FileComponentAccessor();
            string realFile = Path.GetFileName(Assembly.GetAssembly(GetType()).Location);

            Assert.AreEqual(Value.VFALSE, target.parse("[File exists.file(\"cmd.exe\", false)]"));
            //Assert.AreEqual(Value.VFALSE, target.parse("[File exists.file(\"" + realFile + "\", true)]"));
            Assert.AreEqual(Value.VTRUE, target.parse("[File exists.file(\"cmd.exe\", true)]"));
        }

        /// <summary>
        ///A test for parse - stRemote
        ///</summary>
        [TestMethod()]
        public void stRemoteTest1()
        {
            var target = new FileComponent();

            try {
                target.parse("[File remote]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[File remote.download]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[File remote.notRealNode]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[File remote.download(\"addr\", \"file\").notRealNode]");
                Assert.Fail("4");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotSupportedOperationException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - stRemote
        ///</summary>
        [TestMethod()]
        public void stRemoteTest2()
        {
            var target = new FileComponent();

            try {
                target.parse("[File remote.download()]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[File remote.download(\"addr\")]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[File remote.download(\"addr\", \"file\", \"user\")]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - stRemote
        ///</summary>
        [TestMethod()]
        public void stRemoteTest3()
        {
            var target = new FileComponentDownloadAccessor();

            Assert.AreEqual(Value.Empty, target.parse("[File remote.download(\"ftp://192.168.17.04:2021/dir1/non-api.png\", \"non-api.png\", \"user1\", \"mypass123\")]"));
            Assert.AreEqual(target.addr, "ftp://192.168.17.04:2021/dir1/non-api.png");
            Assert.AreEqual(target.output, "non-api.png");
            Assert.AreEqual(target.user, "user1");
            Assert.AreEqual(target.pwd, "mypass123");
        }

        /// <summary>
        ///A test for parse - stCopy
        ///</summary>
        [TestMethod()]
        public void stCopyTest1()
        {
            var target = new FileComponent();

            try {
                target.parse("[File copy]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[File copy.file]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[File copy.directory]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[File copy.notRalNode]");
                Assert.Fail("4");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - stCopy - file
        ///</summary>
        [TestMethod()]
        public void stCopyFileTest1()
        {
            var target = new FileComponent();
            
            try {
                target.parse("[File copy.file()]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[File copy.file(false)]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[File copy.file(\" \", \"dest\", false)]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }

            try {
                target.parse("[File copy.file(\"src\", \" \", false)]");
                Assert.Fail("4");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - stCopy - file
        ///</summary>
        [TestMethod()]
        public void stCopyFileTest2()
        {
            var target = new FileComponentCopyFileAccessor();

            try {
                target.parse("[File copy.file(\"src\", \"dest\", false, {\"src\"})]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }


            target.parse("[File copy.file(\"src\", \"dest\", false, {\"notexists\"})]");
            try {
                target.parse("[File copy.file(\"src\", \"dest\", false, {\"notexists\", false})]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }


            Assert.AreEqual(Value.Empty, target.parse("[File copy.file(\"path\\subdir1\\file1.txt\", \"path2\\file2.txt\", true)]"));
            try {
                Assert.AreEqual(Value.Empty, target.parse("[File copy.file(\"path\\subdir1\\file1.txt\", \"path2\\file2.txt\", true, {\"file1.txt\"})]"));
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - stCopy - copy.file(string src, string dest, bool overwrite, object except)
        ///</summary>
        [TestMethod()]
        public void stCopyFileTest3()
        {
            var target = new FileComponentCopyFileAccessor();

            Assert.AreEqual(Value.Empty, target.parse("[File copy.file(\"path\\subdir1\\file1.txt\", \"path2\\file2.txt\", true, {\"file2.txt\"})]"));
            Assert.AreEqual(true, target.cmpPaths("path2", target.destDir));
            Assert.AreEqual(true, target.cmpPaths("file2.txt", target.destFile));
            Assert.AreEqual(true, target.overwrite);
            Assert.AreEqual(1, target.files.Length);
            Assert.AreEqual(true, target.cmpPaths("path\\subdir1\\file1.txt", target.files[0]));
        }

        /// <summary>
        ///A test for parse - stCopy - copy.file(string src, string dest, bool overwrite)
        ///</summary>
        [TestMethod()]
        public void stCopyFileTest4()
        {
            var target = new FileComponentCopyFileAccessor();

            Assert.AreEqual(Value.Empty, target.parse("[File copy.file(\"path\\subdir1\\file1.txt\", \"path2\\file2.txt\", true)]"));
            Assert.AreEqual(true, target.cmpPaths("path2", target.destDir));
            Assert.AreEqual(true, target.cmpPaths("file2.txt", target.destFile));
            Assert.AreEqual(true, target.overwrite);
            Assert.AreEqual(1, target.files.Length);
            Assert.AreEqual(true, target.cmpPaths("path\\subdir1\\file1.txt", target.files[0]));
        }

        /// <summary>
        ///A test for parse - stCopy - directory
        ///</summary>
        [TestMethod()]
        public void stCopyDirectoryTest1()
        {
            var target = new FileComponent();
            
            try {
                target.parse("[File copy.directory()]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[File copy.directory(false)]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[File copy.directory(\" \", \"dest\", false)]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }

            try {
                target.parse("[File copy.directory(\"src\", \" \", false)]");
                Assert.Fail("4");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - stCopy - copy.directory(string src, string dest, bool force [, bool overwrite])
        ///</summary>
        [TestMethod()]
        public void stCopyDirectoryTest2()
        {
            var target = new FileComponentCopyDirectoryAccessor();

            using(var tf = new TempFile(true)) {
                Assert.AreEqual(Value.Empty, target.parse("[File copy.directory(\"" + tf.dir + "\", \"path2\\sub1\", true, true)]"));
                Assert.AreEqual(true, target.cmpPaths("path2\\sub1", target.dest));
                Assert.AreEqual(true, target.force);
                Assert.AreEqual(true, target.overwrite);
                Assert.AreEqual(1, target.files.Count());
                Assert.AreEqual(true, target.cmpPaths(tf.file, target.files.ElementAt(0)[0]));

                Assert.AreEqual(Value.Empty, target.parse("[File copy.directory(\"" + tf.dir + "\", \"path2\\sub1\", true)]"));
                Assert.AreEqual(true, target.force);
                Assert.AreEqual(false, target.overwrite);
            }
        }

        /// <summary>
        ///A test for parse - stCopy - copy.directory - mkdir
        ///</summary>
        [TestMethod()]
        public void stCopyDirectoryTest3()
        {
            var target = new FileComponentCopyDirectoryAccessor();

            try {
                target.parse("[File copy.directory(\"\", \"path2\\sub1\", false, true)]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }

            Assert.AreEqual(Value.Empty, target.parse("[File copy.directory(\"\", \"path2\\sub1\", true)]"));
            Assert.AreEqual(true, target.cmpPaths("path2\\sub1", target.dest));
        }

        /// <summary>
        ///A test for parse - stDelete
        ///</summary>
        [TestMethod()]
        public void stDeleteTest1()
        {
            var target = new FileComponent();

            try {
                target.parse("[File delete]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[File delete.files]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[File delete.directory]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[File delete.notRalNode]");
                Assert.Fail("4");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - stDelete - Files
        ///</summary>
        [TestMethod()]
        public void stDeleteFilesTest1()
        {
            var target = new FileComponent();

            try {
                target.parse("[File delete.files(\"file\")]");
                Assert.Fail("1");
            }
            catch(Exception ex) {
                Assert.IsTrue(ex.GetType() == typeof(PMArgException));
            }

            try {
                target.parse("[File delete.files({\"file\", false})]");
                Assert.Fail("2");
            }
            catch(Exception ex) {
                Assert.IsTrue(ex.GetType() == typeof(ArgumentException));
            }

            try {
                target.parse("[File delete.files({\"file\"}, {true})]");
                Assert.Fail("3");
            }
            catch(Exception ex) {
                Assert.IsTrue(ex.GetType() == typeof(ArgumentException));
            }
        }

        /// <summary>
        ///A test for parse - stDelete - Files
        ///</summary>
        [TestMethod()]
        public void stDeleteFilesTest2()
        {
            var target = new FileComponentDeleteFilesAccessor();

            Assert.AreEqual(Value.Empty, target.parse("[IO delete.files({\"file1\", \"file2\", \"file3\"})]"));
            Assert.AreEqual(3, target.files.Length);
            Assert.AreEqual(true, target.cmpPaths("file1", target.files[0]));
            Assert.AreEqual(true, target.cmpPaths("file2", target.files[1]));
            Assert.AreEqual(true, target.cmpPaths("file3", target.files[2]));
        }

        /// <summary>
        ///A test for parse - stDelete - Files
        ///</summary>
        [TestMethod()]
        public void stDeleteFilesTest3()
        {
            var target = new FileComponentDeleteFilesAccessor();

            Assert.AreEqual(Value.Empty, target.parse("[IO delete.files({\"file1\", \"file2\", \"file3\"}, {\"file2\", \"file1\"})]"));
            Assert.AreEqual(1, target.files.Length);
            Assert.AreEqual(true, target.cmpPaths("file3", target.files[0]));
        }

        /// <summary>
        ///A test for parse - stDelete - directory
        ///</summary>
        [TestMethod()]
        public void stDeleteDirectoryTest1()
        {
            var target = new FileComponentDeleteDirectoryAccessor();

            try {
                target.parse("[File delete.directory(\"dir\")]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[File delete.directory(\"  \", false)]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }
        }

        private class FileComponentAccessor: FileComponent
        {
            public bool throwError = false;
            protected string content = "content from file";

            public FileComponentAccessor(bool throwError = false)
            {
                this.throwError = throwError;
            }

            protected override string readToEnd(string file, Encoding enc, bool detectEncoding)
            {
                if(throwError) {
                    throw new System.IO.FileNotFoundException(String.Format("Some error for '{0}'", file));
                }
                return content;
            }

            protected override Encoding detectEncodingFromFile(string file)
            {
                return Encoding.UTF8;
            }

            protected override void writeToFile(string file, string data, bool append, bool writeLine, Encoding enc)
            {
                if(throwError) {
                    throw new System.IO.IOException(String.Format("Some error for '{0}'", file));
                }
                content = data;
            }

            protected override string findFile(string file)
            {
                return file;
            }

            protected override string run(string file, string args, bool silent, bool stdOut, int timeout = 0)
            {
                if(throwError) {
                    throw new ComponentException(String.Format("Some error for '{0} {1}'", file, args));
                }
                return String.Format("{0}stdout", silent? "silent ": String.Empty);
            }
        }

        private class FileComponentDownloadAccessor: FileComponent
        {
            public string addr, output, user, pwd;

            protected override string download(string addr, string output, string user = null, string pwd = null)
            {
                this.addr   = addr;
                this.output = output;
                this.user   = user;
                this.pwd    = pwd;
                //return base.download(addr, output, user, pwd);
                return Value.Empty;
            }
        }

        private class FileComponentPath: FileComponent
        {
            public bool cmpPaths(string p1, string p2)
            {
                return p1.TrimStart(Path.DirectorySeparatorChar) == p2.TrimStart(Path.DirectorySeparatorChar);
            }
        }

        private class FileComponentCopyFileAccessor: FileComponentPath
        {
            public string destDir, destFile;
            public bool overwrite;
            public string[] files;

            protected override void copyFile(string destDir, string destFile, bool overwrite, params string[] files)
            {
                this.destDir    = destDir.TrimStart(Path.PathSeparator);
                this.destFile   = destFile.TrimStart(Path.PathSeparator);
                this.overwrite  = overwrite;
                this.files      = files;
                //base.copyFile(destDir, destFile, overwrite, files);
            }
        }

        private class FileComponentCopyDirectoryAccessor: FileComponentPath
        {
            public IEnumerable<string[]> files;
            public string dest;
            public bool force, overwrite;

            protected override void copyDirectory(IEnumerable<string[]> files, string dest, bool force, bool overwrite)
            {
                this.files      = files;
                this.dest       = dest;
                this.force      = force;
                this.overwrite  = overwrite;
                //base.copyDirectory(files, dest, force, overwrite);
            }

            protected override void mkdir(string path)
            {
                this.dest = path;
                //base.mkdir(path);
            }
        }

        private class FileComponentDeleteFilesAccessor: FileComponentPath
        {
            public string[] files;

            protected override void deleteFiles(string[] files)
            {
                this.files = files;
                //base.deleteFiles(files);
            }
        }

        private class FileComponentDeleteDirectoryAccessor: FileComponentPath
        {
            protected override void deleteDirectory(string src, bool force)
            {
                //base.deleteDirectory(src, force);
            }
        }
    }
}
