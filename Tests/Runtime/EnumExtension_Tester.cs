using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace UnityPattern_Test
{
    [Category("UnityPattern")]
    public class PrimitiveExtension_Tester
    {
        enum ETestEnum
        {
            One = 1,
            Two = 2,
            Three = 3,

            Five = 5,
        }


        [Test]
        public void Get_PrevEnum_Test()
        {
            // Arrange
            bool bIsError = false;


            // Act & Assert
            // Three의 Prev는 Two
            {
                bIsError = false;
                Assert.AreEqual(ETestEnum.Three.GetPrevEnum((strError) => bIsError = true), ETestEnum.Two);
                Assert.IsFalse(bIsError);
            }


            // Two의 Prev는 One
            {
                bIsError = false;
                Assert.AreEqual(ETestEnum.Two.GetPrevEnum((strError) => bIsError = true), ETestEnum.One);
                Assert.IsFalse(bIsError);
            }


            // One의 Prev는 없으므로 Error가 출력
            {
                bIsError = false;
                Assert.AreEqual(ETestEnum.One.GetPrevEnum((strError) => bIsError = true), ETestEnum.One);
                Assert.IsTrue(bIsError);
            }


            // Five의 Prev는 Four인데 없으므로 Three가 출력
            {
                bIsError = false;
                Assert.AreEqual(ETestEnum.Five.GetPrevEnum((strError) => bIsError = true), ETestEnum.Three);
                Assert.IsFalse(bIsError);
            }
        }
    }
}
