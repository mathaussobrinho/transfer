using Newtonsoft.Json;

namespace TransferFilesApp.Models
{
    public class SftpConfig
    {
        [JsonProperty("sftp_host")]
        public string SftpHost { get; set; } = string.Empty;

        [JsonProperty("sftp_port")]
        public int SftpPort { get; set; } = 22;

        [JsonProperty("sftp_username")]
        public string SftpUsername { get; set; } = string.Empty;

        [JsonProperty("sftp_password")]
        public string SftpPassword { get; set; } = string.Empty;

        [JsonProperty("sftp_remote_path")]
        public string SftpRemotePath { get; set; } = "/";
    }
}

