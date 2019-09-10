using System;
using System.IO;
using System.Linq;
using System.Reflection;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.Ext;
using SobaScript.Z.ExtTest.Stubs;
using Xunit;

namespace SobaScript.Z.ExtTest
{
    public class FileComponentTest
    {
        [Fact]
        public void ParseTest1()
        {
            var target = new FileComponent(new Soba(), "");

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("#[File get(\"file\")]")
            );

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("File get(\"file\")")
            );
        }

        [Fact]
        public void StGetParseTest1()
        {
            var target = new FileComponent(new Soba(), "");

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File get(file)]")
            );
        }

        [Fact]
        public void StGetParseTest2()
        {
            var target = new FileComponentAcs();
            Assert.Equal("content from file", target.Eval("[File get(\"file\")]"));
        }

        [Fact]
        public void StGetParseTest3()
        {
            var target = new FileComponentAcs(true);

            Assert.Throws<FileNotFoundException>(() =>
                target.Eval("[File get(\"file\")]")
            );
        }

        [Fact]
        public void StCallParseTest1()
        {
            var target = new FileComponent(new Soba(), "");

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File call(file)]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File out(file)]")
            );
        }

        [Fact]
        public void StCallParseTest3()
        {
            var target = new FileComponentAcs();
            Assert.Equal("stdout", target.Eval("[File call(\"file\")]"));
            Assert.Equal("stdout", target.Eval("[File call(\"file\", \"args\")]"));
            Assert.Equal("silent stdout", target.Eval("[File scall(\"file\")]"));
            Assert.Equal("silent stdout", target.Eval("[File scall(\"file\", \"args\")]"));
            Assert.Equal("silent stdout", target.Eval("[File sout(\"file\")]"));
            Assert.Equal("silent stdout", target.Eval("[File sout(\"file\", \"args\")]"));
        }

        [Fact]
        public void StCallParseTest5()
        {
            var target = new FileComponentAcs();

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File call(\"file\", \"args\", \"10\")]")
            );
        }

        [Fact]
        public void StCallParseTest6()
        {
            var target = new FileComponentAcs();

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File call(\"file\", 10)]")
            );
        }

        [Fact]
        public void StCallParseTest7()
        {
            var target = new FileComponentAcs();
            Assert.Equal("stdout", target.Eval("[File call(\"file\", \"args\", 0)]"));
            Assert.Equal("silent stdout", target.Eval("[File scall(\"file\", \"args\", 15)]"));
            Assert.Equal("silent stdout", target.Eval("[File sout(\"file\", \"args\", 10)]"));
        }

        [Fact]
        public void StCallParseTest8()
        {
            var target = new FileComponentAcs();

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[File call(\"file\", \"args\", )]")
            );
        }

        [Fact]
        public void StWriteParseTest1()
        {
            var target = new FileComponentAcs();

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[File write(\"file\")]")
            );
        }

        [Fact]
        public void StWriteParseTest2()
        {
            var target = new FileComponentAcs();
            target.Eval("[File write(\"file\"):data]");
            Assert.Equal("data", target.Eval("[File get(\"file\")]"));

            // append, writeLine, appendLine is identical for current accessor
            // however, also need checking the entry point as for the write() above:

            target.Eval("[File append(\"file\"):data]");
            Assert.Equal("data", target.Eval("[File get(\"file\")]"));
            target.Eval("[File writeLine(\"file\"):data]");
            Assert.Equal("data", target.Eval("[File get(\"file\")]"));
            target.Eval("[File appendLine(\"file\"):data]");
            Assert.Equal("data", target.Eval("[File get(\"file\")]"));
        }

        [Fact]
        public void StWriteParseTest3()
        {
            var target = new FileComponentAcs();
            target.Eval("[File write(\"file\"): multi\nline\" \n 'data'.]");
            Assert.Equal(" multi\nline\" \n 'data'.", target.Eval("[File get(\"file\")]"));
        }

        [Fact]
        public void StWriteParseTest5()
        {
            var target = new FileComponent(new Soba(), "");

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File write(\"file\", true):data]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File write(\"file\", true, true):data]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File write(\"file\", \"true\", \"true\", \"utf-8\"):data]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File append(\"file\", true, true, \"utf-8\"):data]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File appendLine(\"file\", true, true, \"utf-8\"):data]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File writeLine(\"file\", true, true, \"utf-8\"):data]")
            );
        }

        [Fact]
        public void StReplaceParseTest1()
        {
            var target = new FileComponentAcs();

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File replace(file, pattern, replacement)]")
            );
        }

        [Fact]
        public void StReplaceParseTest2()
        {
            var target = new FileComponentAcs();
            target.Eval("[File replace(\"file\", \"from\", \"to\")]");
            Assert.Equal("content to file", target.Eval("[File get(\"file\")]"));
        }

        [Fact]
        public void StReplaceParseTest3()
        {
            var target = new FileComponentAcs();
            target.Eval("[File replace.Regexp(\"file\", \"t\\s*from\", \"t to\")]");
            Assert.Equal("content to file", target.Eval("[File get(\"file\")]"));
        }

        [Fact]
        public void StReplaceParseTest4()
        {
            var target = new FileComponentAcs();
            target.Eval("[File replace.Wildcards(\"file\", \"con*from \", \"\")]");
            Assert.Equal("file", target.Eval("[File get(\"file\")]"));
        }

        [Fact]
        public void StReplaceParseTest5()
        {
            var target = new FileComponentAcs();
            target.Eval("[File replace.Regex(\"file\", \"t\\s*from\", \"t to\")]");
            Assert.Equal("content to file", target.Eval("[File get(\"file\")]"));
        }

        [Fact]
        public void StExistsParseTest1()
        {
            var target = new FileComponentAcs();

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[File exists.stub(\"path\")]")
            );
        }

        [Fact]
        public void StExistsParseTest2()
        {
            var target = new FileComponentAcs();
            string realDir = Path.GetDirectoryName(Assembly.GetAssembly(GetType()).Location);

            Assert.Equal(Value.FALSE, target.Eval("[File exists.directory(\"c:\\stubdir\\notexist\")]"));
            Assert.Equal(Value.TRUE, target.Eval("[File exists.directory(\"" + realDir + "\")]"));
        }

        [Fact]
        public void StExistsParseTest3()
        {
            var target = new FileComponentAcs();

            Assert.Equal(Value.FALSE, target.Eval("[File exists.directory(\"System32\", false)]"));
            //Assert.Equal(Value.VFALSE, target.parse("[File exists.directory(\"" + realDir + "\", true)]"));
            Assert.Equal(Value.TRUE, target.Eval("[File exists.directory(\"System32\", true)]"));
        }

        [Fact]
        public void StExistsParseTest4()
        {
            var target = new FileComponentAcs();
            string realFile = Assembly.GetAssembly(GetType()).Location;

            Assert.Equal(Value.FALSE, target.Eval("[File exists.file(\"file\")]"));
            Assert.Equal(Value.TRUE, target.Eval("[File exists.file(\"" + realFile + "\")]"));
        }

        [Fact]
        public void StExistsParseTest5()
        {
            var target = new FileComponentAcs();
            string realFile = Path.GetFileName(Assembly.GetAssembly(GetType()).Location);

            Assert.Equal(Value.FALSE, target.Eval("[File exists.file(\"cmd.exe\", false)]"));
            //Assert.Equal(Value.VFALSE, target.parse("[File exists.file(\"" + realFile + "\", true)]"));
            Assert.Equal(Value.TRUE, target.Eval("[File exists.file(\"cmd.exe\", true)]"));
        }

        [Fact]
        public void StRemoteTest1()
        {
            var target = new FileComponent(new Soba(), "");

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[File remote]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[File remote.download]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[File remote.notRealNode]")
            );

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[File remote.download(\"addr\", \"file\").notRealNode]")
            );
        }

        [Fact]
        public void StRemoteTest2()
        {
            var target = new FileComponent(new Soba(), "");

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File remote.download()]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File remote.download(\"addr\")]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File remote.download(\"addr\", \"file\", \"user\")]")
            );
        }

        [Fact]
        public void StRemoteTest3()
        {
            var target = new FileComponentDownloadStub();

            Assert.Equal(Value.Empty, target.Eval("[File remote.download(\"ftp://192.168.17.04:2021/dir1/non-api.png\", \"non-api.png\", \"user1\", \"mypass123\")]"));
            Assert.Equal("ftp://192.168.17.04:2021/dir1/non-api.png", target.addr);
            Assert.Equal("non-api.png", target.output);
            Assert.Equal("user1", target.user);
            Assert.Equal("mypass123", target.pwd);
        }

        [Fact]
        public void StCopyTest1()
        {
            var target = new FileComponent(new Soba(), "");

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[File copy]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[File copy.file]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[File copy.directory]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[File copy.notRalNode]")
            );
        }

        [Fact]
        public void StCopyFileTest1()
        {
            var target = new FileComponent(new Soba(), "");

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File copy.file()]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File copy.file(false)]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[File copy.file(\" \", \"dest\", false)]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[File copy.file(\"src\", \" \", false)]")
            );
        }

        [Fact]
        public void StCopyFileTest2()
        {
            var target = new FileComponentCopyFileStub();

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[File copy.file(\"src\", \"dest\", false, {\"src\"})]")
            );

            target.Eval("[File copy.file(\"src\", \"dest\", false, {\"notexists\"})]");
            Assert.Throws<PMArgException>(() =>
                target.Eval("[File copy.file(\"src\", \"dest\", false, {\"notexists\", false})]")
            );


            Assert.Equal(Value.Empty, target.Eval("[File copy.file(\"path\\subdir1\\file1.txt\", \"path2\\file2.txt\", true)]"));
            Assert.Throws<ArgumentException>(() =>
                Assert.Equal(Value.Empty, target.Eval("[File copy.file(\"path\\subdir1\\file1.txt\", \"path2\\file2.txt\", true, {\"file1.txt\"})]"))
            );
        }

        [Fact]
        public void StCopyFileTest3()
        {
            var target = new FileComponentCopyFileStub();

            Assert.Equal(Value.Empty, target.Eval("[File copy.file(\"path\\subdir1\\file1.txt\", \"path2\\file2.txt\", true, {\"file2.txt\"})]"));
            Assert.True(target.CmpPaths("path2", target.destDir));
            Assert.True(target.CmpPaths("file2.txt", target.destFile));
            Assert.True(target.overwrite);
            Assert.Single(target.files);
            Assert.True(target.CmpPaths("path\\subdir1\\file1.txt", target.files[0]));
        }

        [Fact]
        public void StCopyFileTest4()
        {
            var target = new FileComponentCopyFileStub();

            Assert.Equal(Value.Empty, target.Eval("[File copy.file(\"path\\subdir1\\file1.txt\", \"path2\\file2.txt\", true)]"));
            Assert.True(target.CmpPaths("path2", target.destDir));
            Assert.True(target.CmpPaths("file2.txt", target.destFile));
            Assert.True(target.overwrite);
            Assert.Single(target.files);
            Assert.True(target.CmpPaths("path\\subdir1\\file1.txt", target.files[0]));
        }

        [Fact]
        public void StCopyDirectoryTest1()
        {
            var target = new FileComponent(new Soba(), "");

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File copy.directory()]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File copy.directory(false)]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[File copy.directory(\" \", \"dest\", false)]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[File copy.directory(\"src\", \" \", false)]")
            );
        }

        [Fact]
        public void StCopyDirectoryTest2()
        {
            var target = new FileComponentCopyDirectoryStub();

            using(var tf = new TempFile(true))
            {
                Assert.Equal(Value.Empty, target.Eval("[File copy.directory(\"" + tf.Dir + "\", \"path2\\sub1\", true, true)]"));
                Assert.True(target.CmpPaths("path2\\sub1", target.dest));
                Assert.True(target.force);
                Assert.True(target.overwrite);
                Assert.Single(target.files);
                Assert.True(target.CmpPaths(tf.File, target.files.ElementAt(0)[0]));

                Assert.Equal(Value.Empty, target.Eval("[File copy.directory(\"" + tf.Dir + "\", \"path2\\sub1\", true)]"));
                Assert.True(target.force);
                Assert.False(target.overwrite);
            }
        }

        [Fact]
        public void StCopyDirectoryTest3()
        {
            var target = new FileComponentCopyDirectoryStub();

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[File copy.directory(\"\", \"path2\\sub1\", false, true)]")
            );

            Assert.Equal(Value.Empty, target.Eval("[File copy.directory(\"\", \"path2\\sub1\", true)]"));
            Assert.True(target.CmpPaths("path2\\sub1", target.dest));
        }

        [Fact]
        public void StDeleteTest1()
        {
            var target = new FileComponent(new Soba(), "");

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[File delete]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[File delete.files]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[File delete.directory]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[File delete.notRalNode]")
            );
        }

        [Fact]
        public void StDeleteFilesTest1()
        {
            var target = new FileComponent(new Soba(), "");

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File delete.files(\"file\")]")
            );

            Assert.Throws<PMArgException>(() =>
                target.Eval("[File delete.files({\"file\", false})]")
            );

            Assert.Throws<PMArgException>(() =>
                target.Eval("[File delete.files({\"file\"}, {true})]")
            );
        }

        [Fact]
        public void StDeleteFilesTest2()
        {
            var target = new FileComponentDeleteFilesStub();

            Assert.Equal(Value.Empty, target.Eval("[IO delete.files({\"file1\", \"file2\", \"file3\"})]"));
            Assert.Equal(3, target.files.Length);
            Assert.True(target.CmpPaths("file1", target.files[0]));
            Assert.True(target.CmpPaths("file2", target.files[1]));
            Assert.True(target.CmpPaths("file3", target.files[2]));
        }

        [Fact]
        public void StDeleteFilesTest3()
        {
            var target = new FileComponentDeleteFilesStub();

            Assert.Equal(Value.Empty, target.Eval("[IO delete.files({\"file1\", \"file2\", \"file3\"}, {\"file2\", \"file1\"})]"));
            Assert.Single(target.files);
            Assert.True(target.CmpPaths("file3", target.files[0]));
        }

        [Fact]
        public void StDeleteDirectoryTest1()
        {
            var target = new FileComponentDeleteDirectoryStub();

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[File delete.directory(\"dir\")]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[File delete.directory(\"  \", false)]")
            );
        }
    }
}
