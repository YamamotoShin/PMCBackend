using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DropBox.Request
{
    public sealed class Upload
	{
		public string path { get; set; }
		[DataContract]
		public sealed class UploadMode
		{
			[DataMember(Name = ".tag")]
			public string tag { get; set; }
			[DataMember]
			public string update { get; set; }
		}
		public string mode { get; set; }
		public bool autorename { get; set; }
		public bool mute { get; set; }
		public sealed class UploadPropertyGroup
		{
			public string template_id { get; set; }
			public List<UploadPropertyGroupField> fields { get; set; }
			public sealed class UploadPropertyGroupField
			{
				public string name { get; set; }
				public string value { get; set; }
			}
		}
		public bool strict_conflict { get; set; }
	}
}
