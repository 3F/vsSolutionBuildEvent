using System.Linq;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.Core;
using net.r_eg.Varhead;
using SobaScript.Z.CoreTest.Stubs;
using Xunit;

namespace SobaScript.Z.CoreTest
{
    public class TryComponentTest
    {
        [Fact]
        public void ParseTest1()
        {
            var uvar    = new UVars();
            var target  = new TryComponent(SobaAcs.MakeWithTryComponent(uvar));

            Assert.Equal(Value.Empty, target.Eval("[try{}catch{ if error }]"));
            Assert.Equal(Value.Empty, target.Eval("[try{}catch(err, msg){ if error }]"));
            Assert.Equal(Value.Empty, target.Eval("[try{}catch(){ }]"));
            Assert.Empty(uvar.Variables);
        }

        [Fact]
        public void ParseTest2()
        {
            var target = new TryComponent(SobaAcs.MakeWithTryComponent());

            Assert.Equal(Value.Empty, target.Eval("[try\n{}\ncatch\n{ if error }]"));
            Assert.Equal(Value.Empty, target.Eval("[try\n{}\n catch\n { \n} ]"));
        }

        [Fact]
        public void ParseTest3()
        {
            var target = new TryComponent(SobaAcs.MakeWithTryComponent());

            Assert.Throws<IncorrectSyntaxException>(() => 
                target.Eval("[try{ }]")
            );
        }

        [Fact]
        public void ParseTest4()
        {
            var target = new TryComponent(SobaAcs.MakeWithTryComponent());

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[try{ #[notrealcomponentToError] }catch('err', 'msg'){ }]")
            );
        }

        [Fact]
        public void CatchTest1()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithTryComponent(uvar);

            target.Eval("#[try{ $(test = '123') }catch{ $(test2 = '456') }]");

            Assert.Single(uvar.Variables);
            Assert.Equal("123", uvar.GetValue("test", null));
        }

        [Fact]
        public void CatchTest2()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithTryComponent(uvar);

            Assert.Empty(uvar.Variables);
            target.Eval("#[try{ #[notrealcomponentToError]  $(test = '123') }catch{ $(test2 = '456') }]");

            Assert.Single(uvar.Variables);
            Assert.Equal("456", uvar.GetValue("test2", null));
        }

        [Fact]
        public void CatchTest3()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithTryComponent(uvar);

            Assert.Empty(uvar.Variables);
            target.Eval(@"
                            #[( false ){

                                #[try
                                { 
                                     #[notrealcomponentToError]
                                }
                                catch(err, msg)
                                {
                                    $(test1 = '123')
                                }]

                            }
                            else{

                                #[try
                                { 
                                     #[notrealcomponentToError]
                                }
                                catch(err, msg)
                                {
                                    $(test2 = '456')
                                    $(exErr = $(err))
                                    $(exMsg = $(msg))
                                }]

                            }] ");

            Assert.Equal(3, uvar.Variables.Count());
            Assert.Null(uvar.GetValue("test1", null));
            Assert.Equal("456", uvar.GetValue("test2", null));
            Assert.True(!string.IsNullOrWhiteSpace(uvar.GetValue("exErr", null)));
            Assert.True(!string.IsNullOrWhiteSpace(uvar.GetValue("exMsg", null)));
            Assert.Null(uvar.GetValue("err", null));
            Assert.Null(uvar.GetValue("msg", null));
        }

        [Fact]
        public void CatchTest4()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithTryComponent(uvar);

            Assert.Empty(uvar.Variables);
            target.Eval(@"
                            #[( false ){

                                #[try
                                { 
                                     #[notrealcomponentToError]
                                }
                                catch
                                {
                                    $(test1 = '123')
                                }]

                            }
                            else{

                                #[try
                                { 
                                     #[notrealcomponentToError]
                                }
                                catch
                                {
                                    $(test2 = '456')
                                }]

                            }] ");

            Assert.Single(uvar.Variables);
            Assert.Null(uvar.GetValue("test1", null));
            Assert.Equal("456", uvar.GetValue("test2", null));
            Assert.Null(uvar.GetValue("err", null));
            Assert.Null(uvar.GetValue("msg", null));
        }

        [Fact]
        public void CatchTest5()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeWithTryComponent(uvar);

            Assert.Empty(uvar.Variables);
            target.Eval(@"
                            #[try
                            { 
                                #[notrealcomponentToError]
     
                                #[( true ){
                                    $(test1 = '123')
                                }]
                            }
                            catch
                            {
                                #[( true ){
                                    $(test2 = '456')
                                }]
                            }] 
                        ");

            Assert.Single(uvar.Variables);
            Assert.Null(uvar.GetValue("test1", null));
            Assert.Equal("456", uvar.GetValue("test2", null));
        }

    }
}
