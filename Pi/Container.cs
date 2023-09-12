using System.Threading;

namespace Pi
{
    public class Container
    {
        public double Pi
        {
            get;
            set;
        }

        public CountdownEvent Event
        {
            get;
            set;
        }
    }
}