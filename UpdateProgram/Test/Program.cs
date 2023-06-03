using System;
using UpdateProgram;


namespace Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(UpdateProgramFunctions.CheckRDPConnection());

            Console.ReadKey();
        }
    }
}