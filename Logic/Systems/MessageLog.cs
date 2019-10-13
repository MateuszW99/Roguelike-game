using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;

namespace Game.Logic
{
    class MessageLog
    {
        // Maximum number of lines to be stored at a time
        private static readonly int _maxLines = 9;
        private readonly Queue<string> lines;
        private readonly Queue<RLColor> colors;

        public MessageLog()
        {
            lines = new Queue<string>();
            colors = new Queue<RLColor>();
        }

        public void Add(string message, RLColor color)
        {
            lines.Enqueue(message);
            colors.Enqueue(color);
            // When _lines.Count exceeds _maxLines, remove the oldest message
            if (lines.Count > _maxLines)
            {
                lines.Dequeue();
                colors.Dequeue();
            }
        }

        public void Draw(RLConsole console)
        {
            string[] lines = this.lines.ToArray();
            RLColor[] colors = this.colors.ToArray();
            for(int i = 0; i < lines.Length; i++)
            {
                console.Print(1, 1 + i, lines[i], colors[i]);
            }
        }

    }
}
