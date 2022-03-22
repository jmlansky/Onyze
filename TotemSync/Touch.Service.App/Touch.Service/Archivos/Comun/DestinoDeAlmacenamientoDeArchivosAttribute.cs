using System;

namespace Touch.Service.Archivos.Comun
{
    [AttributeUsage(AttributeTargets.All)]
    public class DestinoDeAlmacenamientoDeArchivosAttribute : Attribute
    {
        public DestinoDeAlmacenamientoDeArchivosAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; }


        //private string v;

        //public DestinoDeAlmacenamientoDeArchivosAttribute(string v)
        //{
        //    this.v = v;
        //}
    }
}