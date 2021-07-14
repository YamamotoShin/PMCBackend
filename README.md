# PMCBackend

# Requirement

* .NET Framework 4.8

# Usage

```C#
var dropBoxClient = new DropBoxClient(Properties.Settings.Default.AccessToken);

var localFilePath = @"C:\hoge.txt";
var dropBoxFilePath = @"/hoge.txt";

var listFolder = await dropBoxClient.ListFolder();

var uploadResult = await dropBoxClient.Upload(localFilePath, dropBoxFilePath);

var downloadResult = await dropBoxClient.Download(dropBoxFilePath, localFilePath);

var deleteResult = await dropBoxClient.DeleteV2(dropBoxFilePath);
```
