using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PMCBackend.DropBox.Response
{
    public sealed class DeleteV2
    {
        public DeleteV2Metadata metadata { get; set; }
        [DataContract]
        public sealed class DeleteV2Metadata
        {
            [DataMember(Name = ".tag")]
            public string tag { get; set; }
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public string id { get; set; }
            [DataMember]
            public string client_modified { get; set; }
            [DataMember]
            public string server_modified { get; set; }
            [DataMember]
            public string rev { get; set; }
            [DataMember]
            public int size { get; set; }
            [DataMember]
            public string path_lower { get; set; }
            [DataMember]
            public string path_display { get; set; }
            [DataMember]
            public DeleteV2MetadataSharingInfo sharing_info { get; set; }
            public sealed class DeleteV2MetadataSharingInfo
            {
                public bool read_only { get; set; }
                public string parent_shared_folder_id { get; set; }
                public string modified_by { get; set; }
            }
            [DataMember]
            public bool is_downloadable { get; set; }
            [DataMember]
            public IList<DeleteV2MetadataPropertyGroup> property_groups { get; set; }
            public sealed class DeleteV2MetadataPropertyGroup
            {
                public string template_id { get; set; }
                public IList<DeleteV2MetadataPropertyGroupField> fields { get; set; }
                public sealed class DeleteV2MetadataPropertyGroupField
                {
                    public string name { get; set; }
                    public string value { get; set; }
                }
            }
            [DataMember]
            public bool has_explicit_shared_members { get; set; }
            [DataMember]
            public string content_hash { get; set; }
            [DataMember]
            public DeleteV2MetadataFileLockInfo file_lock_info { get; set; }
            public sealed class DeleteV2MetadataFileLockInfo
            {
                public bool is_lockholder { get; set; }
                public string lockholder_name { get; set; }
                public string created { get; set; }
            }
        }
    }
}
