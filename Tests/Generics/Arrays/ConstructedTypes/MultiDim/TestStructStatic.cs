namespace MultiDim.ConstructedTypes
{

    public struct GenericWrapper<T>
    {
        public T Field;

        public GenericWrapper(T field)
        {
            Field = field;
        }
    }

    public class ArrayHolder
    {
        public static GenericWrapper<int>[,,] GenArray = new GenericWrapper<int>[10, 10, 10];
    }

    public static class TestClassInstance
    {
        public static int Main()
        {
            int size = 10;
            int i, j, k;
            int locationNumber = 0;

            for (i = 0; (i < size); i++)
            {
                for (j = 0; (j < size); j++)
                {
                    for (k = 0; (k < size); k++)
                    {
                        ArrayHolder.GenArray[i, j, k] = new GenericWrapper<int>(locationNumber);
                        locationNumber++;
                    }
                }
            }

            int sum = 0;
            for (i = 0; (i < size); i++)
            {
                for (j = 0; (j < size); j++)
                {
                    for (k = 0; (k < size); k++)
                    {
                        sum += ArrayHolder.GenArray[i, j, k].Field;
                        locationNumber++;
                    }
                }
            }

            if (sum != 499500)
            {
                return 1;
            }

            return 0;
        }
    }
}