using System;
using System.Collections.Generic;
using System.Text;

namespace ylccClientTool
{
    public class CommonModel
    {
        public string VideoId { get; set; }

        public string Uri { get; set; }

        public bool IsInsecure { get; set; }
        public string WindowBackgroundColor { get; set; }

        public CommonModel()
        {
            VideoId = "";
            Uri = "http://127.0.0.1:12345";
            IsInsecure = true;
            WindowBackgroundColor = "#00FFFFFF";
        }
    }
}
