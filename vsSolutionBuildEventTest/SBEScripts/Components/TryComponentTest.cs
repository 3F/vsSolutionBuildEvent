using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.Scripts;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    [TestClass]
    public class TryComponentTest
    {
        [TestMethod]
        public void parseTest1()
        {
            var uvar    = new UserVariable();
            var target  = new TryComponent(new StubEnv(), uvar);

            Assert.AreEqual(Value.Empty, target.parse("[try{}catch{ if error }]"));
            Assert.AreEqual(Value.Empty, target.parse("[try{}catch(err, msg){ if error }]"));
            Assert.AreEqual(Value.Empty, target.parse("[try{}catch(){ }]"));
            Assert.AreEqual(0, uvar.Variables.Count());
        }

        [TestMethod]
        public void parseTest2()
        {
            var target = new TryComponent(new StubEnv(), new UserVariable());

            Assert.AreEqual(Value.Empty, target.parse("[try\n{}\ncatch\n{ if error }]"));
            Assert.AreEqual(Value.Empty, target.parse("[try\n{}\n catch\n { \n} ]"));
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest3()
        {
            var target = new TryComponent(new StubEnv(), new UserVariable());
            target.parse("[try{ }]");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedOperationException))]
        public void parseTest4()
        {
            var target = new TryComponent(new StubEnv(), new UserVariable());
            target.parse("[try{ #[notrealcomponentToError] }catch('err', 'msg'){ }]");
        }

        [TestMethod]
        public void catchTest1()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);       

            target.parse("#[try{ $(test = '123') }catch{ $(test2 = '456') }]");

            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("123", uvar.get("test", null));
        }

        [TestMethod]
        public void catchTest2()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

            Assert.AreEqual(0, uvar.Variables.Count());
            target.parse("#[try{ #[notrealcomponentToError]  $(test = '123') }catch{ $(test2 = '456') }]");

            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("456", uvar.get("test2", null));
        }

        [TestMethod]
        public void catchTest3()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

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
            Assert.AreEqual(null, uvar.get("test1", null));
            Assert.AreEqual("456", uvar.get("test2", null));
            Assert.AreEqual(true, !string.IsNullOrWhiteSpace(uvar.get("exErr", null)));
            Assert.AreEqual(true, !string.IsNullOrWhiteSpace(uvar.get("exMsg", null)));
            Assert.AreEqual(null, uvar.get("err", null));
            Assert.AreEqual(null, uvar.get("msg", null));
        }

        [TestMethod]
        public void catchTest4()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

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
            Assert.AreEqual(null, uvar.get("test1", null));
            Assert.AreEqual("456", uvar.get("test2", null));
            Assert.AreEqual(null, uvar.get("err", null));
            Assert.AreEqual(null, uvar.get("msg", null));
        }

        [TestMethod]
        public void catchTest5()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

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
            Assert.AreEqual(null, uvar.get("test1", null));
            Assert.AreEqual("456", uvar.get("test2", null));
        }

    }
}
