using System;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.Ext;
using SobaScript.Z.ExtTest.Stubs;
using Xunit;

namespace SobaScript.Z.ExtTest
{
    public class SevenZipComponentTest
    {
        [Fact]
        public void ParseTest1()
        {
            var target = new SevenZipComponent(new Soba(), new NullArchiver(), ".");

            Assert.Throws<SubtypeNotFoundException>(() =>
                target.Eval("[7z NotRealSubtype.check]")
            );
        }

        [Fact]
        public void PackTest1()
        {
            var target = new SevenZipComponent(new Soba(), new NullArchiver(), ".");

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[7z pack]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[7z pack files]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[7z pack.files()]")
            );
        }

        [Fact]
        public void PackFilesTest1()
        {
            var target = new SevenZipComponent(new Soba(), new NullArchiver(), ".");

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[7z pack.files(\"files\", \"output\")]")
            );

            Assert.Throws<PMArgException>(() =>
                target.Eval("[7z pack.files({\"f1\", 12}, \"output\")]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[7z pack.files({\"f1\", \"f2\"}, \"output\", SevenZip)]")
            );

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[7z pack.files({\"f1\"}, \"output\").right]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[7z pack.files()]")
            );
        }

        [Fact]
        public void PackFilesTest2()
        {
            var target = new SevenZipComponent(new Soba(), new NullArchiver(), ".");

            Assert.Throws<PMArgException>(() =>
                target.Eval("[7z pack.files({}, \"output\")]")
            );

            using(var tf = new TempFile())
            {
                Assert.Throws<ArgumentException>(() =>
                    target.Eval("[7z pack.files({\"" + tf.File + "\", \" \"}, \"output\")]")
                );

                Assert.Throws<ArgumentException>(() =>
                    target.Eval("[7z pack.files({\" \", \"" + tf.File + "\"}, \"output\")]")
                );

                Assert.Throws<ArgumentException>(() =>
                    target.Eval("[7z pack.files({\"" + tf.File + "\"}, \" \")]")
                );
            }

            Assert.Throws<NotFoundException>(() =>
                target.Eval("[7z pack.files({\"thisisreallyisnotreal.file\", \" \"}, \"output\")]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[7z pack.files({\" \"}, \"output\")]")
            );
        }

        [Fact]
        public void PackFilesTest3()
        {
            var target = new SevenZipComponentInputFilesStub();

            //TODO
            string zip = Guid.NewGuid().ToString() + ".zip";
            using(var tf = new TempFile(false)) {
                Assert.Equal(Value.Empty, target.Eval("[7z pack.files({\"" + tf.File + "\"}, \"" + zip + "\")]"));
                Assert.Equal(Value.Empty, target.Eval("[7z pack.files({\"" + tf.File + "\"}, \"" + zip + "\", SevenZip, Lzma2, 4)]"));
            }
        }

        [Fact]
        public void PackFilesTest4()
        {
            var target  = new SevenZipComponentInputFilesStub();

            string zip = Guid.NewGuid().ToString() + ".zip";

            using(var tf1 = new TempFile())
            using(var tf2 = new TempFile())
            using(var tf3 = new TempFile())
            {
                Assert.Equal(Value.Empty, target.Eval("[7z pack.files({\"" + tf1.File + "\", \"" + tf2.File + "\"}, \"" + zip + "\", {\"" + tf1.File + "\"})]"));
                Assert.Single(target.Archiver.FilesInput);
                Assert.Equal(tf2.File, target.Archiver.FilesInput[0]);

                Assert.Equal(Value.Empty, target.Eval("[7z pack.files({\"" + tf1.File + "\", \"" + tf2.File + "\", \"" + tf3.File + "\"}, \"" + zip + "\", {\"" + tf2.File + "\"}, SevenZip, Lzma2, 4)]"));
                Assert.Equal(2, target.Archiver.FilesInput.Count);
                Assert.Equal(tf1.File, target.Archiver.FilesInput[0]);
                Assert.Equal(tf3.File, target.Archiver.FilesInput[1]);
            }
        }

        [Fact]
        public void PackDirectoryTest1()
        {
            var target = new SevenZipComponent(new Soba(), new NullArchiver(), ".");

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[7z pack.directory(\" \", \"name.zip\")]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[7z pack.directory(\"pathtodir\", \" \")]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[7z pack.directory(\"dir\")]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[7z pack.directory(\"dir\", \"output\", SevenZip)]")
            );

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[7z pack.directory(\"dir\", \"output\").right]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[7z pack.directory()]")
            );
        }

        [Fact]
        public void PackDirectoryTest2()
        {
            var target = new SevenZipComponentInputFilesStub();

            Assert.Throws<NotFoundException>(() =>
                target.Eval("[7z pack.directory(\"notrealdirfortest\", \"output\")]")
            );

            //TODO
            string zip = Guid.NewGuid().ToString() + ".zip";
            using(var tf = new TempFile(true)) {
                Assert.Equal(Value.Empty, target.Eval("[7z pack.directory(\"" + tf.Dir + "\", \"" + zip + "\")]"));
                Assert.Equal(Value.Empty, target.Eval("[7z pack.directory(\"" + tf.Dir + "\", \"" + zip + "\", SevenZip, Lzma2, 4)]"));
            }
        }

        [Fact]
        public void UnpackTest1()
        {
            var target = new SevenZipComponent(new Soba(), new NullArchiver(), ".");

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[7z unpack]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[7z unpack()]")
            );
        }

        [Fact]
        public void UnpackTest2()
        {
            var target = new SevenZipComponentExtractArchiveStub();

            Assert.Throws<NotFoundException>(() =>
                target.Eval("[7z  unpack(\"f1.zip\")]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[7z  unpack(\" \")]")
            );

            using(var tf = new TempFile(false, ".zip"))
            {
                Assert.Equal(Value.Empty, target.Eval("[7z  unpack(\""+ tf.File + "\")]"));
                Assert.Equal(target.file, target.Location(tf.File));
                Assert.Equal(target.output, target.GetDirectoryFromFile(tf.File));
                Assert.False(target.delete);
                Assert.Null(target.pwd);
            }

            using(var tf = new TempFile(false, ".zip"))
            {
                Assert.Equal(Value.Empty, target.Eval("[7z  unpack(\""+ tf.File + "\", true)]"));
                Assert.Equal(target.file, target.Location(tf.File));
                Assert.Equal(target.output, target.GetDirectoryFromFile(tf.File));
                Assert.True(target.delete);
                Assert.Null(target.pwd);
            }

            using(var tf = new TempFile(false, ".zip"))
            {
                Assert.Equal(Value.Empty, target.Eval("[7z  unpack(\"" + tf.File + "\", true, \"pass-123\")]"));
                Assert.Equal(target.file, target.Location(tf.File));
                Assert.Equal(target.output, target.GetDirectoryFromFile(tf.File));
                Assert.True(target.delete);
                Assert.Equal("pass-123", target.pwd);
            }
        }

        [Fact]
        public void UnpackTest3()
        {
            var target = new SevenZipComponentExtractArchiveStub();

            Assert.Throws<ArgumentException>(() =>
            {
                using(var tf = new TempFile(false, ".zip")) {
                    Assert.Equal(Value.Empty, target.Eval("[7z  unpack(\"" + tf.File + "\", \" \")]"));
                }
            });

            using(var tf = new TempFile(false, ".zip"))
            {
                Assert.Equal(Value.Empty, target.Eval("[7z  unpack(\"" + tf.File + "\", \"output-path\")]"));
                Assert.Equal(target.file, target.Location(tf.File));
                Assert.Equal(target.output, target.Location("output-path"));
                Assert.False(target.delete);
                Assert.Null(target.pwd);
            }

            using(var tf = new TempFile(false, ".zip")) {
                Assert.Equal(Value.Empty, target.Eval("[7z  unpack(\"" + tf.File + "\", \"output-path\", \"pass-123\")]"));
                Assert.Equal(target.file, target.Location(tf.File));
                Assert.Equal(target.output, target.Location("output-path"));
                Assert.False(target.delete);
                Assert.Equal("pass-123", target.pwd);
            }

            using(var tf = new TempFile(false, ".zip"))
            {
                Assert.Equal(Value.Empty, target.Eval("[7z  unpack(\"" + tf.File + "\", \"output-path\", true)]"));
                Assert.Equal(target.file, target.Location(tf.File));
                Assert.Equal(target.output, target.Location("output-path"));
                Assert.True(target.delete);
                Assert.Null(target.pwd);
            }

            using(var tf = new TempFile(false, ".zip"))
            {
                Assert.Equal(Value.Empty, target.Eval("[7z  unpack(\"" + tf.File + "\", \"output-path\", true, \"pass-123\")]"));
                Assert.Equal(target.file, target.Location(tf.File));
                Assert.Equal(target.output, target.Location("output-path"));
                Assert.True(target.delete);
                Assert.Equal("pass-123", target.pwd);
            }
        }

        [Fact]
        public void CheckTest1()
        {
            var target = new SevenZipComponent(new Soba(), new NullArchiver(), "");

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[7z check]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[7z check()]")
            );
        }

        [Fact]
        public void CheckTest2()
        {
            var target = new SevenZipComponentCheckArchiveStub();

            Assert.Throws<NotFoundException>(() =>
                target.Eval("[7z  check(\"thisisreallyisnotrealfile.zip\")]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[7z  check(\" \")]")
            );

            using(var tf = new TempFile(false, ".zip"))
            {
                target.Eval("[7z  check(\""+ tf.File +"\")]");
                Assert.Equal(target.Location(tf.File), target.file);
                Assert.Null(target.pwd);
            }

            using(var tf = new TempFile(false, ".zip"))
            {
                target.Eval("[7z  check(\"" + tf.File + "\", \"pass-123\")]");
                Assert.Equal(target.Location(tf.File), target.file);
                Assert.Equal("pass-123", target.pwd);
            }
        }
    }
}