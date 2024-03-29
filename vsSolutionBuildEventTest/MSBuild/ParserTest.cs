﻿using Moq;
using net.r_eg.EvMSBuild;
using net.r_eg.MvsSln;
using Xunit;

namespace net.r_eg.vsSBE.Test.MSBuild
{
    public class ParserTest
    {
        [Fact]
        public void getPropertyTest()
        {
            var mockDte2                    = new Mock<EnvDTE80.DTE2>();
            var mockSolution                = new Mock<EnvDTE.Solution>();
            var mockSolutionBuild           = new Mock<EnvDTE.SolutionBuild>();
            var mockSolutionConfiguration2  = new Mock<EnvDTE80.SolutionConfiguration2>();

            mockSolutionConfiguration2.SetupGet(p => p.Name).Returns("Release");
            mockSolutionConfiguration2.SetupGet(p => p.PlatformName).Returns("x86");

            mockSolutionBuild.SetupGet(p => p.ActiveConfiguration).Returns(mockSolutionConfiguration2.Object);
            mockSolution.SetupGet(p => p.SolutionBuild).Returns(mockSolutionBuild.Object);
            mockDte2.SetupGet(p => p.Solution).Returns(mockSolution.Object);

            var target = new EvMSBuilder(new Environment(mockDte2.Object));
            Assert.NotNull(target.GetPropValue(PropertyNames.CONFIG));
            Assert.NotNull(target.GetPropValue(PropertyNames.PLATFORM));
        }
    }
}