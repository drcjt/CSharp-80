namespace ConstantFolding
{
    public static class Tests
    {
        public static int Main()
        {
            int result = 0;
            result += TestIntSubtractionMorphToAddition(25, 20);
            result += TestIntAdditionWithMultipleConstants(13, 90);
            result += TestIntMultiplicationWithMultipleConstants(13, 4680);

            result += TestNIntSubtractionMorphToAddition(25, 20);
            result += TestNIntAdditionWithMultipleConstants(13, 90);
            result += TestNIntMultiplicationWithMultipleConstants(13, 4680);

            return result;
        }

        private static int TestNIntSubtractionMorphToAddition(nint value, nint expected)
        {

            if (value - 5 != expected)
            {
                return 1;
            }

            return 0;
        }

        private static int TestNIntAdditionWithMultipleConstants(nint value, nint expected)
        {
            if (value + 5 + 72 != expected)
            {
                return 1;
            }

            return 0;
        }

        private static int TestNIntMultiplicationWithMultipleConstants(nint value, nint expected)
        {
            if (value * 5 * 72 != expected)
            {
                return 1;
            }

            return 0;
        }

        private static int TestIntSubtractionMorphToAddition(int value, int expected)
        {

            if (value - 5 != expected)
            {
                return 1;
            }

            return 0;
        }

        private static int TestIntAdditionWithMultipleConstants(int value, int expected)
        {
            if (value + 5 + 72 != expected)
            {
                return 1;
            }

            return 0;
        }

        private static int TestIntMultiplicationWithMultipleConstants(int value, int expected)
        {
            if (value * 5 * 72 != expected)
            {
                return 1;
            }

            return 0;
        }
    }
}