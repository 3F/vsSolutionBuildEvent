using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.SNode;

namespace net.r_eg.vsSBE.Test.SBEScripts.SNode
{
    [TestClass()]
    public class LevelTest
    {
        /// <summary>
        ///A test for Is
        ///</summary>
        [TestMethod()]
        public void IsTest1()
        {
            ILevel level = new Level()
            {
                Data = "hash",
                Type = LevelType.Method,
                Args = new Argument[2] {
                                new Argument() {
                                    data = "abcd123",
                                    type = ArgumentType.StringDouble
                                },
                                new Argument() {
                                    data = "true",
                                    type = ArgumentType.Boolean
                                } }
            };

            Assert.AreEqual(true, level.Is(ArgumentType.StringDouble, ArgumentType.Boolean));
            Assert.AreEqual(false, level.Is(ArgumentType.StringDouble));
            Assert.AreEqual(false, level.Is(ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.Boolean));
            Assert.AreEqual(false, level.Is());
            Assert.AreEqual(false, level.Is(ArgumentType.Boolean, ArgumentType.StringDouble));
        }

        /// <summary>
        ///A test for Is
        ///</summary>
        [TestMethod()]
        public void IsTest2()
        {
            ILevel level = new Level()
            {
                Data = "hash",
                Type = LevelType.Method,
                Args = new Argument[2] {
                                new Argument() {
                                    data = "abcd123",
                                    type = ArgumentType.StringDouble
                                },
                                new Argument() {
                                    data = "true",
                                    type = ArgumentType.Boolean
                                } }
            };

            Assert.AreEqual(true, level.Is(null, ArgumentType.StringDouble, ArgumentType.Boolean));
            Assert.AreEqual(false, level.Is(null, ArgumentType.StringDouble));
            Assert.AreEqual(false, level.Is(null, ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.Boolean));
            Assert.AreEqual(false, level.Is(null, null));
            Assert.AreEqual(false, level.Is(null, ArgumentType.Boolean, ArgumentType.StringDouble));
        }

        /// <summary>
        ///A test for Is
        ///</summary>
        [TestMethod()]
        public void IsTest3()
        {
            ILevel level = new Level()
            {
                Data = "hash",
                Type = LevelType.Method,
                Args = new Argument[2] {
                                new Argument() {
                                    data = "abcd123",
                                    type = ArgumentType.StringDouble
                                },
                                new Argument() {
                                    data = "true",
                                    type = ArgumentType.Boolean
                                } }
            };

            Assert.AreEqual(true, level.Is("hash", ArgumentType.StringDouble, ArgumentType.Boolean)); //should be without exception

            try {
                Assert.AreEqual(false, level.Is("hash", ArgumentType.StringDouble));
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(InvalidArgumentException), ex.GetType().ToString()); }

            try {
                Assert.AreEqual(false, level.Is("hash", ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.Boolean));
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(InvalidArgumentException), ex.GetType().ToString()); }

            try {
                Assert.AreEqual(false, level.Is("hash", null));
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(InvalidArgumentException), ex.GetType().ToString()); }

            try {
                Assert.AreEqual(false, level.Is("hash", ArgumentType.Boolean, ArgumentType.StringDouble));
                Assert.Fail("4");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(InvalidArgumentException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for Is
        ///</summary>
        [TestMethod()]
        public void IsTest4()
        {
            ILevel level = new Level()
            {
                Data = "hash",
                Type = LevelType.Method,
                Args = null
            };

            Assert.AreEqual(false, level.Is());
            Assert.AreEqual(false, level.Is(ArgumentType.StringDouble, ArgumentType.Boolean));
        }

    }
}
