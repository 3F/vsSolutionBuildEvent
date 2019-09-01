using System;
using System.Collections.Generic;
using net.r_eg.Varhead;
using net.r_eg.Varhead.Exceptions;
using Xunit;

namespace net.r_eg.vsSBE.Test.Scripts
{
    public class UserVariableTest
    {
        [Fact]
        public void DefTest1()
        {
            UVars target = new UVars();

            Assert.Empty(target.Definitions);
            Assert.Empty(target.Variables);
        }

        [Fact]
        public void SetTest1()
        {
            UVars target = new UVars();
            target.SetVariable("name", null, "unevaluated");

            Assert.Single(target.Definitions);

            foreach(string v in target.Definitions) {
                Assert.Equal("name", v);
            }
        }

        [Fact]
        public void SetTest2()
        {
            UserVariableAccessor target = new UserVariableAccessor();
            string name = "name";
            string project = "project";
            target.SetVariable(name, project, "unevaluated");

            Assert.Single(target.Definitions);

            foreach(string v in target.Definitions) {
                Assert.Equal(v, target.DefIndex(name, project));
            }
        }

        [Fact]
        public void SetTest3()
        {
            UVars target = new UVars();
            target.SetVariable("name", null, "unevaluated");
            Assert.Single(target.Variables);

            foreach(TVariable v in target.Variables)
            {
                Assert.Null(v.evaluated);
                Assert.Equal("name", v.ident);
                Assert.False(v.persistence);
                Assert.Equal(new TVariable(), v.prev);
                Assert.Equal(ValStatus.Unevaluated, v.status);
                Assert.Equal("unevaluated", v.unevaluated);
            }
        }

        [Fact]
        public void SetTest4()
        {
            UserVariableAccessor target = new UserVariableAccessor();
            string name = "name";
            string project = "project";
            target.SetVariable(name, project, "unevaluated");
            Assert.Single(target.Variables);

            foreach(TVariable v in target.Variables) {
                Assert.Null(v.evaluated);
                Assert.Equal(target.DefIndex(name, project), v.ident);
                Assert.False(v.persistence);
                Assert.Equal(new TVariable(), v.prev);
                Assert.Equal(ValStatus.Unevaluated, v.status);
                Assert.Equal("unevaluated", v.unevaluated);
            }
        }

        [Fact]
        public void SetTest5()
        {
            var target = new UVars();

            Assert.Throws<ArgumentException>(() => {
                target.SetVariable("na%me", "project", "unevaluated");
            });
        }

        [Fact]
        public void SetTest6()
        {
            UVars target = new UVars();
            target.SetVariable("name", "project", null);

            Assert.Single(target.Variables);
            foreach(TVariable v in target.Variables) {
                Assert.Equal(v.unevaluated, String.Empty);
            }
        }

        [Fact]
        public void UnsetAllTest1()
        {
            UVars target = new UVars();
            target.SetVariable("name", null, "unevaluated");
            target.UnsetAll();
            Assert.Empty(target.Variables);
        }

        [Fact]
        public void UnsetTest1()
        {
            UVars target = new UVars();
            target.SetVariable("name", null, "unevaluated");
            target.SetVariable("name2", null, "unevaluated2");

            target.Unset("name", null);
            Assert.Single(target.Definitions);

            foreach(string v in target.Definitions) {
                Assert.Equal("name2", v);
            }
        }

        [Fact]
        public void UnsetTest2()
        {
            UVars target = new UVars();
            target.SetVariable("name", null, "unevaluated");
            target.SetVariable("name2", null, "unevaluated2");

            target.Unset("name");
            Assert.Single(target.Definitions);

            foreach(string v in target.Definitions) {
                Assert.Equal("name2", v);
            }
        }

        [Fact]
        public void IsValidNameTest1()
        {
            UVars target = new UVars();

            Assert.True(target.IsValidName("name"));
            Assert.True(target.IsValidName("n"));
            Assert.True(target.IsValidName("name0"));
            Assert.True(target.IsValidName("name_n"));
            Assert.False(target.IsValidName("0name"));
            Assert.False(target.IsValidName(""));
            Assert.False(target.IsValidName("na%me"));
            Assert.False(target.IsValidName("name "));
        }

        [Fact]
        public void IsUnevaluatedTest1()
        {
            UVars target = new UVars();

            Assert.Throws<KeyNotFoundException>(() => {
                target.IsUnevaluated("name", "project");
            });

            Assert.Throws<KeyNotFoundException>(() => {
                target.IsUnevaluated("name");
            });
        }

        [Fact]
        public void IsUnevaluatedTest2()
        {
            UVars target = new UVars();
            target.SetVariable("name", null, "val");

            Assert.True(target.IsUnevaluated("name"));
        }

        [Fact]
        public void IsUnevaluatedTest3()
        {
            var target = new UVars();
            target.SetVariable("name", "project", "val");

            Assert.True(target.IsUnevaluated("name", "project"));
        }

        [Fact]
        public void IsExistTest1()
        {
            UVars target = new UVars();
            Assert.False(target.IsExist("name", "project"));
        }

        [Fact]
        public void IsExistTest2()
        {
            UVars target = new UVars();
            Assert.False(target.IsExist("name"));
        }

        [Fact]
        public void IsExistTest3()
        {
            UVars target = new UVars();
            target.SetVariable("name", "project", "val");
            Assert.True(target.IsExist("name", "project"));
        }

        [Fact]
        public void IsExistTest4()
        {
            UserVariableAccessor target = new UserVariableAccessor();
            string name = "name";
            string project = "project";
            target.SetVariable(name, project, "val");
            Assert.True(target.IsExist(target.DefIndex(name, project)));
        }

        [Fact]
        public void GetTest1()
        {
            UVars target = new UVars();
            Assert.Null(target.GetValue("name"));
            Assert.Null(target.GetValue("name", "project"));
        }

        [Fact]
        public void GetTest2()
        {
            UserVariableAccessor target = new UserVariableAccessor();
            string name = "name";
            string project = "project";
            target.SetVariable(name, project, "val");
            Assert.Equal(String.Empty, target.GetValue(target.DefIndex(name, project)));
        }

        [Fact]
        public void GetTest3()
        {
            UVars target = new UVars();
            target.SetVariable("name", null, "val");
            Assert.Equal(string.Empty, target.GetValue("name"));
        }

        [Fact]
        public void EvaluateTest1()
        {
            UVars target = new UVars();

            Assert.Throws<DefinitionNotFoundException>(() => {
                target.Evaluate("name", "project", new Evaluator1(), true);
            });

            Assert.Throws<DefinitionNotFoundException>(() => {
                target.Evaluate("name", new Evaluator1(), true);
            });
        }

        [Fact]
        public void EvaluateTest2()
        {
            UVars target = new UVars();
            string name = "name";
            string project = "project";

            Assert.Throws<ArgumentNullException>(() =>
            {
                target.SetVariable(name, project, "val");
                target.Evaluate(name, project, null, true);
            });
        }

        [Fact]
        public void EvaluateTest3()
        {
            UVars target = new UVars();
            string name = "name";
            string project = "project";

            target.SetVariable(name, project, "val");
            target.Evaluate(name, project, new Evaluator1(), true);
            Assert.Equal("[E1:val]", target.GetValue(name, project));
        }

        [Fact]
        public void EvaluateTest4()
        {
            UVars target = new UVars();
            string name = "name";
            string project = "project";

            target.SetVariable(name, project, "val");
            target.Evaluate(name, project, new Evaluator1(), true);
            target.Evaluate(name, project, new Evaluator2(), false);
            Assert.Equal("[E2:[E1:val]]", target.GetValue(name, project));
        }

        [Fact]
        public void EvaluateTest5()
        {
            UVars target = new UVars();
            string name = "name";
            string project = "project";

            target.SetVariable(name, project, "val");
            target.Evaluate(name, project, new Evaluator1(), true);
            target.Evaluate(name, project, new Evaluator2(), true);
            Assert.Equal("[E2:val]", target.GetValue(name, project));
        }

        private class UserVariableAccessor: UVars
        {
            public new string DefIndex(string name, string project)
            {
                return base.DefIndex(name, project);
            }
        }

        private class Evaluator1: IEvaluator
        {
            public string Evaluate(string data)
            {
                return String.Format("[E1:{0}]", data);
            }
        }

        private class Evaluator2: IEvaluator
        {
            public string Evaluate(string data)
            {
                return String.Format("[E2:{0}]", data);
            }
        }
    }
}
