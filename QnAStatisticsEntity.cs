using System;
using Microsoft.Azure.Cosmos.Table;

namespace Functions.Entity.Models
{
    public class QnaStatisticsEntity : TableEntity
    {

        public string QnaId { get; set; } = null;

        public Int32 ClickCount { get; set; } = 0;

        public Int32 AnswerCount { get; set; } = 0;

        public Int32 ResolvedCount { get; set; } = 0;

        public Int32 NotResolvedCount { get; set; } = 0;

    }
}