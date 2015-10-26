// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Windows.Devices.Input;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Microsoft.Xna.Framework
{
    internal class InputEvents
    {
        private readonly CoreWindow _window;
        private readonly TouchQueue _touchQueue;
        private readonly List<Keys> _keys = new List<Keys>();

        public InputEvents(CoreWindow window, UIElement inputElement, TouchQueue touchQueue)
        {
            _window = window;
            _touchQueue = touchQueue;

            // The key events are always tied to the window as those will
            // only arrive here if some other control hasn't gotten it.
            window.KeyDown += CoreWindow_KeyDown;
            window.KeyUp += CoreWindow_KeyUp;
            window.VisibilityChanged += CoreWindow_VisibilityChanged;
            window.Activated += CoreWindow_Activated;
            window.SizeChanged += CoreWindow_SizeChanged;

            if (inputElement != null)
            {
                // If we have an input UIElement then we bind input events
                // to it else we'll get events for overlapping XAML controls.
                inputElement.PointerPressed += UIElement_PointerPressed;
                inputElement.PointerReleased += UIElement_PointerReleased;
                inputElement.PointerCanceled += UIElement_PointerReleased;
                inputElement.PointerMoved += UIElement_PointerMoved;
                inputElement.PointerWheelChanged += UIElement_PointerWheelChanged;
            }
            else
            {
                // If we only have a CoreWindow then use it for input events.
                window.PointerPressed += CoreWindow_PointerPressed;
                window.PointerReleased += CoreWindow_PointerReleased;
                window.PointerMoved += CoreWindow_PointerMoved;
                window.PointerWheelChanged += CoreWindow_PointerWheelChanged;
            }
        }

        #region UIElement Events

        private void UIElement_PointerPressed(object sender, PointerRoutedEventArgs args)
        {
            //Capture this pointer so we continue getting events even if it is dragged off us
            ((UIElement)sender).CapturePointer(args.Pointer);

            var pointerPoint = args.GetCurrentPoint(null);
            PointerPressed(pointerPoint, sender as UIElement, args.Pointer);
            args.Handled = true;
        }

        private void UIElement_PointerMoved(object sender, PointerRoutedEventArgs args)
        {
            var pointerPoint = args.GetCurrentPoint(null);
            PointerMoved(pointerPoint);
            args.Handled = true;
        }

        private void UIElement_PointerReleased(object sender, PointerRoutedEventArgs args)
        {
            ((UIElement)sender).ReleasePointerCapture(args.Pointer);

            var pointerPoint = args.GetCurrentPoint(null);
            PointerReleased(pointerPoint, sender as UIElement, args.Pointer);
            args.Handled = true;
        }

        private void UIElement_PointerWheelChanged(object sender, PointerRoutedEventArgs args)
        {
            var pointerPoint = args.GetCurrentPoint(null);
            UpdateMouse(pointerPoint, GetPosition(pointerPoint));
            args.Handled = true;
        }

        #endregion // UIElement Events

        #region CoreWindow Events

        private void CoreWindow_PointerPressed(object sender, PointerEventArgs args)
        {
            PointerPressed(args.CurrentPoint, null, null);
            args.Handled = true;
        }

        private void CoreWindow_PointerMoved(object sender, PointerEventArgs args)
        {
            PointerMoved(args.CurrentPoint);
            args.Handled = true;
        }

        private void CoreWindow_PointerReleased(object sender, PointerEventArgs args)
        {
            PointerReleased(args.CurrentPoint, null, null);
            args.Handled = true;
        }

        private void CoreWindow_PointerWheelChanged(object sender, PointerEventArgs args)
        {
            UpdateMouse(args.CurrentPoint, GetPosition(args.CurrentPoint));
            args.Handled = true;
        }

        #endregion // CoreWindow Events

        private void PointerPressed(PointerPoint pointerPoint, UIElement target, Pointer pointer)
        {
            var pos = GetPosition(pointerPoint);

            var isTouch = pointerPoint.PointerDevice.PointerDeviceType == PointerDeviceType.Touch;

            _touchQueue.Enqueue((int)pointerPoint.PointerId, TouchLocationState.Pressed, pos, !isTouch);
            
            // Mouse or stylus event.
            UpdateMouse(pointerPoint, pos);

            if (!isTouch)
            {
                // Capture future pointer events until a release.		
                if (target != null)
                    target.CapturePointer(pointer);
            }
        }

        private void PointerMoved(PointerPoint pointerPoint)
        {
            var pos = GetPosition(pointerPoint);

            var isTouch = pointerPoint.PointerDevice.PointerDeviceType == PointerDeviceType.Touch;
            var touchIsDown = pointerPoint.IsInContact;

            if (touchIsDown)
            {
                _touchQueue.Enqueue((int)pointerPoint.PointerId, TouchLocationState.Moved, pos, !isTouch);
            }

            // Mouse or stylus event.
            UpdateMouse(pointerPoint, pos);
        }

        private void PointerReleased(PointerPoint pointerPoint, UIElement target, Pointer pointer)
        {
            var pos = GetPosition(pointerPoint);

            var isTouch = pointerPoint.PointerDevice.PointerDeviceType == PointerDeviceType.Touch;

            _touchQueue.Enqueue((int)pointerPoint.PointerId, TouchLocationState.Released, pos, !isTouch);

            // Mouse or stylus event.
            UpdateMouse(pointerPoint, pos);

            if (!isTouch)
            {
                // Release the captured pointer.
                if (target != null)
                    target.ReleasePointerCapture(pointer);
            }
        }

        private void UpdateMouse(PointerPoint point, Vector2 position)
        {
            var state = point.Properties;
            Mouse.PrimaryWindow.MouseState.X = (int)position.X;
            Mouse.PrimaryWindow.MouseState.Y = (int)position.Y;
            Mouse.PrimaryWindow.MouseState.ScrollWheelValue += state.MouseWheelDelta;
            Mouse.PrimaryWindow.MouseState.LeftButton = state.IsLeftButtonPressed ? ButtonState.Pressed : ButtonState.Released;
            Mouse.PrimaryWindow.MouseState.RightButton = state.IsRightButtonPressed ? ButtonState.Pressed : ButtonState.Released;
            Mouse.PrimaryWindow.MouseState.MiddleButton = state.IsMiddleButtonPressed ? ButtonState.Pressed : ButtonState.Released;
        }


        // Converts the position from device independent pixels to pixels relative to the 
        // graphics device back buffer.
        private Vector2 GetPosition(PointerPoint pointerPoint)
        {
            // To convert from DIPs (device independent pixels) to screen resolution pixels.
            var dipFactor = DisplayProperties.LogicalDpi / 96.0f;

            // To scale from window resolution to graphics device back buffer resolution.
            var graphicsDeviceManager = Game.Instance.graphicsDeviceManager;

#if WINDOWS_UAP
            var clientBounds = UAPGameWindow.Instance.ClientBounds;
#else
            var clientBounds = MetroGameWindow.Instance.ClientBounds;
#endif
            var backBufferScaleX = graphicsDeviceManager.PreferredBackBufferWidth / (float)clientBounds.Width;
            var backBufferScaleY = graphicsDeviceManager.PreferredBackBufferHeight / (float)clientBounds.Height;

            return new Vector2(
                (float)(pointerPoint.Position.X) * dipFactor * backBufferScaleX,
                (float)(pointerPoint.Position.Y) * dipFactor * backBufferScaleY);
        }

        public void UpdateState()
        {
            // Check ALT keys in every tick. (ALT keys don't fire KeyUp/KeyDown events.)
            if (IsDown(Windows.System.VirtualKey.LeftMenu))
            {
                if (!_keys.Contains(Keys.LeftAlt))
                    _keys.Add(Keys.LeftAlt);
            }
            else
                _keys.Remove(Keys.LeftAlt);

            if (IsDown(Windows.System.VirtualKey.RightMenu))
            {
                if (!_keys.Contains(Keys.RightAlt))
                    _keys.Add(Keys.RightAlt);
            }
            else
                _keys.Remove(Keys.RightAlt);

            // Update the keyboard state.
            Keyboard.SetKeys(_keys);
        }


        private bool IsDown(Windows.System.VirtualKey key)
        {
            var keyState = _window.GetKeyState(key);
            return (keyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }

        private static Keys KeyTranslate(Windows.System.VirtualKey inkey, CorePhysicalKeyStatus keyStatus)
        {
            switch (inkey)
            {
                // WinRT does not distinguish between left/right keys
                // We have to check for special keys such as control/shift/alt/ etc
                case Windows.System.VirtualKey.Control:
                    // we can detect right Control by checking the IsExtendedKey value.
                    return (keyStatus.IsExtendedKey) ? Keys.RightControl : Keys.LeftControl;
                case Windows.System.VirtualKey.Shift:
                    // we can detect right shift by checking the scancode value.
                    // left shift is 0x2A, right shift is 0x36. IsExtendedKey is always false.
                    return (keyStatus.ScanCode==0x36) ? Keys.RightShift : Keys.LeftShift;
                // Note that the Alt key is now refered to as Menu.
                case Windows.System.VirtualKey.Menu:
                    return Keys.LeftAlt;

                default:
                    return (Keys)inkey;
            }
        }

        private void CoreWindow_KeyUp(object sender, KeyEventArgs args)
        {
            var xnaKey = KeyTranslate(args.VirtualKey, args.KeyStatus);

            if (_keys.Contains(xnaKey))
                _keys.Remove(xnaKey);
        }

        private void CoreWindow_KeyDown(object sender, KeyEventArgs args)
        {
            var xnaKey = KeyTranslate(args.VirtualKey, args.KeyStatus);

            if (!_keys.Contains(xnaKey))
                _keys.Add(xnaKey);
        }

        private void CoreWindow_SizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            // If the window is resized then also 
            // drop any current key states.
            _keys.Clear();
        }

        private void CoreWindow_Activated(CoreWindow sender, WindowActivatedEventArgs args)
        {
            // Forget about the held keys when we lose focus as we don't
            // receive key events for them while we are in the background
            if (args.WindowActivationState == CoreWindowActivationState.Deactivated)
                _keys.Clear();
        }

        private void CoreWindow_VisibilityChanged(CoreWindow sender, VisibilityChangedEventArgs args)
        {
            // Forget about the held keys when we disappear as we don't
            // receive key events for them while we are in the background
            if (!args.Visible)
                _keys.Clear();
        }
    }
}
