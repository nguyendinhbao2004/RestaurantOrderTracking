using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class VoiceCommand : BaseEntities
    {
        public Guid AccountId { get; private set; }
        public virtual Account Accounts { get; private set; }

        public Guid OrderItemId { get; private set; }
        public virtual OrderItems OrderItems { get; private set; }

        public string AudioUrl { get; private set; }

        public string TranscribedText { get; private set; }

        public Guid ParsedTableNumber { get; private set; } // chưa hiểu

        public string ParsedAction { get; private set; }

        public float ConfidenceScore { get; private set; }

        public VoiceCommandStatus Status { get; set; }

        public VoiceCommand(Guid accountId, Guid orderItemId, string audioUrl)
        {
            AccountId = accountId;
            OrderItemId = orderItemId;
            AudioUrl = audioUrl;
            Status = VoiceCommandStatus.pending;
        }

        
    }
}
