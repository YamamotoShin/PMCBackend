namespace PMCBackend.DropBox.Request
{
    public sealed class ListFolder
    {
        public string path { get; set; }
        public bool recursive { get; set; } = false;
        public bool include_media_info { get; set; } = false;
        public bool include_deleted { get; set; } = false;
        public bool include_has_explicit_shared_members { get; set; } = false;
        public bool include_mounted_folders { get; set; } = true;
        public bool include_non_downloadable_files { get; set; } = true;
    }
}
