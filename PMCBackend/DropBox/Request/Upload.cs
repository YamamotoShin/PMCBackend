using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PMCBackend.DropBox.Request
{
    public sealed class Upload
	{
		public string path { get; set; }
		[DataContract]
		public sealed class RequestUploadMode
		{
			[DataMember(Name = ".tag")]
			public string tag { get; set; }
			[DataMember]
			public string update { get; set; }
		}
		public string mode { get; set; }
		public bool autorename { get; set; }
		public bool mute { get; set; }
		public sealed class RequestUploadPropertyGroups
		{
			public string template_id { get; set; }
			public List<RequestUploadPropertyGroupsFields> fields { get; set; }
			public sealed class RequestUploadPropertyGroupsFields
			{
				public string name { get; set; }
				public string value { get; set; }
			}
		}
		public bool strict_conflict { get; set; }
	}
}
