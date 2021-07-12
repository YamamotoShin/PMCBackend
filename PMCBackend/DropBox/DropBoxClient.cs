using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace PMCBackend.DropBox
{
	public class DropBoxClient
	{
		private static class MediaType
		{
			public static readonly string Json = "application/json";
			public static readonly string Stream = "application/octet-stream";
		}

		private readonly HttpClient m_HttpClient;
		private readonly string AppKey;
		private readonly string AppSecret;
		private string AccessToken { get; set; } = string.Empty;

		public DropBoxClient(string appKey, string appSecret)
		{
			AppKey = appKey;
			AppSecret = appSecret;
			m_HttpClient = new HttpClient();
		}

		public DropBoxClient SetAccessToken(string accessToken)
		{
			AccessToken = accessToken;
			return this;
		}

		private async Task<string> GetResponseContent(HttpResponseMessage response)
		{
			var statusCode = (int)response.StatusCode;
			var responseContent = await response.Content.ReadAsStringAsync();
			switch (statusCode)
			{
				case (int)System.Net.HttpStatusCode.OK:// 200
					return responseContent;
				case (int)System.Net.HttpStatusCode.BadRequest:// 400
					throw new Exception(new StringBuilder()
					.AppendLine("Bad input parameter.")
					.AppendLine("The response body is a plaintext message with more information.")
					.AppendLine(responseContent)
					.ToString());
				case (int)System.Net.HttpStatusCode.Unauthorized: // 401
					throw new Exception(new StringBuilder()
					.AppendLine("Bad or expired token.")
					.AppendLine("This can happen if the access token is expired or if the access token has been revoked by Dropbox or the user.")
					.AppendLine("To fix this, you should re-authenticate the user.")
					.AppendLine("The Content-Type of the response is JSON of typeAuthError")
					.ToString());
				case (int)System.Net.HttpStatusCode.Forbidden: // 403
					throw new Exception(new StringBuilder()
					.AppendLine("The user or team account doesn't have access to the endpoint or feature.")
					.AppendLine("The Content-Type of the response is JSON of typeAccessError")
					.ToString());
				case (int)System.Net.HttpStatusCode.Conflict: // 409

					throw new Exception(new StringBuilder()
					.AppendLine("Endpoint-specific error.")
					.AppendLine("Look to the JSON response body for the specifics of the error.")
					.AppendLine(responseContent)
					.ToString());
				case 429:
					throw new Exception(new StringBuilder()
					.AppendLine("Your app is making too many requests for the given user or team and is being rate limited.")
					.AppendLine("Your app should wait for the number of seconds specified in the \"Retry-After\" response header before trying again.")
					.AppendLine("The Content-Type of the response can be JSON or plaintext.")
					.AppendLine("If it is JSON, it will be typeRateLimitErrorYou can find more information in the data ingress guide.")
					.ToString());
			}
			if (statusCode >= 500 && statusCode <= 599)
			{
				throw new Exception(new StringBuilder()
				.AppendLine("An error occurred on the Dropbox servers.")
				.AppendLine("Check status.dropbox.com for announcements about Dropbox service issues.")
				.ToString());
			}

			throw new Exception();
		}

		public async Task<Response.TokenFromOauth1> TokenFromOauth1()
		{
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://api.dropboxapi.com/2/auth/token/from_oauth1"),
			};
			httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{AppKey}:{AppSecret}")));
			var requestContent = Serialize(new Request.TokenFromOauth1
			{
				oauth1_token = AppKey,
				oauth1_token_secret = AppSecret
			});
			httpRequest.Content = new StringContent(requestContent, Encoding.UTF8, MediaType.Json);
			var response = await m_HttpClient.SendAsync(httpRequest);
			var responseContent = await GetResponseContent(response);

			var tokenFromOauth1 = Deserialize<Response.TokenFromOauth1>(responseContent);
			AccessToken = tokenFromOauth1.oauth2_token;

			return tokenFromOauth1;
		}

		public async Task<Response.OAuth2Token> OAuth2Token(string accessCode)
		{
			var parameters = await new FormUrlEncodedContent(new Dictionary<string, string>()
			{
				{ "code", accessCode },
				{ "grant_type", "authorization_code" }
			}).ReadAsStringAsync();
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri($"https://api.dropbox.com/oauth2/token?{parameters}"),
			};
			httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{AppKey}:{AppSecret}")));
			var response = await m_HttpClient.SendAsync(httpRequest);
			var responseContent = await GetResponseContent(response);

			var oauth2Token = Deserialize<Response.OAuth2Token>(responseContent);
			AccessToken = oauth2Token.access_token;

			return oauth2Token;
		}

		public async Task<Response.CurrentAccount> GetCurrentAccount()
		{
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://api.dropboxapi.com/2/users/get_current_account"),
			};
			httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

			var response = await m_HttpClient.SendAsync(httpRequest);
			var responseContent = await GetResponseContent(response);

			var currentAccount = Deserialize<Response.CurrentAccount>(responseContent);
			return currentAccount;
		}

		/// <summary>
		/// 指定したパスのファイルを取得
		/// </summary>
		/// <param name="path">DropBox上のパス</param>
		/// <returns></returns>
		public async Task<Response.ListFolder> ListFolder(string path = null)
		{
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://api.dropboxapi.com/2/files/list_folder"),
			};
			httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
			var request = new Request.ListFolder { path = path ?? string.Empty };
			var content = Serialize(request);
			var requestContent = new StringContent(content, Encoding.UTF8, MediaType.Json);
			httpRequest.Content = requestContent;

			var response = await m_HttpClient.SendAsync(httpRequest);
			var responseContent = await GetResponseContent(response);

			var listFolder = Deserialize<Response.ListFolder>(responseContent);

			return listFolder;
		}

		public async Task<Response.ListFolder> ListFolderContinue(string cursor)
		{
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://api.dropboxapi.com/2/files/list_folder/continue"),
			};
			httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
			var request = new Request.ListFolderContinue { cursor = cursor };
			var content = Serialize(request);
			var requestContent = new StringContent(content, Encoding.UTF8, MediaType.Json);
			httpRequest.Content = requestContent;

			var response = await m_HttpClient.SendAsync(httpRequest);
			var responseContent = await GetResponseContent(response);

			var listFolder = Deserialize<Response.ListFolder>(responseContent);

			return listFolder;
		}

		/// <summary>
		/// ダウンロード
		/// </summary>
		/// <param name="dropBoxFilePath">ドロップボックス上のファイルパス</param>
		/// <param name="localFilePath">ローカルファイルのパス</param>
		/// <returns>
		/// <para>true:ダウンロード成功</para>
		/// <para>false:ダウンロード失敗</para>
		/// </returns>
		public async Task<bool> Download(string dropBoxFilePath, string localFilePath)
		{
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://content.dropboxapi.com/2/files/download"),
			};
			httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
			var request = new Request.Download { path = dropBoxFilePath };
			var content = Serialize(request);
			httpRequest.Headers.Add("Dropbox-API-Arg", content);

			var response = await m_HttpClient.SendAsync(httpRequest);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return false;
			}

			using (var stream = await response.Content.ReadAsStreamAsync())
			using (var memoryStream = new MemoryStream())
			{
				while (true)
				{
					var buffer = new byte[256];
					int readSize = await stream.ReadAsync(buffer, 0, buffer.Length);
					if (readSize > 0) memoryStream.Write(buffer, 0, readSize);
					else break;
				}
				File.WriteAllBytes(localFilePath, memoryStream.ToArray());
			}
			return true;
		}

		public async Task<Response.DeleteV2> DeleteV2(string path)
		{
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://api.dropboxapi.com/2/files/delete_v2"),
			};
			httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
			var request = new Request.DeleteV2 { path = path };
			var content = Serialize(request);
			var requestContent = new StringContent(content, Encoding.UTF8, MediaType.Json);
			httpRequest.Content = requestContent;

			var response = await m_HttpClient.SendAsync(httpRequest);
			var responseContent = await GetResponseContent(response);

			var delete = Deserialize<Response.DeleteV2>(responseContent);

			return delete;
		}

		/// <summary>
		/// アップロード
		/// </summary>
		/// <param name="localFilePath">ローカルファイルパス</param>
		/// <param name="dropBoxFilePath">ドロップボックス上のファイルパス</param>
		/// <returns></returns>
		public async Task<bool> Upload(string localFilePath, string dropBoxFilePath)
		{
			var url = "https://content.dropboxapi.com/2/files/upload";
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri(url),
			};
			httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
			var parameter = Serialize(new Request.Upload
			{
				path = dropBoxFilePath,
				mode = "add",
				autorename = true,
				mute = false,
				strict_conflict = false
			});
			httpRequest.Headers.Add("Dropbox-API-Arg", parameter);

			ByteArrayContent byteArrayContent;
			using (var fileStream = new FileStream(localFilePath, FileMode.Open))
			using (var memoryStream = new MemoryStream())
			{
				while (true)
				{
					var buffer = new byte[256];
					int readSize = await fileStream.ReadAsync(buffer, 0, buffer.Length);
					if (readSize > 0) memoryStream.Write(buffer, 0, readSize);
					else break;
				}
				byteArrayContent = new ByteArrayContent(memoryStream.ToArray());
			}

			httpRequest.Content = byteArrayContent;
			httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

			var response = await m_HttpClient.SendAsync(httpRequest);

			return response.StatusCode == System.Net.HttpStatusCode.OK;
		}

		public async Task<string> UploadSessionStart(bool close = false)
		{
			var url = "https://content.dropboxapi.com/2/files/upload_session/start";
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri(url),
			};
			httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
			var requestContent = Serialize(new Request.UploadSessionStart { close = close });
			httpRequest.Headers.Add("Dropbox-API-Arg", requestContent);
			httpRequest.Headers.Add("Content-Type", MediaType.Stream);
			httpRequest.Content = new ByteArrayContent(Encoding.UTF8.GetBytes("hogehoge"));

			var response = await m_HttpClient.SendAsync(httpRequest);

			var statusCode = (int)response.StatusCode;
			if (statusCode == 200)
			{
				var responseContent = await response.Content.ReadAsStringAsync();
				return responseContent;
			}

			throw new Exception();
		}

		/// <summary>
		/// シリアライザ
		/// </summary>
		/// <typeparam name="T">変換前のクラス</typeparam>
		/// <param name="value">変換前のインスタンス</param>
		/// <returns>JSON</returns>
		private string Serialize<T>(T value)
			where T : class
		{
			using (var memoryStream = new MemoryStream())
			{
				var serializer = new DataContractJsonSerializer(value.GetType());
				serializer.WriteObject(memoryStream, value);
				return Encoding.UTF8.GetString(memoryStream.ToArray());
			}
		}

		/// <summary>
		/// デシリアライザ
		/// </summary>
		/// <typeparam name="T">変換後のクラス</typeparam>
		/// <param name="value">JSON</param>
		/// <returns>インスタンス</returns>
		private T Deserialize<T>(string value)
			where T : class
		{
			using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
			{
				var serializer = new DataContractJsonSerializer(typeof(T));
				return (T)serializer.ReadObject(memoryStream);
			}
		}
	}
}
