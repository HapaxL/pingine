using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using pingine.Main.Graphics;

namespace pingine.Main.Handlers
{
    public class RenderObject : IDisposable
    {
        private bool initialized;
        private bool disposed;

        private readonly int vertexArray;
        private readonly int vertexBuffer;
        private readonly int verticeCount;

        private readonly int[] textures;

        public RenderObject(int shaderProgram, Sprite[] sprites)
        {
            // spriteCount = sprites.Length;
            var vertices = sprites
                // .SelectMany(s => s.Points.Select(p => p.ToVector()))
                .SelectMany(s => s.Points)
                .ToArray();
            verticeCount = vertices.Length;

            /* generate a vertex array object (VAO) whose ID isn't already in use
             * this VBA will hold our one VBO */
            vertexArray = GL.GenVertexArray();

            /* generate a vertex buffer object (VBO) whose ID isn't already in use
             * this VBO will hold our vertice data */
            vertexBuffer = GL.GenBuffer();

            /* make the VAO active in our OpenGL context */
            GL.BindVertexArray(vertexArray);

            /* make the VBO active in our OpenGL context */
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

            /* populate our buffer with raw data */
            GL.BufferData(
                BufferTarget.ArrayBuffer,               // target buffer object (can either be ArrayBuffer or ElementArrayBuffer, no idea what the latter does)
                (Vector3.SizeInBytes + (sizeof(float) * 4) + Vector2.SizeInBytes) * verticeCount,     // specify the size of the data
                vertices,                               // our data!
                BufferUsageHint.StaticDraw);            /* hint at OpenGL how the data might be accessed, for optimization purposes
                                                         * StaticDraw = the data will be modified once and used many times
                                                         * DynamicDraw = the data will be modified many times and used many times */

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

            /* describe how to read the raw data we just passed with GL.BufferData
             * each call to GL.VertexAttribPointer describes a separate attribute */
            GL.VertexAttribPointer(
                positionIndex,          // index of the first attribute (same as GL.EnableVertexAttribArray)
                3,                      // size of the attribute (we want 4 because we have a vec4)
                VertexAttribPointerType.Float, // contains floats
                false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                Vector3.SizeInBytes + (sizeof(float) * 4) + Vector2.SizeInBytes,            // different values for the same attribute are separated by the size of a vertex
                0);                     // starting position of the data (first data so position 0)

            
            GL.VertexAttribPointer(
                colorIndex,             // index of the second attribute (same as GL.EnableVertexAttribArray)
                4,                      // size of the attribute (we want 4 because we have a vec4)
                VertexAttribPointerType.Float, // contains floats
                false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                Vector3.SizeInBytes + (sizeof(float) * 4) + Vector2.SizeInBytes,            // different values for the same attribute are separated by the size of a vertex
                Vector3.SizeInBytes);   // starting position of the data (comes after a vec4)


            GL.VertexAttribPointer(
                texCoordIndex,
                2,
                VertexAttribPointerType.Float,
                false,
                Vector3.SizeInBytes + (sizeof(float) * 4) + Vector2.SizeInBytes,
                Vector3.SizeInBytes + (sizeof(float) * 4));

            Console.WriteLine("renderobject_beforeloadimage" + GL.GetError());

            var id = 0;
            textures = sprites
                .Select(s => LoadImage(shaderProgram, s.Bitmap, id))
                .ToArray();
            
            Console.WriteLine("load_afterloadimage " + GL.GetError());

            initialized = true;
        }

        /* load an image in the graphics card's memory */
        public int LoadImage(int shaderProgram, System.Drawing.Bitmap bitmap, int id)
        {
            /* generate an ID not already in use for our texture object */
            int texID = GL.GenTexture(); // (one ID per texture)
            
            Console.WriteLine("loadimage_aftergentexture " + GL.GetError());

            /* bind that ID as a 2D texture */
            GL.BindTexture(TextureTarget.Texture2D, texID);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Linear);

            Console.WriteLine("loadimage_afterbindtexture " + GL.GetError());

            /* this is a System.Drawing(.Common) operation: lock our bitmap in
             * system memory so that we can access the data programmatically */
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Console.WriteLine("loadimage_afterlockbits " + GL.GetError());

            /* load our raw byte data into our texture object */
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            Console.WriteLine("loadimage_afterteximage " + GL.GetError());

            /* we don't need to keep the data locked in system memory
             * now that we have loaded it into the GC's memory */
            bitmap.UnlockBits(data);
            
            Console.WriteLine("loadimage_afterunlockbits " + GL.GetError());

            /* this generates "mipmaps" (i love this name) for our texture,
             * those are scaled down versions of the texture that will be used
             * when the texture is being displayed at a lower resolution.
             * since we're using pixel graphics here, we will always display
             * pictures at 1:1 resolution so mipmaps are useless (sadly) */
            // GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }

        public void Render(int program)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textures[0]);

            Console.WriteLine("render_afterbindtexture " + GL.GetError());

            var texUniformLocation = GL.GetUniformLocation(program, "tex");

            Console.WriteLine("render_aftergetlocation " + GL.GetError());

            /* whenever we want the fragment (texture) shader to execute
             * operations on more than one "texture unit" (mashing together
             * two textures on a single sprite for example) we will need
             * to specify texture unit IDs, thanks to uniforms.
             * we probably don't need to do that right now, so the default
             * value of 0 will work fine. the line below gets the location
             * of a uniform by its name, and assigns a value to it. */
            GL.Uniform1(texUniformLocation, 0);

            Console.WriteLine("render_afteruniform " + GL.GetError());


            /* we specify that we want to take <count> VAO from our (active) vertex array(s),
             * starting at index <first>, and treat it as a <mode>
             * this gives us a primitive of the specified type that will be processed by the pipeline
             * or something like that idk */
            // GL.BindVertexArray(vertexArray);

            /* draw <count> vertices considered as groups of <type> starting with <first> */
            GL.DrawArrays(PrimitiveType.Quads, 0, verticeCount);

            Console.WriteLine("render_afterdraw " + GL.GetError());

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

        /* IDisposable stuff */
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RenderObject()
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
                    /* TODO: use variables for the IDs (position, color) */
                    /* TODO: factorize foreach vertex attribute */
                    GL.DisableVertexAttribArray(0);
                    GL.DisableVertexAttribArray(1);

                    GL.DeleteVertexArray(vertexArray);
                    GL.DeleteBuffer(vertexBuffer);
                    initialized = false;
                }
            }

            disposed = true;
        }
    }
}