using net.r_eg.vsSBE.Receiver.Output;
using Xunit;

namespace net.r_eg.vsSBE.Test.Receiver.Output
{
    public class MatcherTest
    {
        [Fact]
        public void mWildcardsTest()
        {
            MatcherAccessor.ToWildcards target = new MatcherAccessor.ToWildcards();
            
            string raw          = "new tes;ted project-12, and 75_protection of various systems.";
            string rawExpected  = raw;
            
            bool actual = target.mWildcards("pro*system", ref raw);
            Assert.Equal(rawExpected, raw);
            Assert.True(actual);
        }

        [Fact]
        public void mWildcardsTest1()
        {
            MatcherAccessor.ToWildcards target = new MatcherAccessor.ToWildcards();

            string raw = "new tes;ted project-12, and 75_protection of various systems";
            
            Assert.True(target.mWildcards("new*7*systems", ref raw));
            Assert.True(target.mWildcards("", ref raw));
            Assert.True(target.mWildcards("*", ref raw));
            Assert.False(target.mWildcards("new*express", ref raw));
            Assert.False(target.mWildcards("tes*ting*project", ref raw));

            Assert.True(target.mWildcards("?", ref raw));
            Assert.True(target.mWildcards("project?12", ref raw));
            Assert.False(target.mWildcards("pro?tect", ref raw));

            Assert.True(target.mWildcards("+", ref raw));
            Assert.True(target.mWildcards("new+systems", ref raw));
            Assert.False(target.mWildcards("systems+", ref raw));
        }

        private class MatcherAccessor
        {
            public class Accessor: Matcher {}

            public class ToWildcards: Accessor
            {
                public new bool mWildcards(string term, ref string raw)
                {
                    return base.mWildcards(term, ref raw);
                }
            }
        }
    }
}