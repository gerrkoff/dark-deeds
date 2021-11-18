using System;
using System.Linq;

namespace DarkDeeds.MongoMigrator.Helpers
{
    public class ScaleRenderer
    {
        private int _rendered;
        private int _total;
        private int _scale = 50;

        public void Prepare(int total)
        {
            _rendered = 0;
            _total = total;
            Console.Write("1");
            Console.Write(string.Join("", Enumerable.Repeat(" ", _scale / 2 - 2)));
            Console.Write("50");
            Console.Write(string.Join("", Enumerable.Repeat(" ", _scale / 2 - 4)));
            Console.WriteLine("100");
        }

        public void RenderProgress(int current)
        {
            var toRender = (int) Math.Floor(_scale * 1.0 * current / _total);
            toRender -= _rendered;
            _rendered += toRender;
            Console.Write(string.Join("", Enumerable.Repeat("#", toRender)));
        }
    }
}