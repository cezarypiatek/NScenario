using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NScenario
{
    internal class NestingCounter
    {
        class LevelCounter
        {
            public int Counter { get; set; }
        }

        private readonly Stack<LevelCounter> _stack = new Stack<LevelCounter>();

        public NestingCounter()
        {
            _stack.Push(new LevelCounter());
        }

        public Level StartLevel()
        {
            _stack.Peek().Counter++;
            _stack.Push(new LevelCounter());
            var data = _stack.ToArray().Skip(1).Reverse().Select(x => x.Counter).ToArray();
            return new Level(() => _stack.Pop(), data);
        }
     
    }

    public class Level : IDisposable
    {
        public int Nesting { get; }
        private readonly Action _action;

        public Level(Action action, int[] levelData)
        {
            Nesting = levelData.Length - 1;
            LevelPath = levelData;
            _action = action;
        }

        public IReadOnlyList<int> LevelPath { get; }

        public void Dispose() => _action.Invoke();
    }
}