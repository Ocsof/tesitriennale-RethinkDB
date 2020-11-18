﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RethinkDbApp.Exception
{
    [Serializable]
    public class NewGuidException : System.Exception
    {
        private readonly static string message = "Il Guid creato è già presente";
        public NewGuidException() : base(message)
        {

        }
    }
}
