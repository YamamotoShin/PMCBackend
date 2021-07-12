using System.Runtime.Serialization;

namespace PMCBackend.DropBox.Response
{
    public sealed class Errors
    {
        public string error_summary { get; set; }
        public ErrorsError error { get; set; }
        [DataContract]
        public sealed class ErrorsError
        {
            [DataMember(Name = ".tag")]
            public string tag { get; set; }
        }
    }
}
