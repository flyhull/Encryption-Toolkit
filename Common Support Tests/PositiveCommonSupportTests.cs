using System.Text;
using Common_Support;
using Testing_Support;

namespace Common_Support_Tests
{
    [TestClass]
    public sealed class PositiveCommonSupportTests
    {
        [TestMethod]
        public void TempByteArray()
        {
            //// arrange
            byte[] payload = TestingSupport.GetRandomNumberOfRandomBytes(CommonSupport.PracticalInt32Min, CommonSupport.PracticalInt32Max);

            string expected = TestingSupport.GetHashOfBytes(payload).Base64String;

            //// act
            TempByteArray intermediate = new TempByteArray(payload);

            //// assert
            Assert.AreEqual<int>(payload.Length, intermediate.bytes.Length);
            Assert.AreEqual<string>(expected, TestingSupport.GetHashOfBytes(intermediate.bytes).Base64String);

            //// act
            intermediate.Redact(false);

            //// assert
            Assert.AreEqual<int>(payload.Length, intermediate.bytes.Length);
            Assert.IsTrue(intermediate.bytes.All<byte>(x => x == 0));

            //// act
            intermediate.Redact();

            //// assert
            Assert.AreEqual<int>(0, intermediate.bytes.Length);
            
        }

        [TestMethod]
        public void TempBytesThatHoldsDateTime()
        {
            //// arrange
            DateTime raw = new DateTime(0, DateTimeKind.Utc);
            DateTime cooked = raw.AddTicks(750);
            DateTime original = new DateTime(raw.Year, raw.Month, raw.Day, raw.Hour, raw.Minute, raw.Second, DateTimeKind.Utc);

            //// assert
            Assert.AreEqual<DateTime>(raw, original);

            //// act
            TempBytesThatHoldsDateTime intermediate = new TempBytesThatHoldsDateTime(raw);

            //// assert
            Assert.AreEqual<DateTime>(original,intermediate.Timestamp);

            //// act
            intermediate = new TempBytesThatHoldsDateTime(cooked);

            //// assert
            Assert.AreEqual<DateTime>(original, intermediate.Timestamp);
        }
    }
}
