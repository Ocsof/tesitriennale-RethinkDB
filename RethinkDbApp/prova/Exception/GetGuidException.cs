﻿using System;

namespace RethinkDbApp.Exception
{
    [Serializable]
    public class GetGuidException : System.Exception
    {
        private readonly static string message = "Il Guid non è presente sul db";
        public GetGuidException() : base(message)
        {

        }
    }
}
