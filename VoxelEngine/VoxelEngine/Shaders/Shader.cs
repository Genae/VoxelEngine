using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace VoxelEngine.Shaders
{
    public class Shader : IDisposable
    {
        public enum Type
        {
            Vertex = 0x1,
            Fragment = 0x2
        }
        
        public static bool IsSupported => (new Version(GL.GetString(StringName.Version).Substring(0, 3)) >= new Version(2, 0));

        public int _program;
        private readonly Dictionary<string, int> _variables = new Dictionary<string, int>();

        public Shader(string source, Type type)
        {
            if (!IsSupported)
            {
                Console.WriteLine("Failed to create Shader." + Environment.NewLine + "Your system doesn't support Shader.", "Error");
                return;
            }

            if (type == Type.Vertex)
                Compile(source, "");
            else
                Compile("", source);
        }

        public Shader(string vsource, string fsource)
        {
            if (!IsSupported)
            {
                Console.WriteLine("Failed to create Shader." + Environment.NewLine + "Your system doesn't support Shader.", "Error");
                return;
            }

            Compile(vsource, fsource);
        }

        // I prefer to return the bool rather than throwing an exception lol
        private void Compile(string vertexSource, string fragmentSource)
        {
            int statusCode;
            string info;

            if (vertexSource == "" && fragmentSource == "")
            {
                Console.WriteLine("Failed to compile Shader." + Environment.NewLine + "Nothing to Compile.", "Error");
                return;
            }

            if (_program > 0)
                 GL.DeleteProgram(_program);

            _variables.Clear();

            _program = GL.CreateProgram();

            if (vertexSource != "")
            {
                var vertexShader = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(vertexShader, vertexSource);
                GL.CompileShader(vertexShader);
                GL.GetShaderInfoLog(vertexShader, out info);
                GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out statusCode);

                if (statusCode != 1)
                {
                    Console.WriteLine("Failed to Compile Vertex Shader Source." + Environment.NewLine + info + Environment.NewLine + "Status Code: " + statusCode);

                     GL.DeleteShader(vertexShader);
                     GL.DeleteProgram(_program);
                    _program = 0;

                    return;
                }

                 GL.AttachShader(_program, vertexShader);
                 GL.DeleteShader(vertexShader);
            }

            if (fragmentSource != "")
            {
                int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(fragmentShader, fragmentSource);
                GL.CompileShader(fragmentShader);
                GL.GetShaderInfoLog(fragmentShader, out info);
                GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out statusCode);

                if (statusCode != 1)
                {
                    Console.WriteLine("Failed to Compile Fragment Shader Source." + Environment.NewLine + info + Environment.NewLine + "Status Code: " + statusCode);

                     GL.DeleteShader(fragmentShader);
                     GL.DeleteProgram(_program);
                    _program = 0;

                    return;
                }

                 GL.AttachShader(_program, fragmentShader);
                 GL.DeleteShader(fragmentShader);
            }

            //add shader attributes here
            

            GL.LinkProgram(_program);
            GL.GetProgramInfoLog(_program, out info);
            GL.GetProgram(_program, GetProgramParameterName.LinkStatus, out statusCode);

            if (statusCode != 1)
            {
                Console.WriteLine("Failed to Link Shader Program." + Environment.NewLine + info + Environment.NewLine + "Status Code: " + statusCode);

                 GL.DeleteProgram(_program);
                _program = 0;
            }
        }

        private int GetVariableLocation(string name)
        {
            if (_variables.ContainsKey(name))
                return _variables[name];

            int location = GL.GetUniformLocation(_program, name);

            if (location != -1)
                _variables.Add(name, location);
            else
                Console.WriteLine("Failed to retrieve Variable Location." +
                    Environment.NewLine + "Variable Name not found.", "Error");

            return location;
        }

        public void SetVariable(string name, float x)
        {
            if (_program > 0)
            {
                 GL.UseProgram(_program);

                int location = GetVariableLocation(name);
                if (location != -1)
                     GL.Uniform1(location, x);

                 GL.UseProgram(0);
            }
        }

        public void SetVariable(string name, float x, float y)
        {
            if (_program > 0)
            {
                 GL.UseProgram(_program);

                int location = GetVariableLocation(name);
                if (location != -1)
                     GL.Uniform2(location, x, y);

                 GL.UseProgram(0);
            }
        }

        public void SetVariable(string name, float x, float y, float z)
        {
            if (_program > 0)
            {
                 GL.UseProgram(_program);

                int location = GetVariableLocation(name);
                if (location != -1)
                     GL.Uniform3(location, x, y, z);

                 GL.UseProgram(0);
            }
        }

        public void SetVariable(string name, float x, float y, float z, float w)
        {
            if (_program > 0)
            {
                 GL.UseProgram(_program);

                int location = GetVariableLocation(name);
                if (location != -1)
                     GL.Uniform4(location, x, y, z, w);

                 GL.UseProgram(0);
            }
        }

        public void SetVariable(string name, Matrix4 matrix)
        {
            if (_program > 0)
            {
                 GL.UseProgram(_program);

                int location = GetVariableLocation(name);
                if (location != -1)
                {
                    GL.UniformMatrix4(location, false, ref matrix);
                }

                 GL.UseProgram(0);
            }
        }

        public void SetVariable(string name, Vector2 vector)
        {
            SetVariable(name, vector.X, vector.Y);
        }

        public void SetVariable(string name, Vector3 vector)
        {
            SetVariable(name, vector.X, vector.Y, vector.Z);
        }

        public void SetVariable(string name, Color color)
        {
            SetVariable(name, color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        public static void Bind(Shader shader)
        {
            if (shader != null && shader._program > 0)
            {
                 GL.UseProgram(shader._program);
            }
            else
            {
                 GL.UseProgram(0);
            }
        }

        public void Dispose()
        {
            if (_program != 0)
                 GL.DeleteProgram(_program);
        }
    }
}