using System;
#if WINDOWS && DIRECTX || WINDOWS_UAP
using SharpDX.DXGI;
using SharpDX.Direct3D11;
#endif
#if WINDOWS_UAP
using SharpDX;
using SharpDX.Mathematics.Interop;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
#endif


namespace Microsoft.Xna.Framework.Graphics
{
#if WINDOWS && DIRECTX || WINDOWS_UAP

    /// <summary>
    /// A swap chain used for rendering to a secondary GameWindow or SwapChainPanel (UWP).
    /// </summary>
    /// <remarks>
    /// This is an extension and not part of stock XNA.
    /// It is currently implemented for Windows desktop (DirectX only) and UWP.
    /// </remarks>
#if WINDOWS_UAP
    [CLSCompliant(false)]
#endif
    public class SwapChainRenderTarget : RenderTarget2D
    {
        // TODO: Switch to a new SharpDX version for Windows and merge the code paths.
        // This class contains slightly different code paths for desktop and UWP because MonoGame
        // for Windows uses SharpDX 2.6.3, which does not support SwapChain1, whereas newer versions
        // of SharpDX do.

#if WINDOWS_UAP
        private readonly SwapChainPanel _swapChainPanel;
        private SwapChain1 _swapChain;
#else
        private IntPtr _windowHandle;
        private SwapChain _swapChain;
#endif

        public PresentInterval PresentInterval;

        public SwapChainRenderTarget(GraphicsDevice graphicsDevice,
#if WINDOWS_UAP
                                     SwapChainPanel swapChainPanel,
                                     double width,
                                     double height,
                                     float compositionScaleX,
                                     float compositionScaleY)
#else
                                     IntPtr windowHandle,
                                     int width,
                                     int height)
#endif

            : this(
                graphicsDevice,
#if WINDOWS_UAP
                swapChainPanel,
                width,
                height,
                compositionScaleX,
                compositionScaleY,
#else
                windowHandle, 
                width,
                height,
#endif
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24,
                0,
                RenderTargetUsage.DiscardContents,
                PresentInterval.Default)
        {
        }


        public SwapChainRenderTarget(GraphicsDevice graphicsDevice,
#if WINDOWS_UAP
                                     SwapChainPanel swapChainPanel,
                                     double width,
                                     double height,
                                     float compositionScaleX,
                                     float compositionScaleY,
#else
                                     IntPtr windowHandle,
                                     int width,
                                     int height,
#endif
                                     bool mipMap,
                                     SurfaceFormat surfaceFormat,
                                     DepthFormat depthFormat,
                                     int preferredMultiSampleCount,
                                     RenderTargetUsage usage,
                                     PresentInterval presentInterval)
            : base(
                graphicsDevice,
                (int)width,
                (int)height,
                mipMap,
                surfaceFormat,
                depthFormat,
                preferredMultiSampleCount,
                usage,
                SurfaceType.SwapChainRenderTarget)
        {
#if WINDOWS_UAP
            if (swapChainPanel == null)
                throw new ArgumentNullException("swapChainPanel");

            _swapChainPanel = swapChainPanel;
            PresentInterval = presentInterval;
            CreateOrResize(width, height, compositionScaleX, compositionScaleY);
#else
            _windowHandle = windowHandle;
            PresentInterval = presentInterval;
            CreateOrResize(width, height);
#endif
        }


        private void CreateOrResize(
#if WINDOWS_UAP
                                    double width,
                                    double height,
                                    float compositionScaleX,
                                    float compositionScaleY
#else
                                    int width,
                                    int height
#endif
                                   )
        {
            var dxgiFormat = Format == SurfaceFormat.Color
                             ? SharpDX.DXGI.Format.B8G8R8A8_UNorm
                             : SharpDXHelper.ToFormat(Format);

            var multisampleDesc = new SampleDescription(1, 0);
            if (MultiSampleCount > 1)
            {
                multisampleDesc.Count = MultiSampleCount;
                multisampleDesc.Quality = (int)StandardMultisampleQualityLevels.StandardMultisamplePattern;
            }

            var d3dDevice = GraphicsDevice._d3dDevice;

#if WINDOWS_UAP
            // Convert width/height from FrameworkElement.ActualWidth/ActualHeight to the required
            // render target size.
            width = (int)Math.Ceiling(Math.Max(width * compositionScaleX, 1));
            height = (int)Math.Ceiling(Math.Max(height * compositionScaleY, 1));
#endif

            this.width = (int)width;
            this.height = (int)height;

            if (_swapChain != null)
            {
                // Swap chain panel exists. Resize it.
                _renderTargetViews[0].Dispose();
                SharpDX.Utilities.Dispose(ref _depthStencilView);
                SharpDX.Utilities.Dispose(ref _resourceView);
                SharpDX.Utilities.Dispose(ref _texture);

                _swapChain.ResizeBuffers(2, this.width, this.height, dxgiFormat, SwapChainFlags.None);
            }
            else
            {
                // Create new swap chain.
#if WINDOWS_UAP
                var desc = new SwapChainDescription1
                {
                    // Automatic sizing
                    Width = this.width,
                    Height = this.height,
                    Format = dxgiFormat,
                    Stereo = false,
                    SampleDescription = multisampleDesc,
                    Usage = Usage.RenderTargetOutput,
                    BufferCount = 2,
                    SwapEffect = SharpDXHelper.ToSwapEffect(PresentInterval),
                    Scaling = Scaling.Stretch,
                    AlphaMode = AlphaMode.Premultiplied, // TODO: Use Ignore for performance if we do not need alpha.
                };
#else
                var desc = new SwapChainDescription()
                {
                    ModeDescription =
                    {
                        Format = dxgiFormat,
                        Scaling = DisplayModeScaling.Stretched,
                        Width = this.width,
                        Height = this.height,
                    },

                    OutputHandle = _windowHandle,
                    SampleDescription = multisampleDesc,
                    Usage = Usage.RenderTargetOutput,
                    BufferCount = 2,
                    SwapEffect = SharpDXHelper.ToSwapEffect(PresentInterval),
                    IsWindowed = true,
                };
#endif

                // Once the desired swap chain description is configured, it must 
                // be created on the same adapter as our D3D Device

                // Retrieve the underlying DXGI Device from the D3D Device and create the SwapChain.
#if WINDOWS_UAP
                using (var dxgiDevice2 = d3dDevice.QueryInterface<SharpDX.DXGI.Device2>())
                using (var dxgiAdapter = dxgiDevice2.Adapter)
                using (var dxgiFactory2 = dxgiAdapter.GetParent<Factory2>())
                {
                    _swapChain = new SwapChain1(dxgiFactory2, dxgiDevice2, ref desc, null);

                    // The native panel has to be accessed on the UI thread.
                    var dispatcher = _swapChainPanel.Dispatcher;
                    if (dispatcher.HasThreadAccess)
                    {
                        using (var nativePanel = ComObject.As<ISwapChainPanelNative>(_swapChainPanel))
                            nativePanel.SwapChain = _swapChain;
                    }
                    else
                    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            using (var nativePanel = ComObject.As<ISwapChainPanelNative>(_swapChainPanel))
                                nativePanel.SwapChain = _swapChain;
                        });
#pragma warning restore CS4014
                    }
                }

#else
                using (var dxgiDevice = d3dDevice.QueryInterface<SharpDX.DXGI.Device1>())
                using (var dxgiAdapter = dxgiDevice.Adapter)
                using (var dxgiFactory = dxgiAdapter.GetParent<Factory1>())
                {
                    _swapChain = new SwapChain(dxgiFactory, dxgiDevice, desc);
                }
#endif
            }

#if WINDOWS_UAP
            _swapChain.Rotation = DisplayModeRotation.Identity;

            // Counter act the composition scale of the render target as 
            // we already handle this in the platform window code. 
            using (var swapChain2 = _swapChain.QueryInterface<SwapChain2>())
            {
                var inverseScale = new RawMatrix3x2();
                inverseScale.M11 = 1.0f / compositionScaleX;
                inverseScale.M22 = 1.0f / compositionScaleY;
                swapChain2.MatrixTransform = inverseScale;
            }
#endif

            // Obtain the backbuffer for this window which will be the final 3D rendertarget.
            var backBuffer = SharpDX.Direct3D11.Resource.FromSwapChain<SharpDX.Direct3D11.Texture2D>(_swapChain, 0);

            // Create a view interface on the rendertarget to use on bind.
            _renderTargetViews = new[] { new RenderTargetView(d3dDevice, backBuffer) };

            // Get the rendertarget dimensions for later.
            var backBufferDesc = backBuffer.Description;
            var targetSize = new Point(backBufferDesc.Width, backBufferDesc.Height);

            _texture = backBuffer;

            // Create the depth buffer if we need it.
            if (DepthStencilFormat != DepthFormat.None)
            {
                dxgiFormat = SharpDXHelper.ToFormat(DepthStencilFormat);

                // Allocate a 2-D surface as the depth/stencil buffer.
                using (
                    var depthBuffer = new SharpDX.Direct3D11.Texture2D(d3dDevice,
                                                                       new Texture2DDescription()
                                                                           {
                                                                               Format = dxgiFormat,
                                                                               ArraySize = 1,
                                                                               MipLevels = 1,
                                                                               Width = targetSize.X,
                                                                               Height = targetSize.Y,
                                                                               SampleDescription = multisampleDesc,
                                                                               Usage = ResourceUsage.Default,
                                                                               BindFlags = BindFlags.DepthStencil,
                                                                           }))

                    // Create a DepthStencil view on this surface to use on bind.
                    _depthStencilView = new DepthStencilView(d3dDevice, depthBuffer);
            }
        }

        /// <summary>
        /// Resizes the swap chain.
        /// </summary>
        /// <param name="width">
        /// The new width of the game window in pixels or the actual width of the swap chain panel.
        /// </param>
        /// <param name="height">
        /// The new height of the game window in pixels or the actual height of the swap chain panel.
        /// </param>
#if WINDOWS_UAP
        /// <param name="compositionScaleX">
        /// The new x-axis scale factor of the swap chain panel.
        /// </param>
        /// <param name="compositionScaleY">
        /// The new y-axis scale factor of the swap chain panel.
        /// </param>
#endif
        public void Resize(
#if WINDOWS_UAP
                           double width,
                           double height,
                           float compositionScaleX,
                           float compositionScaleY
#else
                           int width,
                           int height
#endif
                          )
        {
#if WINDOWS_UAP
            CreateOrResize(width, height, compositionScaleX, compositionScaleY);
#else
            CreateOrResize(width, height);
#endif
        }
    
        // TODO: We need to expose the other Present() overloads
        // for passing source/dest rectangles.

        /// <summary>
        /// Displays the contents of the active back buffer to the screen.
        /// </summary>
        public void Present()
        {
#if WINDOWS_UAP
            var parameters = new PresentParameters();
#endif
            lock (GraphicsDevice._d3dContext)
            {
                try
                {
#if WINDOWS_UAP
                    _swapChain.Present(1, PresentFlags.None, parameters);
#else
                    _swapChain.Present(PresentInterval.GetFrameLatency(), PresentFlags.None);
#endif
                }
                catch (SharpDX.SharpDXException)
                {
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SharpDX.Utilities.Dispose(ref _swapChain);
            }

            base.Dispose(disposing);
        }
    }

#endif // WINDOWS && DIRECTX
}
