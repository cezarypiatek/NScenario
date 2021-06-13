using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NScenario
{
    internal class NestingCounter
    {
        private readonly ConcurrentDictionary<string, Stack<(bool, int)>> _levelNumber = new ConcurrentDictionary<string, Stack<(bool, int)>>();
        public Level StartLevel(string levelKey)
        {
            _ = _levelNumber.AddOrUpdate(levelKey, s =>
            {
                var stack = new Stack<(bool, int)>();
                stack.Push((true, 0));
                return stack;
            }, (s, stack) =>
            {
                var (used, counter) = stack.Pop();
                stack.Push((true, counter));
                if (used)
                {
                    stack.Push((true, 0));
                }

                return stack;
            });
            var levelNumber = _levelNumber[levelKey].Count;
            var currentLevelData = IncrementCurrentStepNumber(levelKey);
            return new Level(() => EndLevel(levelKey, levelNumber), currentLevelData);
        }

        private void EndLevel(string scenario, int height)
        {
            if (_levelNumber.TryGetValue(scenario, out var stepStack))
            {
                if (height < stepStack.Count)
                {
                    stepStack.Pop();
                }

                var (_, counter) = stepStack.Pop();
                stepStack.Push((false, counter));
            }
        }

        private Stack<(bool, int)> IncrementCurrentStepNumber(string scenario)
        {
            var stepStack = _levelNumber[scenario];
            var (used, counter) = stepStack.Pop();
            counter++;
            stepStack.Push((used, counter));
            return stepStack;
        }
    }

    public class Level:IDisposable
    {
        public int Nesting { get; }
        private readonly Action _action;

        public Level(Action action, Stack<(bool, int)> levelData)
        {
            Nesting = levelData.Count -1;
            LevelPath = levelData.ToArray().Reverse().Select(x => x.Item2);
            _action = action;
        }

        public IEnumerable<int> LevelPath { get; }

        public void Dispose() => _action.Invoke();
    }
}