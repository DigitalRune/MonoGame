#region License
/*
Microsoft Public License (Ms-PL)
MonoGame - Copyright © 2009 The MonoGame Team

All rights reserved.

This license governs use of the accompanying software. If you use the software, you accept this license. If you do not
accept the license, do not use the software.

1. Definitions
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under 
U.S. copyright law.

A "contribution" is the original software, or any additions or changes to the software.
A "contributor" is any person that distributes its contribution under this license.
"Licensed patents" are a contributor's patent claims that read directly on its contribution.

2. Grant of Rights
(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

3. Conditions and Limitations
(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
your patent license from such contributor to the software ends automatically.
(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution 
notices that are present in the software.
(D) If you distribute any portion of the software in source code form, you may do so only under this license by including 
a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object 
code form, you may only do so under a license that complies with this license.
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees
or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent
permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular
purpose and non-infringement.
*/
#endregion License

using System;

#if MONOMAC || WINDOWS
using System.Runtime.InteropServices;
using System.Drawing;
#endif

#if OPENGL
#if DESKTOPGL
using MouseInfo = OpenTK.Input.Mouse;
#elif MONOMAC
using MonoMac.Foundation;
using MonoMac.AppKit;
#endif
#endif
#if WINDOWS && DIRECTX
using SharpDX.Multimedia;
using SharpDX.RawInput;
#endif
#if (WINDOWS_STOREAPP && !WINDOWS_PHONE81) || WINDOWS_UAP
using Windows.Devices.Input;
#endif


namespace Microsoft.Xna.Framework.Input
{
    /// <summary>
    /// Allows reading position and button click information from mouse.
    /// </summary>
    public static class Mouse
    {
        internal static GameWindow PrimaryWindow = null;

#if ANDROID
        internal static MouseState CurrentState = new MouseState();
#else
        private static readonly MouseState _defaultState = new MouseState();
#endif

        // Fields for relative mouse movement info:
        // For a first person shooter, the mouse position info needs to be relative and not be 
        // limited by the screen border. This can be achieved by calling SetPosition() each frame
        // and measuring the position change between the current pos and the pos specified in 
        // SetPosition(). However, this is not accurate when the frame rate is high (>>60 Hz).
        // (Mouse seems to be slow.) In WINDOWS && DIRECTX we can use the raw mouse device for
        // accurate mouse movement. (See also SetPosition() and MouseState.DeltaX/DeltaY.)
        private static int _defaultPositionX;
        private static int _defaultPositionY;
        private static bool _isRelative;
#if (WINDOWS && DIRECTX) || (WINDOWS_STOREAPP && !WINDOWS_PHONE81) || WINDOWS_UAP
        private static int _deltaX;
        private static int _deltaY;
#endif

        /// <summary>
        /// Gets or sets a value indicating whether the game requires relative mouse movement data
        /// instead of absolute mouse movement data.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the game requires relative mouse movement data; otherwise, 
        /// <see langword="false" />.
        /// </value>
        /// <remarks>
        /// Absolute mouse movement is used in desktop applications. Relative mouse movement is 
        /// required, for example, for first person shooter controls. When <see cref="IsRelative"/> 
        /// is <see langword="true"/>, the mouse cursor is not limited by the screen boundaries,
        /// and <see cref="MouseState.DeltaX"/> and <see cref="MouseState.DeltaY"/> can be used
        /// to get the relative mouse movement since the last <see cref="SetPosition"/> call.
        /// </remarks>
        public static bool IsRelative
        {
            get { return _isRelative; }
            set
            {
                _isRelative = value;

#if WINDOWS && DIRECTX || (WINDOWS_STOREAPP && !WINDOWS_PHONE81) || WINDOWS_UAP
                if (_isRelative)
                {
#if WINDOWS && DIRECTX
                    // Register the raw mouse device once, and never unregister the device or event.
                    Device.RegisterDevice(UsagePage.Generic, UsageId.GenericMouse, DeviceFlags.InputSink, WindowHandle);
                    Device.MouseInput += OnRawMouseInput;
#elif WINDOWS_STOREAPP || WINDOWS_UAP
                    MouseDevice.GetForCurrentView().MouseMoved += OnWinStoreMouseMoved;
#endif
                }
                else
                {
#if WINDOWS && DIRECTX
                    Device.MouseInput -= OnRawMouseInput;
#elif WINDOWS_STOREAPP || WINDOWS_UAP
                    // We have to unregister the event handler because while the event is handled 
                    // the mouse behaves differently than the default absolute mouse.
                    MouseDevice.GetForCurrentView().MouseMoved -= OnWinStoreMouseMoved;
#endif
                }
#endif
            }
        }

#if DESKTOPGL || ANGLE

        static OpenTK.INativeWindow Window;

        internal static void setWindows(GameWindow window)
        {
            PrimaryWindow = window;
            if (window is OpenTKGameWindow)
            {
                Window = (window as OpenTKGameWindow).Window;
            }
        }

#elif (WINDOWS && DIRECTX)

        static System.Windows.Forms.Form Window;

        internal static void SetWindows(System.Windows.Forms.Form window)
        {
            Window = window;
        }

#elif MONOMAC
        internal static GameWindow Window;
        internal static float ScrollWheelValue;
#endif

        /// <summary>
        /// Gets or sets the window handle for current mouse processing.
        /// </summary> 
        public static IntPtr WindowHandle 
        { 
            get
            { 
#if DESKTOPGL || ANGLE
                return Window.WindowInfo.Handle;
#elif WINRT
                return IntPtr.Zero; // WinRT platform does not create traditionally window, so returns IntPtr.Zero.
#elif(WINDOWS && DIRECTX)
                return Window.Handle; 
#elif MONOMAC
                return IntPtr.Zero;
#else
                return IntPtr.Zero;
#endif
            }
            set
            {
                // only for XNA compatibility, yet
            }
        }

        #region Public methods

        /// <summary>
        /// This API is an extension to XNA.
        /// Gets mouse state information that includes position and button
        /// presses for the provided window
        /// </summary>
        /// <returns>Current state of the mouse.</returns>
        public static MouseState GetState(GameWindow window)
        {
#if MONOMAC
            //We need to maintain precision...
            window.MouseState.ScrollWheelValue = (int)ScrollWheelValue;

#elif DESKTOPGL || ANGLE

            var state = OpenTK.Input.Mouse.GetCursorState();
            var pc = ((OpenTKGameWindow)window).Window.PointToClient(new System.Drawing.Point(state.X, state.Y));
            window.MouseState.X = pc.X;
            window.MouseState.Y = pc.Y;

            window.MouseState.LeftButton = (ButtonState)state.LeftButton;
            window.MouseState.RightButton = (ButtonState)state.RightButton;
            window.MouseState.MiddleButton = (ButtonState)state.MiddleButton;
            window.MouseState.XButton1 = (ButtonState)state.XButton1;
            window.MouseState.XButton2 = (ButtonState)state.XButton2;

            // XNA uses the winapi convention of 1 click = 120 delta
            // OpenTK scales 1 click = 1.0 delta, so make that match
            window.MouseState.ScrollWheelValue = (int)(state.Scroll.Y * 120);
#endif

#if WINDOWS && DIRECTX || WINDOWS_STOREAPP && !WINDOWS_PHONE81 || WINDOWS_UAP
            window.MouseState.DeltaX = _deltaX;
            window.MouseState.DeltaY = _deltaY;
#else
          window.MouseState.DeltaX = window.MouseState.X - _defaultPositionX;
            window.MouseState.DeltaY = window.MouseState.Y - _defaultPositionY;
#endif

            return window.MouseState;
        }

        /// <summary>
        /// Gets mouse state information that includes position and button presses
        /// for the primary window
        /// </summary>
        /// <returns>Current state of the mouse.</returns>
        public static MouseState GetState()
        {
#if ANDROID

            // Before MouseState was changed to take in a 
            // gamewindow, Android seemed to never update 
            // the previous static MouseState that existed.
            // This implies that the default behavior is to return
            // default(MouseState); A static one is used to prevent
            // constant reallocations
            // This will need to change when MonoGame supports desktop Android.
            // Related discussion: https://github.com/mono/MonoGame/pull/1749

            //return _defaultState;

            return CurrentState;
#else
            if (PrimaryWindow != null)
                return GetState(PrimaryWindow);

            return _defaultState;
#endif
        }

        /// <summary>
        /// Sets mouse cursor's relative position to game-window.
        /// </summary>
        /// <param name="x">Relative horizontal position of the cursor.</param>
        /// <param name="y">Relative vertical position of the cursor.</param>
        public static void SetPosition(int x, int y)
        {
            _defaultPositionX = x;
            _defaultPositionY = y;
#if WINDOWS && DIRECTX || WINDOWS_STOREAPP && !WINDOWS_PHONE81 || WINDOWS_UAP
            _deltaX = 0;
            _deltaY = 0;
#endif

            UpdateStatePosition(x, y);

#if (WINDOWS && DIRECTX) || DESKTOPGL || ANGLE
            // correcting the coordinate system
            // Only way to set the mouse position !!!
            var pt = Window.PointToScreen(new System.Drawing.Point(x, y));
#elif WINDOWS
            var pt = new System.Drawing.Point(0, 0);
#endif

#if DESKTOPGL || ANGLE
            OpenTK.Input.Mouse.SetPosition(pt.X, pt.Y);
#elif WINDOWS
            SetCursorPos(pt.X, pt.Y);
#elif MONOMAC
            var mousePt = NSEvent.CurrentMouseLocation;
            NSScreen currentScreen = null;

            if (NSScreen.Screens.Length >= 1)
                currentScreen = NSScreen.Screens[0];

            foreach (var screen in NSScreen.Screens) {
                if (screen.Frame.Contains(mousePt)) {
                    currentScreen = screen;
                    break;
                }
            }
            
            var point = new PointF(x, Window.ClientBounds.Height-y);
            var windowPt = Window.ConvertPointToView(point, null);
            var screenPt = Window.Window.ConvertBaseToScreen(windowPt);
            var flippedPt = new PointF(screenPt.X, currentScreen.Frame.Size.Height-screenPt.Y);
            flippedPt.Y += currentScreen.Frame.Location.Y;
            
            
            CGSetLocalEventsSuppressionInterval(0.0);
            CGWarpMouseCursorPosition(flippedPt);
            CGSetLocalEventsSuppressionInterval(0.25);
#elif WEB
            PrimaryWindow.MouseState.X = x;
            PrimaryWindow.MouseState.Y = y;
#endif
        }

        #endregion Public methods
    

#if WINDOWS && DIRECTX
        private static void OnRawMouseInput(object sender, MouseInputEventArgs mouseEventArgs)
        {
            _deltaX += mouseEventArgs.X;
            _deltaY += mouseEventArgs.Y;
        }
#endif

#if WINDOWS_STOREAPP && !WINDOWS_PHONE81 || WINDOWS_UAP
        private static void OnWinStoreMouseMoved(MouseDevice sender, MouseEventArgs mouseEventArgs)
        {
            _deltaX += mouseEventArgs.MouseDelta.X;
            _deltaY += mouseEventArgs.MouseDelta.Y;
        }
#endif

        private static void UpdateStatePosition(int x, int y)
        {
            PrimaryWindow.MouseState.X = x;
            PrimaryWindow.MouseState.Y = y;
        }

#if WINDOWS

        [DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        internal static extern bool SetCursorPos(int X, int Y);

        /// <summary>
        /// Struct representing a point. 
        /// (Suggestion : Make another class for mouse extensions)
        /// </summary>
        [StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        internal struct POINT
        {
            public int X;
            public int Y;

            public System.Drawing.Point ToPoint()
            {
                return new System.Drawing.Point(X, Y);
            }

        }

#elif MONOMAC
        [DllImport (MonoMac.Constants.CoreGraphicsLibrary)]
        extern static void CGWarpMouseCursorPosition(PointF newCursorPosition);
        
        [DllImport (MonoMac.Constants.CoreGraphicsLibrary)]
        extern static void CGSetLocalEventsSuppressionInterval(double seconds);
#endif

    }
}

