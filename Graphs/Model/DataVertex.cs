using System;
using GraphX.PCL.Common.Models;

namespace Graphs.Model
{
    public class DataVertex : VertexBase, ICloneable
    {
        public string Text { get; set; }

        public string Color { get; set; }

        /// <summary>
        /// Степень
        /// </summary>
        public int E { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Color))
                return Text + "(" + Color + ")";
            return Text;
        }

        public object Clone()
        {
            return new DataVertex
            {
                Angle = Angle,
                Color = Color,
                E = E,
                GroupId = GroupId,
                ID = ID,
                SkipProcessing = SkipProcessing,
                Text = Text
            };
        }

        /// <summary>
        /// Default constructor for this class
        /// (required for serialization).
        /// </summary>
        public DataVertex() : this(string.Empty)
        {
        }

        public DataVertex(string text = "")
        {
            Text = string.IsNullOrEmpty(text) ? "New Vertex" : text;
        }


    }
}
