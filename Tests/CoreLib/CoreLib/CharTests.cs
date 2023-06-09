namespace CoreLib
{
    public class CharTests
    {
        public static void IsBetweenCharTests()
        {
            Assert.AreEquals(true, char.IsBetween('a', 'a', 'a'));
            Assert.AreEquals(false, char.IsBetween((char)('a' - 1), 'a', 'a'));
            Assert.AreEquals(false, char.IsBetween((char)('a' + 1), 'a', 'a'));
            Assert.AreEquals(true, char.IsBetween('a', 'a', 'b'));
            Assert.AreEquals(true, char.IsBetween('b', 'a', 'b'));
            Assert.AreEquals(false, char.IsBetween((char)('a' - 1), 'a', 'b'));
            Assert.AreEquals(false, char.IsBetween((char)('b' + 1), 'a', 'b'));
            Assert.AreEquals(true, char.IsBetween('a', 'a', 'z'));
            Assert.AreEquals(true, char.IsBetween('m', 'a', 'z'));
            Assert.AreEquals(true, char.IsBetween('z', 'a', 'z'));
            Assert.AreEquals(false, char.IsBetween((char)('a' - 1), 'a', 'z'));
            Assert.AreEquals(false, char.IsBetween((char)('z' + 1), 'a', 'z'));
            Assert.AreEquals(false, char.IsBetween('b', 'c', 'd'));
            Assert.AreEquals(true, char.IsBetween('b', 'd', 'c'));
        }

        public static void IsAsciiDigit_WithAsciiDigits_ReturnsTrue()
        {
            char[] validAsciiDigits = new char[] { '\u0030', '\u0031', '\u0032', '\u0033', '\u0034', '\u0035', '\u0036', '\u0037', '\u0038', '\u0039' };

            for (int i = 0; i < validAsciiDigits.Length; i++) 
            { 
                char ch = validAsciiDigits[i];
                Assert.AreEquals(true, char.IsAsciiDigit(ch));
            }
        }

        public static void IsAsciiDigit_WithNonAsciiDigits_ReturnsFalse()
        {
            char[] invalidAsciiDigits = new char[] 
            { 
                '\u0047','\u004c','\u0051','\u0056','\u00c0','\u00c5','\u00ca','\u00cf','\u00d4','\u00da', /* Uppercase Letters */
                '\u0062','\u0068','\u006e','\u0074','\u007a','\u00e1','\u00e7','\u00ed','\u00f3','\u00fa', /* Lowercase Letters */
                '\u00aa','\u00ba', /* Other Letters */
                '\u00b2','\u00b3','\u00b9','\u00bc','\u00bd','\u00be', /* Other Number */
                '\u0020','\u00a0', /* Space Separators */
                '\u0005','\u000b','\u0011','\u0017','\u001d','\u0082','\u0085','\u008e','\u0094','\u009a', /* Control Characters */
                '\u00ad', /* Format characters */
                '\u005f', /* Connector punctuation */
                '\u002d', /* Dash punctuation */
                '\u0028','\u005b','\u007b', /* Open punctuation */
                '\u0029','\u005d','\u007d', /* Close punctuation */
                '\u00ab', /* Initial Quote punctuation */
                '\u00bb', /* Final quote punctuation */
                '\u002e','\u002f','\u003a','\u003b','\u003f','\u0040','\u005c','\u00a1','\u00a7','\u00b6','\u00b7','\u00bf', /* Other punctuation */
                '\u002b','\u003c','\u003d','\u003e','\u007c','\u007e','\u00ac','\u00b1','\u00d7','\u00f7', /* Math symbols */
                '\u0024','\u00a2','\u00a3','\u00a4','\u00a5', /* Currency symbols */
                '\u005e','\u0060','\u00a8','\u00af','\u00b4','\u00b8', /* Modifier symbols */
                '\u00a6','\u00a9','\u00ae','\u00b0', /* Other symbols */
            };

            for (int i = 0; i < invalidAsciiDigits.Length; i++)
            {
                char ch = invalidAsciiDigits[i];
                Assert.AreEquals(false, char.IsAsciiDigit(ch));
            }
        }
    }
}
