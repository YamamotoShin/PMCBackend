using System.Runtime.Serialization;

namespace DropBox.Response
{
    public sealed class CurrentAccount
	{
		public string account_id { get; set; }
		public CurrentAccountName name { get; set; }
		public sealed class CurrentAccountName
		{
			public string given_name { get; set; }
			public string surname { get; set; }
			public string familiar_name { get; set; }
			public string display_name { get; set; }
			public string abbreviated_name { get; set; }
		}
		public string email { get; set; }
		public bool email_verified { get; set; }
		public bool disabled { get; set; }
		public string country { get; set; }
		public string locale { get; set; }
		public string referral_link { get; set; }
		public bool is_paired { get; set; }
		public CurrentAccountIsPaired account_type { get; set; }
		[DataContract]
		public sealed class CurrentAccountIsPaired
		{
			[DataMember(Name = ".tag")]
			public string tag { get; set; }
		}
		public CurrentAccountRootInfo root_info { get; set; }
		[DataContract]
		public sealed class CurrentAccountRootInfo
		{
			[DataMember(Name = ".tag")]
			public string tag { get; set; }
			[DataMember]
			public string root_namespace_id { get; set; }
			[DataMember]
			public string home_namespace_id { get; set; }
		}
	}
}
