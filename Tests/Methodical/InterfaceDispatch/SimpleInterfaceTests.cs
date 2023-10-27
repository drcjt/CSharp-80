namespace InterfaceDispatch
{
    interface IMyInterface
    {
        int GetAnInt();
    }

    class Foo0 : IMyInterface { public int GetAnInt() => 0; }
    class Foo1 : IMyInterface { public int GetAnInt() => 1; }
    class Foo2 : IMyInterface { public int GetAnInt() => 2; }
    class Foo3 : IMyInterface { public int GetAnInt() => 3; }
    class Foo4 : IMyInterface { public int GetAnInt() => 4; }

    internal class SimpleInterfaceTests
    {
        private static IMyInterface[] MakeInterfaceArray()
        {
            IMyInterface[] interfaces =
            [
                new Foo0(),
                new Foo1(),
                new Foo2(),
                new Foo3(),
                new Foo4(),
            ];
            return interfaces;
        }

        public static int RunTests()
        {
            IMyInterface[] interfaces = MakeInterfaceArray();

            int counter = 0;
            for (int i = 0; i < interfaces.Length; i++)
            {
                counter += interfaces[i].GetAnInt();
            }

            if (counter != 10)
            {
                return 1;
            }

            return 0;
        }
    }
}