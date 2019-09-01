using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.SBEScripts.Components;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    [TestClass]
    public class TryComponentTest
    {
        [TestMethod]
        public void parseTest1()
        {
            var uvar    = new UVars();
            var target  = new TryComponent(StubSoba.MakeNew(uvar));

            Assert.AreEqual(Value.Empty, target.parse("[try{}catch{ if error }]"));
            Assert.AreEqual(Value.Empty, target.parse("[try{}catch(err, msg){ if error }]"));
            Assert.AreEqual(Value.Empty, target.parse("[try{}catch(){ }]"));
            Assert.AreEqual(0, uvar.Variables.Count());
        }

        [TestMethod]
        public void parseTest2()
        {
            var target = new TryComponent(new Soba());

            Assert.AreEqual(Value.Empty, target.parse("[try\n{}\ncatch\n{ if error }]"));
            Assert.AreEqual(Value.Empty, target.parse("[try\n{}\n catch\n { \n} ]"));
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest3()
        {
            var target = new TryComponent(new Soba());
            target.parse("[try{ }]");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedOperationException))]
        public void parseTest4()
        {
            var target = new TryComponent(new Soba());
            target.parse("[try{ #[notrealcomponentToError] }catch('err', 'msg'){ }]");
        }

        [TestMethod]
        public void catchTest1()
        {
            var uvar    = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            target.parse("#[try{ $(test = '123') }catch{ $(test2 = '456') }]");

            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("123", uvar.GetValue("test", null));
        }

        [TestMethod]
        public void catchTest2()
        {
            var uvar    = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            Assert.AreEqual(0, uvar.Variables.Count());
            target.parse("#[try{ #[notrealcomponentToError]  $(test = '123') }catch{ $(test2 = '456') }]");

            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("456", uvar.GetValue("test2", null));
        }

        [TestMethod]
        public void catchTest3()
        {
            var uvar    = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            Assert.AreEqual(0, uvar.Variables.Count());
            target.parse(@"
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

            Assert.AreEqual(3, uvar.Variables.Count());
            Assert.AreEqual(null, uvar.GetValue("test1", null));
            Assert.AreEqual("456", uvar.GetValue("test2", null));
            Assert.AreEqual(true, !string.IsNullOrWhiteSpace(uvar.GetValue("exErr", null)));
            Assert.AreEqual(true, !string.IsNullOrWhiteSpace(uvar.GetValue("exMsg", null)));
            Assert.AreEqual(null, uvar.GetValue("err", null));
            Assert.AreEqual(null, uvar.GetValue("msg", null));
        }

        [TestMethod]
        public void catchTest4()
        {
            var uvar    = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            Assert.AreEqual(0, uvar.Variables.Count());
            target.parse(@"
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

            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual(null, uvar.GetValue("test1", null));
            Assert.AreEqual("456", uvar.GetValue("test2", null));
            Assert.AreEqual(null, uvar.GetValue("err", null));
            Assert.AreEqual(null, uvar.GetValue("msg", null));
        }

        [TestMethod]
        public void catchTest5()
        {
            var uvar    = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            Assert.AreEqual(0, uvar.Variables.Count());
            target.parse(@"
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

            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual(null, uvar.GetValue("test1", null));
            Assert.AreEqual("456", uvar.GetValue("test2", null));
        }

    }
}
