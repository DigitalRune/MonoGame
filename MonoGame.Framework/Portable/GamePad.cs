// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.


namespace Microsoft.Xna.Framework.Input
{
    public static class GamePad
    {
        public static GamePadCapabilities GetCapabilities(PlayerIndex playerIndex)
        {
            throw MonoGame.Portable.NotImplementedException;
        }

        public static GamePadState GetState(PlayerIndex playerIndex)
        {
            return GetState(playerIndex, GamePadDeadZone.IndependentAxes);
        }

        public static GamePadState GetState(PlayerIndex playerIndex, GamePadDeadZone deadZoneMode)
        {
            throw MonoGame.Portable.NotImplementedException;
        }
    }
}
