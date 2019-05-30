using System;

namespace Spiritmonger.Cmon.Types
{
    [Flags]
    public enum MetaType : byte
    {
        None = 0,
        Lands = 1,
        Interaction = 2,
        Manipulation = 4,
        Finisher = 8,
        Ramp = 16
    }
}
