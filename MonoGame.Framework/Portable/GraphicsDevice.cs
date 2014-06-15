// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;


namespace Microsoft.Xna.Framework.Graphics
{
    public class GraphicsDevice : IDisposable
    {
        public TextureCollection Textures { get; private set; }

        public SamplerStateCollection SamplerStates { get; private set; }

        // TODO Graphics Device events need implementing
        public event EventHandler<EventArgs> DeviceLost;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;
        public event EventHandler<ResourceCreatedEventArgs> ResourceCreated;
        public event EventHandler<ResourceDestroyedEventArgs> ResourceDestroyed;
        public event EventHandler<EventArgs> Disposing;


        public bool IsDisposed
        {
            get
            {
                throw MonoGame.Portable.NotImplementedException;
            }
        }


        public bool IsContentLost
        {
            get
            {
                throw MonoGame.Portable.NotImplementedException;
            }
        }


        /// <summary>
        /// Returns a handle to internal device object. Valid only on DirectX platforms.
        /// For usage, convert this to SharpDX.Direct3D11.Device.
        /// </summary>
        public object Handle { get { return null; } }


        public GraphicsAdapter Adapter { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsDevice" /> class.
        /// </summary>
        /// <param name="graphicsProfile">The graphics profile.</param>
        /// <param name="presentationParameters">The presentation options.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="presentationParameters"/> is <see langword="null"/>.
        /// </exception>
        public GraphicsDevice(GraphicsAdapter adapter, GraphicsProfile graphicsProfile, PresentationParameters presentationParameters)
        {
            throw MonoGame.Portable.NotImplementedException;
        }


        ~GraphicsDevice()
        {
            Dispose(false);
        }


        public RasterizerState RasterizerState { get; set; }


        public Color BlendFactor { get; set; }


        public BlendState BlendState { get; set; }


        public DepthStencilState DepthStencilState { get; set; }


        public void Clear(Color color)
        {
        }


        public void Clear(ClearOptions options, Color color, float depth, int stencil)
        {
        }


        public void Clear(ClearOptions options, Vector4 color, float depth, int stencil)
        {
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
        }


        public void Present()
        {
        }


        public DisplayMode DisplayMode
        {
            get
            {
                return Adapter.CurrentDisplayMode;
            }
        }


        public GraphicsDeviceStatus GraphicsDeviceStatus
        {
            get
            {
                return GraphicsDeviceStatus.Normal;
            }
        }


        public PresentationParameters PresentationParameters
        {
            get;
            private set;
        }


        public Viewport Viewport { get; set; }


        public GraphicsProfile GraphicsProfile { get; set; }


        public Rectangle ScissorRectangle { get; set; }


        public void SetRenderTarget(RenderTarget2D renderTarget)
        {
        }


        public void SetRenderTarget(RenderTargetCube renderTarget, CubeMapFace cubeMapFace)
        {
        }


        public void SetRenderTargets(params RenderTargetBinding[] renderTargets)
        {
        }


        public RenderTargetBinding[] GetRenderTargets()
        {
            throw MonoGame.Portable.NotImplementedException;
        }


        public void SetVertexBuffer(VertexBuffer vertexBuffer)
        {
        }


        public IndexBuffer Indices { get; set; }


        public bool ResourcesLost { get; set; }


        /// <summary>
        /// Draw geometry by indexing into the vertex buffer.
        /// </summary>
        /// <param name="primitiveType">
        /// The type of primitive to render. <see cref="PrimitiveType.PointList"/> is not supported
        /// with this method.
        /// </param>
        /// <param name="baseVertex">Used to offset the vertex range indexed from the vertex buffer.</param>
        /// <param name="minVertexIndex">A hint of the lowest vertex indexed relative to baseVertex.</param>
        /// <param name="numVertices">An hint of the maximum vertex indexed.</param>
        /// <param name="startIndex">The index within the index buffer to start drawing from.</param>
        /// <param name="primitiveCount">The number of primitives to render from the index buffer.</param>
        /// <remarks>Note that minVertexIndex and numVertices are unused in MonoGame and will be ignored.</remarks>
        public void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount)
        {
        }


        public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount) where T : struct, IVertexType
        {
        }


        public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
        {
        }


        public void DrawPrimitives(PrimitiveType primitiveType, int vertexStart, int primitiveCount)
        {
        }


        public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount) where T : struct, IVertexType
        {
        }


        public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
        {
        }


        public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, int[] indexData, int indexOffset, int primitiveCount) where T : struct, IVertexType
        {
        }


        public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, int[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct, IVertexType
        {
        }
    }
}
