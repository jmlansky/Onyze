using System;

namespace Framework.Attributes
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