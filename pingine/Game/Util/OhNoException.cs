using System;

namespace pingine.Game.Util
{
    /* programmers made a mistake, this is not supposed to happen */
    public class OhNoException : Exception
    {
        public OhNoException(string message) : base(message) { }
    }
}
