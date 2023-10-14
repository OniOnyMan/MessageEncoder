using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using MessageEncoder.Applications;

namespace MessageEncoder
{
    class Program
    {   
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! \n");
            //new FileToText().Run(args);
            new TextToFile().Run(args);
        }
    }
}
