using System;

namespace Touch.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class EntityRepositoryAttribute : Attribute
    {
        public EntityRepositoryAttribute(string name)
        {
            EntityRepository = name;
        }

        public string EntityRepository { get; }
    }
}