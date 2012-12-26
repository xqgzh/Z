using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Z.Entity
{
    /// <summary>
    /// Describe the How to user IDataObject interface in EntityTools
    /// </summary>
    public enum EnumDataObjectUsage
    {
        /// <summary>
        /// DO Not Use IDataObject Interface
        /// </summary>
        NONE,

        /// <summary>
        /// GET
        /// </summary>
        GET,

        /// <summary>
        /// SET
        /// </summary>
        SET
    }
}
