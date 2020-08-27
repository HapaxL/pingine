using System;
using System.Linq;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using HapaxTools;

using TexId = System.Int32;
using ProgramId = System.Int32;
using System.Collections.Generic;
using pingine.Game.State;

namespace pingine.Game.Handlers
{
    public class RenderHandlerSmooth : RenderHandler
    {
        public override void SetTexParameters()
        {
            /* texture parameters, specify how to display them, must be called once per texture, after binding
            * for smooth, interpolated movements: set texture wrap to ClampToEdge (no repeat), with linear interpolation, floating position values */

            /* specifies how to display textures by default (interpolation), or textures that are minified:
             * nearest or linear, either with mipmaps or not (we do not want mipmaps i believe)
             * (this line is mandatory for displaying textures) */
            /* for smooth movement we need Linear */
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            /* this one is for magnified textures. either linear or nearest? probably depends on how we decide to implement zooms (TODO) */
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);

            /* texture wrap.
             * set to Repeat by default, but with linear interpolation it leads to weird lines on the borders of moving textures
             * so if we need repeating textures we need to be careful to only apply Repeat to those, and ClampToEdge to the others */
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
        }
    }
}