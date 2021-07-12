using System;
using System.Collections.Generic;

namespace PMCBackend.DropBox.Response
{
    public sealed class Upload
    {
        public string name { get; set; }
        public string id { get; set; }
        public DateTime client_modified { get; set; }
        public DateTime server_modified { get; set; }
        public string rev { get; set; }
        public int size { get; set; }
        public string path_lower { get; set; }
        public string path_display { get; set; }
        public UploadSharingInfo sharing_info { get; set; }
        public sealed class UploadSharingInfo
        {
            public bool read_only { get; set; }
            public string parent_shared_folder_id { get; set; }
            public string modified_by { get; set; }
        }
        public bool is_downloadable { get; set; }
        public UploadPropertyGroups property_groups { get; set; }
        public sealed class UploadPropertyGroups
        {
            public string template_id { get; set; }
            public List<UploadPropertyGroupsFields> fields { get; set; }
            public sealed class UploadPropertyGroupsFields
            {
                public string name { get; set; }
                public string value { get; set; }
            }
        }
        public bool has_explicit_shared_members { get; set; }
        public string content_hash { get; set; }
        public UploadFileLockInfo file_lock_info { get; set; }
        public sealed class UploadFileLockInfo
        {
            public bool is_lockholder { get; set; }
            public string lockholder_name { get; set; }
            public DateTime created { get; set; }
        }
    }
}
