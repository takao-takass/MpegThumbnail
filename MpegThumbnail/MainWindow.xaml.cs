using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MpegThumbnail
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 実行ボタン押下イベント
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            CreateThumbImage();
        }

        /// <summary>
        /// サムネイル画像を作成する
        /// </summary>
        private void CreateThumbImage()
        {

            // 動画ファイルの情報を取得する
            var fileinfoList = CreateMovieFileInfo(this.movieFolderPathInput.Text, this.movieFormatInput.Text);

            // サムネ切り出し対象の時間を取得する
            var timeSeconds = this.thumbTrimTimeSecondsInput.Text;
            var timeMinutes = this.thumbTrimTimeMinutesInput.Text;
            var seek = Int32.Parse(timeMinutes) * 60 + Int32.Parse(timeSeconds);

            // チェックボックスの選択状態を取得
            // 未選択(IsCheckedがnullになる)の場合はfalseとして扱う
            var isChecked = (this.targetCheckBox.IsChecked ?? false);

            // サムネイル作成処理を行う
            foreach (var fileinfo in fileinfoList)
            {
                // サムネ未作成のみ対象とする場合は、サムネ作成済みの動画をスキップする
                // サムネ作成済みも対象とする場合は、既存のサムネを削除する
                if (fileinfo.IsCreatedThumb)
                {
                    if (isChecked)
                    {
                        continue;
                    }
                    else
                    {
                        System.IO.File.Delete(fileinfo.ThumbPath);
                    }
                }

                // FFMPEGのコマンド引数を作成する
                var input = fileinfo.FilePath;
                var output = fileinfo.ThumbPath;
                var arguments = string.Format("-ss {1} -i \"{0}\" -vframes 1 -f image2 \"{2}\"", input, seek, output);

                // FFMPEG.exeを実行する
                var ffmpegPath = this.ffmpegFilePathInput.Text;
                var process = Process.Start(new ProcessStartInfo(ffmpegPath, arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                });

                // プロセスを終了する
                process.WaitForExit();
                process.Close();

            }

            // 完了メッセージ表示
            MessageBox.Show("処理が完了しました", String.Empty, MessageBoxButton.OK);

        }

        /// <summary>
        /// 対象の動画ファイルの情報を作成する
        /// </summary>
        /// <returns></returns>
        private List<MovieFileModel> CreateMovieFileInfo(string folderPath, string movieFormat)
        {
            var fileinfoList = new List<MovieFileModel>();

            // 指定フォルダの動画ファイルを取得し、動画ファイル情報を作成する
            var filePathArray = System.IO.Directory.GetFiles(folderPath, String.Format("*{0}", movieFormat));
            foreach (var filePath in filePathArray)
            {
                // サムネ画像のファイルパスを作成
                var thumbPath = filePath.Replace(movieFormat, ".jpg");

                // サムネ画像が既に作成されているかチェック
                var isCreatedThumb = false;
                if (System.IO.File.Exists(thumbPath))
                {
                    isCreatedThumb = true;
                }

                // 動画ファイル情報を作成する
                fileinfoList.Add(new MovieFileModel()
                {
                    FilePath = filePath,
                    ThumbPath = thumbPath,
                    IsCreatedThumb = isCreatedThumb
                });

            }

            return fileinfoList;
        }

    }
}
