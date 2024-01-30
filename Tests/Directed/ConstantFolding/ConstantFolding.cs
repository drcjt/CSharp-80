namespace ConstantFolding
{
    public static class Tests
    {
        public static int Main()
        {
            TestSubtractionMorphToAddition(25, 20);
            TestAdditionWithMultipleConstants(13, 90);
            TestMultiplicationWithMultipleConstants(13, 4680);
            return 0;
        }

        private static int TestSubtractionMorphToAddition(int value, int expected)
        {

            if (value - 5 != expected)
            {
                return 1;
            }

            return 0;
        }

        private static int TestAdditionWithMultipleConstants(int value, int expected)
        {
            if (value + 5 + 72 != expected)
            {
                return 1;
            }

            return 0;
        }

        private static int TestMultiplicationWithMultipleConstants(int value, int expected)
        {
            if (value * 5 * 72 != expected)
            {
                return 1;
            }

            return 0;
        }
    }
}