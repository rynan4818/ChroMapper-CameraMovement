using ScriptMapper.Core.Abstractions;
using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChroMapper_CameraMovement.Util
{
    public class ScriptMapperLogger : ILogger, IDisposable
    {
        private MemoryStream _memoryStream;
        private StreamWriter _streamWriter;

        public ScriptMapperLogger()
        {
            _memoryStream = new MemoryStream();
            _streamWriter = new StreamWriter(_memoryStream, Encoding.UTF8);
            _streamWriter.AutoFlush = true;
        }

        public void Log(string message)
        {
            _streamWriter.WriteLine(message);
        }

        /// <summary>
        /// 現在メモリに保持されているログの内容を文字列として取得します。
        /// </summary>
        /// <returns>ログ全体の文字列</returns>
        public string GetLogContent()
        {
            // まだストリームに書き込まれていないバッファ内のデータをフラッシュする
            _streamWriter.Flush();

            // MemoryStreamのバイト配列を取り出し、UTF-8文字列に変換して返す
            return Encoding.UTF8.GetString(_memoryStream.ToArray());
        }

        /// <summary>
        /// 蓄積されたログを指定されたファイルパスに非同期で保存します。
        /// </summary>
        public async Task SaveToFileAsync(string path)
        {
            try
            {
                // 1. メモリ上のログデータを確定させる
                _streamWriter.Flush();
                byte[] logBodyData = _memoryStream.ToArray();

                // 2. ヘッダ文字列を作成し、バイト配列に変換する
                string header = $"logfile at {DateTime.Now:yyyy-MM-dd_HH_mm_ss}\n\n";
                byte[] headerData = Encoding.UTF8.GetBytes(header);

                // 3. ファイルストリームを開いて順次書き込む
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    // 先頭にヘッダを書き込む
                    await fs.WriteAsync(headerData, 0, headerData.Length);

                    // 続けてログ本文を書き込む
                    await fs.WriteAsync(logBodyData, 0, logBodyData.Length);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"ScriptMapper Log Save Error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _streamWriter?.Dispose();
            _memoryStream?.Dispose();
        }
    }
}