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
    public class RenderHandler : IDisposable
    {
        private bool initialized;
        private bool disposed;

        private int VertexArray { get; set; }
        private int VertexBuffer { get; set; }

        private readonly OrderedMultiset<float, ulong, Entity> toRender;
        private readonly HashSet<Entity> toUpdate;

        public RenderHandler()
        {
            toRender = new OrderedMultiset<float, ulong, Entity>();
            toUpdate = new HashSet<Entity>();
        }

        public void Load()
        {
            /* generate a vertex array object (VAO) whose ID isn't already in use
             * this VBA will hold our one VBO */
            VertexArray = GL.GenVertexArray();

            /* generate a vertex buffer object (VBO) whose ID isn't already in use
             * this VBO will hold our vertice data */
            VertexBuffer = GL.GenBuffer();

            /* make the VAO active in our OpenGL context */
            GL.BindVertexArray(VertexArray);

            /* make the VBO active in our OpenGL context */
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);

            // /* populate our buffer with raw data */
            //GL.BufferData(
            //    BufferTarget.ArrayBuffer,               // target buffer object (can either be ArrayBuffer or ElementArrayBuffer, no idea what the latter does)
            //    (Vector2.SizeInBytes + (sizeof(float) * 4) + Vector2.SizeInBytes) * verticeCount,     // specify the size of the data
            //    vertices,                               // our data!
            //    BufferUsageHint.StaticDraw);            /* hint at OpenGL how the data might be accessed, for optimization purposes
            //                                             * StaticDraw = the data will be modified once and used many times
            //                                             * DynamicDraw = the data will be modified many times and used many times */

            /* define indexes for our vertex attributes
             * TODO: we should name these attribute locations by using GL.BindAttribLocation(program, index, name)
             * and then fetch them back by using GL.GetAttribLocation(program, name) in order to use them as arguments
             * for these calls to GL.EnableVertexAttribArray */
            int positionIndex = 0;
            int colorIndex = 1;
            int texCoordIndex = 2;

            /* Enable the vertex attributes */
            GL.EnableVertexAttribArray(positionIndex);
            GL.EnableVertexAttribArray(colorIndex);
            GL.EnableVertexAttribArray(texCoordIndex);

            var dataSize = Vector2.SizeInBytes + (sizeof(float) * 4) + (sizeof(int) * 2);

            /* describe how to read the raw data we just passed with GL.BufferData
             * each call to GL.VertexAttribPointer describes a separate attribute */
            GL.VertexAttribPointer(
                positionIndex,          // index of the first attribute (same as GL.EnableVertexAttribArray)
                2,                      // size of the attribute (we want 2 because we have a vec2)
                VertexAttribPointerType.Float, // contains floats
                false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                dataSize,               // different values for the same attribute are separated by the size of a vertex
                0);                     // starting position of the data (first data so position 0)

            
            GL.VertexAttribPointer(
                colorIndex,             // index of the second attribute (same as GL.EnableVertexAttribArray)
                4,                      // size of the attribute (we want 4 because we have a vec4)
                VertexAttribPointerType.Float, // contains floats
                false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                dataSize,               // different values for the same attribute are separated by the size of a vertex
                Vector2.SizeInBytes);   // starting position of the data (comes after a vec2)


            GL.VertexAttribIPointer(
                texCoordIndex,
                2,
                VertexAttribIntegerType.Int,
                // false,                  // normalizing transforms integer values in the 0-255 range into float values in the 0-1 range. set to false cuz we want ints
                dataSize,
                new IntPtr(Vector2.SizeInBytes + (sizeof(float) * 4)));

            Game.LogHandler.LogGLError("renderobject_beforeloadimage", GL.GetError());

            initialized = true;
        }

        public void AddEntity(Entity obj)
        {
            toRender.Add(obj.Depth, obj.Id, obj);
            // Game.LogHandler.LogGLError("load_afterrenderobjectload", GL.GetError());
        }

        public void RemoveEntity(Entity obj)
        {
            toRender.Remove(obj.Depth, obj.Id);
            // Game.LogHandler.LogGLError("load_afterrenderobjectunload", GL.GetError());
        }

        public void MustUpdateRender(Entity obj)
        {
            toUpdate.Add(obj);
        }

        public void Render(ProgramId program)
        {
            // var test = new OrderedMultiset<int, int, RenderObject>();
            // spriteCount = sprites.Length;

            foreach (var obj in toUpdate)
            {
                toRender.Remove(obj.OldDepth, obj.Id);
                toRender.Add(obj.Depth, obj.Id, obj);
                obj.UpdateDepth();
            }
            toUpdate.Clear();

            var vertices = toRender.Items
                // .SelectMany(s => s.Points.Select(p => p.ToVector()))
                .SelectMany(s => s.GetRenderData())
                .ToArray();

            /* populate our buffer with raw data */
            GL.BufferData(
                BufferTarget.ArrayBuffer,               // target buffer object (can either be ArrayBuffer or ElementArrayBuffer, no idea what the latter does)
                (Vector2.SizeInBytes + (sizeof(float) * 4) + Vector2.SizeInBytes) * vertices.Length,     // specify the size of the data
                vertices,                               // our data!
                BufferUsageHint.StaticDraw);            /* hint at OpenGL how the data might be accessed, for optimization purposes
                                                         * StaticDraw = the data will be modified once and used many times
                                                         * DynamicDraw = the data will be modified many times and used many times */
                                                        /* this does not actually change a lot, so whatever */

            /* get location of variable in the shader that we will bind to
             * a specific value for the duration of the shader's use */
            var texUniformLocation = GL.GetUniformLocation(program, "tex");

            Game.LogHandler.LogGLError("render_aftergetlocation", GL.GetError());

            var verticesDrawn = 0;

            foreach (var o in toRender.Items)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, o.TexId.Value);

                Game.LogHandler.LogGLError("render_afterbindtexture", GL.GetError());
                
                /* whenever we want the fragment (texture) shader to execute
                 * operations on more than one "texture unit" (mashing together
                 * two textures on a single sprite for example) we will need
                 * to specify texture unit IDs, thanks to uniforms.
                 * the default value of 0 works fine since we're always using
                 * 0 as our active texture. the line below gets the location
                 * of a uniform by its name, and assigns a value to it. */
                GL.Uniform1(texUniformLocation, 0);

                Game.LogHandler.LogGLError("render_afteruniform", GL.GetError());

                /* we specify that we want to take <count> VAO from our (active) vertex array(s),
                 * starting at index <first>, and treat it as a <mode>
                 * this gives us a primitive of the specified type that will be processed by the pipeline
                 * or something like that idk */
                GL.BindVertexArray(VertexArray);
                /* draw <count> vertices considered as groups of <mode> starting with <first> */
                GL.DrawArrays(PrimitiveType.Quads, verticesDrawn, 4);
                verticesDrawn += 4;

                Game.LogHandler.LogGLError("render_afterdraw", GL.GetError());

                GL.BindVertexArray(0);

                /* for complex objects made of a great number of connected polygons,
                 * we prefer using DrawElements because it factorizes reused vertices
                 * like if we have a triangle ABC and a triangle BCD, DrawArray will 
                 * have to draw six vertices (ABCBCD) when DrawElements will only
                 * require the four individual vertices and we feed it an array of
                 * indices (123234) which specify which vertices it can reuse */
                /* however, DrawElements only saves draw calls for those complex
                 * models, whereas we only draw simple individual quads
                 * so since we would have to call DrawElements as many times as we
                 * call DrawArrays, there is no difference except the overhead from
                 * using indices in DrawElements so DrawArrays is better */
                // GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, new int[] { 0, 1, 2, 1, 2, 3 });
            }
        }

        /* load an image in the graphics card's memory */
        public TexId LoadBitmap(System.Drawing.Bitmap bitmap)
        {
            /* modifies subsequent calls to use texture 0 as the current texture */
            GL.ActiveTexture(TextureUnit.Texture0);

            /* generate an ID not already in use for our texture object */
            TexId texId = GL.GenTexture(); // (one ID per texture)

            Game.LogHandler.LogGLError("loadimage_aftergentexture", GL.GetError());

            /* bind that ID as a 2D texture */
            GL.BindTexture(TextureTarget.Texture2D, texId);

            Game.LogHandler.LogGLError("loadimage_afterbindtexture", GL.GetError());

            /* TODO explain this line
             * (this line is mandatory for displaying textures)
             * also needs to be called once per texture, after you bind it */
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            /* this is a System.Drawing(.Common) operation: lock our bitmap in
             * system memory so that we can access the data programmatically */
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Game.LogHandler.LogGLError("loadimage_afterlockbits", GL.GetError());

            /* load our raw byte data into our texture object */
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            Game.LogHandler.LogGLError("loadimage_afterteximage", GL.GetError());

            /* we don't need to keep the data locked in system memory
             * now that we have loaded it into the GC's memory */
            bitmap.UnlockBits(data);

            Game.LogHandler.LogGLError("loadimage_afterunlockbits", GL.GetError());

            /* this generates "mipmaps" (i love this name) for our texture,
             * those are scaled down versions of the texture that will be used
             * when the texture is being displayed at a lower resolution.
             * since we're using pixel graphics here, we will always display
             * pictures at 1:1 resolution so mipmaps are useless (sadly) */
            // GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texId;
        }
        
        public void UnloadBitmap(TexId texId)
        {
            /* unload the texture after we don't need it anymore */
            GL.DeleteTexture(texId);
        }


        /* IDisposable stuff */
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RenderHandler()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                if (initialized)
                {
                    /* TODO: use variables for the IDs (position, color, texcoords) */
                    /* TODO: factorize foreach vertex attribute */
                    GL.DisableVertexAttribArray(0);
                    GL.DisableVertexAttribArray(1);
                    GL.DisableVertexAttribArray(2);

                    /* delete our VAO(s) and our VBO(s) */
                    GL.DeleteVertexArray(VertexArray);
                    GL.DeleteBuffer(VertexBuffer);
                    initialized = false;
                }
            }

            disposed = true;
        }
    }
}