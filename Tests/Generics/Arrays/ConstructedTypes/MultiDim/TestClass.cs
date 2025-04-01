namespace MultiDim.ConstructedTypes
{
    public class GenericWrapper<T>
    {
        public T Field;

        public GenericWrapper(T field)
        {
            Field = field;
        }
    }

    public static class TestClass
    {
        public static int Main()
        {
            int size = 10;
            int i, j, k;
            int locationNumber = 0;

            GenericWrapper<int>[,,] GenArray = new GenericWrapper<int>[size, size, size];
            for (i = 0; (i < size); i++)
            {
                for (j = 0; (j < size); j++)
                {
                    for (k = 0; (k < size); k++)
                    {
                        GenArray[i, j, k] = new GenericWrapper<int>(locationNumber);
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
                        sum += GenArray[i, j, k].Field;
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