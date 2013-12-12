namespace WebApplication1.Lib.Domain.Java.JavaModels
{
    class JavaClass
    {
        public string ClassVisibility { get; set; }
        public string ClassName { get; set; }
        public JavaField[] Fields { get; set; }
        public JavaMethod[] Methods { get; set; }

        //TODO: Need to implement ToString() in order to generate text for a class diagram.
    }
}
