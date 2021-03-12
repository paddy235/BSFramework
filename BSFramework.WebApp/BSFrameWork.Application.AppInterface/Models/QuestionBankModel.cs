using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class answerPost
    {
        public string titleid { get; set; }
        public int sort { get; set; }
        public string answer { get; set; }
    }

    public class HistoryAnswerTitleModel
    {
        public string category { get; set; }
        public string endTime { get; set; }
        public string startTime { get; set; }
        public string iscomplete { get; set; }
    }
}