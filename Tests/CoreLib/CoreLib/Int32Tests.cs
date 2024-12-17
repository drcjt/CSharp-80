namespace CoreLib
{
    public class Int32Tests
    {
        public static void Parse_Valid()
        {
            Assert.AreEqual(0, int.Parse("0"));
            Assert.AreEqual(0, int.Parse("0000000000000000000000000000000000000000000000000000000000"));
            Assert.AreEqual(1, int.Parse("0000000000000000000000000000000000000000000000000000000001"));
            Assert.AreEqual(2147483647, int.Parse("2147483647"));
            Assert.AreEqual(2147483647, int.Parse("02147483647"));
            Assert.AreEqual(2147483647, int.Parse("00000000000000000000000000000000000000000000000002147483647"));
            Assert.AreEqual(123, int.Parse("123\0\0"));

            Assert.AreEqual(123, int.Parse("123"));
        }
    }
}
