// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;
using System;

namespace Microsoft.Xna.Framework.Media
{
    partial class VideoPlayer
    {
        private void PlatformInitialize()
        {
        }

        private Texture2D PlatformGetTexture()
        {
            return null;
        }

        private void PlatformPause()
        {
        }

        private void PlatformPlay()
        {
        }

        private void PlatformResume()
        {
        }

        private void PlatformStop()
        {
        }

        private void PlatformSetVolume()
        {
        }

        private void PlatformSetIsLooped()
        {
        }

        private void PlatformSetIsMuted()
        {
        }

        private TimeSpan PlatformGetPlayPosition()
        {
            return TimeSpan.Zero;
        }

        private void PlatformDispose(bool disposing)
        {
        }
    }
}
