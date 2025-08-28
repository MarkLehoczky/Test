using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace VidHub.Core
{
    public class Video : IComparable<Video>, IEquatable<Video>
    {
        public static List<string> ExtensionTypes => [".mp4", ".mov", ".wmv", ".mkv"];
        private static int IDProvider = 0;

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
        public string VideoPreview { get; set; }
        public string FilePath { get; set; }
        [JsonIgnore] public int ID { get; set; }


        public Video()
        {
            Title = string.Empty;
            Date = DateTime.MinValue;
            Duration = TimeSpan.Zero;
            VideoPreview = string.Empty;
            FilePath = string.Empty;
            ID = IDProvider;
            Interlocked.Increment(ref IDProvider);
        }
        public Video(string filePath) : this()
        {
            FilePath = Path.GetFullPath(filePath);
        }


        public void Load()
        {
            Title = Path.GetFileNameWithoutExtension(FilePath);
            Date = File.GetLastWriteTime(FilePath);
            Date = ExtractMetaDate();
            Duration = ExtractMetaDuration();
            VideoPreview = ExtractVideoPreview();
        }

        public bool TryLoad()
        {
            bool successful = true;


            var actions = new List<Action>
            {
                () => Title = Path.GetFileNameWithoutExtension(FilePath),
                () => Date = File.GetLastWriteTime(FilePath),
                () => Date = ExtractMetaDate(),
                () => Duration = ExtractMetaDuration(),
                () => VideoPreview = ExtractVideoPreview()
            };

            foreach (var action in actions)
            {
                try
                {
                    action();
                }
                catch
                {
                    successful = false;
                }
            }

            return successful;
        }


        private DateTime ExtractMetaDate()
        {
            try
            {
                using var ffprobe = new Process();
                ffprobe.StartInfo.FileName = "ffprobe";
                ffprobe.StartInfo.CreateNoWindow = true;
                ffprobe.StartInfo.UseShellExecute = false;
                ffprobe.StartInfo.ArgumentList.Add("-v");
                ffprobe.StartInfo.ArgumentList.Add("error");
                ffprobe.StartInfo.ArgumentList.Add("-show_entries");
                ffprobe.StartInfo.ArgumentList.Add("format_tags=creation_time");
                ffprobe.StartInfo.ArgumentList.Add("-of");
                ffprobe.StartInfo.ArgumentList.Add("default=noprint_wrappers=1:nokey=1");
                ffprobe.StartInfo.ArgumentList.Add(FilePath);
                ffprobe.StartInfo.RedirectStandardOutput = true;
                ffprobe.StartInfo.RedirectStandardError = true;

                ffprobe.Start();

                if (!ffprobe.WaitForExit(5000))
                {
                    ffprobe.Kill();
                    throw new TimeoutException();
                }

                string standardOutput = ffprobe.StandardOutput.ReadToEnd();
                string errorOutput = ffprobe.StandardError.ReadToEnd();

                if (ffprobe.ExitCode != 0)
                {
                    throw new InvalidDataException();
                }

                return DateTime.Parse(standardOutput);
            }
            catch (Win32Exception)
            {
                throw new Win32Exception("FFprobe is not installed or not available in the system PATH.");
            }
            catch (TimeoutException)
            {
                throw new TimeoutException("FFprobe process timed out while extracting the video creation date.");
            }
            catch (InvalidDataException)
            {
                throw new InvalidDataException("Failed to extract video creation date.");
            }
            catch (FormatException)
            {
                throw new FormatException("Failed to parse extracted video creation date.");
            }
            catch (Exception)
            {
                throw new Exception("An unknown error occurred while extracting the video creation date.");
            }
        }

        private TimeSpan ExtractMetaDuration()
        {
            try
            {
                using var ffprobe = new Process();
                ffprobe.StartInfo.FileName = "ffprobe";
                ffprobe.StartInfo.CreateNoWindow = true;
                ffprobe.StartInfo.UseShellExecute = false;
                ffprobe.StartInfo.ArgumentList.Add("-v");
                ffprobe.StartInfo.ArgumentList.Add("error");
                ffprobe.StartInfo.ArgumentList.Add("-show_entries");
                ffprobe.StartInfo.ArgumentList.Add("format=duration");
                ffprobe.StartInfo.ArgumentList.Add("-of");
                ffprobe.StartInfo.ArgumentList.Add("default=noprint_wrappers=1:nokey=1");
                ffprobe.StartInfo.ArgumentList.Add(FilePath);
                ffprobe.StartInfo.RedirectStandardOutput = true;
                ffprobe.StartInfo.RedirectStandardError = true;

                ffprobe.Start();

                if (!ffprobe.WaitForExit(5000))
                {
                    ffprobe.Kill();
                    throw new TimeoutException();
                }

                string standardOutput = ffprobe.StandardOutput.ReadToEnd();
                string errorOutput = ffprobe.StandardError.ReadToEnd();

                if (ffprobe.ExitCode != 0)
                {
                    throw new InvalidDataException();
                }

                return TimeSpan.FromSeconds(double.Parse(standardOutput.Trim(), CultureInfo.InvariantCulture));
            }
            catch (Win32Exception)
            {
                throw new Win32Exception("FFprobe is not installed or not available in the system PATH.");
            }
            catch (TimeoutException)
            {
                throw new TimeoutException("FFprobe process timed out while extracting the video duration.");
            }
            catch (InvalidDataException)
            {
                throw new InvalidDataException("Failed to extract video duration.");
            }
            catch (FormatException)
            {
                throw new FormatException("Failed to parse extracted video duration.");
            }
            catch (Exception)
            {
                throw new Exception("An unknown error occurred while extracting the video duration.");
            }
        }

        private string ExtractVideoPreview()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), "VidHub");
            string hashFileName = $"{BitConverter.ToString(MD5.HashData(Encoding.UTF8.GetBytes(FilePath))).Replace("-", "").ToLowerInvariant()}.jpg";
            string outputFilePath = Path.Combine(tempDirectory, hashFileName);

            Directory.CreateDirectory(tempDirectory);

            if (File.Exists(outputFilePath))
            {
                return outputFilePath;
            }

            try
            {
                using var ffmpeg = new Process();
                ffmpeg.StartInfo.FileName = "ffmpeg";
                ffmpeg.StartInfo.CreateNoWindow = true;
                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.ArgumentList.Add("-v");
                ffmpeg.StartInfo.ArgumentList.Add("error");
                ffmpeg.StartInfo.ArgumentList.Add("-ss");
                ffmpeg.StartInfo.ArgumentList.Add((Duration.TotalSeconds / 2).ToString());
                ffmpeg.StartInfo.ArgumentList.Add("-i");
                ffmpeg.StartInfo.ArgumentList.Add(FilePath);
                ffmpeg.StartInfo.ArgumentList.Add("-frames:v");
                ffmpeg.StartInfo.ArgumentList.Add("1");
                ffmpeg.StartInfo.ArgumentList.Add(outputFilePath);
                ffmpeg.StartInfo.RedirectStandardOutput = true;
                ffmpeg.StartInfo.RedirectStandardError = true;

                ffmpeg.Start();

                if (!ffmpeg.WaitForExit(10000))
                {
                    ffmpeg.Kill();
                    throw new TimeoutException();
                }

                string errorOutput = ffmpeg.StandardError.ReadToEnd();

                if (ffmpeg.ExitCode != 0 || !File.Exists(outputFilePath))
                {
                    throw new InvalidDataException();
                }

                return outputFilePath;
            }
            catch (Win32Exception)
            {
                throw new Win32Exception("FFmpeg is not installed or not available in the system PATH.");
            }
            catch (TimeoutException)
            {
                throw new TimeoutException("FFmpeg process timed out while extracting the video preview.");
            }
            catch (InvalidDataException)
            {
                throw new InvalidDataException("Failed to extract video preview.");
            }
            catch (Exception)
            {
                throw new Exception("An unknown error occurred while extracting the video preview.");
            }

        }


        public int CompareTo(Video? other)
        {
            if (other == null) return -1;
            if (ReferenceEquals(this, other)) return 0;
            int comparison = ID.CompareTo(other.ID);
            if (comparison != 0) return comparison;
            comparison = Title.CompareTo(other.Title);
            if (comparison != 0) return comparison;
            comparison = Date.CompareTo(other.Date);
            if (comparison != 0) return comparison;
            return Duration.CompareTo(other.Duration);
        }

        public bool Equals(Video? other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.Title != Title) return false;
            if (other.Date != Date) return false;
            if (other.Duration != Duration) return false;
            return true;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Video);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Date, Duration);
        }

        public override string ToString()
        {
            return $"Title: {Title}\nDate: {Date}\nDuration: {Duration}";
        }
    }
}
