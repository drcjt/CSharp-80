namespace Regression
{
    internal class SpillImportAppendTests
    {
        private int x = 2;
        public int SpillOnStFldImport()
        {
            var temp = x;
            x = 3;
            return temp;
        }
    }
}
