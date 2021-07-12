using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PMCBackend.DropBox.Response
{
    public sealed class FileRequest
	{
		public List<FileRequestFileRequests> file_requests { get; set; }
		public sealed class FileRequestFileRequests
		{
			public string id { get; set; }
			public string url { get; set; }
			public string title { get; set; }
			public DateTime created { get; set; }
			public bool is_open { get; set; }
			public int file_count { get; set; }
			public string destination { get; set; }
			public FileRequestFileRequestsDeadline deadline { get; set; }
			public class FileRequestFileRequestsDeadline
			{
				public DateTime deadline { get; set; }
				public FileRequestFileRequestsDeadlineAllowLateUploads allow_late_uploads { get; set; }
				[DataContract]
				public class FileRequestFileRequestsDeadlineAllowLateUploads
				{
					[DataMember(Name = ".tag")]
					public string tag { get; set; }
				}
			}
			public string description { get; set; }
		}
		public string cursor { get; set; }
		public bool has_more { get; set; }
	}
}
