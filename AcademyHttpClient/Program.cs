﻿using AcademyHttpClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AcademyHttpClient
{

    public class Program
    {
        static void Main()
        {
            AcademyClient.RunAsync().Wait();
        }

    }
}


