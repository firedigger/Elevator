using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.DataObjects
{
    public class DirectedElevatorCall
    {
        public uint SourceFloor { get; set; }
        public Direction Direction { get; set; }
    }
}
