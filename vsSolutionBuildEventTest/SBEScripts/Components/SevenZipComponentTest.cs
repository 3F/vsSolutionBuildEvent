﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Components;
using SevenZip;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    [TestClass()]
    public class SevenZipComponentTest
    {
        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SubtypeNotFoundException))]
        public void parseTest1()
        {
            var target = new SevenZipComponent();
            target.parse("[7z NotRealSubtype.check]");
        }

        /// <summary>
        ///A test for parse - pack
        ///</summary>
        [TestMethod()]
        public void packTest1()
        {
            var target = new SevenZipComponent();

            try {
                target.parse("[7z pack]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[7z pack files]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[7z pack.files()]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMLevelException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - pack.files
        ///</summary>
        [TestMethod()]
        public void packFilesTest1()
        {
            var target = new SevenZipComponent();

            try {
                target.parse("[7z pack.files(\"files\", \"output\")]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMLevelException), ex.GetType().ToString()); }

            try {
                target.parse("[7z pack.files({\"f1\", 12}, \"output\")]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            try {
                target.parse("[7z pack.files({\"f1\", \"f2\"}, \"output\", SevenZip)]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMLevelException), ex.GetType().ToString()); }

            try {
                target.parse("[7z pack.files({\"f1\"}, \"output\").right]");
                Assert.Fail("4");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotSupportedOperationException), ex.GetType().ToString()); }

            try {
                target.parse("[7z pack.files()]");
                Assert.Fail("5");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMLevelException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - pack.files
        ///</summary>
        [TestMethod()]
        public void packFilesTest2()
        {
            var target = new SevenZipComponent();

            try {
                target.parse("[7z pack.files({}, \"output\")]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMArgException), ex.GetType().ToString()); }

            using(var tf = new TempFile())
            {
                try {
                    target.parse("[7z pack.files({\"" + tf.file + "\", \" \"}, \"output\")]");
                    Assert.Fail("2");
                }
                catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }

                try {
                    target.parse("[7z pack.files({\" \", \"" + tf.file + "\"}, \"output\")]");
                    Assert.Fail("3");
                }
                catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }

                try {
                    target.parse("[7z pack.files({\"" + tf.file + "\"}, \" \")]");
                    Assert.Fail("4");
                }
                catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }
            }

            try {
                target.parse("[7z pack.files({\"thisisreallyisnotreal.file\", \" \"}, \"output\")]");
                Assert.Fail("5");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotFoundException), ex.GetType().ToString()); }

            try {
                target.parse("[7z pack.files({\" \"}, \"output\")]");
                Assert.Fail("6");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - pack.files
        ///</summary>
        [TestMethod()]
        public void packFilesTest3()
        {
            var target = new SevenZipComponentInputFilesAccessor();

            //TODO
            string zip = Guid.NewGuid().ToString() + ".zip";
            using(var tf = new TempFile(false)) {
                Assert.AreEqual(Value.Empty, target.parse("[7z pack.files({\"" + tf.file + "\"}, \"" + zip + "\")]"));
                Assert.AreEqual(Value.Empty, target.parse("[7z pack.files({\"" + tf.file + "\"}, \"" + zip + "\", SevenZip, Lzma2, 4)]"));
            }
        }

        /// <summary>
        ///A test for parse - pack.files /the 'except' argument
        ///</summary>
        [TestMethod()]
        public void packFilesTest4()
        {
            var target  = new SevenZipComponentInputFilesAccessor();

            string zip = Guid.NewGuid().ToString() + ".zip";

            using(var tf1 = new TempFile())
            using(var tf2 = new TempFile())
            using(var tf3 = new TempFile())
            {
                Assert.AreEqual(Value.Empty, target.parse("[7z pack.files({\"" + tf1.file + "\", \"" + tf2.file + "\"}, \"" + zip + "\", {\"" + tf1.file + "\"})]"));
                Assert.AreEqual(1, target.FilesInput.Length);
                Assert.AreEqual(tf2.file, target.FilesInput[0]);

                Assert.AreEqual(Value.Empty, target.parse("[7z pack.files({\"" + tf1.file + "\", \"" + tf2.file + "\", \"" + tf3.file + "\"}, \"" + zip + "\", {\"" + tf2.file + "\"}, SevenZip, Lzma2, 4)]"));
                Assert.AreEqual(2, target.FilesInput.Length);
                Assert.AreEqual(tf1.file, target.FilesInput[0]);
                Assert.AreEqual(tf3.file, target.FilesInput[1]);
            }
        }

        /// <summary>
        ///A test for parse - pack.directory
        ///</summary>
        [TestMethod()]
        public void packDirectoryTest1()
        {
            var target = new SevenZipComponent();

            try {
                target.parse("[7z pack.directory(\" \", \"name.zip\")]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }

            try {
                target.parse("[7z pack.directory(\"pathtodir\", \" \")]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }

            try {
                target.parse("[7z pack.directory(\"dir\")]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMLevelException), ex.GetType().ToString()); }

            try {
                target.parse("[7z pack.directory(\"dir\", \"output\", SevenZip)]");
                Assert.Fail("4");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMLevelException), ex.GetType().ToString()); }

            try {
                target.parse("[7z pack.directory(\"dir\", \"output\").right]");
                Assert.Fail("5");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotSupportedOperationException), ex.GetType().ToString()); }

            try {
                target.parse("[7z pack.directory()]");
                Assert.Fail("6");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMLevelException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - pack.files
        ///</summary>
        [TestMethod()]
        public void packDirectoryTest2()
        {
            var target = new SevenZipComponentInputFilesAccessor();

            try {
                target.parse("[7z pack.directory(\"notrealdirfortest\", \"output\")]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotFoundException), ex.GetType().ToString()); }

            //TODO
            string zip = Guid.NewGuid().ToString() + ".zip";
            using(var tf = new TempFile(true)) {
                Assert.AreEqual(Value.Empty, target.parse("[7z pack.directory(\"" + tf.dir + "\", \"" + zip + "\")]"));
                Assert.AreEqual(Value.Empty, target.parse("[7z pack.directory(\"" + tf.dir + "\", \"" + zip + "\", SevenZip, Lzma2, 4)]"));
            }
        }

        /// <summary>
        ///A test for parse - unpack
        ///</summary>
        [TestMethod()]
        public void unpackTest1()
        {
            var target = new SevenZipComponent();

            try {
                target.parse("[7z unpack]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[7z unpack()]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMLevelException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - unpack
        ///</summary>
        [TestMethod()]
        public void unpackTest2()
        {
            var target = new SevenZipComponentExtractArchiveAccessor();

            try {
                target.parse("[7z  unpack(\"f1.zip\")]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotFoundException), ex.GetType().ToString()); }

            try {
                target.parse("[7z  unpack(\" \")]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }

            using(var tf = new TempFile(false, ".zip")) {
                Assert.AreEqual(Value.Empty, target.parse("[7z  unpack(\""+ tf.file + "\")]"));
                Assert.AreEqual(target.file, target.location(tf.file));
                Assert.AreEqual(target.output, target.getDirectoryFromFile(tf.file));
                Assert.AreEqual(false, target.delete);
                Assert.AreEqual(null, target.pwd);
            }

            using(var tf = new TempFile(false, ".zip")) {
                Assert.AreEqual(Value.Empty, target.parse("[7z  unpack(\""+ tf.file + "\", true)]"));
                Assert.AreEqual(target.file, target.location(tf.file));
                Assert.AreEqual(target.output, target.getDirectoryFromFile(tf.file));
                Assert.AreEqual(true, target.delete);
                Assert.AreEqual(null, target.pwd);
            }

            using(var tf = new TempFile(false, ".zip")) {
                Assert.AreEqual(Value.Empty, target.parse("[7z  unpack(\"" + tf.file + "\", true, \"pass-123\")]"));
                Assert.AreEqual(target.file, target.location(tf.file));
                Assert.AreEqual(target.output, target.getDirectoryFromFile(tf.file));
                Assert.AreEqual(true, target.delete);
                Assert.AreEqual("pass-123", target.pwd);
            }
        }

        /// <summary>
        ///A test for parse - unpack
        ///</summary>
        [TestMethod()]
        public void unpackTest3()
        {
            var target = new SevenZipComponentExtractArchiveAccessor();

            try {
                using(var tf = new TempFile(false, ".zip")) {
                    Assert.AreEqual(Value.Empty, target.parse("[7z  unpack(\"" + tf.file + "\", \" \")]"));
                }
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }

            using(var tf = new TempFile(false, ".zip")) {
                Assert.AreEqual(Value.Empty, target.parse("[7z  unpack(\"" + tf.file + "\", \"output-path\")]"));
                Assert.AreEqual(target.file, target.location(tf.file));
                Assert.AreEqual(target.output, target.location("output-path"));
                Assert.AreEqual(false, target.delete);
                Assert.AreEqual(null, target.pwd);
            }

            using(var tf = new TempFile(false, ".zip")) {
                Assert.AreEqual(Value.Empty, target.parse("[7z  unpack(\"" + tf.file + "\", \"output-path\", \"pass-123\")]"));
                Assert.AreEqual(target.file, target.location(tf.file));
                Assert.AreEqual(target.output, target.location("output-path"));
                Assert.AreEqual(false, target.delete);
                Assert.AreEqual("pass-123", target.pwd);
            }

            using(var tf = new TempFile(false, ".zip")) {
                Assert.AreEqual(Value.Empty, target.parse("[7z  unpack(\"" + tf.file + "\", \"output-path\", true)]"));
                Assert.AreEqual(target.file, target.location(tf.file));
                Assert.AreEqual(target.output, target.location("output-path"));
                Assert.AreEqual(true, target.delete);
                Assert.AreEqual(null, target.pwd);
            }

            using(var tf = new TempFile(false, ".zip")) {
                Assert.AreEqual(Value.Empty, target.parse("[7z  unpack(\"" + tf.file + "\", \"output-path\", true, \"pass-123\")]"));
                Assert.AreEqual(target.file, target.location(tf.file));
                Assert.AreEqual(target.output, target.location("output-path"));
                Assert.AreEqual(true, target.delete);
                Assert.AreEqual("pass-123", target.pwd);
            }
        }

        /// <summary>
        ///A test for parse - check
        ///</summary>
        [TestMethod()]
        public void checkTest1()
        {
            var target = new SevenZipComponent();

            try {
                target.parse("[7z check]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[7z check()]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(PMLevelException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - check
        ///</summary>
        [TestMethod()]
        public void checkTest2()
        {
            var target = new SevenZipComponentCheckArchiveAccessor();

            try {
                target.parse("[7z  check(\"thisisreallyisnotrealfile.zip\")]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotFoundException), ex.GetType().ToString()); }

            try {
                target.parse("[7z  check(\" \")]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(ArgumentException), ex.GetType().ToString()); }

            using(var tf = new TempFile(false, ".zip")) {
                target.parse("[7z  check(\""+ tf.file +"\")]");
                Assert.AreEqual(target.location(tf.file), target.file);
                Assert.AreEqual(null, target.pwd);
            }

            using(var tf = new TempFile(false, ".zip")) {
                target.parse("[7z  check(\"" + tf.file + "\", \"pass-123\")]");
                Assert.AreEqual(target.location(tf.file), target.file);
                Assert.AreEqual("pass-123", target.pwd);
            }
        }


        private class SevenZipComponentInputFilesAccessor: SevenZipComponent
        {
            public string ArchiveName { get; set; }
            public string[] FilesInput { get; set; }
            public string DirPath { get; set; }

            protected override void compressFiles(SevenZipCompressor zip, string name, params string[] input)
            {
                ArchiveName = name;
                FilesInput  = input;
                //base.compressFiles(zip, name, input);
            }

            protected override void compressDirectory(SevenZipCompressor zip, string path, string name)
            {
                ArchiveName = name;
                DirPath     = path;
                //base.compressDirectory(zip, path, name);
            }
        }

        private class SevenZipComponentExtractArchiveAccessor: SevenZipComponent
        {
            public string file { get; set; }
            public string output { get; set; }
            public bool delete { get; set; }
            public string pwd { get; set; }

            public new string location(string file)
            {
                return base.location(file);
            }

            public new string getDirectoryFromFile(string output)
            {
                return base.getDirectoryFromFile(output);
            }

            protected override void extractArchive(string file, string output, bool delete, string pwd)
            {
                this.file   = file;
                this.output = output;
                this.delete = delete;
                this.pwd    = pwd;
                //base.extractArchive(file, output, delete, pwd);
            }
        }

        private class SevenZipComponentCheckArchiveAccessor: SevenZipComponentExtractArchiveAccessor
        {
            protected override string checkArchive(string file, string pwd)
            {
                this.file   = file;
                this.pwd    = pwd;
                return Value.Empty;
                //return base.checkArchive(file, pwd);
            }
        }
    }
}