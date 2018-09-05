using System;
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

        public RenderObject(Vertex[] vertices)
        {
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
                BufferTarget.ArrayBuffer,       // target buffer object (can either be ArrayBuffer or ElementArrayBuffer, no idea what the latter does)
                Vertex.Size * verticeCount,     // specify the size of the data
                vertices,                       // our data!
                BufferUsageHint.StaticDraw);    /* hint at OpenGL how the data might be accessed, for optimization purposes
                                                 * StaticDraw = the data will be modified once and used many times
                                                 * DynamicDraw = the data will be modified many times and used many times */

            /* define indexes for our vertex attributes
             * TODO: we should name these attribute locations by using GL.BindAttribLocation(program, index, name)
             * and then fetch them back by using GL.GetAttribLocation(program, name) in order to use them as arguments
             * for these calls to GL.EnableVertexAttribArray */
            int positionIndex = 0;
            int colorIndex = 1;

            /* Enable the vertex attributes */
            GL.EnableVertexAttribArray(positionIndex);
            GL.EnableVertexAttribArray(colorIndex);

            /* describe how to read the raw data we just passed with GL.BufferData
             * each call to GL.VertexAttribPointer describes a separate attribute */
            GL.VertexAttribPointer(
                positionIndex,          // index of the first attribute (same as GL.EnableVertexAttribArray)
                4,                      // size of the attribute (we want 4 because we have a vec4)
                VertexAttribPointerType.Float, // contains floats
                false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                Vertex.Size,            // different values for the same attribute are separated by the size of a vertex
                0);                     // starting position of the data (first data so position 0)

            GL.VertexAttribPointer(
                colorIndex,             // index of the second attribute (same as GL.EnableVertexAttribArray)
                4,                      // size of the attribute (we want 4 because we have a vec4)
                VertexAttribPointerType.Float, // contains floats
                false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                Vertex.Size,            // different values for the same attribute are separated by the size of a vertex
                Vector4.SizeInBytes);   // starting position of the data (comes after a vec4)
            
            initialized = true;
        }

        public void Render()
        {
            /* we specify that we want to take <count> VAO from our (active) vertex array(s),
             * starting at index <first>, and treat it as a <mode>
             * this gives us a primitive of the specified type that will be processed by the pipeline
             * or something like that idk */
            // GL.BindVertexArray(vertexArray);

            /* draw <count> things considered as <type> starting with <first> */
            GL.DrawArrays(PrimitiveType.Triangles, 0, verticeCount);

            /* we prefer using DrawElements because it factorizes reused vertices
             * like if we have a triangle ABC and a triangle BCD, DrawArray will 
             * have to draw six vertices (ABCBCD) when DrawElements will only
             * require the four individual vertices and we feed it an array of
             * indices (123234) which specify which vertices it can reuse */
            /* actually i'm not even sure we do prefer DrawElements, it only saves draw calls for more complex models
             * but in the case of quads, if we have to call DrawElements as many times as we would call DrawArrays
             * there is no difference except the overhead from using indices in DrawElements so DrawArrays is better */
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
