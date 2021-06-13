using System;

namespace NScenario
{
    internal class DecoratorBuilder<T>
    {
        private T _underlying;

        public void WrapWith(Func<T, T> func) => _underlying = func(_underlying);

        public T Build() => _underlying;
    }
}