using System;

namespace MessageEncoder.Contracts
{
    public interface IApplication
    {
        string Name { get; }

        void Launch(string[] args);
    }
}
