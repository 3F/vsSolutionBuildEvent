using System;
using System.Collections.Generic;
using net.r_eg.vsSBE.Receiver.Output;
using Xunit;

namespace net.r_eg.vsSBE.Test.Receiver.Output
{
    public class BuildItemTest
    {
        [Fact]
        public void BuildItemConstructorTest()
        {
            string rawdata          = "warning CS1762: A reference was created to embedded interop assembly";
            string rawdataExpected  = rawdata;

            BuildItemAccessor.ToRawData target = new BuildItemAccessor.ToRawData();
            target.updateRaw(rawdata);
            Assert.Equal(rawdataExpected, rawdata);
        }
        
        /// <summary>
        ///A test for extract
        ///</summary>
        [Fact]
        public void extractTest()
        {
            string rawdata = String.Empty;
            BuildItemAccessor.ToExtract target = new BuildItemAccessor.ToExtract();

            Assert.True(target.ErrorsCount < 1);
            Assert.True(target.WarningsCount < 1);

            target.rawdata = "2>  thread_clock.cpp";
            target.extract();

            Assert.True(target.ErrorsCount < 1);
            Assert.True(target.WarningsCount < 1);
        }
        
        /// <summary>
        ///A test for extract
        ///</summary>
        [Fact]
        public void extractTest2()
        {
            string rawdata = @"9>C:\VC\atlmfc\include\atlhost.h(422): warning C4505: 'ATL::CAxHostWindow::AddRef' : unreferenced local function has been removed";
            ItemEW target = new ItemEW();
            target.updateRaw(rawdata);
            Assert.True(target.ErrorsCount < 1);
            Assert.True(target.WarningsCount == 1);
        }

        /// <summary>
        ///A test for extract
        ///</summary>
        [Fact]
        public void extractTest3()
        {
            string rawdata = @"11>windows\Search.cpp(2246): error C4430: missing type specifier - int assumed. Note: C++ does not support default-int";
            ItemEW target = new ItemEW();
            target.updateRaw(rawdata);
            Assert.True(target.ErrorsCount == 1);
            Assert.True(target.WarningsCount < 1);
        }
        
        /// <summary>
        ///A test for checkRule
        ///</summary>
        [Fact]
        public void checkRuleTest()
        {
            string rawdata = String.Empty;
            BuildItemAccessor.ToCheckRule target = new BuildItemAccessor.ToCheckRule();
            target.updateRaw(rawdata);
            
            target.warnings = new List<string>() { "C4505" };
            Assert.True(target.checkRule(EWType.Warnings, true, new List<string>()));
            target.warnings.Clear();

            target.errors = new List<string>() { "C4430" };
            Assert.True(target.checkRule(EWType.Errors, true, new List<string>()));
            target.errors.Clear();
        }

        /// <summary>
        ///A test for checkRule
        ///</summary>
        [Fact]
        public void checkRuleTest2()
        {
            string rawdata = String.Empty;
            BuildItemAccessor.ToCheckRule target = new BuildItemAccessor.ToCheckRule();
            target.updateRaw(rawdata);

            target.warnings = new List<string>() { "C4505", "C4507" };
            Assert.False(target.checkRule(EWType.Warnings, true, new List<string>() { "C4506" }));
            target.warnings.Clear();

            target.errors = new List<string>() { "C4430", "C4432" };
            Assert.False(target.checkRule(EWType.Errors, true, new List<string>() { "C4431" }));
            target.errors.Clear();
        }

        /// <summary>
        ///A test for checkRule
        ///</summary>
        [Fact]
        public void checkRuleTest3()
        {
            string rawdata = String.Empty;
            BuildItemAccessor.ToCheckRule target = new BuildItemAccessor.ToCheckRule();
            target.updateRaw(rawdata);

            target.warnings = new List<string>() { "C4505", "C4507" };
            Assert.True(target.checkRule(EWType.Warnings, true, new List<string>() { "C4507" }));
            target.warnings.Clear();

            target.errors = new List<string>() { "C4430", "C4432" };
            Assert.True(target.checkRule(EWType.Errors, true, new List<string>() { "C4432" }));
            target.errors.Clear();
        }

        /// <summary>
        ///A test for checkRule
        ///</summary>
        [Fact]
        public void checkRuleTest4()
        {
            string rawdata = String.Empty;
            BuildItemAccessor.ToCheckRule target = new BuildItemAccessor.ToCheckRule();
            target.updateRaw(rawdata);

            target.warnings = new List<string>() { "C4505", "C4507" };
            Assert.False(target.checkRule(EWType.Warnings, false, new List<string>()));
            target.warnings.Clear();
            Assert.False(target.checkRule(EWType.Warnings, false, new List<string>()));

            target.errors = new List<string>() { "C4430", "C4432" };
            Assert.False(target.checkRule(EWType.Errors, false, new List<string>()));
            target.errors.Clear();
            Assert.False(target.checkRule(EWType.Errors, false, new List<string>()));
        }

        /// <summary>
        ///A test for checkRule
        ///</summary>
        [Fact]
        public void checkRuleTest5()
        {
            string rawdata = String.Empty;
            BuildItemAccessor.ToCheckRule target = new BuildItemAccessor.ToCheckRule();
            target.updateRaw(rawdata);

            target.warnings = new List<string>() { "C4505", "C4507" };
            Assert.True(target.checkRule(EWType.Warnings, false, new List<string>() { "C4507" }));
            target.warnings.Clear();

            target.errors = new List<string>() { "C4430", "C4432" };
            Assert.True(target.checkRule(EWType.Errors, false, new List<string>() { "C4432" }));
            target.errors.Clear();
        }

        /// <summary>
        ///A test for checkRule
        ///</summary>
        [Fact]
        public void checkRuleTest6()
        {
            string rawdata = String.Empty;
            BuildItemAccessor.ToCheckRule target = new BuildItemAccessor.ToCheckRule();
            target.updateRaw(rawdata);

            target.warnings = new List<string>() { "C4505" };
            Assert.False(target.checkRule(EWType.Warnings, false, new List<string>() { "C4505" }));
            target.warnings.Clear();

            target.errors = new List<string>() { "C4430" };
            Assert.False(target.checkRule(EWType.Errors, false, new List<string>() { "C4430" }));
            target.errors.Clear();
        }

        private class BuildItemAccessor
        {
            public class Accessor: ItemEW
            {
                //public Accessor(ref string rawdata): base(ref rawdata) { }
            }

            public class ToCheckRule: Accessor
            {
                public new List<string> errors
                {
                    get { return base.errors; }
                    set { base.errors = value; }
                }

                public new List<string> warnings
                {
                    get { return base.warnings; }
                    set { base.warnings = value; }
                }

                //public ToCheckRule(ref string rawdata): base(ref rawdata) { }
            }

            public class ToRawData: Accessor
            {
                public new string rawdata
                {
                    get { return base.rawdata; }
                    set { base.rawdata = value; }
                }
            }

            public class ToExtract: ToRawData
            {
                public new void extract()
                {
                    base.extract();
                }
            }
        }
    }
}