// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

#if PORTABLE
using System;


namespace MonoGame
{
    internal static class Portable
    {
        public const string Message = "This functionality is not implemented in the PCL (Portable Class Library) implementation of MonoGame. To use this functionality the executable should reference a platform-specific MonoGame DLL.";


        public static NotImplementedException NotImplementedException
        {
            get { return new NotImplementedException(Message); }
        }
    }
}
#endif