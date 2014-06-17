// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

using SharpDX;
using SharpDX.Direct3D11;

namespace Microsoft.Xna.Framework.Graphics
{
	public partial class TextureCube
	{
        private bool _renderTarget;
        private bool _mipMap;

        private void PlatformConstruct(GraphicsDevice graphicsDevice, int size, bool mipMap, SurfaceFormat format, bool renderTarget)
        {
            _renderTarget = renderTarget;
            _mipMap = mipMap;

            // Create texture
            GetTexture();
        }

        internal override SharpDX.Direct3D11.Resource CreateTexture()
        {
            var description = new Texture2DDescription
            {
                Width = size,
                Height = size,
                MipLevels = _levelCount,
                ArraySize = 6, // A texture cube is a 2D texture array with 6 textures.
                Format = SharpDXHelper.ToFormat(_format),
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.TextureCube
            };

            if (_renderTarget)
            {
                description.BindFlags |= BindFlags.RenderTarget;
                if (_mipMap)
                    description.OptionFlags |= ResourceOptionFlags.GenerateMipMaps;
            }

            return new SharpDX.Direct3D11.Texture2D(GraphicsDevice._d3dDevice, description);
        }

        private void PlatformGetData<T>(CubeMapFace cubeMapFace, T[] data) where T : struct
        {
            // IMPORTANT: On some tablets (e.g. Microsoft Surface RT) and some phones 
            // (e.g. Samsungs ATIV S) the method fails if the cubemap contains mipmaps.
            // Only the first cubemap face returns valid data - the other faces return
            // garbage. (This problem also appears, for example, when GetData<T>() is
            // used in a WP7 XNA game on Samsung ATIV S, so it seems to be a general 
            // limitations of these devices and not caused by the MonoGame implementation.)
            // 
            // --> Avoid mipmaps if GetData<T>() is used.

            // Create a temp staging resource for copying the data.
            var desc = new Texture2DDescription();
            desc.Width = size;
            desc.Height = size;
            desc.MipLevels = 1;
            desc.ArraySize = 1;
            desc.Format = SharpDXHelper.ToFormat(_format);
            desc.BindFlags = BindFlags.None;
            desc.CpuAccessFlags = CpuAccessFlags.Read;
            desc.SampleDescription.Count = 1;
            desc.SampleDescription.Quality = 0;
            desc.Usage = ResourceUsage.Staging;
            desc.OptionFlags = ResourceOptionFlags.None;

            var d3dContext = GraphicsDevice._d3dContext;
            using (var stagingTexture = new SharpDX.Direct3D11.Texture2D(GraphicsDevice._d3dDevice, desc))
            {
                lock (d3dContext)
                {
                    // Copy the data from the GPU to the staging texture.
                    int subresourceIndex = (int)cubeMapFace * _levelCount;
                    d3dContext.CopySubresourceRegion(_texture, subresourceIndex, null, stagingTexture, 0);

                    // Copy the data to the array.
                    DataStream stream;
                    var box = d3dContext.MapSubresource(stagingTexture, 0, MapMode.Read, MapFlags.None, out stream);
                    if (box.RowPitch == GetPitch(size))
                    {
                        stream.ReadRange(data, 0, data.Length);
                    }
                    else
                    {
                        int offset = 0;
                        IntPtr ptr = box.DataPointer;
                        for (int i = 0; i < size; i++)
                        {
                            SharpDX.Utilities.Read(ptr, data, offset, size);
                            ptr += box.RowPitch;
                            offset += size;
                        }
                    }
                    stream.Dispose();
                }
            }
        }

        private void PlatformSetData<T>(CubeMapFace face, int level, IntPtr dataPtr, int xOffset, int yOffset, int width, int height)
        {
                var box = new DataBox(dataPtr, GetPitch(width), 0);

            int subresourceIndex = (int)face * _levelCount + level;

                var region = new ResourceRegion
                {
                    Top = yOffset,
                    Front = 0,
                    Back = 1,
                    Bottom = yOffset + height,
                    Left = xOffset,
                    Right = xOffset + width
                };

            var d3dContext = GraphicsDevice._d3dContext;
            lock (d3dContext)
                d3dContext.UpdateSubresource(box, GetTexture(), subresourceIndex, region);
        }
	}
}

