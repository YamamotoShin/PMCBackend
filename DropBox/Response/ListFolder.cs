using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DropBox.Response
{
    public sealed class ListFolder
    {
        public IList<ListFolderEntry> entries { get; set; }
        [DataContract]
        public sealed class ListFolderEntry
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
            public ListFolderEntrySharingInfo sharing_info { get; set; }
            public sealed class ListFolderEntrySharingInfo
            {
                public bool read_only { get; set; }
                public string parent_shared_folder_id { get; set; }
                public string modified_by { get; set; }
            }
            [DataMember]
            public bool is_downloadable { get; set; }

            [DataMember]
            public IList<ListFolderEntryPropertyGroup> property_groups { get; set; }
            public sealed class ListFolderEntryPropertyGroup
            {
                public string template_id { get; set; }
                public IList<ListFolderEntryPropertyGroupField> fields { get; set; }
                public sealed class ListFolderEntryPropertyGroupField
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
            public ListFolderEntryFileLockInfo file_lock_info { get; set; }
            public sealed class ListFolderEntryFileLockInfo
            {
                public bool is_lockholder { get; set; }
                public string lockholder_name { get; set; }
                public string created { get; set; }
            }
        }
        public string cursor { get; set; }
        public bool has_more { get; set; }
    }
}
