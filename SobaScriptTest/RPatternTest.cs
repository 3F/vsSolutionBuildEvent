using System.Text.RegularExpressions;
using net.r_eg.SobaScript;
using Xunit;

namespace SobaScriptTest
{
    public class RPatternTest
    {
        [Fact]
        public void SquareBracketsContentTest()
        {
            string data = " #[var name] ";
            Match actual = Regex.Match(data, Pattern.SquareBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.True(actual.Success);
            Assert.Equal("var name", actual.Groups[1].Value);
        }

        [Fact]
        public void SquareBracketsContentTest2()
        {
            string data = " [ test [name [ data]  ]] ";
            Match actual = Regex.Match(data, Pattern.SquareBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.True(actual.Success);
            Assert.Equal(" test [name [ data]  ]", actual.Groups[1].Value);
        }

        [Fact]
        public void SquareBracketsContentTest3()
        {
            string data = " [ test name [ data]  p]] ";
            Match actual = Regex.Match(data, Pattern.SquareBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.True(actual.Success);
            Assert.Equal(" test name [ data]  p", actual.Groups[1].Value);
        }

        [Fact]
        public void SquareBracketsContentTest4()
        {
            string data = " data] [test ";
            bool actual = Regex.IsMatch(data, Pattern.SquareBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.False(actual);
        }

        [Fact]
        public void SquareBracketsContentTest5()
        {
            string data = " data ";
            bool actual = Regex.IsMatch(data, Pattern.SquareBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.False(actual);
        }

        [Fact]
        public void RoundBracketsContentTest()
        {
            string data = " $(var name) ";
            Match actual = Regex.Match(data, Pattern.RoundBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.True(actual.Success);
            Assert.Equal("var name", actual.Groups[1].Value);
        }

        [Fact]
        public void RoundBracketsContentTest2()
        {
            string data = " ( test (name ( data)  )) ";
            Match actual = Regex.Match(data, Pattern.RoundBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.True(actual.Success);
            Assert.Equal(" test (name ( data)  )", actual.Groups[1].Value);
        }

        [Fact]
        public void RoundBracketsContentTest3()
        {
            string data = " ( test name ( data)  p)) ";
            Match actual = Regex.Match(data, Pattern.RoundBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.True(actual.Success);
            Assert.Equal(" test name ( data)  p", actual.Groups[1].Value);
        }

        [Fact]
        public void RoundBracketsContentTest4()
        {
            string data = " data) (test ";
            bool actual = Regex.IsMatch(data, Pattern.RoundBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.False(actual);
        }

        [Fact]
        public void RoundBracketsContentTest5()
        {
            string data = " data ";
            bool actual = Regex.IsMatch(data, Pattern.RoundBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.False(actual);
        }

        [Fact]
        public void CurlyBracketsContentTest()
        {
            string data = " { body1 } ";
            Match actual = Regex.Match(data, Pattern.CurlyBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.True(actual.Success);
            Assert.Equal(" body1 ", actual.Groups[1].Value);
        }

        [Fact]
        public void CurlyBracketsContentTest2()
        {
            string data = " { test {name { data}  }} ";
            Match actual = Regex.Match(data, Pattern.CurlyBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.True(actual.Success);
            Assert.Equal(" test {name { data}  }", actual.Groups[1].Value);
        }

        [Fact]
        public void CurlyBracketsContentTest3()
        {
            string data = " { test name { data}  p}} ";
            Match actual = Regex.Match(data, Pattern.CurlyBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.True(actual.Success);
            Assert.Equal(" test name { data}  p", actual.Groups[1].Value);
        }

        [Fact]
        public void CurlyBracketsContentTest4()
        {
            string data = " data} {test ";
            bool actual = Regex.IsMatch(data, Pattern.CurlyBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.False(actual);
        }

        [Fact]
        public void CurlyBracketsContentTest5()
        {
            string data = " data ";
            bool actual = Regex.IsMatch(data, Pattern.CurlyBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.False(actual);
        }

        [Fact]
        public void DoubleQuotesContentTest()
        {
            string data = " test \"123\" ";
            Match actual = Regex.Match(data, Pattern.DoubleQuotesContent, RegexOptions.IgnorePatternWhitespace);
            Assert.True(actual.Success);
            Assert.Equal("123", actual.Groups[1].Value);
        }

        [Fact]
        public void DoubleQuotesContentTest2()
        {
            string data = " test \\\"123\\\" "; // \"data\"
            bool actual = Regex.IsMatch(data, Pattern.DoubleQuotesContent, RegexOptions.IgnorePatternWhitespace);
            Assert.False(actual);
        }

        [Fact]
        public void DoubleQuotesContentTest3()
        {
            string data = " test 123\" ";
            Assert.False(Regex.IsMatch(data, Pattern.DoubleQuotesContent, RegexOptions.IgnorePatternWhitespace));
        }

        [Fact]
        public void DoubleQuotesContentTest4()
        {
            string data     = "\"\\\",\"p\""; //->  "\","p"
            string expected = "\\\","; //->  \",
            Assert.Equal(expected, Regex.Match(data, Pattern.DoubleQuotesContent, RegexOptions.IgnorePatternWhitespace).Groups[1].Value);
        }

        [Fact]
        public void DoubleQuotesContentTest5()
        {
            string data     = "\"\\\\\",\"p\""; //->  "\\","p"
            string expected = "\\\\"; //->  \\
            Assert.Equal(expected, Regex.Match(data, Pattern.DoubleQuotesContent, RegexOptions.IgnorePatternWhitespace).Groups[1].Value);
        }

        [Fact]
        public void DoubleQuotesContentTest6()
        {
            string data = "\"\\ \",\"p\""; //->  "\ ","p"
            string expected = "\\ "; //->  \ 
            Assert.Equal(expected, Regex.Match(data, Pattern.DoubleQuotesContent, RegexOptions.IgnorePatternWhitespace).Groups[1].Value);
        }

        [Fact]
        public void DoubleQuotesContentTest7()
        {
            string data = "\"\\\\\\\",\"p\""; //->  "\\\","p"
            string expected = "\\\\\\\","; //->  \\\",
            Assert.Equal(expected, Regex.Match(data, Pattern.DoubleQuotesContent, RegexOptions.IgnorePatternWhitespace).Groups[1].Value);
        }

        [Fact]
        public void SingleQuotesContentTest()
        {
            string data = " test '123' ";
            Match actual = Regex.Match(data, Pattern.SingleQuotesContent, RegexOptions.IgnorePatternWhitespace);
            Assert.True(actual.Success);
            Assert.Equal("123", actual.Groups[1].Value);
        }

        [Fact]
        public void SingleQuotesContentTest2()
        {
            string data = " test \\'123\\' "; // \'data\'
            bool actual = Regex.IsMatch(data, Pattern.SingleQuotesContent, RegexOptions.IgnorePatternWhitespace);
            Assert.False(actual);
        }

        [Fact]
        public void SingleQuotesContentTest3()
        {
            string data = " test 123' ";
            bool actual = Regex.IsMatch(data, Pattern.SingleQuotesContent, RegexOptions.IgnorePatternWhitespace);
            Assert.False(actual);
        }

        [Fact]
        public void SingleQuotesContentTest4()
        {
            string data     = "'\\','p'"; //-> '\','p'
            string expected = "\\',"; //-> \',
            Assert.Equal(expected, Regex.Match(data, Pattern.SingleQuotesContent, RegexOptions.IgnorePatternWhitespace).Groups[1].Value);
        }

        [Fact]
        public void SingleQuotesContentTest5()
        {
            string data = "'\\\\','p'"; //->  '\\','p'
            string expected = "\\\\"; //->  \\
            Assert.Equal(expected, Regex.Match(data, Pattern.SingleQuotesContent, RegexOptions.IgnorePatternWhitespace).Groups[1].Value);
        }

        [Fact]
        public void SingleQuotesContentTest6()
        {
            string data = "'\\ ','p'"; //->  '\ ','p'
            string expected = "\\ "; //->  \ 
            Assert.Equal(expected, Regex.Match(data, Pattern.SingleQuotesContent, RegexOptions.IgnorePatternWhitespace).Groups[1].Value);
        }

        [Fact]
        public void SingleQuotesContentTest7()
        {
            string data = "'\\\\\\','p'"; //->  '\\\','p'
            string expected = "\\\\\\',"; //->  \\\',
            Assert.Equal(expected, Regex.Match(data, Pattern.SingleQuotesContent, RegexOptions.IgnorePatternWhitespace).Groups[1].Value);
        }
    }
}
