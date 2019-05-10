using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpegThumbnail
{
    class MovieFileModel
    {
        /// <summary>
        /// 動画ファイルのパス
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// サムネファイルのパス
        /// </summary>
        public string ThumbPath { get; set; }

        /// <summary>
        /// サムネイルが作成済みか否か
        /// </summary>
        public bool IsCreatedThumb { get; set; }
    }
}
